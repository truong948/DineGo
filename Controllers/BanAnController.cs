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
    [Route("api/ban-an")]
    public class BanAnController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BanAnController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/ban-an (Lấy danh sách toàn bộ bàn bao gồm tọa độ X, Y)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
        {
            var query = _context.BanAns.AsQueryable();
            if (isActive.HasValue)
            {
                query = query.Where(b => b.IsActive == isActive.Value);
            }

            var banAns = await query
                .OrderBy(b => b.SoBan)
                .Select(b => new
                {
                    b.SoBan,
                    b.LoaiBan,
                    b.SucChua,
                    b.CoordinateX,
                    b.CoordinateY,
                    b.TrangThaiHienTai,
                    b.IsActive,
                    b.NgayTao,
                    b.NgayCapNhat
                })
                .ToListAsync();

            return Ok(banAns);
        }

        // 2. GET: api/ban-an/{soBan}
        [HttpGet("{soBan}")]
        public async Task<IActionResult> GetBySoBan(string soBan)
        {
            var banAn = await _context.BanAns
                .Select(b => new
                {
                    b.SoBan,
                    b.LoaiBan,
                    b.SucChua,
                    b.CoordinateX,
                    b.CoordinateY,
                    b.TrangThaiHienTai,
                    b.IsActive,
                    b.NgayTao,
                    b.NgayCapNhat
                })
                .FirstOrDefaultAsync(b => b.SoBan == soBan);

            if (banAn == null)
            {
                return NotFound(new { message = $"Không tìm thấy bàn ăn {soBan}" });
            }

            return Ok(banAn);
        }

        // 3. POST: api/ban-an (Thêm bàn ăn mới)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] BanAnCreateDto dto)
        {
            if (await _context.BanAns.AnyAsync(b => b.SoBan == dto.SoBan))
            {
                return BadRequest(new { message = $"Bàn ăn với số hiệu {dto.SoBan} đã tồn tại trong hệ thống." });
            }

            var banAn = new BanAn
            {
                SoBan = dto.SoBan,
                LoaiBan = dto.LoaiBan,
                SucChua = dto.SucChua,
                CoordinateX = dto.CoordinateX,
                CoordinateY = dto.CoordinateY,
                TrangThaiHienTai = TrangThaiBan.Trong,
                IsActive = true,
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow
            };

            _context.BanAns.Add(banAn);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBySoBan), new { soBan = banAn.SoBan }, banAn);
        }

        // 4. PUT: api/ban-an/{soBan} (Cập nhật thông tin cơ bản)
        [HttpPut("{soBan}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string soBan, [FromBody] BanAnUpdateDto dto)
        {
            var banAn = await _context.BanAns.FirstOrDefaultAsync(b => b.SoBan == soBan);
            if (banAn == null)
            {
                return NotFound(new { message = $"Không tìm thấy bàn ăn {soBan}" });
            }

            banAn.LoaiBan = dto.LoaiBan;
            banAn.SucChua = dto.SucChua;
            banAn.IsActive = dto.IsActive;
            banAn.NgayCapNhat = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thông tin bàn ăn thành công." });
        }

        // 5. PUT: api/ban-an/coordinates (Cập nhật tọa độ kéo thả hàng loạt)
        [HttpPut("coordinates")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateCoordinates([FromBody] List<BanAnCoordinateUpdateDto> coordinates)
        {
            if (coordinates == null || !coordinates.Any())
            {
                return BadRequest(new { message = "Danh sách tọa độ cập nhật không hợp lệ." });
            }

            var soBans = coordinates.Select(c => c.SoBan).ToList();
            var banAns = await _context.BanAns.Where(b => soBans.Contains(b.SoBan)).ToListAsync();

            foreach (var coord in coordinates)
            {
                var banAn = banAns.FirstOrDefault(b => b.SoBan == coord.SoBan);
                if (banAn != null)
                {
                    banAn.CoordinateX = coord.CoordinateX;
                    banAn.CoordinateY = coord.CoordinateY;
                    banAn.NgayCapNhat = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đồng bộ tọa độ sơ đồ bàn ăn thành công." });
        }

        // 6. PUT: api/ban-an/{soBan}/trang-thai (Đổi trạng thái bàn ăn)
        [HttpPut("{soBan}/trang-thai")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateStatus(string soBan, [FromBody] BanAnStatusUpdateDto dto)
        {
            var banAn = await _context.BanAns.FirstOrDefaultAsync(b => b.SoBan == soBan);
            if (banAn == null)
            {
                return NotFound(new { message = $"Không tìm thấy bàn ăn {soBan}" });
            }

            banAn.TrangThaiHienTai = dto.TrangThai;
            banAn.NgayCapNhat = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã cập nhật trạng thái bàn {soBan} thành {dto.TrangThai}." });
        }

        // 7. DELETE: api/ban-an/{soBan} (Xóa bàn ăn hoặc Vô hiệu hóa)
        [HttpDelete("{soBan}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string soBan, [FromQuery] bool hardDelete = false)
        {
            var banAn = await _context.BanAns
                .Include(b => b.PhieuYeuCaus)
                .FirstOrDefaultAsync(b => b.SoBan == soBan);

            if (banAn == null)
            {
                return NotFound(new { message = $"Không tìm thấy bàn ăn {soBan}" });
            }

            // Kiểm tra xem bàn ăn có phiếu yêu cầu nào chưa thanh toán/đang hoạt động hay không
            var hasActiveBookings = banAn.PhieuYeuCaus.Any(p => 
                p.TrangThai == TrangThaiPhieu.Pending || 
                p.TrangThai == TrangThaiPhieu.Confirmed || 
                p.TrangThai == TrangThaiPhieu.Arrived);

            if (hasActiveBookings)
            {
                return BadRequest(new { message = "Bàn ăn đang có giao dịch hoạt động (Khách đang ăn hoặc đặt trước). Không thể xóa." });
            }

            if (hardDelete)
            {
                // Nếu có lịch sử đặt bàn cũ, EF Core Restrict delete sẽ báo lỗi, nên cần check
                if (banAn.PhieuYeuCaus.Any())
                {
                    return BadRequest(new { message = "Bàn ăn này đã có lịch sử đặt món. Hãy dùng soft delete (vô hiệu hóa) thay vì xóa cứng." });
                }
                _context.BanAns.Remove(banAn);
            }
            else
            {
                banAn.IsActive = false;
                banAn.NgayCapNhat = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = hardDelete ? "Xóa bàn ăn thành công khỏi cơ sở dữ liệu." : "Đã vô hiệu hóa bàn ăn thành công." });
        }
    }
}
