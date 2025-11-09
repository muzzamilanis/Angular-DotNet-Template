using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class WriterPortalEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssignedWriterId",
                table: "SaleOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFilenames",
                table: "SaleOrders",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionDate",
                table: "SaleOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionStatus",
                table: "SaleOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedFilename",
                table: "SaleOrders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Role",
                value: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_Users_AssignedWriterId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_AssignedWriterId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssignedWriterId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "AttachmentFilenames",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "SubmissionDate",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "SubmissionStatus",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "SubmittedFilename",
                table: "SaleOrders");
        }
    }
}
