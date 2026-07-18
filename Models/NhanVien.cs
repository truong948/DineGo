using System.ComponentModel.DataAnnotations;

namespace DineGo.Models
{
    /// <summary>
    /// Nhân Viên - Employee entity
    /// ChucVu valid values: "Admin", "Staff", "Chef"
    /// </summary>
    public class NhanVien
    {
        [Key]
        public Guid MaNV { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string HoTenNV { get; set; } = null!;

        /// <summary>Admin | Staff | Chef</summary>
        [Required]
        [MaxLength(50)]
        public string ChucVu { get; set; } = null!;

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(20)]
        public string? DienThoai { get; set; }

        // Navigation
        public TaiKhoan? TaiKhoan { get; set; }
        public ICollection<PhieuThanhToan> PhieuThanhToans { get; set; } = new List<PhieuThanhToan>();
        public ICollection<PhieuNhapThucPham> PhieuNhapThucPhams { get; set; } = new List<PhieuNhapThucPham>();
    }
}
