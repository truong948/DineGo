using System.ComponentModel.DataAnnotations;

namespace DineGo.Models
{
    /// <summary>
    /// Nhóm Món Ăn - Meal Category entity
    /// </summary>
    public class NhomMonAn
    {
        [Key]
        [MaxLength(20)]
        public string MaNhom { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string TenNhom { get; set; } = null!;

        // Navigation
        public ICollection<MonAn> MonAns { get; set; } = new List<MonAn>();
    }
}
