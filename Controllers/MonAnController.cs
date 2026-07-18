using DineGo.Data;
using DineGo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DineGo.Controllers
{
    [ApiController]
    [Route("api")]
    public class MonAnController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MonAnController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/mon-an (Tìm kiếm, lọc danh sách món ăn)
        [HttpGet("mon-an")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? maNhom,
            [FromQuery] bool? isAvailable,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var query = _context.MonAns.Include(m => m.NhomMonAn).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(m => m.TenMon.ToLower().Contains(lowerSearch) || m.MaMon.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrWhiteSpace(maNhom))
            {
                query = query.Where(m => m.MaNhom == maNhom);
            }

            if (isAvailable.HasValue)
            {
                query = query.Where(m => m.IsAvailable == isAvailable.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(m => m.DonGia >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(m => m.DonGia <= maxPrice.Value);
            }

            var monAns = await query
                .OrderBy(m => m.MaMon)
                .Select(m => new
                {
                    m.MaMon,
                    m.TenMon,
                    m.DVT,
                    m.DonGia,
                    m.MaNhom,
                    TenNhom = m.NhomMonAn.TenNhom,
                    m.HinhAnh,
                    m.IsAvailable
                })
                .ToListAsync();

            return Ok(monAns);
        }

        // 2. GET: api/mon-an/{maMon}
        [HttpGet("mon-an/{maMon}")]
        public async Task<IActionResult> GetById(string maMon)
        {
            var monAn = await _context.MonAns
                .Include(m => m.NhomMonAn)
                .Select(m => new
                {
                    m.MaMon,
                    m.TenMon,
                    m.DVT,
                    m.DonGia,
                    m.MaNhom,
                    TenNhom = m.NhomMonAn.TenNhom,
                    m.HinhAnh,
                    m.IsAvailable
                })
                .FirstOrDefaultAsync(m => m.MaMon == maMon);

            if (monAn == null)
            {
                return NotFound(new { message = $"Không tìm thấy món ăn {maMon}" });
            }

            return Ok(monAn);
        }

        // 3. GET: api/danh-muc/{maNhom}/mon-an (Xem thực đơn theo danh mục)
        [HttpGet("danh-muc/{maNhom}/mon-an")]
        public async Task<IActionResult> GetByDanhMuc(string maNhom)
        {
            if (!await _context.NhomMonAns.AnyAsync(n => n.MaNhom == maNhom))
            {
                return NotFound(new { message = $"Không tìm thấy nhóm món ăn {maNhom}" });
            }

            var monAns = await _context.MonAns
                .Where(m => m.MaNhom == maNhom)
                .OrderBy(m => m.TenMon)
                .Select(m => new
                {
                    m.MaMon,
                    m.TenMon,
                    m.DVT,
                    m.DonGia,
                    m.HinhAnh,
                    m.IsAvailable
                })
                .ToListAsync();

            return Ok(monAns);
        }

        // 4. POST: api/mon-an (Thêm món ăn mới)
        [HttpPost("mon-an")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] MonAn model)
        {
            if (await _context.MonAns.AnyAsync(m => m.MaMon == model.MaMon))
            {
                return BadRequest(new { message = $"Mã món ăn {model.MaMon} đã tồn tại." });
            }

            if (!await _context.NhomMonAns.AnyAsync(n => n.MaNhom == model.MaNhom))
            {
                return BadRequest(new { message = $"Không tìm thấy nhóm món ăn {model.MaNhom}." });
            }

            // Xóa navigation property để tránh EF Core tự thêm mới NhomMonAn
            model.NhomMonAn = null!;

            _context.MonAns.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { maMon = model.MaMon }, model);
        }

        // 5. PUT: api/mon-an/{maMon} (Sửa món ăn & Cập nhật giá)
        [HttpPut("mon-an/{maMon}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string maMon, [FromBody] MonAn model)
        {
            var monAn = await _context.MonAns.FirstOrDefaultAsync(m => m.MaMon == maMon);
            if (monAn == null)
            {
                return NotFound(new { message = $"Không tìm thấy món ăn {maMon}" });
            }

            if (!await _context.NhomMonAns.AnyAsync(n => n.MaNhom == model.MaNhom))
            {
                return BadRequest(new { message = $"Không tìm thấy nhóm món ăn {model.MaNhom}." });
            }

            monAn.TenMon = model.TenMon;
            monAn.DVT = model.DVT;
            monAn.DonGia = model.DonGia;
            monAn.MaNhom = model.MaNhom;
            monAn.HinhAnh = model.HinhAnh;
            monAn.IsAvailable = model.IsAvailable;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thông tin món ăn thành công." });
        }

        // 6. PATCH: api/mon-an/{maMon}/availability (Cập nhật nhanh trạng thái còn/hết hàng)
        [HttpPatch("mon-an/{maMon}/availability")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateAvailability(string maMon, [FromBody] bool isAvailable)
        {
            var monAn = await _context.MonAns.FirstOrDefaultAsync(m => m.MaMon == maMon);
            if (monAn == null)
            {
                return NotFound(new { message = $"Không tìm thấy món ăn {maMon}" });
            }

            monAn.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã cập nhật trạng thái món {maMon} thành {(isAvailable ? "Còn hàng" : "Hết hàng")}." });
        }

        // 7. DELETE: api/mon-an/{maMon}
        [HttpDelete("mon-an/{maMon}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string maMon)
        {
            var monAn = await _context.MonAns
                .Include(m => m.ChiTietPhieuYeuCaus)
                .FirstOrDefaultAsync(m => m.MaMon == maMon);

            if (monAn == null)
            {
                return NotFound(new { message = $"Không tìm thấy món ăn {maMon}" });
            }

            // Nếu món ăn đã có trong lịch sử đặt món, thực hiện Soft Delete để đảm bảo toàn vẹn dữ liệu
            if (monAn.ChiTietPhieuYeuCaus.Any())
            {
                monAn.IsAvailable = false;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Món ăn đã có lịch sử đặt bàn. Đã thực hiện ngưng kinh doanh món ăn (Soft Delete) thay vì xóa vật lý." });
            }

            _context.MonAns.Remove(monAn);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa món ăn thành công khỏi cơ sở dữ liệu." });
        }
    }
}
