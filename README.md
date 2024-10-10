# DigitalWallet
The **DigitalWallet** is an E-Commerce User Wallet Service, offering a secure and user-friendly solution for managing funds within an online platform. It is built using ASP.NET Core with a focus on clean architecture and scalability, ensuring a robust and maintainable service.

## ⭐ Give a Star!
If you find the `DigitalWallet` project valuable and appreciate the use of **ASP.NET Core** and **Vertical Slice Architecture**, please consider showing your support by starring this repository! It helps others discover this project and recognizes the effort put into building a comprehensive wallet service.

## Getting Started

This repository provides all the necessary resources to set up and build your own Digital Wallet application. Follow the instructions below to get started:

![DigitalWallet Diagram](https://github.com/thisisnabi/DigitalWallet/assets/3371886/6cc50499-5130-4ec6-b976-8424a4ca5e04)

### Features
- **MultiCurrency**
  - CreateCurrency
  - UpdateRatio
  - GetAll
- **UserWallet**
  - CreateWallet
  - GetBalance
  - Active
  - Suspend
  - ChangeTitle
  - GetTransactions
- **Transactions**
  - IncreaseWalletBalance
  - DecreaseWalletBalance
  - WalletFunds
  - WalletTransactions

These features enable users to manage their wallets and transactions seamlessly, with support for multiple currencies and various wallet statuses like active and suspended.

## Database Setup

To set up the database for the Digital Wallet service, use the following command to scaffold the Entity Framework Core context from your existing SQL Server database:

```bash
dotnet ef dbcontext scaffold "Server=.;Database=DigitalWallet;User Id=sa;Password=sasa@123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o WalletDbContextModels -c WalletDbContext -f
```

### SQL Server Database Creation
Here is the SQL script for creating the **DigitalWallet** database with the required tables and schemas:

```sql
USE [master];
GO

-- Create DigitalWallet Database
CREATE DATABASE [DigitalWallet] ON PRIMARY 
( NAME = N'DigitalWallet', FILENAME = N'/var/opt/mssql/data/DigitalWallet.mdf', SIZE = 8192KB, MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
LOG ON 
( NAME = N'DigitalWallet_log', FILENAME = N'/var/opt/mssql/data/DigitalWallet_log.ldf', SIZE = 8192KB, MAXSIZE = 2048GB, FILEGROWTH = 65536KB );
GO

-- Enable Full-Text Search if available
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
BEGIN
    EXEC [DigitalWallet].[dbo].[sp_fulltext_database] @action = 'enable';
END;
GO

-- Database settings for optimal performance
ALTER DATABASE [DigitalWallet] SET ANSI_NULL_DEFAULT OFF;
ALTER DATABASE [DigitalWallet] SET ANSI_NULLS OFF;
ALTER DATABASE [DigitalWallet] SET AUTO_UPDATE_STATISTICS ON;
ALTER DATABASE [DigitalWallet] SET READ_COMMITTED_SNAPSHOT ON;
GO

-- Create wallet schema and tables
USE [DigitalWallet];
GO

CREATE SCHEMA [wallet];
GO

-- Currencies Table
CREATE TABLE [wallet].[Currencies](
    [Id] [uniqueidentifier] NOT NULL,
      NOT NULL,
      NOT NULL,
    [Ratio] [decimal](18, 6) NOT NULL,
      NOT NULL,
    CONSTRAINT [PK_Currencies] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Wallets Table
CREATE TABLE [wallet].[Wallets](
    [Id] [uniqueidentifier] NOT NULL,
    [UserId] [uniqueidentifier] NOT NULL,
      NULL,
    [Balance] [decimal](18, 6) NOT NULL,
      NOT NULL,
    [CurrencyId] [uniqueidentifier] NOT NULL,
    [Status] [int] NOT NULL,
    CONSTRAINT [PK_Wallets] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Transactions Table
CREATE TABLE [wallet].[Transactions](
    [Id] [uniqueidentifier] NOT NULL,
    [WalletId] [uniqueidentifier] NOT NULL,
      NOT NULL,
    [Amount] [decimal](18, 6) NOT NULL,
      NOT NULL,
    [Kind] [int] NOT NULL,
    [Type] [int] NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Indexes for fast lookups
CREATE UNIQUE NONCLUSTERED INDEX [IX_Currencies_Code] ON [wallet].[Currencies]([Code] ASC);
CREATE NONCLUSTERED INDEX [IX_Transactions_WalletId] ON [wallet].[Transactions]([WalletId] ASC);
CREATE NONCLUSTERED INDEX [IX_Wallets_CurrencyId] ON [wallet].[Wallets]([CurrencyId] ASC);

-- Foreign Key Constraints
ALTER TABLE [wallet].[Transactions] ADD CONSTRAINT [FK_Transactions_Wallets_WalletId] FOREIGN KEY([WalletId])
REFERENCES [wallet].[Wallets]([Id]) ON DELETE CASCADE;
ALTER TABLE [wallet].[Wallets] ADD CONSTRAINT [FK_Wallets_Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [wallet].[Currencies]([Id]) ON DELETE CASCADE;
GO

-- Set the database to read-write mode
USE [master];
GO
ALTER DATABASE [DigitalWallet] SET READ_WRITE;
GO
```

### Explanation:
- **Database Creation**: Sets up the `DigitalWallet` database, along with schemas and tables to store currencies, wallets, and transactions.
- **Schema Management**: Uses the `wallet` schema to organize tables and ensure proper structure.
- **Tables**: Defines `Currencies` for storing exchange rates, `Wallets` for user accounts, and `Transactions` for recording all wallet activities.
- **Indexes**: Adds indexes on common lookup fields (`Code`, `WalletId`, `CurrencyId`) to improve query performance.
- **Foreign Keys**: Establishes relationships between tables for referential integrity, such as linking transactions to wallets and wallets to currencies.

This setup provides a solid foundation for managing user funds and transactions in an e-commerce environment, ensuring secure and efficient handling of financial data.