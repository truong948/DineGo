using System.ComponentModel.DataAnnotations;

namespace DineGo.Models
{
    /// <summary>
    /// Thực Phẩm - Inventory Item entity
    /// </summary>
    public class ThucPham
    {
        [Key]
        [MaxLength(20)]
        public string MaThucPham { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string TenThucPham { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string DVT { get; set; } = null!;

        public double TonKhoHienTai { get; set; }

        // Navigation
        public ICollection<ChiTietPhieuNhapThucPham> ChiTietPhieuNhapThucPhams { get; set; } = new List<ChiTietPhieuNhapThucPham>();
    }
}
