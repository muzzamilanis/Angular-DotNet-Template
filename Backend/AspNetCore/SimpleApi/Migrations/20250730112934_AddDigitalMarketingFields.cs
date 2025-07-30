using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalMarketingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceOfPayment",
                table: "SaleOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "SaleOrderItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceOfPayment",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "SaleOrderItems");

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8622));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8626));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8690));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8692));

            migrationBuilder.UpdateData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8694));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8879));

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 24, 10, 32, 38, 610, DateTimeKind.Utc).AddTicks(8882));

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
        }
    }
}
