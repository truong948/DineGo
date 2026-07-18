using DineGo.Data;
using DineGo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DineGo.Controllers
{
    [ApiController]
    [Route("api/danh-muc")]
    public class DanhMucController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DanhMucController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/danh-muc (Lấy danh sách nhóm món ăn)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var danhMucs = await _context.NhomMonAns
                .OrderBy(d => d.TenNhom)
                .Select(d => new
                {
                    d.MaNhom,
                    d.TenNhom
                })
                .ToListAsync();

            return Ok(danhMucs);
        }

        // 2. GET: api/danh-muc/{maNhom}
        [HttpGet("{maNhom}")]
        public async Task<IActionResult> GetById(string maNhom)
        {
            var danhMuc = await _context.NhomMonAns
                .Select(d => new
                {
                    d.MaNhom,
                    d.TenNhom
                })
                .FirstOrDefaultAsync(d => d.MaNhom == maNhom);

            if (danhMuc == null)
            {
                return NotFound(new { message = $"Không tìm thấy nhóm món ăn {maNhom}" });
            }

            return Ok(danhMuc);
        }

        // 3. POST: api/danh-muc
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] NhomMonAn model)
        {
            if (await _context.NhomMonAns.AnyAsync(n => n.MaNhom == model.MaNhom))
            {
                return BadRequest(new { message = $"Mã danh mục {model.MaNhom} đã tồn tại." });
            }

            _context.NhomMonAns.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { maNhom = model.MaNhom }, model);
        }

        // 4. PUT: api/danh-muc/{maNhom}
        [HttpPut("{maNhom}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string maNhom, [FromBody] NhomMonAn model)
        {
            var danhMuc = await _context.NhomMonAns.FirstOrDefaultAsync(d => d.MaNhom == maNhom);
            if (danhMuc == null)
            {
                return NotFound(new { message = $"Không tìm thấy danh mục {maNhom}" });
            }

            danhMuc.TenNhom = model.TenNhom;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật tên danh mục thành công." });
        }

        // 5. DELETE: api/danh-muc/{maNhom}
        [HttpDelete("{maNhom}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string maNhom)
        {
            var danhMuc = await _context.NhomMonAns
                .Include(d => d.MonAns)
                .FirstOrDefaultAsync(d => d.MaNhom == maNhom);

            if (danhMuc == null)
            {
                return NotFound(new { message = $"Không tìm thấy danh mục {maNhom}" });
            }

            if (danhMuc.MonAns.Any())
            {
                return BadRequest(new { message = "Danh mục này đang chứa món ăn, không thể xóa." });
            }

            _context.NhomMonAns.Remove(danhMuc);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa danh mục món ăn thành công." });
        }
    }
}
