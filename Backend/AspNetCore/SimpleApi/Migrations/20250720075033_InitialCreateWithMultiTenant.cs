using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SimpleApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithMultiTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessNatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ConfigSchema = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessNatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CustomerAddress = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    BusinessNatureId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CustomFields = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProfiles_BusinessNatures_BusinessNatureId",
                        column: x => x.BusinessNatureId,
                        principalTable: "BusinessNatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndustryFormConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BusinessNatureId = table.Column<int>(type: "INTEGER", nullable: false),
                    FormType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FieldSchema = table.Column<string>(type: "TEXT", nullable: false),
                    ValidationRules = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryFormConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndustryFormConfigs_BusinessNatures_BusinessNatureId",
                        column: x => x.BusinessNatureId,
                        principalTable: "BusinessNatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndustryModules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BusinessNatureId = table.Column<int>(type: "INTEGER", nullable: false),
                    ModuleName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ModuleConfig = table.Column<string>(type: "TEXT", nullable: true),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndustryModules_BusinessNatures_BusinessNatureId",
                        column: x => x.BusinessNatureId,
                        principalTable: "BusinessNatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SaleOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderItems_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompanyProfileId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_CompanyProfiles_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "CompanyProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "BusinessNatures",
                columns: new[] { "Id", "ConfigSchema", "CreatedDate", "Description", "DisplayName", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "{\"dashboardWidgets\":[\"campaignMetrics\",\"clientBilling\",\"adSpend\"],\"forms\":{\"purchase\":[\"vendor\",\"campaignId\",\"adPlatform\",\"budget\"],\"sales\":[\"clientId\",\"serviceType\",\"deliverables\",\"roi\"]},\"reports\":[\"campaignROI\",\"clientProfitability\",\"monthlyRevenue\"]}", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Digital marketing and advertising agencies", "Digital Marketing Agency", true, "DigitalMarketing" },
                    { 2, "{\"dashboardWidgets\":[\"inventory\",\"salesVolume\",\"serviceRevenue\"],\"forms\":{\"purchase\":[\"supplier\",\"partNumber\",\"vehicleModel\",\"quantity\"],\"sales\":[\"customerId\",\"vehicleVin\",\"financing\",\"warranty\"]},\"reports\":[\"inventoryTurnover\",\"salesPerformance\",\"serviceAnalytics\"]}", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Automotive sales and service businesses", "Automobile Industry", true, "Automobile" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "CreatedDate", "Email", "IsActive", "Name", "Phone", "TenantId" },
                values: new object[,]
                {
                    { 1, "123 Main St", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "john@example.com", true, "John Doe", "555-1234", null },
                    { 2, "456 Oak Ave", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "jane@example.com", true, "Jane Smith", "555-5678", null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedDate", "Description", "IsActive", "Name", "Price", "TenantId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Description for Product A", true, "Product A", 10.50m, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Description for Product B", true, "Product B", 25.00m, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Description for Product C", true, "Product C", 15.75m, null }
                });

            migrationBuilder.InsertData(
                table: "IndustryFormConfigs",
                columns: new[] { "Id", "BusinessNatureId", "CreatedDate", "FieldSchema", "FormType", "IsActive", "ValidationRules" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"fields\":[{\"name\":\"vendor\",\"type\":\"text\",\"label\":\"Vendor\",\"required\":true},{\"name\":\"campaignId\",\"type\":\"text\",\"label\":\"Campaign ID\",\"required\":true},{\"name\":\"adPlatform\",\"type\":\"select\",\"label\":\"Ad Platform\",\"options\":[\"Google Ads\",\"Facebook Ads\",\"LinkedIn Ads\",\"Twitter Ads\"],\"required\":true},{\"name\":\"budget\",\"type\":\"number\",\"label\":\"Budget\",\"required\":true,\"min\":0}]}", "purchase", true, "{\"vendor\":{\"minLength\":2,\"maxLength\":100},\"budget\":{\"min\":0,\"max\":1000000}}" },
                    { 2, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"fields\":[{\"name\":\"clientId\",\"type\":\"text\",\"label\":\"Client ID\",\"required\":true},{\"name\":\"serviceType\",\"type\":\"select\",\"label\":\"Service Type\",\"options\":[\"PPC Management\",\"Social Media Marketing\",\"SEO\",\"Content Marketing\",\"Email Marketing\"],\"required\":true},{\"name\":\"deliverables\",\"type\":\"textarea\",\"label\":\"Deliverables\",\"required\":true},{\"name\":\"roi\",\"type\":\"number\",\"label\":\"Expected ROI (%)\",\"required\":false,\"min\":0}]}", "sales", true, "{\"clientId\":{\"minLength\":3,\"maxLength\":50},\"deliverables\":{\"minLength\":10,\"maxLength\":1000}}" },
                    { 3, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"fields\":[{\"name\":\"supplier\",\"type\":\"text\",\"label\":\"Supplier\",\"required\":true},{\"name\":\"partNumber\",\"type\":\"text\",\"label\":\"Part Number\",\"required\":true},{\"name\":\"vehicleModel\",\"type\":\"text\",\"label\":\"Vehicle Model\",\"required\":true},{\"name\":\"quantity\",\"type\":\"number\",\"label\":\"Quantity\",\"required\":true,\"min\":1}]}", "purchase", true, "{\"supplier\":{\"minLength\":2,\"maxLength\":100},\"partNumber\":{\"pattern\":\"^[A-Z0-9-]+$\"},\"quantity\":{\"min\":1,\"max\":10000}}" },
                    { 4, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"fields\":[{\"name\":\"customerId\",\"type\":\"text\",\"label\":\"Customer ID\",\"required\":true},{\"name\":\"vehicleVin\",\"type\":\"text\",\"label\":\"Vehicle VIN\",\"required\":true},{\"name\":\"financing\",\"type\":\"select\",\"label\":\"Financing\",\"options\":[\"Cash\",\"Bank Loan\",\"Dealer Financing\",\"Lease\"],\"required\":true},{\"name\":\"warranty\",\"type\":\"select\",\"label\":\"Warranty\",\"options\":[\"Basic\",\"Extended\",\"Premium\"],\"required\":false}]}", "sales", true, "{\"customerId\":{\"minLength\":3,\"maxLength\":50},\"vehicleVin\":{\"pattern\":\"^[A-HJ-NPR-Z0-9]{17}$\",\"message\":\"Invalid VIN format\"}}" }
                });

            migrationBuilder.InsertData(
                table: "IndustryModules",
                columns: new[] { "Id", "BusinessNatureId", "CreatedDate", "DisplayName", "IsEnabled", "ModuleConfig", "ModuleName" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Campaign Management", true, "{\"features\":[\"createCampaign\",\"trackMetrics\",\"budgetManagement\"],\"permissions\":[\"campaign.create\",\"campaign.view\",\"campaign.edit\"]}", "CampaignManagement" },
                    { 2, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Client Billing", true, "{\"features\":[\"invoiceGeneration\",\"paymentTracking\",\"roiReporting\"],\"permissions\":[\"billing.create\",\"billing.view\",\"billing.edit\"]}", "ClientBilling" },
                    { 3, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Inventory Management", true, "{\"features\":[\"stockTracking\",\"partsCatalog\",\"supplierManagement\"],\"permissions\":[\"inventory.create\",\"inventory.view\",\"inventory.edit\"]}", "InventoryManagement" },
                    { 4, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vehicle Sales", true, "{\"features\":[\"vehicleCatalog\",\"salesTracking\",\"financingOptions\"],\"permissions\":[\"sales.create\",\"sales.view\",\"sales.edit\"]}", "VehicleSales" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessNatures_Name",
                table: "BusinessNatures",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_BusinessNatureId",
                table: "CompanyProfiles",
                column: "BusinessNatureId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_TenantId",
                table: "CompanyProfiles",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId",
                table: "Customers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryFormConfigs_BusinessNatureId_FormType",
                table: "IndustryFormConfigs",
                columns: new[] { "BusinessNatureId", "FormType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndustryModules_BusinessNatureId_ModuleName",
                table: "IndustryModules",
                columns: new[] { "BusinessNatureId", "ModuleName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderItems_ProductId",
                table: "SaleOrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderItems_SaleOrderId",
                table: "SaleOrderItems",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_TenantId",
                table: "SaleOrders",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyProfileId",
                table: "Users",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "IndustryFormConfigs");

            migrationBuilder.DropTable(
                name: "IndustryModules");

            migrationBuilder.DropTable(
                name: "SaleOrderItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "SaleOrders");

            migrationBuilder.DropTable(
                name: "CompanyProfiles");

            migrationBuilder.DropTable(
                name: "BusinessNatures");
        }
    }
}
