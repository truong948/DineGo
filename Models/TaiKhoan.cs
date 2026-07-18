using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DineGo.Models
{
    /// <summary>
    /// Tài khoản đăng nhập hệ thống - liên kết 1-1 với Nhân viên
    /// </summary>
    public class TaiKhoan
    {
        [Key]
        public Guid MaNV { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenDangNhap { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string MatKhauHash { get; set; } = null!;

        // Navigation
        [ForeignKey(nameof(MaNV))]
        public NhanVien NhanVien { get; set; } = null!;
    }
}
