using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddConstructionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConstructionProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProjectDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EstimatedBudget = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ActualCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpectedEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProjectManager = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Contractor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstructionProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConstructionProjects_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CnicOrNtn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SharePercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ConstructionProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectClients_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectClients_ConstructionProjects_ConstructionProjectId",
                        column: x => x.ConstructionProjectId,
                        principalTable: "ConstructionProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpenseNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExpenseCategory = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VendorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ConstructionProjectId = table.Column<int>(type: "integer", nullable: false),
                    VoucherId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectExpenses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectExpenses_ConstructionProjects_ConstructionProjectId",
                        column: x => x.ConstructionProjectId,
                        principalTable: "ConstructionProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectExpenses_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProjectInvestors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvestorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CnicOrNtn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InvestmentAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SharePercentage = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ConstructionProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInvestors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectInvestors_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectInvestors_ConstructionProjects_ConstructionProjectId",
                        column: x => x.ConstructionProjectId,
                        principalTable: "ConstructionProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PaymentSource = table.Column<int>(type: "integer", nullable: false),
                    ProjectClientId = table.Column<int>(type: "integer", nullable: true),
                    ProjectInvestorId = table.Column<int>(type: "integer", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ConstructionProjectId = table.Column<int>(type: "integer", nullable: false),
                    VoucherId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectPayments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectPayments_ConstructionProjects_ConstructionProjectId",
                        column: x => x.ConstructionProjectId,
                        principalTable: "ConstructionProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectPayments_ProjectClients_ProjectClientId",
                        column: x => x.ProjectClientId,
                        principalTable: "ProjectClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectPayments_ProjectInvestors_ProjectInvestorId",
                        column: x => x.ProjectInvestorId,
                        principalTable: "ProjectInvestors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectPayments_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ClientLedgerEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntryNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EntryType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ProjectClientId = table.Column<int>(type: "integer", nullable: false),
                    ConstructionProjectId = table.Column<int>(type: "integer", nullable: false),
                    ProjectPaymentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLedgerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientLedgerEntries_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientLedgerEntries_ConstructionProjects_ConstructionProjec~",
                        column: x => x.ConstructionProjectId,
                        principalTable: "ConstructionProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientLedgerEntries_ProjectClients_ProjectClientId",
                        column: x => x.ProjectClientId,
                        principalTable: "ProjectClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientLedgerEntries_ProjectPayments_ProjectPaymentId",
                        column: x => x.ProjectPaymentId,
                        principalTable: "ProjectPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InvestorLedgerEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntryNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EntryType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ProjectInvestorId = table.Column<int>(type: "integer", nullable: false),
                    ConstructionProjectId = table.Column<int>(type: "integer", nullable: false),
                    ProjectPaymentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorLedgerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestorLedgerEntries_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestorLedgerEntries_ConstructionProjects_ConstructionProj~",
                        column: x => x.ConstructionProjectId,
                        principalTable: "ConstructionProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestorLedgerEntries_ProjectInvestors_ProjectInvestorId",
                        column: x => x.ProjectInvestorId,
                        principalTable: "ProjectInvestors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestorLedgerEntries_ProjectPayments_ProjectPaymentId",
                        column: x => x.ProjectPaymentId,
                        principalTable: "ProjectPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLedgerEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntryNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntryType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ConstructionProjectId = table.Column<int>(type: "integer", nullable: false),
                    ProjectPaymentId = table.Column<int>(type: "integer", nullable: true),
                    ProjectExpenseId = table.Column<int>(type: "integer", nullable: true),
                    VoucherId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLedgerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLedgerEntries_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLedgerEntries_ConstructionProjects_ConstructionProje~",
                        column: x => x.ConstructionProjectId,
                        principalTable: "ConstructionProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLedgerEntries_ProjectExpenses_ProjectExpenseId",
                        column: x => x.ProjectExpenseId,
                        principalTable: "ProjectExpenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectLedgerEntries_ProjectPayments_ProjectPaymentId",
                        column: x => x.ProjectPaymentId,
                        principalTable: "ProjectPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectLedgerEntries_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "BusinessNatures",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayName", "IsActive", "Name" },
                values: new object[] { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Construction project management with client, investor, and financial tracking", "Construction", true, "Construction" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientLedgerEntries_CompanyId",
                table: "ClientLedgerEntries",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLedgerEntries_ConstructionProjectId",
                table: "ClientLedgerEntries",
                column: "ConstructionProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLedgerEntries_ProjectClientId",
                table: "ClientLedgerEntries",
                column: "ProjectClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLedgerEntries_ProjectPaymentId",
                table: "ClientLedgerEntries",
                column: "ProjectPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConstructionProjects_CompanyId",
                table: "ConstructionProjects",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorLedgerEntries_CompanyId",
                table: "InvestorLedgerEntries",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorLedgerEntries_ConstructionProjectId",
                table: "InvestorLedgerEntries",
                column: "ConstructionProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorLedgerEntries_ProjectInvestorId",
                table: "InvestorLedgerEntries",
                column: "ProjectInvestorId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorLedgerEntries_ProjectPaymentId",
                table: "InvestorLedgerEntries",
                column: "ProjectPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClients_CompanyId",
                table: "ProjectClients",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectClients_ConstructionProjectId",
                table: "ProjectClients",
                column: "ConstructionProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExpenses_CompanyId",
                table: "ProjectExpenses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExpenses_ConstructionProjectId",
                table: "ProjectExpenses",
                column: "ConstructionProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExpenses_VoucherId",
                table: "ProjectExpenses",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvestors_CompanyId",
                table: "ProjectInvestors",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvestors_ConstructionProjectId",
                table: "ProjectInvestors",
                column: "ConstructionProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLedgerEntries_CompanyId",
                table: "ProjectLedgerEntries",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLedgerEntries_ConstructionProjectId",
                table: "ProjectLedgerEntries",
                column: "ConstructionProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLedgerEntries_ProjectExpenseId",
                table: "ProjectLedgerEntries",
                column: "ProjectExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLedgerEntries_ProjectPaymentId",
                table: "ProjectLedgerEntries",
                column: "ProjectPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLedgerEntries_VoucherId",
                table: "ProjectLedgerEntries",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayments_CompanyId",
                table: "ProjectPayments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayments_ConstructionProjectId",
                table: "ProjectPayments",
                column: "ConstructionProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayments_ProjectClientId",
                table: "ProjectPayments",
                column: "ProjectClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayments_ProjectInvestorId",
                table: "ProjectPayments",
                column: "ProjectInvestorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayments_VoucherId",
                table: "ProjectPayments",
                column: "VoucherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientLedgerEntries");

            migrationBuilder.DropTable(
                name: "InvestorLedgerEntries");

            migrationBuilder.DropTable(
                name: "ProjectLedgerEntries");

            migrationBuilder.DropTable(
                name: "ProjectExpenses");

            migrationBuilder.DropTable(
                name: "ProjectPayments");

            migrationBuilder.DropTable(
                name: "ProjectClients");

            migrationBuilder.DropTable(
                name: "ProjectInvestors");

            migrationBuilder.DropTable(
                name: "ConstructionProjects");

            migrationBuilder.DeleteData(
                table: "BusinessNatures",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
