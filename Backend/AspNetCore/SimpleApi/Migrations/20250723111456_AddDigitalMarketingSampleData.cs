using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalMarketingSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(3546));

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "CompanyNature", "ContactPerson", "CreatedAt", "Email", "IsActive", "Name", "Phone" },
                values: new object[] { 2, "456 Marketing Ave", "Digital Marketing", "Marketing Manager", new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(3549), "admin@digitalmarketingpro.com", true, "Digital Marketing Pro", "555-0002" });

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
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4580));

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CompanyId", "CreatedDate", "IsActive", "ProjectDescription", "ProjectName", "ProjectStatus", "UpdatedDate" },
                values: new object[,]
                {
                    { 3, 2, new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4761), true, "Comprehensive social media marketing strategy", "Social Media Marketing", 1, new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4762) },
                    { 4, 2, new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4764), true, "Blog and content creation for digital presence", "Content Creation Campaign", 3, new DateTime(2025, 7, 23, 11, 14, 56, 177, DateTimeKind.Utc).AddTicks(4764) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(1513));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(3164), new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(3165) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(3169), new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(3169) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(2978));
        }
    }
}
