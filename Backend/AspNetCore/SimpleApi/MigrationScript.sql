IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Address] nvarchar(500) NULL,
    [Phone] nvarchar(50) NULL,
    [Email] nvarchar(200) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Price] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SaleOrders] (
    [Id] int NOT NULL IDENTITY,
    [OrderNumber] nvarchar(50) NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [CustomerName] nvarchar(200) NULL,
    [CustomerAddress] nvarchar(500) NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_SaleOrders] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SaleOrderItems] (
    [Id] int NOT NULL IDENTITY,
    [SaleOrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_SaleOrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SaleOrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SaleOrderItems_SaleOrders_SaleOrderId] FOREIGN KEY ([SaleOrderId]) REFERENCES [SaleOrders] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'Email', N'IsActive', N'Name', N'Phone') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] ON;
INSERT INTO [Customers] ([Id], [Address], [Email], [IsActive], [Name], [Phone])
VALUES (1, N'123 Main St', N'john@example.com', CAST(1 AS bit), N'John Doe', N'555-1234'),
(2, N'456 Oak Ave', N'jane@example.com', CAST(1 AS bit), N'Jane Smith', N'555-5678');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'Email', N'IsActive', N'Name', N'Phone') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name', N'Price') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] ([Id], [Description], [IsActive], [Name], [Price])
VALUES (1, N'Description for Product A', CAST(1 AS bit), N'Product A', 10.5),
(2, N'Description for Product B', CAST(1 AS bit), N'Product B', 25.0),
(3, N'Description for Product C', CAST(1 AS bit), N'Product C', 15.75);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name', N'Price') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] OFF;
GO

CREATE INDEX [IX_SaleOrderItems_ProductId] ON [SaleOrderItems] ([ProductId]);
GO

CREATE INDEX [IX_SaleOrderItems_SaleOrderId] ON [SaleOrderItems] ([SaleOrderId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250719094659_BusinessDB', N'8.0.17');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [SaleOrders] ADD [CompanyId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Products] ADD [CompanyId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Customers] ADD [CompanyId] int NOT NULL DEFAULT 0;
GO

CREATE TABLE [Companies] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Address] nvarchar(500) NULL,
    [Phone] nvarchar(50) NULL,
    [Email] nvarchar(200) NULL,
    [ContactPerson] nvarchar(100) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [IsSuperAdmin] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CompanyId] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'ContactPerson', N'CreatedAt', N'Email', N'IsActive', N'Name', N'Phone') AND [object_id] = OBJECT_ID(N'[Companies]'))
    SET IDENTITY_INSERT [Companies] ON;
INSERT INTO [Companies] ([Id], [Address], [ContactPerson], [CreatedAt], [Email], [IsActive], [Name], [Phone])
VALUES (1, N'123 Business St', N'System Admin', '2025-07-21T16:34:54.7526516Z', N'admin@defaultcompany.com', CAST(1 AS bit), N'Default Company', N'555-0001');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'ContactPerson', N'CreatedAt', N'Email', N'IsActive', N'Name', N'Phone') AND [object_id] = OBJECT_ID(N'[Companies]'))
    SET IDENTITY_INSERT [Companies] OFF;
GO

UPDATE [Customers] SET [CompanyId] = 1
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Customers] SET [CompanyId] = 1
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Products] SET [CompanyId] = 1
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Products] SET [CompanyId] = 1
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Products] SET [CompanyId] = 1
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_SaleOrders_CompanyId] ON [SaleOrders] ([CompanyId]);
GO

CREATE INDEX [IX_Products_CompanyId] ON [Products] ([CompanyId]);
GO

CREATE INDEX [IX_Customers_CompanyId] ON [Customers] ([CompanyId]);
GO

