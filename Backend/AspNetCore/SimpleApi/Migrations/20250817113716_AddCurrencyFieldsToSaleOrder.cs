using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyFieldsToSaleOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "SaleOrders",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateUsed",
                table: "SaleOrders",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ForeignCurrencyAmount",
                table: "SaleOrders",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PkrAmount",
                table: "SaleOrders",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ExchangeRateUsed",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ForeignCurrencyAmount",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "PkrAmount",
                table: "SaleOrders");
        }
    }
}
