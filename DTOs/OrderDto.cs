using DineGo.Models.Enums;

namespace DineGo.DTOs
{
    public class CheckInRequestDto
    {
        public string SoBan { get; set; } = null!;
        public string HoTenKhachHang { get; set; } = "Khách vãng lai";
        public string? DienThoai { get; set; }
    }

    public class OrderItemDto
    {
        public string MaMon { get; set; } = null!;
        public int SoLuong { get; set; }
        public string? GhiChu { get; set; }
    }

    public class UpdateOrderItemStatusDto
    {
        public TrangThaiCheBien TrangThaiCheBien { get; set; }
    }

    public class ChuyenBanRequestDto
    {
        public string SoBanMoi { get; set; } = null!;
    }

    public class GopBanRequestDto
    {
        public Guid SoPhieuNguon { get; set; }
        public Guid SoPhieuDich { get; set; }
    }

    public class PhieuYeuCauDto
    {
        public Guid SoPhieuYeuCau { get; set; }
        public string SoBan { get; set; } = null!;
        public string TenKhachHang { get; set; } = null!;
        public DateOnly NgayYeuCau { get; set; }
        public TimeSpan GioDat { get; set; }
        public TrangThaiPhieu TrangThai { get; set; }
        public List<ChiTietOrderDto> ChiTietMonAns { get; set; } = new();
    }

    public class ChiTietOrderDto
    {
        public Guid Id { get; set; }
        public string MaMon { get; set; } = null!;
        public string TenMon { get; set; } = null!;
        public int SoLuong { get; set; }
        public decimal DonGiaDat { get; set; }
        public decimal ThanhTien => SoLuong * DonGiaDat;
        public TrangThaiCheBien TrangThaiCheBien { get; set; }
        public string? GhiChu { get; set; }
    }
}
