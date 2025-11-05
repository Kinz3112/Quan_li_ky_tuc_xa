using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quan_li_ky_tuc_xa.Migrations
{
    /// <inheritdoc />
    public partial class MakeMaTruongPhongNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Phong_MaTruongPhong",
                table: "Phong");

            migrationBuilder.AlterColumn<string>(
                name: "Ten",
                table: "Phong",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MaTruongPhong",
                table: "Phong",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "MaToa",
                table: "Phong",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoaiPhong",
                table: "Phong",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Phong_MaTruongPhong",
                table: "Phong",
                column: "MaTruongPhong",
                unique: true,
                filter: "[MaTruongPhong] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Phong_MaTruongPhong",
                table: "Phong");

            migrationBuilder.AlterColumn<string>(
                name: "Ten",
                table: "Phong",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaTruongPhong",
                table: "Phong",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaToa",
                table: "Phong",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiPhong",
                table: "Phong",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Phong_MaTruongPhong",
                table: "Phong",
                column: "MaTruongPhong",
                unique: true);
        }
    }
}
