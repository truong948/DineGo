namespace DineGo.DTOs
{
    public class TamTinhResponseDto
    {
        public Guid SoPhieuYeuCau { get; set; }
        public string SoBan { get; set; } = null!;
        public string TenKhachHang { get; set; } = null!;
        public DateOnly NgayYeuCau { get; set; }
        public TimeSpan GioVao { get; set; }
        public List<ChiTietOrderDto> ChiTietMonAns { get; set; } = new();
        public decimal TienMon { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThueVAT { get; set; }
        public decimal TongTienThanhToan { get; set; }
    }

    public class ThanhToanRequestDto
    {
        public Guid SoPhieuYeuCau { get; set; }
        public Guid MaNV { get; set; }
        public string PhuongThucThanhToan { get; set; } = "Tiền mặt";
        public decimal GiamGiaPhanTram { get; set; } = 0;
        public decimal ThuePhanTram { get; set; } = 10;
    }

    public class ThanhToanResponseDto
    {
        public Guid SoPhieuThanhToan { get; set; }
        public Guid SoPhieuYeuCau { get; set; }
        public string SoBan { get; set; } = null!;
        public string TenNhanVien { get; set; } = null!;
        public string TenKhachHang { get; set; } = null!;
        public DateTime NgayThanhToan { get; set; }
        public decimal TongTien { get; set; }
        public string PhuongThucThanhToan { get; set; } = null!;
        public bool DaThanhToan { get; set; }
    }

    public class BieuDoDoanhThuDto
    {
        public string ThoiGian { get; set; } = null!;
        public decimal DoanhThu { get; set; }
        public int SoHoaDon { get; set; }
    }

    public class ThongKeDoanhThuResponseDto
    {
        public decimal TongDoanhThu { get; set; }
        public int TongSoHoaDon { get; set; }
        public decimal DoanhThuTrungBinhHoaDon => TongSoHoaDon > 0 ? TongDoanhThu / TongSoHoaDon : 0;
        public List<BieuDoDoanhThuDto> BieuDoDoanhThu { get; set; } = new();
    }
}
