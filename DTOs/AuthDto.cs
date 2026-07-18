namespace DineGo.DTOs
{
    public class LoginRequest
    {
        public string TenDangNhap { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public int ExpiresIn { get; set; }
        public NhanVienInfo NhanVien { get; set; } = null!;
    }

    public class NhanVienInfo
    {
        public Guid MaNV { get; set; }
        public string HoTenNV { get; set; } = null!;
        public string ChucVu { get; set; } = null!;
    }
}
