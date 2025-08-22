using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class CompanyLogoURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Companies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "Companies",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LogoUrl", "PrimaryColor" },
                values: new object[] { null, "#667eea" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LogoUrl", "PrimaryColor" },
                values: new object[] { null, "#667eea" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "Companies");
        }
    }
}
