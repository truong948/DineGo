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
    [Route("api/hoa-don")]
    public class HoaDonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HoaDonController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/hoa-don/tam-tinh/{soPhieuYeuCau} (Xem trước hóa đơn tạm tính)
        [HttpGet("tam-tinh/{soPhieuYeuCau}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetPrePrintBill(Guid soPhieuYeuCau, [FromQuery] decimal giamGiaPhanTram = 0, [FromQuery] decimal thuePhanTram = 10)
        {
            var phieu = await _context.PhieuYeuCaus
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietPhieuYeuCaus)
                    .ThenInclude(d => d.MonAn)
                .FirstOrDefaultAsync(p => p.SoPhieuYeuCau == soPhieuYeuCau);

            if (phieu == null)
            {
                return NotFound(new { message = "Không tìm thấy phiếu yêu cầu đang hoạt động." });
            }

            if (phieu.TrangThai == TrangThaiPhieu.Completed || phieu.TrangThai == TrangThaiPhieu.Cancelled)
            {
                return BadRequest(new { message = "Phiếu yêu cầu đã được thanh toán hoặc đã bị hủy trước đó." });
            }

            var chiTietDtos = phieu.ChiTietPhieuYeuCaus.Select(d => new ChiTietOrderDto
            {
                Id = d.Id,
                MaMon = d.MaMon,
                TenMon = d.MonAn.TenMon,
                SoLuong = d.SoLuong,
                DonGiaDat = d.DonGiaDat,
                TrangThaiCheBien = d.TrangThaiCheBien,
                GhiChu = d.GhiChu
            }).ToList();

            decimal tienMon = chiTietDtos.Sum(d => d.ThanhTien);
            decimal giamGia = (tienMon * giamGiaPhanTram) / 100;
            decimal thueVAT = ((tienMon - giamGia) * thuePhanTram) / 100;
            decimal tongTien = tienMon - giamGia + thueVAT;

            var response = new TamTinhResponseDto
            {
                SoPhieuYeuCau = phieu.SoPhieuYeuCau,
                SoBan = phieu.SoBan,
                TenKhachHang = phieu.KhachHang.HoTen,
                NgayYeuCau = phieu.NgayYeuCau,
                GioVao = phieu.GioDat,
                ChiTietMonAns = chiTietDtos,
                TienMon = tienMon,
                GiamGia = giamGia,
                ThueVAT = thueVAT,
                TongTienThanhToan = tongTien
            };

            return Ok(response);
        }

        // 2. POST: api/hoa-don/thanh-toan (Xác nhận thanh toán & Đóng bàn)
        [HttpPost("thanh-toan")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> MakePayment([FromBody] ThanhToanRequestDto request)
        {
            var phieu = await _context.PhieuYeuCaus
                .Include(p => p.ChiTietPhieuYeuCaus)
                .FirstOrDefaultAsync(p => p.SoPhieuYeuCau == request.SoPhieuYeuCau);

            if (phieu == null)
            {
                return NotFound(new { message = "Không tìm thấy phiếu yêu cầu thanh toán." });
            }

            if (phieu.TrangThai == TrangThaiPhieu.Completed)
            {
                return BadRequest(new { message = "Phiếu yêu cầu này đã được thanh toán rồi." });
            }

            if (phieu.TrangThai == TrangThaiPhieu.Cancelled)
            {
                return BadRequest(new { message = "Phiếu yêu cầu này đã bị hủy, không thể thanh toán." });
            }

            // Kiểm tra nhân viên thực hiện
            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(n => n.MaNV == request.MaNV);
            if (nhanVien == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên thu ngân trong hệ thống." });
            }

            // Tính tổng tiền
            decimal tienMon = phieu.ChiTietPhieuYeuCaus.Sum(d => d.SoLuong * d.DonGiaDat);
            decimal giamGia = (tienMon * request.GiamGiaPhanTram) / 100;
            decimal thueVAT = ((tienMon - giamGia) * request.ThuePhanTram) / 100;
            decimal tongTien = tienMon - giamGia + thueVAT;

            // Kiểm tra xem đã có hóa đơn nào liên kết chưa (One-to-One)
            var duplicatePayment = await _context.PhieuThanhToans.AnyAsync(pt => pt.SoPhieuYeuCau == request.SoPhieuYeuCau);
            if (duplicatePayment)
            {
                return BadRequest(new { message = "Đã tồn tại hóa đơn cho phiếu yêu cầu này." });
            }

            // Tạo hóa đơn mới
            var hoaDon = new PhieuThanhToan
            {
                SoPhieuThanhToan = Guid.NewGuid(),
                SoPhieuYeuCau = request.SoPhieuYeuCau,
                NgayThanhToan = DateTime.UtcNow,
                MaNV = request.MaNV,
                TongTien = tongTien,
                PhuongThucThanhToan = request.PhuongThucThanhToan,
                DaThanhToan = true
            };

            // Cập nhật trạng thái phiếu yêu cầu sang Completed
            phieu.TrangThai = TrangThaiPhieu.Completed;

            // Giải phóng bàn ăn tương ứng sang trạng thái dọn dẹp
            var banAn = await _context.BanAns.FirstOrDefaultAsync(b => b.SoBan == phieu.SoBan);
            if (banAn != null)
            {
                banAn.TrangThaiHienTai = TrangThaiBan.DangDonDep;
                banAn.NgayCapNhat = DateTime.UtcNow;
            }

            _context.PhieuThanhToans.Add(hoaDon);
            await _context.SaveChangesAsync();

            var response = new ThanhToanResponseDto
            {
                SoPhieuThanhToan = hoaDon.SoPhieuThanhToan,
                SoPhieuYeuCau = hoaDon.SoPhieuYeuCau,
                SoBan = phieu.SoBan,
                TenNhanVien = nhanVien.HoTenNV,
                TenKhachHang = await _context.KhachHangs.Where(k => k.MaKH == phieu.MaKH).Select(k => k.HoTen).FirstOrDefaultAsync() ?? "Khách vãng lai",
                NgayThanhToan = hoaDon.NgayThanhToan,
                TongTien = hoaDon.TongTien,
                PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
                DaThanhToan = hoaDon.DaThanhToan
            };

            return Ok(response);
        }

        // 3. GET: api/hoa-don/lich-su (Lấy lịch sử hóa đơn thanh toán)
        [HttpGet("lich-su")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? tuNgay = null,
            [FromQuery] DateTime? denNgay = null,
            [FromQuery] Guid? maNV = null,
            [FromQuery] string? phuongThucThanhToan = null)
        {
            var query = _context.PhieuThanhToans
                .Include(pt => pt.NhanVien)
                .Include(pt => pt.PhieuYeuCau)
                    .ThenInclude(p => p.KhachHang)
                .AsQueryable();

            if (tuNgay.HasValue)
            {
                query = query.Where(pt => pt.NgayThanhToan >= tuNgay.Value.ToUniversalTime());
            }

            if (denNgay.HasValue)
            {
                query = query.Where(pt => pt.NgayThanhToan <= denNgay.Value.ToUniversalTime());
            }

            if (maNV.HasValue)
            {
                query = query.Where(pt => pt.MaNV == maNV.Value);
            }

            if (!string.IsNullOrWhiteSpace(phuongThucThanhToan))
            {
                query = query.Where(pt => pt.PhuongThucThanhToan.ToLower() == phuongThucThanhToan.ToLower());
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var items = await query
                .OrderByDescending(pt => pt.NgayThanhToan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(pt => new
                {
                    pt.SoPhieuThanhToan,
                    pt.SoPhieuYeuCau,
                    pt.PhieuYeuCau.SoBan,
                    TenKhachHang = pt.PhieuYeuCau.KhachHang.HoTen,
                    TenThuNgan = pt.NhanVien.HoTenNV,
                    pt.NgayThanhToan,
                    pt.TongTien,
                    pt.PhuongThucThanhToan,
                    pt.DaThanhToan
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                totalPages,
                currentPage = page,
                pageSize,
                items
            });
        }

        // 4. GET: api/hoa-don/thong-ke/doanh-thu (Thống kê doanh thu)
        [HttpGet("thong-ke/doanh-thu")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRevenueStatistics(
            [FromQuery] string loaiThongKe = "ngay", // ngay | thang | nam
            [FromQuery] DateTime? tuNgay = null,
            [FromQuery] DateTime? denNgay = null)
        {
            var query = _context.PhieuThanhToans.Where(pt => pt.DaThanhToan).AsQueryable();

            DateTime start = tuNgay ?? DateTime.UtcNow.AddDays(-30);
            DateTime end = denNgay ?? DateTime.UtcNow;

            query = query.Where(pt => pt.NgayThanhToan >= start.ToUniversalTime() && pt.NgayThanhToan <= end.ToUniversalTime());

            var rawData = await query.ToListAsync();

            decimal totalRevenue = rawData.Sum(pt => pt.TongTien);
            int totalBills = rawData.Count;

            List<BieuDoDoanhThuDto> bieuDo = new();

            if (loaiThongKe == "ngay")
            {
                bieuDo = rawData
                    .GroupBy(pt => pt.NgayThanhToan.ToLocalTime().ToString("yyyy-MM-dd"))
                    .Select(g => new BieuDoDoanhThuDto
                    {
                        ThoiGian = g.Key,
                        DoanhThu = g.Sum(pt => pt.TongTien),
                        SoHoaDon = g.Count()
                    })
                    .OrderBy(b => b.ThoiGian)
                    .ToList();
            }
            else if (loaiThongKe == "thang")
            {
                bieuDo = rawData
                    .GroupBy(pt => pt.NgayThanhToan.ToLocalTime().ToString("yyyy-MM"))
                    .Select(g => new BieuDoDoanhThuDto
                    {
                        ThoiGian = g.Key,
                        DoanhThu = g.Sum(pt => pt.TongTien),
                        SoHoaDon = g.Count()
                    })
                    .OrderBy(b => b.ThoiGian)
                    .ToList();
            }
            else // nam
            {
                bieuDo = rawData
                    .GroupBy(pt => pt.NgayThanhToan.ToLocalTime().ToString("yyyy"))
                    .Select(g => new BieuDoDoanhThuDto
                    {
                        ThoiGian = g.Key,
                        DoanhThu = g.Sum(pt => pt.TongTien),
                        SoHoaDon = g.Count()
                    })
                    .OrderBy(b => b.ThoiGian)
                    .ToList();
            }

            var result = new ThongKeDoanhThuResponseDto
            {
                TongDoanhThu = totalRevenue,
                TongSoHoaDon = totalBills,
                BieuDoDoanhThu = bieuDo
            };

            return Ok(result);
        }
    }
}