CREATE INDEX [IX_Users_CompanyId] ON [Users] ([CompanyId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

ALTER TABLE [Customers] ADD CONSTRAINT [FK_Customers_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Products] ADD CONSTRAINT [FK_Products_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [SaleOrders] ADD CONSTRAINT [FK_SaleOrders_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250721163455_AddMultiTenancySupport', N'8.0.17');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [Companies] SET [CreatedAt] = '2025-07-23T06:36:33.3037075Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'CreatedAt', N'Email', N'IsActive', N'IsSuperAdmin', N'Name', N'PasswordHash') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [CompanyId], [CreatedAt], [Email], [IsActive], [IsSuperAdmin], [Name], [PasswordHash])
VALUES (1, 1, '2025-07-23T06:36:33.3038095Z', N'admin@aspnetboilerplate.com', CAST(1 AS bit), CAST(1 AS bit), N'System Administrator', N'+/s4bv6mfoFvLdoKjJSpjrIDdXrrs/VfGDdVoZLURGc=');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'CreatedAt', N'Email', N'IsActive', N'IsSuperAdmin', N'Name', N'PasswordHash') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250723063634_AddDefaultAdminUser', N'8.0.17');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [SaleOrders] ADD [AcademicLevel] nvarchar(100) NULL;
GO

ALTER TABLE [SaleOrders] ADD [AmountReceived] decimal(18,2) NULL;
GO

ALTER TABLE [SaleOrders] ADD [AssignedTo] nvarchar(200) NULL;
GO

ALTER TABLE [SaleOrders] ADD [ClientName] nvarchar(200) NULL;
GO

ALTER TABLE [SaleOrders] ADD [Code] nvarchar(50) NULL;
GO

ALTER TABLE [SaleOrders] ADD [ContactEmail] nvarchar(200) NULL;
GO

ALTER TABLE [SaleOrders] ADD [ContactPhone] nvarchar(50) NULL;
GO

ALTER TABLE [SaleOrders] ADD [Country] nvarchar(100) NULL;
GO

ALTER TABLE [SaleOrders] ADD [Deadline] datetime2 NULL;
GO

ALTER TABLE [SaleOrders] ADD [PaymentStatus] nvarchar(50) NULL;
GO

ALTER TABLE [SaleOrders] ADD [ProjectId] int NULL;
GO

ALTER TABLE [SaleOrders] ADD [Status] nvarchar(50) NULL;
GO

ALTER TABLE [SaleOrderItems] ADD [ItemDeadline] datetime2 NULL;
GO

ALTER TABLE [SaleOrderItems] ADD [PageCount] int NULL;
GO

ALTER TABLE [SaleOrderItems] ADD [PaperType] nvarchar(100) NULL;
GO

ALTER TABLE [SaleOrderItems] ADD [WordCount] int NULL;
GO

ALTER TABLE [SaleOrderItems] ADD [WorkTitle] nvarchar(200) NULL;
GO

ALTER TABLE [Companies] ADD [CompanyNature] nvarchar(100) NOT NULL DEFAULT N'';
GO

CREATE TABLE [Projects] (
    [Id] int NOT NULL IDENTITY,
    [ProjectName] nvarchar(200) NOT NULL,
    [ProjectDescription] nvarchar(1000) NULL,
    [ProjectStatus] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CompanyId] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Projects_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);
GO

UPDATE [Companies] SET [CompanyNature] = N'General', [CreatedAt] = '2025-07-23T10:51:54.0621513Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'CreatedDate', N'IsActive', N'ProjectDescription', N'ProjectName', N'ProjectStatus', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Projects]'))
    SET IDENTITY_INSERT [Projects] ON;
INSERT INTO [Projects] ([Id], [CompanyId], [CreatedDate], [IsActive], [ProjectDescription], [ProjectName], [ProjectStatus], [UpdatedDate])
VALUES (1, 1, '2025-07-23T10:51:54.0623164Z', CAST(1 AS bit), N'Corporate website development project', N'Website Development', 1, '2025-07-23T10:51:54.0623165Z'),
(2, 1, '2025-07-23T10:51:54.0623169Z', CAST(1 AS bit), N'Search engine optimization campaign', N'SEO Campaign', 1, '2025-07-23T10:51:54.0623169Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'CreatedDate', N'IsActive', N'ProjectDescription', N'ProjectName', N'ProjectStatus', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Projects]'))
    SET IDENTITY_INSERT [Projects] OFF;
GO

UPDATE [Users] SET [CreatedAt] = '2025-07-23T10:51:54.0622978Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_SaleOrders_ProjectId] ON [SaleOrders] ([ProjectId]);
GO

CREATE INDEX [IX_Projects_CompanyId] ON [Projects] ([CompanyId]);
GO

ALTER TABLE [SaleOrders] ADD CONSTRAINT [FK_SaleOrders_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE SET NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250723105154_AddProjectsAndDigitalMarketingFields', N'8.0.17');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [Companies] SET [CreatedAt] = '2025-07-23T11:14:56.1773546Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'CompanyNature', N'ContactPerson', N'CreatedAt', N'Email', N'IsActive', N'Name', N'Phone') AND [object_id] = OBJECT_ID(N'[Companies]'))
    SET IDENTITY_INSERT [Companies] ON;
