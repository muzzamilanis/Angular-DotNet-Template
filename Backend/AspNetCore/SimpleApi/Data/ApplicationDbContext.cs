using Microsoft.EntityFrameworkCore;
using SimpleApi.Models;

namespace SimpleApi.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<Customer> Customers { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<SaleOrder> SaleOrders { get; set; }
		public DbSet<SaleOrderItem> SaleOrderItems { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<BusinessNature> BusinessNatures { get; set; }
		public DbSet<ServiceType> ServiceTypes { get; set; }
		public DbSet<Currency> Currencies { get; set; }
		public DbSet<OrderInstruction> OrderInstructions { get; set; }
		
		// Real Estate Module
		public DbSet<Property> Properties { get; set; }
		public DbSet<PropertyPurchase> PropertyPurchases { get; set; }
		public DbSet<PropertySale> PropertySales { get; set; }
		public DbSet<OwnershipTransfer> OwnershipTransfers { get; set; }
		
		// Accounting Module
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Voucher> Vouchers { get; set; }
		public DbSet<JournalEntry> JournalEntries { get; set; }
		public DbSet<Vendor> Vendors { get; set; }
		
		// Construction Module
		public DbSet<ConstructionProject> ConstructionProjects { get; set; }
		public DbSet<ProjectClient> ProjectClients { get; set; }
		public DbSet<ProjectInvestor> ProjectInvestors { get; set; }
		public DbSet<ProjectPayment> ProjectPayments { get; set; }
		public DbSet<ProjectExpense> ProjectExpenses { get; set; }
		public DbSet<ClientLedgerEntry> ClientLedgerEntries { get; set; }
		public DbSet<InvestorLedgerEntry> InvestorLedgerEntries { get; set; }
		public DbSet<ProjectLedgerEntry> ProjectLedgerEntries { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure BusinessNature
			modelBuilder.Entity<BusinessNature>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
				entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Description).HasMaxLength(500);
			});

			// Configure Company
			modelBuilder.Entity<Company>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Address).HasMaxLength(500);
				entity.Property(e => e.Phone).HasMaxLength(50);
				entity.Property(e => e.Email).HasMaxLength(200);
				entity.Property(e => e.ContactPerson).HasMaxLength(100);
				entity.Property(e => e.BusinessNatureId).IsRequired();

				entity.HasOne(e => e.BusinessNature)
					  .WithMany(e => e.Companies)
					  .HasForeignKey(e => e.BusinessNatureId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Project
			modelBuilder.Entity<Project>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.ProjectName).IsRequired().HasMaxLength(200);
				entity.Property(e => e.ProjectDescription).HasMaxLength(1000);
				entity.Property(e => e.ProjectStatus).IsRequired();

				entity.HasOne(e => e.Company)
					  .WithMany(e => e.Projects)
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ServiceType)
					  .WithMany()
					  .HasForeignKey(e => e.ServiceTypeId)
					  .OnDelete(DeleteBehavior.SetNull);
			});

			// Configure ServiceType
			modelBuilder.Entity<ServiceType>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Description).HasMaxLength(200);

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Currency
			modelBuilder.Entity<Currency>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Symbol).IsRequired().HasMaxLength(10);

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure User
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
				entity.Property(e => e.PasswordHash).IsRequired();

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasIndex(e => e.Email).IsUnique();
			});

			// Configure Customer
			modelBuilder.Entity<Customer>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Address).HasMaxLength(500);
				entity.Property(e => e.Phone).HasMaxLength(50);
				entity.Property(e => e.Email).HasMaxLength(200);

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Vendor
			modelBuilder.Entity<Vendor>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Address).HasMaxLength(500);
				entity.Property(e => e.Phone).HasMaxLength(50);
				entity.Property(e => e.Email).HasMaxLength(200);
				entity.Property(e => e.ContactPerson).HasMaxLength(100);
				entity.Property(e => e.Description).HasMaxLength(500);

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Product
			modelBuilder.Entity<Product>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure SaleOrder
			modelBuilder.Entity<SaleOrder>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
				entity.Property(e => e.CustomerName).HasMaxLength(200);
				entity.Property(e => e.CustomerAddress).HasMaxLength(500);
				entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.AmountReceived).HasColumnType("decimal(18,2)");
				entity.Property(e => e.AmountReceivedInPkr).HasColumnType("decimal(18,2)");
				entity.Property(e => e.TotalPayment).HasColumnType("decimal(18,2)");

				// Digital marketing specific field configurations
				entity.Property(e => e.Code).HasMaxLength(50);
				entity.Property(e => e.ClientName).HasMaxLength(200);
				entity.Property(e => e.Status).HasMaxLength(50);
				entity.Property(e => e.PaymentStatus).HasMaxLength(50);
				entity.Property(e => e.ContactEmail).HasMaxLength(200);
				entity.Property(e => e.ContactPhone).HasMaxLength(50);
				entity.Property(e => e.AssignedTo).HasMaxLength(200);
				entity.Property(e => e.OrderDetails).HasMaxLength(1000);
				entity.Property(e => e.WordsOrPages).HasMaxLength(100);
				entity.Property(e => e.AcademicLevel).HasMaxLength(100);
				entity.Property(e => e.Country).HasMaxLength(100);

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Project)
					  .WithMany(e => e.SaleOrders)
					  .HasForeignKey(e => e.ProjectId)
					  .OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(e => e.ServiceType)
					  .WithMany()
					  .HasForeignKey(e => e.ServiceTypeId)
					  .OnDelete(DeleteBehavior.SetNull);

				entity.HasMany(e => e.Items)
					  .WithOne(e => e.SaleOrder)
					  .HasForeignKey(e => e.SaleOrderId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// Configure SaleOrderItem
			modelBuilder.Entity<SaleOrderItem>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

				// Digital marketing specific field configurations
				entity.Property(e => e.WorkTitle).HasMaxLength(200);
				entity.Property(e => e.PaperType).HasMaxLength(100);

				entity.HasOne(e => e.SaleOrder)
					  .WithMany(e => e.Items)
					  .HasForeignKey(e => e.SaleOrderId);

				entity.HasOne(e => e.Product)
					  .WithMany()
					  .HasForeignKey(e => e.ProductId);
			});

			// Configure OrderInstruction
			modelBuilder.Entity<OrderInstruction>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
				entity.Property(e => e.AttachmentFilenames).HasMaxLength(500);
				entity.Property(e => e.CreatedAt).IsRequired();

				entity.HasOne(e => e.SaleOrder)
					  .WithMany()
					  .HasForeignKey(e => e.SaleOrderId)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(e => e.Sender)
					  .WithMany()
					  .HasForeignKey(e => e.SenderId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Property
			modelBuilder.Entity<Property>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.PropertyName).IsRequired().HasMaxLength(200);
				entity.Property(e => e.PropertyType).IsRequired().HasMaxLength(100);
				entity.Property(e => e.TotalArea).HasColumnType("decimal(18,2)");
				entity.Property(e => e.LawnArea).HasColumnType("decimal(18,2)");
				entity.Property(e => e.CurrentValue).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure PropertyPurchase
			modelBuilder.Entity<PropertyPurchase>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.PurchaseNumber).IsRequired().HasMaxLength(100);
				entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18,2)");
				entity.Property(e => e.AdvancePayment).HasColumnType("decimal(18,2)");
				entity.Property(e => e.RemainingPayment).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Property)
					  .WithMany(e => e.Purchases)
					  .HasForeignKey(e => e.PropertyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Voucher)
					  .WithMany()
					  .HasForeignKey(e => e.VoucherId)
					  .OnDelete(DeleteBehavior.SetNull);
			});

			// Configure PropertySale
			modelBuilder.Entity<PropertySale>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.SaleNumber).IsRequired().HasMaxLength(100);
				entity.Property(e => e.SalePrice).HasColumnType("decimal(18,2)");
				entity.Property(e => e.CostPrice).HasColumnType("decimal(18,2)");
				entity.Property(e => e.ProfitOrLoss).HasColumnType("decimal(18,2)");
				entity.Property(e => e.AdvanceReceived).HasColumnType("decimal(18,2)");
				entity.Property(e => e.RemainingReceivable).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Property)
					  .WithMany(e => e.Sales)
					  .HasForeignKey(e => e.PropertyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Voucher)
					  .WithMany()
					  .HasForeignKey(e => e.VoucherId)
					  .OnDelete(DeleteBehavior.SetNull);
			});

			// Configure OwnershipTransfer
			modelBuilder.Entity<OwnershipTransfer>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.TransferNumber).IsRequired().HasMaxLength(100);
				entity.Property(e => e.TransferValue).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Property)
					  .WithMany(e => e.OwnershipTransfers)
					  .HasForeignKey(e => e.PropertyId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Account
			modelBuilder.Entity<Account>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.AccountCode).IsRequired().HasMaxLength(50);
				entity.Property(e => e.AccountName).IsRequired().HasMaxLength(200);
				entity.Property(e => e.AccountType).IsRequired().HasMaxLength(50);
				entity.Property(e => e.OpeningBalance).HasColumnType("decimal(18,2)");
				entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ParentAccount)
					  .WithMany(e => e.SubAccounts)
					  .HasForeignKey(e => e.ParentAccountId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Voucher
			modelBuilder.Entity<Voucher>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.VoucherNumber).IsRequired().HasMaxLength(100);
				entity.Property(e => e.VoucherType).IsRequired().HasMaxLength(50);
				entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.PostedByUser)
					  .WithMany()
					  .HasForeignKey(e => e.PostedByUserId)
					  .OnDelete(DeleteBehavior.SetNull);
			});

			// Configure JournalEntry
			modelBuilder.Entity<JournalEntry>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.EntryType).IsRequired().HasMaxLength(10);
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Voucher)
					  .WithMany(e => e.JournalEntries)
					  .HasForeignKey(e => e.VoucherId)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(e => e.Account)
					  .WithMany(e => e.JournalEntries)
					  .HasForeignKey(e => e.AccountId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure ConstructionProject
			modelBuilder.Entity<ConstructionProject>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.ProjectName).IsRequired().HasMaxLength(200);
				entity.Property(e => e.EstimatedBudget).HasColumnType("decimal(18,2)");
				entity.Property(e => e.ActualCost).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure ProjectClient
			modelBuilder.Entity<ProjectClient>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.ClientName).IsRequired().HasMaxLength(200);
				entity.Property(e => e.SharePercentage).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ConstructionProject)
					  .WithMany(e => e.ProjectClients)
					  .HasForeignKey(e => e.ConstructionProjectId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure ProjectInvestor
			modelBuilder.Entity<ProjectInvestor>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.InvestorName).IsRequired().HasMaxLength(200);
				entity.Property(e => e.InvestmentAmount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.SharePercentage).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ConstructionProject)
					  .WithMany(e => e.ProjectInvestors)
					  .HasForeignKey(e => e.ConstructionProjectId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure ProjectPayment
			modelBuilder.Entity<ProjectPayment>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.PaymentNumber).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ConstructionProject)
					  .WithMany(e => e.ProjectPayments)
					  .HasForeignKey(e => e.ConstructionProjectId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ProjectClient)
					  .WithMany()
					  .HasForeignKey(e => e.ProjectClientId)
					  .OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(e => e.ProjectInvestor)
					  .WithMany()
					  .HasForeignKey(e => e.ProjectInvestorId)
					  .OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(e => e.Voucher)
					  .WithMany()
					  .HasForeignKey(e => e.VoucherId)
					  .OnDelete(DeleteBehavior.SetNull);
			});

			// Configure ProjectExpense
			modelBuilder.Entity<ProjectExpense>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.ExpenseNumber).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ConstructionProject)
					  .WithMany(e => e.ProjectExpenses)
					  .HasForeignKey(e => e.ConstructionProjectId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Voucher)
					  .WithMany()
					  .HasForeignKey(e => e.VoucherId)
					  .OnDelete(DeleteBehavior.SetNull);
			});

			// Configure ClientLedgerEntry
			modelBuilder.Entity<ClientLedgerEntry>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.EntryNumber).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ProjectClient)
					  .WithMany(e => e.LedgerEntries)
					  .HasForeignKey(e => e.ProjectClientId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ConstructionProject)
					  .WithMany()
					  .HasForeignKey(e => e.ConstructionProjectId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ProjectPayment)
					  .WithMany()
					  .HasForeignKey(e => e.ProjectPaymentId)
					  .OnDelete(DeleteBehavior.SetNull);
			});

			// Configure InvestorLedgerEntry
			modelBuilder.Entity<InvestorLedgerEntry>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.EntryNumber).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ProjectInvestor)
					  .WithMany(e => e.LedgerEntries)
					  .HasForeignKey(e => e.ProjectInvestorId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ConstructionProject)
					  .WithMany()
					  .HasForeignKey(e => e.ConstructionProjectId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ProjectPayment)
					  .WithMany()
					  .HasForeignKey(e => e.ProjectPaymentId)
					  .OnDelete(DeleteBehavior.SetNull);
			});

			// Configure ProjectLedgerEntry
			modelBuilder.Entity<ProjectLedgerEntry>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.EntryNumber).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");

				entity.HasOne(e => e.Company)
					  .WithMany()
					  .HasForeignKey(e => e.CompanyId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ConstructionProject)
					  .WithMany(e => e.ProjectLedgerEntries)
					  .HasForeignKey(e => e.ConstructionProjectId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ProjectPayment)
					  .WithMany()
					  .HasForeignKey(e => e.ProjectPaymentId)
					  .OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(e => e.ProjectExpense)
					  .WithMany()
					  .HasForeignKey(e => e.ProjectExpenseId)
					  .OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(e => e.Voucher)
					  .WithMany()
					  .HasForeignKey(e => e.VoucherId)
					  .OnDelete(DeleteBehavior.SetNull);
			});

			// Define a fixed seed date to avoid constant model changes
			var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			// Seed data - Create business natures first
			modelBuilder.Entity<BusinessNature>().HasData(
				new BusinessNature { Id = 1, Name = "DigitalMarketingAgency", DisplayName = "Digital Marketing Agency", Description = "Digital marketing and content creation services", IsActive = true, CreatedAt = seedDate },
				new BusinessNature { Id = 2, Name = "Automobile", DisplayName = "Automobile", Description = "Automotive industry and vehicle-related services", IsActive = true, CreatedAt = seedDate },
				new BusinessNature { Id = 3, Name = "General", DisplayName = "General", Description = "General business operations and services", IsActive = true, CreatedAt = seedDate },
				new BusinessNature { Id = 4, Name = "Retail", DisplayName = "Retail & E-commerce", Description = "Traditional retail and online commerce operations", IsActive = true, CreatedAt = seedDate },
				new BusinessNature { Id = 5, Name = "Manufacturing", DisplayName = "Manufacturing", Description = "Manufacturing and production businesses", IsActive = true, CreatedAt = seedDate },
				new BusinessNature { Id = 6, Name = "RealEstate", DisplayName = "Real Estate", Description = "Real estate property management, buying, selling, and rental services", IsActive = true, CreatedAt = seedDate },
				new BusinessNature { Id = 7, Name = "Construction", DisplayName = "Construction", Description = "Construction project management with client, investor, and financial tracking", IsActive = true, CreatedAt = seedDate },
				new BusinessNature { Id = 8, Name = "Accounting", DisplayName = "Accounting & Finance", Description = "Accounting, bookkeeping, and financial management services", IsActive = true, CreatedAt = seedDate }
			);

			// Seed data - Create companies with business nature references
			modelBuilder.Entity<Company>().HasData(
				new Company { Id = 1, Name = "Default Company", Address = "123 Business St", Phone = "555-0001", Email = "admin@defaultcompany.com", ContactPerson = "System Admin", BusinessNatureId = 3, IsActive = true, CreatedAt = seedDate },
				new Company { Id = 2, Name = "Digital Marketing Pro", Address = "456 Marketing Ave", Phone = "555-0002", Email = "admin@digitalmarketingpro.com", ContactPerson = "Marketing Manager", BusinessNatureId = 1, IsActive = true, CreatedAt = seedDate }
			);

			// Seed data for default admin user
			// Password "123qwe" hashed with SHA256
			var adminPasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("123qwe")));
			modelBuilder.Entity<User>().HasData(
				new User { Id = 1, Name = "System Administrator", Email = "admin@aspnetboilerplate.com", PasswordHash = adminPasswordHash, IsActive = true, IsSuperAdmin = true, CompanyId = 1, CreatedAt = seedDate }
			);

			// Seed data for Customers (with CompanyId)
			modelBuilder.Entity<Customer>().HasData(
				new Customer { Id = 1, Name = "John Doe", Address = "123 Main St", Phone = "555-1234", Email = "john@example.com", IsActive = true, CompanyId = 1 },
				new Customer { Id = 2, Name = "Jane Smith", Address = "456 Oak Ave", Phone = "555-5678", Email = "jane@example.com", IsActive = true, CompanyId = 1 }
			);

			// Seed data for Products (with CompanyId)
			modelBuilder.Entity<Product>().HasData(
				new Product { Id = 1, Name = "Product A", Description = "Description for Product A", Price = 10.50m, IsActive = true, CompanyId = 1 },
				new Product { Id = 2, Name = "Product B", Description = "Description for Product B", Price = 25.00m, IsActive = true, CompanyId = 1 },
				new Product { Id = 3, Name = "Product C", Description = "Description for Product C", Price = 15.75m, IsActive = true, CompanyId = 1 }
			);

			// Seed data for Projects
			modelBuilder.Entity<Project>().HasData(
				new Project { Id = 1, ProjectName = "Website Development", ProjectDescription = "Corporate website development project", ProjectStatus = ProjectStatus.Active, ServiceTypeId = 2, CreatedDate = seedDate, UpdatedDate = seedDate, CompanyId = 1, IsActive = true },
				new Project { Id = 2, ProjectName = "SEO Campaign", ProjectDescription = "Search engine optimization campaign", ProjectStatus = ProjectStatus.Active, ServiceTypeId = 6, CreatedDate = seedDate, UpdatedDate = seedDate, CompanyId = 1, IsActive = true },
				new Project { Id = 3, ProjectName = "Social Media Marketing", ProjectDescription = "Comprehensive social media marketing strategy", ProjectStatus = ProjectStatus.Active, ServiceTypeId = 4, CreatedDate = seedDate, UpdatedDate = seedDate, CompanyId = 2, IsActive = true },
				new Project { Id = 4, ProjectName = "Content Creation Campaign", ProjectDescription = "Blog and content creation for digital presence", ProjectStatus = ProjectStatus.Completed, ServiceTypeId = 1, CreatedDate = seedDate, UpdatedDate = seedDate, CompanyId = 2, IsActive = true }
			);

			// Seed data for ServiceTypes
			modelBuilder.Entity<ServiceType>().HasData(
				new ServiceType { Id = 1, Name = "Content Writing", Description = "Blog posts, articles, and other written content", IsActive = true, CompanyId = 1, CreatedAt = seedDate },
				new ServiceType { Id = 2, Name = "Web Design", Description = "Website design and development services", IsActive = true, CompanyId = 1, CreatedAt = seedDate },
				new ServiceType { Id = 3, Name = "SEO Writing", Description = "Search engine optimized content creation", IsActive = true, CompanyId = 1, CreatedAt = seedDate },
				new ServiceType { Id = 4, Name = "Strategy Planning", Description = "Digital marketing strategy development", IsActive = true, CompanyId = 1, CreatedAt = seedDate },
				new ServiceType { Id = 5, Name = "Email Marketing", Description = "Email campaign development and management", IsActive = true, CompanyId = 1, CreatedAt = seedDate },
				new ServiceType { Id = 6, Name = "SEO", Description = "Search engine optimization services", IsActive = true, CompanyId = 2, CreatedAt = seedDate },
				new ServiceType { Id = 7, Name = "Writing Service", Description = "General writing and content creation", IsActive = true, CompanyId = 2, CreatedAt = seedDate }
			);
		}
	}
}
