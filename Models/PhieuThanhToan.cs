using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DineGo.Models
{
    /// <summary>
    /// Phiếu Thanh Toán - Invoice/Bill entity
    /// Has a 1-to-1 unique relationship with PhieuYeuCau.
    /// </summary>
    public class PhieuThanhToan
    {
        [Key]
        public Guid SoPhieuThanhToan { get; set; } = Guid.NewGuid();

        public DateTime NgayThanhToan { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid MaNV { get; set; }

        /// <summary>FK to PhieuYeuCau — enforced as UNIQUE by Fluent API</summary>
        [Required]
        public Guid SoPhieuYeuCau { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [Required]
        [MaxLength(50)]
        public string PhuongThucThanhToan { get; set; } = null!;

        public bool DaThanhToan { get; set; } = false;

        // Navigation
        [ForeignKey(nameof(MaNV))]
        public NhanVien NhanVien { get; set; } = null!;

        [ForeignKey(nameof(SoPhieuYeuCau))]
        public PhieuYeuCau PhieuYeuCau { get; set; } = null!;
    }
}
