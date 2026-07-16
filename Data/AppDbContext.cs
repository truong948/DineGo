using DineGo.Models;
using DineGo.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DineGo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ─── DbSets ────────────────────────────────────────────────────────────
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<BanAn> BanAns { get; set; }
        public DbSet<PhieuYeuCau> PhieuYeuCaus { get; set; }
        public DbSet<ChiTietPhieuYeuCau> ChiTietPhieuYeuCaus { get; set; }
        public DbSet<MonAn> MonAns { get; set; }
        public DbSet<NhomMonAn> NhomMonAns { get; set; }
        public DbSet<PhieuThanhToan> PhieuThanhToans { get; set; }
        public DbSet<ThucPham> ThucPhams { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public DbSet<PhieuNhapThucPham> PhieuNhapThucPhams { get; set; }
        public DbSet<ChiTietPhieuNhapThucPham> ChiTietPhieuNhapThucPhams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── KhachHang ──────────────────────────────────────────────────────
            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.ToTable("KhachHang");
                entity.HasKey(e => e.MaKH);
                entity.Property(e => e.HoTen).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DiaChi).HasMaxLength(255);
                entity.Property(e => e.DienThoai).HasMaxLength(20);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("NOW()");
            });

            // ── NhanVien ───────────────────────────────────────────────────────
            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.ToTable("NhanVien");
                entity.HasKey(e => e.MaNV);
                entity.Property(e => e.HoTenNV).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ChucVu).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DiaChi).HasMaxLength(255);
                entity.Property(e => e.DienThoai).HasMaxLength(20);
            });

            // ── BanAn ──────────────────────────────────────────────────────────
            modelBuilder.Entity<BanAn>(entity =>
            {
                entity.ToTable("BanAn");
                entity.HasKey(e => e.SoBan);
                entity.Property(e => e.SoBan).HasMaxLength(20);
                entity.Property(e => e.LoaiBan).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // ── NhomMonAn ──────────────────────────────────────────────────────
            modelBuilder.Entity<NhomMonAn>(entity =>
            {
                entity.ToTable("NhomMonAn");
                entity.HasKey(e => e.MaNhom);
                entity.Property(e => e.MaNhom).HasMaxLength(20);
                entity.Property(e => e.TenNhom).IsRequired().HasMaxLength(100);
            });

            // ── MonAn ──────────────────────────────────────────────────────────
            modelBuilder.Entity<MonAn>(entity =>
            {
                entity.ToTable("MonAn");
                entity.HasKey(e => e.MaMon);
                entity.Property(e => e.MaMon).HasMaxLength(20);
                entity.Property(e => e.TenMon).IsRequired().HasMaxLength(150);
                entity.Property(e => e.DVT).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DonGia).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IsAvailable).HasDefaultValue(true);

                // MonAn -> NhomMonAn: RESTRICT (cannot delete a NhomMonAn if MonAn references it)
                entity.HasOne(e => e.NhomMonAn)
                      .WithMany(n => n.MonAns)
                      .HasForeignKey(e => e.MaNhom)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── PhieuYeuCau ────────────────────────────────────────────────────
            modelBuilder.Entity<PhieuYeuCau>(entity =>
            {
                entity.ToTable("PhieuYeuCau");
                entity.HasKey(e => e.SoPhieuYeuCau);

                entity.Property(e => e.SoBan).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TrangThai)
                      .HasConversion<int>()
                      .HasDefaultValue(TrangThaiPhieu.Pending);
                entity.Property(e => e.NgayYeuCau).HasColumnType("date");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                // ── OPTIMISTIC CONCURRENCY: PostgreSQL xmin system column ──────
                entity.UseXminAsConcurrencyToken();

                // ── ANTI-DOUBLE-BOOKING: Partial Unique Index ─────────────────
                // Prevents same table from being double-booked on the same date/time
                // EXCEPT when TrangThai = 4 (Cancelled) — allows re-booking
                entity.HasIndex(e => new { e.SoBan, e.NgayYeuCau, e.GioDat })
                      .IsUnique()
                      .HasFilter("\"TrangThai\" != 4")
                      .HasDatabaseName("IX_PhieuYeuCau_SoBan_NgayYeuCau_GioDat_NotCancelled");

                // PhieuYeuCau -> KhachHang
                entity.HasOne(e => e.KhachHang)
                      .WithMany(k => k.PhieuYeuCaus)
                      .HasForeignKey(e => e.MaKH)
                      .OnDelete(DeleteBehavior.Restrict);

                // PhieuYeuCau -> BanAn
                entity.HasOne(e => e.BanAn)
                      .WithMany(b => b.PhieuYeuCaus)
                      .HasForeignKey(e => e.SoBan)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── ChiTietPhieuYeuCau ─────────────────────────────────────────────
            modelBuilder.Entity<ChiTietPhieuYeuCau>(entity =>
            {
                entity.ToTable("ChiTietPhieuYeuCau");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MaMon).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DonGiaDat).HasColumnType("decimal(18,2)");

                // ChiTietPhieuYeuCau -> PhieuYeuCau
                entity.HasOne(e => e.PhieuYeuCau)
                      .WithMany(p => p.ChiTietPhieuYeuCaus)
                      .HasForeignKey(e => e.SoPhieuYeuCau)
                      .OnDelete(DeleteBehavior.Cascade);

                // ChiTietPhieuYeuCau -> MonAn: RESTRICT (cannot delete MonAn if referenced)
                entity.HasOne(e => e.MonAn)
                      .WithMany(m => m.ChiTietPhieuYeuCaus)
                      .HasForeignKey(e => e.MaMon)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── PhieuThanhToan ─────────────────────────────────────────────────
            modelBuilder.Entity<PhieuThanhToan>(entity =>
            {
                entity.ToTable("PhieuThanhToan");
                entity.HasKey(e => e.SoPhieuThanhToan);
                entity.Property(e => e.PhuongThucThanhToan).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TongTien).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NgayThanhToan).HasDefaultValueSql("NOW()");

                // ── 1-to-1: PhieuThanhToan <-> PhieuYeuCau ────────────────────
                // UNIQUE enforced on SoPhieuYeuCau to guarantee 1-to-1 integrity
                entity.HasIndex(e => e.SoPhieuYeuCau)
                      .IsUnique()
                      .HasDatabaseName("IX_PhieuThanhToan_SoPhieuYeuCau_Unique");

                entity.HasOne(e => e.PhieuYeuCau)
                      .WithOne(p => p.PhieuThanhToan)
                      .HasForeignKey<PhieuThanhToan>(e => e.SoPhieuYeuCau)
                      .OnDelete(DeleteBehavior.Restrict);

                // PhieuThanhToan -> NhanVien
                entity.HasOne(e => e.NhanVien)
                      .WithMany(n => n.PhieuThanhToans)
                      .HasForeignKey(e => e.MaNV)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── ThucPham ───────────────────────────────────────────────────────
            modelBuilder.Entity<ThucPham>(entity =>
            {
                entity.ToTable("ThucPham");
                entity.HasKey(e => e.MaThucPham);
                entity.Property(e => e.MaThucPham).HasMaxLength(20);
                entity.Property(e => e.TenThucPham).IsRequired().HasMaxLength(150);
                entity.Property(e => e.DVT).IsRequired().HasMaxLength(20);
            });

            // ── NhaCungCap ─────────────────────────────────────────────────────
            modelBuilder.Entity<NhaCungCap>(entity =>
            {
                entity.ToTable("NhaCungCap");
                entity.HasKey(e => e.MaNCC);
                entity.Property(e => e.MaNCC).HasMaxLength(20);
                entity.Property(e => e.TenNCC).IsRequired().HasMaxLength(150);
                entity.Property(e => e.DiaChi).HasMaxLength(255);
                entity.Property(e => e.DienThoai).HasMaxLength(20);
            });

            // ── PhieuNhapThucPham ──────────────────────────────────────────────
            modelBuilder.Entity<PhieuNhapThucPham>(entity =>
            {
                entity.ToTable("PhieuNhapThucPham");
                entity.HasKey(e => e.SoPhieuNhap);
                entity.Property(e => e.NgayNhap).HasDefaultValueSql("NOW()");
                entity.Property(e => e.MaNCC).IsRequired().HasMaxLength(20);

                // PhieuNhapThucPham -> NhanVien
                entity.HasOne(e => e.NhanVien)
                      .WithMany(n => n.PhieuNhapThucPhams)
                      .HasForeignKey(e => e.MaNV)
                      .OnDelete(DeleteBehavior.Restrict);

                // PhieuNhapThucPham -> NhaCungCap
                entity.HasOne(e => e.NhaCungCap)
                      .WithMany(n => n.PhieuNhapThucPhams)
                      .HasForeignKey(e => e.MaNCC)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── ChiTietPhieuNhapThucPham ───────────────────────────────────────
            modelBuilder.Entity<ChiTietPhieuNhapThucPham>(entity =>
            {
                entity.ToTable("ChiTietPhieuNhapThucPham");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MaThucPham).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DonGia).HasColumnType("decimal(18,2)");

                // ChiTietPhieuNhapThucPham -> PhieuNhapThucPham
                entity.HasOne(e => e.PhieuNhapThucPham)
                      .WithMany(p => p.ChiTietPhieuNhapThucPhams)
                      .HasForeignKey(e => e.SoPhieuNhap)
                      .OnDelete(DeleteBehavior.Cascade);

                // ChiTietPhieuNhapThucPham -> ThucPham: RESTRICT
                entity.HasOne(e => e.ThucPham)
                      .WithMany(t => t.ChiTietPhieuNhapThucPhams)
                      .HasForeignKey(e => e.MaThucPham)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
