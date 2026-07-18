using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIneGo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaForApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "MonAn",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "ChiTietPhieuYeuCau",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiCheBien",
                table: "ChiTietPhieuYeuCau",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhat",
                table: "BanAn",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "BanAn",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiHienTai",
                table: "BanAn",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaNV = table.Column<Guid>(type: "uuid", nullable: false),
                    TenDangNhap = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MatKhauHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaNV);
                    table.ForeignKey(
                        name: "FK_TaiKhoan_NhanVien_MaNV",
                        column: x => x.MaNV,
                        principalTable: "NhanVien",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "MonAn");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "ChiTietPhieuYeuCau");

            migrationBuilder.DropColumn(
                name: "TrangThaiCheBien",
                table: "ChiTietPhieuYeuCau");

            migrationBuilder.DropColumn(
                name: "NgayCapNhat",
                table: "BanAn");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "BanAn");

            migrationBuilder.DropColumn(
                name: "TrangThaiHienTai",
                table: "BanAn");
        }
    }
}
