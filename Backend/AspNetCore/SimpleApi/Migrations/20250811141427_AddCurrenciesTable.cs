using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrenciesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Currencies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(5920));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(5923));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(5926));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(5929));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(5931));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(6606));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(6610));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7842), new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7843) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7848), new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7849) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7852), new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7853) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7855), new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7856) });

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7909));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7912));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7914));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7916));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7918));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7920));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7922));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 11, 14, 14, 24, 698, DateTimeKind.Utc).AddTicks(7602));

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CompanyId",
                table: "Currencies",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(2555));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(2557));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(2559));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(2561));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(2563));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(2723));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(2725));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3603), new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3605) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3611), new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3612) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3614), new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3615) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3617), new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3618) });

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3654));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3659));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3661));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3662));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3664));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3665));

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3667));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3491));
        }
    }
}
