using DineGo.Data;
using DineGo.DTOs;
using DineGo.Models;
using DineGo.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DineGo.Controllers
{
    [ApiController]
    [Route("api/phieu-yeu-cau")]
    public class PhieuYeuCauController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhieuYeuCauController(AppDbContext context)
        {
            _context = context;
        }

        // 1. POST: api/phieu-yeu-cau/check-in (Khách vào bàn / Bắt đầu check-in bàn ăn)
        [HttpPost("check-in")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto dto)
        {
            var banAn = await _context.BanAns.FirstOrDefaultAsync(b => b.SoBan == dto.SoBan);
            if (banAn == null)
            {
                return NotFound(new { message = $"Không tìm thấy bàn ăn {dto.SoBan}" });
            }

            if (!banAn.IsActive)
            {
                return BadRequest(new { message = "Bàn ăn này đang bị khóa hoạt động." });
            }

            // Kiểm tra xem bàn có đang có khách hay không (TrangThaiHienTai = DangCoKhach)
            if (banAn.TrangThaiHienTai == TrangThaiBan.DangCoKhach)
            {
                return BadRequest(new { message = $"Bàn {dto.SoBan} đang có khách ăn. Không thể Check-in bàn này." });
            }

            // Tìm hoặc tạo khách hàng
            KhachHang? khachHang = null;
            if (!string.IsNullOrWhiteSpace(dto.DienThoai))
            {
                khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.DienThoai == dto.DienThoai);
            }

            if (khachHang == null)
            {
                khachHang = new KhachHang
                {
                    MaKH = Guid.NewGuid(),
                    HoTen = dto.HoTenKhachHang,
                    DienThoai = dto.DienThoai,
                    NgayTao = DateTime.UtcNow
                };
                _context.KhachHangs.Add(khachHang);
            }

            // Tạo phiếu yêu cầu mới (Trạng thái Arrived - Khách đã vào ngồi tại bàn)
            var phieu = new PhieuYeuCau
            {
                SoPhieuYeuCau = Guid.NewGuid(),
                SoBan = dto.SoBan,
                MaKH = khachHang.MaKH,
                TrangThai = TrangThaiPhieu.Arrived,
                NgayYeuCau = DateOnly.FromDateTime(DateTime.Today),
                GioDat = DateTime.Now.TimeOfDay,
                CreatedAt = DateTime.UtcNow
            };

            // Cập nhật trạng thái bàn ăn sang Đang có khách
            banAn.TrangThaiHienTai = TrangThaiBan.DangCoKhach;
            banAn.NgayCapNhat = DateTime.UtcNow;

            _context.PhieuYeuCaus.Add(phieu);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Check-in thành công cho khách tại bàn {dto.SoBan}.",
                soPhieuYeuCau = phieu.SoPhieuYeuCau,
                maKH = khachHang.MaKH,
                hoTen = khachHang.HoTen
            });
        }

        // 2. POST: api/phieu-yeu-cau/{soPhieuYeuCau}/goi-mon (Gọi món / Thêm món)
        [HttpPost("{soPhieuYeuCau}/goi-mon")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Order(Guid soPhieuYeuCau, [FromBody] List<OrderItemDto> items)
        {
            var phieu = await _context.PhieuYeuCaus
                .Include(p => p.ChiTietPhieuYeuCaus)
                .FirstOrDefaultAsync(p => p.SoPhieuYeuCau == soPhieuYeuCau);

            if (phieu == null)
            {
                return NotFound(new { message = "Không tìm thấy phiếu yêu cầu gọi món." });
            }

            if (phieu.TrangThai == TrangThaiPhieu.Completed || phieu.TrangThai == TrangThaiPhieu.Cancelled)
            {
                return BadRequest(new { message = "Phiếu yêu cầu này đã hoàn thành hoặc đã bị hủy. Không thể gọi thêm món." });
            }

            var maMons = items.Select(i => i.MaMon).ToList();
            var monAns = await _context.MonAns.Where(m => maMons.Contains(m.MaMon)).ToListAsync();

            foreach (var item in items)
            {
                var monAn = monAns.FirstOrDefault(m => m.MaMon == item.MaMon);
                if (monAn == null)
                {
                    return NotFound(new { message = $"Món ăn mã {item.MaMon} không tồn tại." });
                }

                if (!monAn.IsAvailable)
                {
                    return BadRequest(new { message = $"Món {monAn.TenMon} tạm thời hết hàng." });
                }

                // Kiểm tra xem món này đã có trong order và chưa chế biến hay chưa để cộng dồn
                var existingDetail = phieu.ChiTietPhieuYeuCaus.FirstOrDefault(d => 
                    d.MaMon == item.MaMon && 
                    d.TrangThaiCheBien == TrangThaiCheBien.ChoCungUng &&
                    d.GhiChu == item.GhiChu);

                if (existingDetail != null)
                {
                    existingDetail.SoLuong += item.SoLuong;
                }
                else
                {
                    var detail = new ChiTietPhieuYeuCau
                    {
                        Id = Guid.NewGuid(),
                        SoPhieuYeuCau = soPhieuYeuCau,
                        MaMon = item.MaMon,
                        SoLuong = item.SoLuong,
                        DonGiaDat = monAn.DonGia,
                        TrangThaiCheBien = TrangThaiCheBien.ChoCungUng,
                        GhiChu = item.GhiChu
                    };
                    _context.ChiTietPhieuYeuCaus.Add(detail);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Gọi món và thêm món vào phiếu yêu cầu thành công." });
        }

        // 3. PUT: api/phieu-yeu-cau/{soPhieuYeuCau}/chi-tiet/{chiTietId}/trang-thai-che-bien (Nhà bếp cập nhật trạng thái chế biến)
        [HttpPut("{soPhieuYeuCau}/chi-tiet/{chiTietId}/trang-thai-che-bien")]
        [Authorize(Roles = "Admin,Staff,Chef")]
        public async Task<IActionResult> UpdateOrderItemStatus(Guid soPhieuYeuCau, Guid chiTietId, [FromBody] UpdateOrderItemStatusDto dto)
        {
            var detail = await _context.ChiTietPhieuYeuCaus
                .FirstOrDefaultAsync(d => d.Id == chiTietId && d.SoPhieuYeuCau == soPhieuYeuCau);

            if (detail == null)
            {
                return NotFound(new { message = "Không tìm thấy chi tiết order được yêu cầu." });
            }

            detail.TrangThaiCheBien = dto.TrangThaiCheBien;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã cập nhật trạng thái món ăn thành {dto.TrangThaiCheBien}." });
        }

        // 4. POST: api/phieu-yeu-cau/{soPhieuYeuCau}/chuyen-ban (Chuyển bàn)
        [HttpPost("{soPhieuYeuCau}/chuyen-ban")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ChuyenBan(Guid soPhieuYeuCau, [FromBody] ChuyenBanRequestDto dto)
        {
            var phieu = await _context.PhieuYeuCaus.FirstOrDefaultAsync(p => p.SoPhieuYeuCau == soPhieuYeuCau);
            if (phieu == null)
            {
                return NotFound(new { message = "Không tìm thấy phiếu yêu cầu cần chuyển bàn." });
            }

            if (phieu.TrangThai != TrangThaiPhieu.Arrived && phieu.TrangThai != TrangThaiPhieu.Confirmed)
            {
                return BadRequest(new { message = "Phiếu yêu cầu không trong trạng thái được phép chuyển bàn." });
            }

            var banAnMoi = await _context.BanAns.FirstOrDefaultAsync(b => b.SoBan == dto.SoBanMoi);
            if (banAnMoi == null)
            {
                return NotFound(new { message = $"Bàn ăn mới {dto.SoBanMoi} không tồn tại." });
            }

            if (!banAnMoi.IsActive || banAnMoi.TrangThaiHienTai == TrangThaiBan.DangCoKhach)
            {
                return BadRequest(new { message = $"Bàn ăn mới {dto.SoBanMoi} không có sẵn hoặc đang có khách." });
            }

            // Ghi nhận bàn cũ và chuyển
            var soBanCu = phieu.SoBan;
            phieu.SoBan = dto.SoBanMoi;

            // Giải phóng bàn cũ sang Trống (hoặc dọn dẹp)
            var banAnCu = await _context.BanAns.FirstOrDefaultAsync(b => b.SoBan == soBanCu);
            if (banAnCu != null)
            {
                banAnCu.TrangThaiHienTai = TrangThaiBan.Trong;
                banAnCu.NgayCapNhat = DateTime.UtcNow;
            }

            // Đặt bàn mới sang Có khách
            banAnMoi.TrangThaiHienTai = TrangThaiBan.DangCoKhach;
            banAnMoi.NgayCapNhat = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã chuyển toàn bộ hóa đơn từ bàn {soBanCu} sang bàn {dto.SoBanMoi} thành công." });
        }

        // 5. POST: api/phieu-yeu-cau/gop-ban (Gộp bàn / Gộp hóa đơn)
        [HttpPost("gop-ban")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GopBan([FromBody] GopBanRequestDto dto)
        {
            var phieuNguon = await _context.PhieuYeuCaus
                .Include(p => p.ChiTietPhieuYeuCaus)
                .FirstOrDefaultAsync(p => p.SoPhieuYeuCau == dto.SoPhieuNguon);

            var phieuDich = await _context.PhieuYeuCaus
                .Include(p => p.ChiTietPhieuYeuCaus)
                .FirstOrDefaultAsync(p => p.SoPhieuYeuCau == dto.SoPhieuDich);

            if (phieuNguon == null || phieuDich == null)
            {
                return NotFound(new { message = "Không tìm thấy một trong hai phiếu yêu cầu nguồn/đích." });
            }

            if (phieuNguon.TrangThai != TrangThaiPhieu.Arrived || phieuDich.TrangThai != TrangThaiPhieu.Arrived)
            {
                return BadRequest(new { message = "Cả hai phiếu yêu cầu gộp bàn phải đang hoạt động tại bàn ăn (Arrived)." });
            }

            // Chuyển toàn bộ chi tiết món từ nguồn sang đích
            foreach (var detailNguon in phieuNguon.ChiTietPhieuYeuCaus.ToList())
            {
                // Tìm xem phiếu đích đã có món này chưa
                var existingDetailDich = phieuDich.ChiTietPhieuYeuCaus.FirstOrDefault(d => 
                    d.MaMon == detailNguon.MaMon && 
                    d.TrangThaiCheBien == detailNguon.TrangThaiCheBien &&
                    d.GhiChu == detailNguon.GhiChu);

                if (existingDetailDich != null)
                {
                    existingDetailDich.SoLuong += detailNguon.SoLuong;
                }
                else
                {
                    detailNguon.SoPhieuYeuCau = dto.SoPhieuDich;
                }
            }

            // Hủy phiếu nguồn (chuyển sang Cancelled hoặc đóng)
            phieuNguon.TrangThai = TrangThaiPhieu.Cancelled;

            // Giải phóng bàn ăn nguồn
            var banAnNguon = await _context.BanAns.FirstOrDefaultAsync(b => b.SoBan == phieuNguon.SoBan);
            if (banAnNguon != null)
            {
                banAnNguon.TrangThaiHienTai = TrangThaiBan.DangDonDep;
                banAnNguon.NgayCapNhat = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã gộp hóa đơn thành công từ phiếu bàn {phieuNguon.SoBan} vào phiếu bàn {phieuDich.SoBan}." });
        }
    }
}
