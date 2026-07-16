using System.ComponentModel.DataAnnotations;

namespace DineGo.Models
{
    /// <summary>
    /// Khách Hàng - Customer entity
    /// </summary>
    public class KhachHang
    {
        [Key]
        public Guid MaKH { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string HoTen { get; set; } = null!;

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(20)]
        public string? DienThoai { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<PhieuYeuCau> PhieuYeuCaus { get; set; } = new List<PhieuYeuCau>();
    }
}
