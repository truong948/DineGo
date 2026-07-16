using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DineGo.Models
{
    /// <summary>
    /// Chi Tiết Phiếu Nhập Thực Phẩm - Inventory Receipt Detail entity
    /// </summary>
    public class ChiTietPhieuNhapThucPham
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SoPhieuNhap { get; set; }

        [Required]
        [MaxLength(20)]
        public string MaThucPham { get; set; } = null!;

        public double SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        // Navigation
        [ForeignKey(nameof(SoPhieuNhap))]
        public PhieuNhapThucPham PhieuNhapThucPham { get; set; } = null!;

        [ForeignKey(nameof(MaThucPham))]
        public ThucPham ThucPham { get; set; } = null!;
    }
}
