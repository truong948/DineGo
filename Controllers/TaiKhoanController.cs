using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DineGo.Data;
using DineGo.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DineGo.Controllers
{
    [ApiController]
    [Route("api/tai-khoan")]
    public class TaiKhoanController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public TaiKhoanController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 1. POST: api/tai-khoan/login (Đăng nhập và sinh JWT Token)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap == request.TenDangNhap);

            if (taiKhoan == null || !BCrypt.Net.BCrypt.Verify(request.MatKhau, taiKhoan.MatKhauHash))
            {
                return BadRequest(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác." });
            }

            var nhanVien = taiKhoan.NhanVien;

            // Tạo các Claims cho JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, nhanVien.MaNV.ToString()),
                new Claim(ClaimTypes.Name, nhanVien.HoTenNV),
                new Claim(ClaimTypes.Role, nhanVien.ChucVu) // Phân quyền Authorize(Roles = "...") dựa trên ChucVu
            };

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "DineGoSuperSecretSecurityKey123456!!!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "180");

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResponse
            {
                Token = jwtToken,
                ExpiresIn = expiryMinutes * 60,
                NhanVien = new NhanVienInfo
                {
                    MaNV = nhanVien.MaNV,
                    HoTenNV = nhanVien.HoTenNV,
                    ChucVu = nhanVien.ChucVu
                }
            });
        }

        // 2. PUT: api/tai-khoan/{maNV}/phan-quyen (Phân quyền tài khoản)
        [HttpPut("{maNV}/phan-quyen")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PhanQuyen(Guid maNV, [FromBody] PhanQuyenDto dto)
        {
            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(n => n.MaNV == maNV);
            if (nhanVien == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên." });
            }

            var validRoles = new[] { "Admin", "Staff", "Chef" };
            if (!validRoles.Contains(dto.ChucVuMoi))
            {
                return BadRequest(new { message = "Chức vụ mới không hợp lệ. Chỉ chấp nhận: Admin, Staff, Chef." });
            }

            nhanVien.ChucVu = dto.ChucVuMoi;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã phân quyền thành công tài khoản nhân viên sang vai trò '{dto.ChucVuMoi}'." });
        }
    }
}
