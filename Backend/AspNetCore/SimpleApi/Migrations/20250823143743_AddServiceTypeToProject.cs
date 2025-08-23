using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTypeToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceTypeId",
                table: "Projects",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "PrimaryColor",
                value: null);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "PrimaryColor",
                value: null);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                column: "ServiceTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                column: "ServiceTypeId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                column: "ServiceTypeId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                column: "ServiceTypeId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ServiceTypeId",
                table: "Projects",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ServiceTypes_ServiceTypeId",
                table: "Projects",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ServiceTypes_ServiceTypeId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ServiceTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ServiceTypeId",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "PrimaryColor",
                value: "#667eea");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "PrimaryColor",
                value: "#667eea");
        }
    }
}
