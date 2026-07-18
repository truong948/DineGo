using DineGo.Data;
using DineGo.DTOs;
using DineGo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DineGo.Controllers
{
    [ApiController]
    [Route("api/nhan-vien")]
    [Authorize(Roles = "Admin")]
    public class NhanVienController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NhanVienController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/nhan-vien (Lấy danh sách nhân viên)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var nhanViens = await _context.NhanViens
                .Include(n => n.TaiKhoan)
                .OrderBy(n => n.HoTenNV)
                .Select(n => new
                {
                    n.MaNV,
                    n.HoTenNV,
                    n.ChucVu,
                    n.DiaChi,
                    n.DienThoai,
                    TenDangNhap = n.TaiKhoan != null ? n.TaiKhoan.TenDangNhap : null
                })
                .ToListAsync();

            return Ok(nhanViens);
        }

        // 2. GET: api/nhan-vien/{maNV}
        [HttpGet("{maNV}")]
        public async Task<IActionResult> GetById(Guid maNV)
        {
            var nhanVien = await _context.NhanViens
                .Include(n => n.TaiKhoan)
                .Select(n => new
                {
                    n.MaNV,
                    n.HoTenNV,
                    n.ChucVu,
                    n.DiaChi,
                    n.DienThoai,
                    TenDangNhap = n.TaiKhoan != null ? n.TaiKhoan.TenDangNhap : null
                })
                .FirstOrDefaultAsync(n => n.MaNV == maNV);

            if (nhanVien == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên." });
            }

            return Ok(nhanVien);
        }

        // 3. POST: api/nhan-vien (Tạo nhân viên & Đồng thời tạo tài khoản liên kết)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NhanVienCreateDto dto)
        {
            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == dto.TenDangNhap))
            {
                return BadRequest(new { message = $"Tên đăng nhập '{dto.TenDangNhap}' đã tồn tại trong hệ thống." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var maNV = Guid.NewGuid();

                // 1. Tạo nhân viên
                var nhanVien = new NhanVien
                {
                    MaNV = maNV,
                    HoTenNV = dto.HoTenNV,
                    ChucVu = dto.ChucVu, // Admin | Staff | Chef
                    DiaChi = dto.DiaChi,
                    DienThoai = dto.DienThoai
                };
                _context.NhanViens.Add(nhanVien);

                // 2. Tạo tài khoản đăng nhập (Băm mật khẩu bằng BCrypt)
                var matKhauHash = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau);
                var taiKhoan = new TaiKhoan
                {
                    MaNV = maNV,
                    TenDangNhap = dto.TenDangNhap,
                    MatKhauHash = matKhauHash
                };
                _context.TaiKhoans.Add(taiKhoan);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { maNV = nhanVien.MaNV }, new
                {
                    nhanVien.MaNV,
                    nhanVien.HoTenNV,
                    nhanVien.ChucVu,
                    nhanVien.DiaChi,
                    nhanVien.DienThoai,
                    taiKhoan.TenDangNhap
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo nhân viên và tài khoản.", error = ex.Message });
            }
        }

        // 4. PUT: api/nhan-vien/{maNV} (Sửa thông tin cá nhân nhân viên)
        [HttpPut("{maNV}")]
        public async Task<IActionResult> Update(Guid maNV, [FromBody] NhanVienUpdateDto dto)
        {
            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(n => n.MaNV == maNV);
            if (nhanVien == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên cần sửa." });
            }

            nhanVien.HoTenNV = dto.HoTenNV;
            nhanVien.ChucVu = dto.ChucVu;
            nhanVien.DiaChi = dto.DiaChi;
            nhanVien.DienThoai = dto.DienThoai;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thông tin nhân viên thành công." });
        }

        // 5. DELETE: api/nhan-vien/{maNV} (Xóa nhân viên - Tự động xóa Tài khoản qua Cascade Delete)
        [HttpDelete("{maNV}")]
        public async Task<IActionResult> Delete(Guid maNV)
        {
            var nhanVien = await _context.NhanViens
                .Include(n => n.PhieuThanhToans)
                .Include(n => n.PhieuNhapThucPhams)
                .FirstOrDefaultAsync(n => n.MaNV == maNV);

            if (nhanVien == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên cần xóa." });
            }

            // Nếu nhân viên đã lập hóa đơn hoặc phiếu nhập, không được xóa vật lý để đảm bảo toàn vẹn dữ liệu
            if (nhanVien.PhieuThanhToans.Any() || nhanVien.PhieuNhapThucPhams.Any())
            {
                // Thay vì xóa cứng, ta có thể vô hiệu hóa tài khoản
                var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaNV == maNV);
                if (taiKhoan != null)
                {
                    _context.TaiKhoans.Remove(taiKhoan); // Xóa tài khoản đăng nhập để không cho đăng nhập nữa
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Nhân viên đã có lịch sử lập hóa đơn/nhập kho. Đã xóa tài khoản đăng nhập để vô hiệu hóa quyền truy cập, giữ lại hồ sơ nhân sự." });
                }
                return BadRequest(new { message = "Nhân viên đã có dữ liệu giao dịch và tài khoản đã bị vô hiệu hóa từ trước." });
            }

            _context.NhanViens.Remove(nhanVien); // Xóa nhân viên, cascade sẽ tự xóa TaiKhoan liên kết
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa hoàn toàn thông tin nhân viên và tài khoản liên kết." });
        }
    }
}
