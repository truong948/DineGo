namespace DineGo.DTOs
{
    public class NhanVienCreateDto
    {
        public string HoTenNV { get; set; } = null!;
        public string ChucVu { get; set; } = null!; // Admin | Staff | Chef
        public string? DiaChi { get; set; }
        public string? DienThoai { get; set; }
        public string TenDangNhap { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
    }

    public class NhanVienUpdateDto
    {
        public string HoTenNV { get; set; } = null!;
        public string ChucVu { get; set; } = null!;
        public string? DiaChi { get; set; }
        public string? DienThoai { get; set; }
    }

    public class PhanQuyenDto
    {
        public string ChucVuMoi { get; set; } = null!;
    }
}
