using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectsAndDigitalMarketingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcademicLevel",
                table: "SaleOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountReceived",
                table: "SaleOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "SaleOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "SaleOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "SaleOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "SaleOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "SaleOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "SaleOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "SaleOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "SaleOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "SaleOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SaleOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ItemDeadline",
                table: "SaleOrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "SaleOrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaperType",
                table: "SaleOrderItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WordCount",
                table: "SaleOrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkTitle",
                table: "SaleOrderItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyNature",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProjectDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProjectStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompanyNature", "CreatedAt" },
                values: new object[] { "General", new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(1513) });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CompanyId", "CreatedDate", "IsActive", "ProjectDescription", "ProjectName", "ProjectStatus", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(3164), true, "Corporate website development project", "Website Development", 1, new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(3165) },
                    { 2, 1, new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(3169), true, "Search engine optimization campaign", "SEO Campaign", 1, new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(3169) }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 10, 51, 54, 62, DateTimeKind.Utc).AddTicks(2978));

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_ProjectId",
                table: "SaleOrders",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CompanyId",
                table: "Projects",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_Projects_ProjectId",
                table: "SaleOrders",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_Projects_ProjectId",
                table: "SaleOrders");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_ProjectId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "AcademicLevel",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "AmountReceived",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ItemDeadline",
                table: "SaleOrderItems");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "SaleOrderItems");

            migrationBuilder.DropColumn(
                name: "PaperType",
                table: "SaleOrderItems");

            migrationBuilder.DropColumn(
                name: "WordCount",
                table: "SaleOrderItems");

            migrationBuilder.DropColumn(
                name: "WorkTitle",
                table: "SaleOrderItems");

            migrationBuilder.DropColumn(
                name: "CompanyNature",
                table: "Companies");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 6, 36, 33, 303, DateTimeKind.Utc).AddTicks(7075));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 23, 6, 36, 33, 303, DateTimeKind.Utc).AddTicks(8095));
        }
    }
}
