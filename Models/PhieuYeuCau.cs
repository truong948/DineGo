using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DineGo.Models.Enums;

namespace DineGo.Models
{
    /// <summary>
    /// Phiếu Yêu Cầu - Booking/Reservation entity
    /// Uses PostgreSQL xmin as optimistic concurrency token (configured via Fluent API).
    /// </summary>
    public class PhieuYeuCau
    {
        [Key]
        public Guid SoPhieuYeuCau { get; set; } = Guid.NewGuid();

        /// <summary>Date-only booking date</summary>
        [Column(TypeName = "date")]
        public DateOnly NgayYeuCau { get; set; }

        public TimeSpan GioDat { get; set; }

        [Required]
        public Guid MaKH { get; set; }

        [Required]
        [MaxLength(20)]
        public string SoBan { get; set; } = null!;

        public TrangThaiPhieu TrangThai { get; set; } = TrangThaiPhieu.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(MaKH))]
        public KhachHang KhachHang { get; set; } = null!;

        [ForeignKey(nameof(SoBan))]
        public BanAn BanAn { get; set; } = null!;

        public ICollection<ChiTietPhieuYeuCau> ChiTietPhieuYeuCaus { get; set; } = new List<ChiTietPhieuYeuCau>();

        public PhieuThanhToan? PhieuThanhToan { get; set; }
    }
}
