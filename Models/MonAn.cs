using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DineGo.Models
{
    /// <summary>
    /// Món Ăn - Meal/Dish entity
    /// </summary>
    public class MonAn
    {
        [Key]
        [MaxLength(20)]
        public string MaMon { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string TenMon { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string DVT { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [Required]
        [MaxLength(20)]
        public string MaNhom { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;

        // Navigation
        [ForeignKey(nameof(MaNhom))]
        public NhomMonAn NhomMonAn { get; set; } = null!;

        public ICollection<ChiTietPhieuYeuCau> ChiTietPhieuYeuCaus { get; set; } = new List<ChiTietPhieuYeuCau>();
    }
}