INSERT INTO [Companies] ([Id], [Address], [CompanyNature], [ContactPerson], [CreatedAt], [Email], [IsActive], [Name], [Phone])
VALUES (2, N'456 Marketing Ave', N'Digital Marketing', N'Marketing Manager', '2025-07-23T11:14:56.1773549Z', N'admin@digitalmarketingpro.com', CAST(1 AS bit), N'Digital Marketing Pro', N'555-0002');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'CompanyNature', N'ContactPerson', N'CreatedAt', N'Email', N'IsActive', N'Name', N'Phone') AND [object_id] = OBJECT_ID(N'[Companies]'))
    SET IDENTITY_INSERT [Companies] OFF;
GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-23T11:14:56.1774754Z', [UpdatedDate] = '2025-07-23T11:14:56.1774755Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-23T11:14:56.1774758Z', [UpdatedDate] = '2025-07-23T11:14:56.1774758Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Users] SET [CreatedAt] = '2025-07-23T11:14:56.1774580Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'CreatedDate', N'IsActive', N'ProjectDescription', N'ProjectName', N'ProjectStatus', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Projects]'))
    SET IDENTITY_INSERT [Projects] ON;
INSERT INTO [Projects] ([Id], [CompanyId], [CreatedDate], [IsActive], [ProjectDescription], [ProjectName], [ProjectStatus], [UpdatedDate])
VALUES (3, 2, '2025-07-23T11:14:56.1774761Z', CAST(1 AS bit), N'Comprehensive social media marketing strategy', N'Social Media Marketing', 1, '2025-07-23T11:14:56.1774762Z'),
(4, 2, '2025-07-23T11:14:56.1774764Z', CAST(1 AS bit), N'Blog and content creation for digital presence', N'Content Creation Campaign', 3, '2025-07-23T11:14:56.1774764Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'CreatedDate', N'IsActive', N'ProjectDescription', N'ProjectName', N'ProjectStatus', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Projects]'))
    SET IDENTITY_INSERT [Projects] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250723111456_AddDigitalMarketingSampleData', N'8.0.17');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'CompanyNature');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Companies] DROP COLUMN [CompanyNature];
GO

ALTER TABLE [Companies] ADD [BusinessNatureId] int NULL;
GO

CREATE TABLE [BusinessNatures] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [DisplayName] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BusinessNatures] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Description', N'DisplayName', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[BusinessNatures]'))
    SET IDENTITY_INSERT [BusinessNatures] ON;
INSERT INTO [BusinessNatures] ([Id], [CreatedAt], [Description], [DisplayName], [IsActive], [Name])
VALUES (1, '2025-07-24T10:32:38.6108622Z', N'Digital marketing and content creation services', N'Digital Marketing Agency', CAST(1 AS bit), N'DigitalMarketingAgency'),
(2, '2025-07-24T10:32:38.6108626Z', N'Automotive industry and vehicle-related services', N'Automobile', CAST(1 AS bit), N'Automobile'),
(3, '2025-07-24T10:32:38.6108690Z', N'General business operations and services', N'General', CAST(1 AS bit), N'General'),
(4, '2025-07-24T10:32:38.6108692Z', N'Traditional retail and online commerce operations', N'Retail & E-commerce', CAST(1 AS bit), N'Retail'),
(5, '2025-07-24T10:32:38.6108694Z', N'Manufacturing and production businesses', N'Manufacturing', CAST(1 AS bit), N'Manufacturing');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Description', N'DisplayName', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[BusinessNatures]'))
    SET IDENTITY_INSERT [BusinessNatures] OFF;
GO

UPDATE [Companies] SET [BusinessNatureId] = 3, [CreatedAt] = '2025-07-24T10:32:38.6108879Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Companies] SET [BusinessNatureId] = 1, [CreatedAt] = '2025-07-24T10:32:38.6108882Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-24T10:32:38.6109728Z', [UpdatedDate] = '2025-07-24T10:32:38.6109729Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-24T10:32:38.6109732Z', [UpdatedDate] = '2025-07-24T10:32:38.6109732Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-24T10:32:38.6109735Z', [UpdatedDate] = '2025-07-24T10:32:38.6109735Z'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-24T10:32:38.6109738Z', [UpdatedDate] = '2025-07-24T10:32:38.6109738Z'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Users] SET [CreatedAt] = '2025-07-24T10:32:38.6109625Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_Companies_BusinessNatureId] ON [Companies] ([BusinessNatureId]);
GO

ALTER TABLE [Companies] ADD CONSTRAINT [FK_Companies_BusinessNatures_BusinessNatureId] FOREIGN KEY ([BusinessNatureId]) REFERENCES [BusinessNatures] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250724103239_AddBusinessNatureTable', N'8.0.17');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [SaleOrders] ADD [SourceOfPayment] nvarchar(100) NULL;
GO

