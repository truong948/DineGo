using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DineGo.Models
{
    /// <summary>
    /// Phiếu Nhập Thực Phẩm - Inventory Receipt entity
    /// </summary>
    public class PhieuNhapThucPham
    {
        [Key]
        public Guid SoPhieuNhap { get; set; } = Guid.NewGuid();

        public DateTime NgayNhap { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid MaNV { get; set; }

        [Required]
        [MaxLength(20)]
        public string MaNCC { get; set; } = null!;

        // Navigation
        [ForeignKey(nameof(MaNV))]
        public NhanVien NhanVien { get; set; } = null!;

        [ForeignKey(nameof(MaNCC))]
        public NhaCungCap NhaCungCap { get; set; } = null!;

        public ICollection<ChiTietPhieuNhapThucPham> ChiTietPhieuNhapThucPhams { get; set; } = new List<ChiTietPhieuNhapThucPham>();
    }
}
