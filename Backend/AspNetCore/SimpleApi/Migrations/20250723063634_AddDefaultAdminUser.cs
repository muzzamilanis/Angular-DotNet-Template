using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 6, 36, 33, 303, DateTimeKind.Utc).AddTicks(7075));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CompanyId", "CreatedAt", "Email", "IsActive", "IsSuperAdmin", "Name", "PasswordHash" },
                values: new object[] { 1, 1, new DateTime(2025, 7, 23, 6, 36, 33, 303, DateTimeKind.Utc).AddTicks(8095), "admin@aspnetboilerplate.com", true, true, "System Administrator", "+/s4bv6mfoFvLdoKjJSpjrIDdXrrs/VfGDdVoZLURGc=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 21, 16, 34, 54, 752, DateTimeKind.Utc).AddTicks(6516));
        }
    }
}