ALTER TABLE [SaleOrderItems] ADD [ServiceType] nvarchar(100) NULL;
GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T11:29:34.2093692Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T11:29:34.2093694Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T11:29:34.2093697Z'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T11:29:34.2093699Z'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T11:29:34.2093701Z'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Companies] SET [CreatedAt] = '2025-07-30T11:29:34.2093929Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Companies] SET [CreatedAt] = '2025-07-30T11:29:34.2093932Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-30T11:29:34.2094874Z', [UpdatedDate] = '2025-07-30T11:29:34.2094875Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-30T11:29:34.2094878Z', [UpdatedDate] = '2025-07-30T11:29:34.2094878Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-30T11:29:34.2094881Z', [UpdatedDate] = '2025-07-30T11:29:34.2094881Z'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-30T11:29:34.2094884Z', [UpdatedDate] = '2025-07-30T11:29:34.2094884Z'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Users] SET [CreatedAt] = '2025-07-30T11:29:34.2094764Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250730112934_AddDigitalMarketingFields', N'8.0.17');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [SaleOrders] ADD [OrderDetails] nvarchar(1000) NULL;
GO

ALTER TABLE [SaleOrders] ADD [ServiceTypeId] int NULL;
GO

ALTER TABLE [SaleOrders] ADD [TotalPayment] decimal(18,2) NULL;
GO

ALTER TABLE [SaleOrders] ADD [WordsOrPages] nvarchar(100) NULL;
GO

CREATE TABLE [ServiceTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(200) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CompanyId] int NOT NULL,
    CONSTRAINT [PK_ServiceTypes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ServiceTypes_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);
GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T13:48:24.9612555Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T13:48:24.9612557Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T13:48:24.9612559Z'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T13:48:24.9612561Z'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [BusinessNatures] SET [CreatedAt] = '2025-07-30T13:48:24.9612563Z'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Companies] SET [CreatedAt] = '2025-07-30T13:48:24.9612723Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Companies] SET [CreatedAt] = '2025-07-30T13:48:24.9612725Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-30T13:48:24.9613603Z', [UpdatedDate] = '2025-07-30T13:48:24.9613605Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-30T13:48:24.9613611Z', [UpdatedDate] = '2025-07-30T13:48:24.9613612Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-30T13:48:24.9613614Z', [UpdatedDate] = '2025-07-30T13:48:24.9613615Z'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Projects] SET [CreatedDate] = '2025-07-30T13:48:24.9613617Z', [UpdatedDate] = '2025-07-30T13:48:24.9613618Z'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'CreatedAt', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceTypes]'))
    SET IDENTITY_INSERT [ServiceTypes] ON;
INSERT INTO [ServiceTypes] ([Id], [CompanyId], [CreatedAt], [Description], [IsActive], [Name])
VALUES (1, 1, '2025-07-30T13:48:24.9613654Z', N'Blog posts, articles, and other written content', CAST(1 AS bit), N'Content Writing'),
(2, 1, '2025-07-30T13:48:24.9613659Z', N'Website design and development services', CAST(1 AS bit), N'Web Design'),
(3, 1, '2025-07-30T13:48:24.9613661Z', N'Search engine optimized content creation', CAST(1 AS bit), N'SEO Writing'),
(4, 1, '2025-07-30T13:48:24.9613662Z', N'Digital marketing strategy development', CAST(1 AS bit), N'Strategy Planning'),
(5, 1, '2025-07-30T13:48:24.9613664Z', N'Email campaign development and management', CAST(1 AS bit), N'Email Marketing'),
(6, 2, '2025-07-30T13:48:24.9613665Z', N'Search engine optimization services', CAST(1 AS bit), N'SEO'),
(7, 2, '2025-07-30T13:48:24.9613667Z', N'General writing and content creation', CAST(1 AS bit), N'Writing Service');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompanyId', N'CreatedAt', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[ServiceTypes]'))
    SET IDENTITY_INSERT [ServiceTypes] OFF;
GO

UPDATE [Users] SET [CreatedAt] = '2025-07-30T13:48:24.9613491Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_SaleOrders_ServiceTypeId] ON [SaleOrders] ([ServiceTypeId]);
GO

CREATE INDEX [IX_ServiceTypes_CompanyId] ON [ServiceTypes] ([CompanyId]);
GO

ALTER TABLE [SaleOrders] ADD CONSTRAINT [FK_SaleOrders_ServiceTypes_ServiceTypeId] FOREIGN KEY ([ServiceTypeId]) REFERENCES [ServiceTypes] ([Id]) ON DELETE SET NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250730134825_AddServiceTypeAndNewFields', N'8.0.17');
GO

COMMIT;
GO

