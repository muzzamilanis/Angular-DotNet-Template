using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderWorkflowStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLedgerEntries_Vouchers_VoucherId",
                table: "ProjectLedgerEntries");

            migrationBuilder.CreateTable(
                name: "OrderWorkflowStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    Stage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AssignedTo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderWorkflowStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderWorkflowStatuses_SaleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderWorkflowStatuses_OrderId",
                table: "OrderWorkflowStatuses",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLedgerEntries_Vouchers_VoucherId",
                table: "ProjectLedgerEntries",
                column: "VoucherId",
                principalTable: "Vouchers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLedgerEntries_Vouchers_VoucherId",
                table: "ProjectLedgerEntries");

            migrationBuilder.DropTable(
                name: "OrderWorkflowStatuses");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLedgerEntries_Vouchers_VoucherId",
                table: "ProjectLedgerEntries",
                column: "VoucherId",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
