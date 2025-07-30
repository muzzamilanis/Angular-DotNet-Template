using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTypeAndNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderDetails",
                table: "SaleOrders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceTypeId",
                table: "SaleOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPayment",
                table: "SaleOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WordsOrPages",
                table: "SaleOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTypes_Companies_CompanyId",
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

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "CompanyId", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3654), "Blog posts, articles, and other written content", true, "Content Writing" },
                    { 2, 1, new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3659), "Website design and development services", true, "Web Design" },
                    { 3, 1, new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3661), "Search engine optimized content creation", true, "SEO Writing" },
                    { 4, 1, new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3662), "Digital marketing strategy development", true, "Strategy Planning" },
                    { 5, 1, new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3664), "Email campaign development and management", true, "Email Marketing" },
                    { 6, 2, new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3665), "Search engine optimization services", true, "SEO" },
                    { 7, 2, new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3667), "General writing and content creation", true, "Writing Service" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 13, 48, 24, 961, DateTimeKind.Utc).AddTicks(3491));

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_ServiceTypeId",
                table: "SaleOrders",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypes_CompanyId",
                table: "ServiceTypes",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_ServiceTypes_ServiceTypeId",
                table: "SaleOrders",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_ServiceTypes_ServiceTypeId",
                table: "SaleOrders");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_ServiceTypeId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "OrderDetails",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ServiceTypeId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "TotalPayment",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "WordsOrPages",
                table: "SaleOrders");

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(3692));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(3694));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(3697));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(3699));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(3701));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(3929));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(3932));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(4874), new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(4875) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(4878), new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(4878) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(4881), new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(4881) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(4884), new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(4884) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 30, 11, 29, 34, 209, DateTimeKind.Utc).AddTicks(4764));
        }
    }
}
