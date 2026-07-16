using System.ComponentModel.DataAnnotations;

namespace DineGo.Models
{
    /// <summary>
    /// Nhà Cung Cấp - Supplier entity
    /// </summary>
    public class NhaCungCap
    {
        [Key]
        [MaxLength(20)]
        public string MaNCC { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string TenNCC { get; set; } = null!;

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(20)]
        public string? DienThoai { get; set; }

        // Navigation
        public ICollection<PhieuNhapThucPham> PhieuNhapThucPhams { get; set; } = new List<PhieuNhapThucPham>();
    }
}
