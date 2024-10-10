# DigitalWallet
The E-Commerce User Wallet Service is a vital component of any platform, designed to provide users with a secure and convenient way to manage their funds within the ecosystem.

## Give a Star! ⭐
If you find this `DigitalWallet` valuable and believe in the importance of ASP.NET Core, Vertical Slice Architecture, consider showing your support by giving this repository a star!
 
## Getting Started

This repository provides various resources to get you started with building your Digital Wallet application:

![image](https://github.com/thisisnabi/DigitalWallet/assets/3371886/6cc50499-5130-4ec6-b976-8424a4ca5e04)

### Features
- [x] MultiCurrency
  - [x] CreateCurrency
  - [x] UpdateRatio
  - [x] GetAll 
- [x] UserWallet
  - [x] CreateWallet
  - [x] GetBalance
  - [x] Active
  - [x] Suspend
  - [x] ChangeTitle
  - [x] GetTransactions
- [x] Transactions
  - [x] IncreaseWalletBalance
  - [x] DecreaseWalletBalance
  - [x] WalletFunds
  - [x] WalletTransactions



dotnet ef dbcontext scaffold "Server=.;Database=DigitalWallet;User Id=sa;Password=sasa@123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o WalletDbContextModels -c WalletDbContext -f

```sql
USE [master]
GO
/****** Object:  Database [DigitalWallet]    Script Date: 10/11/2024 2:04:20 AM ******/
CREATE DATABASE [DigitalWallet] ON  PRIMARY 
( NAME = N'DigitalWallet', FILENAME = N'/var/opt/mssql/data/DigitalWallet.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DigitalWallet_log', FILENAME = N'/var/opt/mssql/data/DigitalWallet_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DigitalWallet].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DigitalWallet] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DigitalWallet] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DigitalWallet] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DigitalWallet] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DigitalWallet] SET ARITHABORT OFF 
GO
ALTER DATABASE [DigitalWallet] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DigitalWallet] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DigitalWallet] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DigitalWallet] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DigitalWallet] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DigitalWallet] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DigitalWallet] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DigitalWallet] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DigitalWallet] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DigitalWallet] SET  ENABLE_BROKER 
GO
ALTER DATABASE [DigitalWallet] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DigitalWallet] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DigitalWallet] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DigitalWallet] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DigitalWallet] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DigitalWallet] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [DigitalWallet] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DigitalWallet] SET RECOVERY FULL 
GO
ALTER DATABASE [DigitalWallet] SET  MULTI_USER 
GO
ALTER DATABASE [DigitalWallet] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DigitalWallet] SET DB_CHAINING OFF 
GO
EXEC sys.sp_db_vardecimal_storage_format N'DigitalWallet', N'ON'
GO
USE [DigitalWallet]
GO
/****** Object:  Schema [wallet]    Script Date: 10/11/2024 2:04:21 AM ******/
CREATE SCHEMA [wallet]
GO
/****** Object:  Table [wallet].[Currencies]    Script Date: 10/11/2024 2:04:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wallet].[Currencies](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
	[Ratio] [decimal](18, 6) NOT NULL,
	[ModifiedOnUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Currencies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [wallet].[Transactions]    Script Date: 10/11/2024 2:04:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wallet].[Transactions](
	[Id] [uniqueidentifier] NOT NULL,
	[WalletId] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[Amount] [decimal](18, 6) NOT NULL,
	[CreatedOnUtc] [datetime2](7) NOT NULL,
	[Kind] [int] NOT NULL,
	[Type] [int] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [wallet].[Wallets]    Script Date: 10/11/2024 2:04:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wallet].[Wallets](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](30) NULL,
	[Balance] [decimal](18, 6) NOT NULL,
	[CreatedOnUtc] [datetime2](7) NOT NULL,
	[CurrencyId] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Wallets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Currencies_Code]    Script Date: 10/11/2024 2:04:21 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Currencies_Code] ON [wallet].[Currencies]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Transactions_WalletId]    Script Date: 10/11/2024 2:04:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_Transactions_WalletId] ON [wallet].[Transactions]
(
	[WalletId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Wallets_CurrencyId]    Script Date: 10/11/2024 2:04:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_Wallets_CurrencyId] ON [wallet].[Wallets]
(
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [wallet].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_Wallets_WalletId] FOREIGN KEY([WalletId])
REFERENCES [wallet].[Wallets] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [wallet].[Transactions] CHECK CONSTRAINT [FK_Transactions_Wallets_WalletId]
GO
ALTER TABLE [wallet].[Wallets]  WITH CHECK ADD  CONSTRAINT [FK_Wallets_Currencies_CurrencyId] FOREIGN KEY([CurrencyId])
REFERENCES [wallet].[Currencies] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [wallet].[Wallets] CHECK CONSTRAINT [FK_Wallets_Currencies_CurrencyId]
GO
USE [master]
GO
ALTER DATABASE [DigitalWallet] SET  READ_WRITE 
GO

```
