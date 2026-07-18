using DineGo.Models.Enums;

namespace DineGo.DTOs
{
    public class BanAnCreateDto
    {
        public string SoBan { get; set; } = null!;
        public string LoaiBan { get; set; } = null!;
        public int SucChua { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
    }

    public class BanAnUpdateDto
    {
        public string LoaiBan { get; set; } = null!;
        public int SucChua { get; set; }
        public bool IsActive { get; set; }
    }

    public class BanAnCoordinateUpdateDto
    {
        public string SoBan { get; set; } = null!;
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
    }

    public class BanAnStatusUpdateDto
    {
        public TrangThaiBan TrangThai { get; set; }
    }
}
