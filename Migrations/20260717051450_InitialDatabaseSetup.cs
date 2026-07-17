using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIneGo.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabaseSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BanAn",
                columns: table => new
                {
                    SoBan = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LoaiBan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SucChua = table.Column<int>(type: "integer", nullable: false),
                    CoordinateX = table.Column<int>(type: "integer", nullable: false),
                    CoordinateY = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanAn", x => x.SoBan);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKH = table.Column<Guid>(type: "uuid", nullable: false),
                    HoTen = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DienThoai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    MaNCC = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TenNCC = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DiaChi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DienThoai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.MaNCC);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNV = table.Column<Guid>(type: "uuid", nullable: false),
                    HoTenNV = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ChucVu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DiaChi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DienThoai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNV);
                });

            migrationBuilder.CreateTable(
                name: "NhomMonAn",
                columns: table => new
                {
                    MaNhom = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TenNhom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomMonAn", x => x.MaNhom);
                });

            migrationBuilder.CreateTable(
                name: "ThucPham",
                columns: table => new
                {
                    MaThucPham = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TenThucPham = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DVT = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TonKhoHienTai = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThucPham", x => x.MaThucPham);
                });

            migrationBuilder.CreateTable(
                name: "PhieuYeuCau",
                columns: table => new
                {
                    SoPhieuYeuCau = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayYeuCau = table.Column<DateOnly>(type: "date", nullable: false),
                    GioDat = table.Column<TimeSpan>(type: "interval", nullable: false),
                    MaKH = table.Column<Guid>(type: "uuid", nullable: false),
                    SoBan = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuYeuCau", x => x.SoPhieuYeuCau);
                    table.ForeignKey(
                        name: "FK_PhieuYeuCau_BanAn_SoBan",
                        column: x => x.SoBan,
                        principalTable: "BanAn",
                        principalColumn: "SoBan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuYeuCau_KhachHang_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhapThucPham",
                columns: table => new
                {
                    SoPhieuNhap = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    MaNV = table.Column<Guid>(type: "uuid", nullable: false),
                    MaNCC = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhapThucPham", x => x.SoPhieuNhap);
                    table.ForeignKey(
                        name: "FK_PhieuNhapThucPham_NhaCungCap_MaNCC",
                        column: x => x.MaNCC,
                        principalTable: "NhaCungCap",
                        principalColumn: "MaNCC",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuNhapThucPham_NhanVien_MaNV",
                        column: x => x.MaNV,
                        principalTable: "NhanVien",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonAn",
                columns: table => new
                {
                    MaMon = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TenMon = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DVT = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MaNhom = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonAn", x => x.MaMon);
                    table.ForeignKey(
                        name: "FK_MonAn_NhomMonAn_MaNhom",
                        column: x => x.MaNhom,
                        principalTable: "NhomMonAn",
                        principalColumn: "MaNhom",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuThanhToan",
                columns: table => new
                {
                    SoPhieuThanhToan = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    MaNV = table.Column<Guid>(type: "uuid", nullable: false),
                    SoPhieuYeuCau = table.Column<Guid>(type: "uuid", nullable: false),
                    TongTien = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DaThanhToan = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuThanhToan", x => x.SoPhieuThanhToan);
                    table.ForeignKey(
                        name: "FK_PhieuThanhToan_NhanVien_MaNV",
                        column: x => x.MaNV,
                        principalTable: "NhanVien",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThanhToan_PhieuYeuCau_SoPhieuYeuCau",
                        column: x => x.SoPhieuYeuCau,
                        principalTable: "PhieuYeuCau",
                        principalColumn: "SoPhieuYeuCau",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuNhapThucPham",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SoPhieuNhap = table.Column<Guid>(type: "uuid", nullable: false),
                    MaThucPham = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SoLuong = table.Column<double>(type: "double precision", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuNhapThucPham", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhapThucPham_PhieuNhapThucPham_SoPhieuNhap",
                        column: x => x.SoPhieuNhap,
                        principalTable: "PhieuNhapThucPham",
                        principalColumn: "SoPhieuNhap",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhapThucPham_ThucPham_MaThucPham",
                        column: x => x.MaThucPham,
                        principalTable: "ThucPham",
                        principalColumn: "MaThucPham",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuYeuCau",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SoPhieuYeuCau = table.Column<Guid>(type: "uuid", nullable: false),
                    MaMon = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    DonGiaDat = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuYeuCau", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuYeuCau_MonAn_MaMon",
                        column: x => x.MaMon,
                        principalTable: "MonAn",
                        principalColumn: "MaMon",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuYeuCau_PhieuYeuCau_SoPhieuYeuCau",
                        column: x => x.SoPhieuYeuCau,
                        principalTable: "PhieuYeuCau",
                        principalColumn: "SoPhieuYeuCau",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhapThucPham_MaThucPham",
                table: "ChiTietPhieuNhapThucPham",
                column: "MaThucPham");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhapThucPham_SoPhieuNhap",
                table: "ChiTietPhieuNhapThucPham",
                column: "SoPhieuNhap");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuYeuCau_MaMon",
                table: "ChiTietPhieuYeuCau",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuYeuCau_SoPhieuYeuCau",
                table: "ChiTietPhieuYeuCau",
                column: "SoPhieuYeuCau");

            migrationBuilder.CreateIndex(
                name: "IX_MonAn_MaNhom",
                table: "MonAn",
                column: "MaNhom");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhapThucPham_MaNCC",
                table: "PhieuNhapThucPham",
                column: "MaNCC");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhapThucPham_MaNV",
                table: "PhieuNhapThucPham",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhToan_MaNV",
                table: "PhieuThanhToan",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThanhToan_SoPhieuYeuCau_Unique",
                table: "PhieuThanhToan",
                column: "SoPhieuYeuCau",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuYeuCau_MaKH",
                table: "PhieuYeuCau",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuYeuCau_SoBan_NgayYeuCau_GioDat_NotCancelled",
                table: "PhieuYeuCau",
                columns: new[] { "SoBan", "NgayYeuCau", "GioDat" },
                unique: true,
                filter: "\"TrangThai\" != 4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietPhieuNhapThucPham");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuYeuCau");

            migrationBuilder.DropTable(
                name: "PhieuThanhToan");

            migrationBuilder.DropTable(
                name: "PhieuNhapThucPham");

            migrationBuilder.DropTable(
                name: "ThucPham");

            migrationBuilder.DropTable(
                name: "MonAn");

            migrationBuilder.DropTable(
                name: "PhieuYeuCau");

            migrationBuilder.DropTable(
                name: "NhaCungCap");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "NhomMonAn");

            migrationBuilder.DropTable(
                name: "BanAn");

            migrationBuilder.DropTable(
                name: "KhachHang");
        }
    }
}
