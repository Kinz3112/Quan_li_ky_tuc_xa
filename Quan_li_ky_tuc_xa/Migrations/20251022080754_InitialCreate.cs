using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quan_li_ky_tuc_xa.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    MaDichVu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NhaCungCap = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVu", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Loai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "BanQuanLi",
                columns: table => new
                {
                    MaQL = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaGiamDoc = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanQuanLi", x => x.MaQL);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sdt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaQL = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NhanVien_BanQuanLi_MaQL",
                        column: x => x.MaQL,
                        principalTable: "BanQuanLi",
                        principalColumn: "MaQL",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Toa",
                columns: table => new
                {
                    MaToa = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaNhanVienQuanLi = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toa", x => x.MaToa);
                    table.ForeignKey(
                        name: "FK_Toa_NhanVien_MaNhanVienQuanLi",
                        column: x => x.MaNhanVienQuanLi,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHoaDon = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TongTien = table.Column<float>(type: "real", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MaHopDong = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.MaHoaDon);
                });

            migrationBuilder.CreateTable(
                name: "HopDong",
                columns: table => new
                {
                    MaHopDong = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaNhanVienQuanLi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaSinhVien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoaiHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaDichVu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaPhong = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDong", x => x.MaHopDong);
                    table.ForeignKey(
                        name: "FK_HopDong_DichVu_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVu",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HopDong_NhanVien_MaNhanVienQuanLi",
                        column: x => x.MaNhanVienQuanLi,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phong",
                columns: table => new
                {
                    MaPhong = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaToa = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoaiPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoNguoi = table.Column<int>(type: "int", nullable: false),
                    MaTruongPhong = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phong", x => x.MaPhong);
                    table.ForeignKey(
                        name: "FK_Phong_Toa_MaToa",
                        column: x => x.MaToa,
                        principalTable: "Toa",
                        principalColumn: "MaToa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SinhVien",
                columns: table => new
                {
                    MaSinhVien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HovaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sdt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaPhong = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinhVien", x => x.MaSinhVien);
                    table.ForeignKey(
                        name: "FK_SinhVien_Phong_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phong",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BanQuanLi_MaGiamDoc",
                table: "BanQuanLi",
                column: "MaGiamDoc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaHopDong",
                table: "HoaDon",
                column: "MaHopDong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaDichVu",
                table: "HopDong",
                column: "MaDichVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaNhanVienQuanLi",
                table: "HopDong",
                column: "MaNhanVienQuanLi");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaPhong",
                table: "HopDong",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaSinhVien",
                table: "HopDong",
                column: "MaSinhVien");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaQL",
                table: "NhanVien",
                column: "MaQL");

            migrationBuilder.CreateIndex(
                name: "IX_Phong_MaToa",
                table: "Phong",
                column: "MaToa");

            migrationBuilder.CreateIndex(
                name: "IX_Phong_MaTruongPhong",
                table: "Phong",
                column: "MaTruongPhong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SinhVien_MaPhong",
                table: "SinhVien",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_Toa_MaNhanVienQuanLi",
                table: "Toa",
                column: "MaNhanVienQuanLi",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BanQuanLi_NhanVien_MaGiamDoc",
                table: "BanQuanLi",
                column: "MaGiamDoc",
                principalTable: "NhanVien",
                principalColumn: "MaNhanVien",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_HopDong_MaHopDong",
                table: "HoaDon",
                column: "MaHopDong",
                principalTable: "HopDong",
                principalColumn: "MaHopDong",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HopDong_Phong_MaPhong",
                table: "HopDong",
                column: "MaPhong",
                principalTable: "Phong",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HopDong_SinhVien_MaSinhVien",
                table: "HopDong",
                column: "MaSinhVien",
                principalTable: "SinhVien",
                principalColumn: "MaSinhVien",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phong_SinhVien_MaTruongPhong",
                table: "Phong",
                column: "MaTruongPhong",
                principalTable: "SinhVien",
                principalColumn: "MaSinhVien",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BanQuanLi_NhanVien_MaGiamDoc",
                table: "BanQuanLi");

            migrationBuilder.DropForeignKey(
                name: "FK_Toa_NhanVien_MaNhanVienQuanLi",
                table: "Toa");

            migrationBuilder.DropForeignKey(
                name: "FK_SinhVien_Phong_MaPhong",
                table: "SinhVien");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "HopDong");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "BanQuanLi");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropTable(
                name: "SinhVien");

            migrationBuilder.DropTable(
                name: "Toa");
        }
    }
}
