using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessNatureTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyNature",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "BusinessNatureId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessNatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessNatures", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BusinessNatures",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayName", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8622), "Digital marketing and content creation services", "Digital Marketing Agency", true, "DigitalMarketingAgency" },
                    { 2, new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8626), "Automotive industry and vehicle-related services", "Automobile", true, "Automobile" },
                    { 3, new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8690), "General business operations and services", "General", true, "General" },
                    { 4, new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8692), "Traditional retail and online commerce operations", "Retail & E-commerce", true, "Retail" },
                    { 5, new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8694), "Manufacturing and production businesses", "Manufacturing", true, "Manufacturing" }
                });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BusinessNatureId", "CreatedAt" },
                values: new object[] { 3, new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8879) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BusinessNatureId", "CreatedAt" },
                values: new object[] { 1, new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8882) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(9728), new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(9729) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(9732), new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(9732) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(9735), new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(9735) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(9738), new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(9738) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(9625));

            migrationBuilder.CreateIndex(
                name: "IX_Companies_BusinessNatureId",
                table: "Companies",
                column: "BusinessNatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_BusinessNatures_BusinessNatureId",
                table: "Companies",
                column: "BusinessNatureId",
                principalTable: "BusinessNatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_BusinessNatures_BusinessNatureId",
                table: "Companies");

            migrationBuilder.DropTable(
                name: "BusinessNatures");

            migrationBuilder.DropIndex(
                name: "IX_Companies_BusinessNatureId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BusinessNatureId",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "CompanyNature",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompanyNature", "CreatedAt" },
                values: new object[] { "General", new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(3546) });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CompanyNature", "CreatedAt" },
                values: new object[] { "Digital Marketing", new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(3549) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4754), new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4755) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4758), new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4758) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4761), new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4762) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4764), new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4764) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4580));
        }
    }
}
