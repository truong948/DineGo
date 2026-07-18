using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DineGo.Models.Enums;

namespace DineGo.Models
{
    /// <summary>
    /// Chi Tiết Phiếu Yêu Cầu - Booking Detail / Pre-ordered Meals entity
    /// </summary>
    public class ChiTietPhieuYeuCau
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SoPhieuYeuCau { get; set; }

        [Required]
        [MaxLength(20)]
        public string MaMon { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaDat { get; set; }

        public TrangThaiCheBien TrangThaiCheBien { get; set; } = TrangThaiCheBien.ChoCungUng;

        [MaxLength(255)]
        public string? GhiChu { get; set; }

        // Navigation
        [ForeignKey(nameof(SoPhieuYeuCau))]
        public PhieuYeuCau PhieuYeuCau { get; set; } = null!;

        [ForeignKey(nameof(MaMon))]
        public MonAn MonAn { get; set; } = null!;
    }
}
