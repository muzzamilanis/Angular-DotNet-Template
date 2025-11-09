using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAssignedWriterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_Users_AssignedWriterId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_AssignedWriterId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "AssignedWriterId",
                table: "SaleOrders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedWriterId",
                table: "SaleOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_AssignedWriterId",
                table: "SaleOrders",
                column: "AssignedWriterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_Users_AssignedWriterId",
                table: "SaleOrders",
                column: "AssignedWriterId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
