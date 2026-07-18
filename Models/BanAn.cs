using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DineGo.Models.Enums;

namespace DineGo.Models
{
    /// <summary>
    /// Bàn Ăn - Restaurant Table entity
    /// SoBan is the natural PK (e.g., "B10", "VIP01")
    /// </summary>
    public class BanAn
    {
        [Key]
        [MaxLength(20)]
        public string SoBan { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LoaiBan { get; set; } = null!;

        /// <summary>Capacity: 1-100</summary>
        [Range(1, 100)]
        public int SucChua { get; set; }

        public int CoordinateX { get; set; }

        public int CoordinateY { get; set; }

        public bool IsActive { get; set; } = true;

        public TrangThaiBan TrangThaiHienTai { get; set; } = TrangThaiBan.Trong;

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<PhieuYeuCau> PhieuYeuCaus { get; set; } = new List<PhieuYeuCau>();
    }
}
