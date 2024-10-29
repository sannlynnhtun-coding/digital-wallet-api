# DigitalWallet
```sql
USE [DigitalWallet]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 10/29/2024 4:22:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[Id] [uniqueidentifier] NOT NULL,
	[FromWalletId] [uniqueidentifier] NOT NULL,
	[ToWalletId] [uniqueidentifier] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[TransactionType] [varchar](50) NULL,
	[CreatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK__Transact__3214EC0709E166A8] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 10/29/2024 4:22:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wallets]    Script Date: 10/29/2024 4:22:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wallets](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,
	[CurrencyId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Transactions] ([Id], [FromWalletId], [ToWalletId], [Amount], [TransactionType], [CreatedAt]) VALUES (N'284b2741-3180-40f8-938f-547dc4a4b744', N'619011ff-15ae-496a-916c-089da8a7d361', N'a3a1ee0a-946a-4a3d-9f10-134e9cae7698', CAST(20.00 AS Decimal(18, 2)), N'Debit', CAST(N'2024-10-29T06:28:21.893' AS DateTime))
GO
INSERT [dbo].[Transactions] ([Id], [FromWalletId], [ToWalletId], [Amount], [TransactionType], [CreatedAt]) VALUES (N'0bf934d4-58ea-41dd-971c-675e2fe2667a', N'a3a1ee0a-946a-4a3d-9f10-134e9cae7698', N'619011ff-15ae-496a-916c-089da8a7d361', CAST(20.00 AS Decimal(18, 2)), N'Credit', CAST(N'2024-10-29T06:28:21.893' AS DateTime))
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash]) VALUES (N'67926fa7-822e-4817-bcfd-339844e6f67e', N'admin', N'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=')
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash]) VALUES (N'af560817-d384-45c8-a702-861fff38056b', N'user3', N'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=')
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash]) VALUES (N'bf734c0d-7381-4303-9590-8947fcc1b743', N'user2', N'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=')
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash]) VALUES (N'90c72622-d910-49df-8698-de4f413ecab5', N'user1', N'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=')
GO
INSERT [dbo].[Wallets] ([Id], [UserId], [Balance], [CurrencyId]) VALUES (N'619011ff-15ae-496a-916c-089da8a7d361', N'af560817-d384-45c8-a702-861fff38056b', CAST(260.00 AS Decimal(18, 2)), N'42a10123-6612-497a-be58-fc4e3bbbc55b')
GO
INSERT [dbo].[Wallets] ([Id], [UserId], [Balance], [CurrencyId]) VALUES (N'a3a1ee0a-946a-4a3d-9f10-134e9cae7698', N'bf734c0d-7381-4303-9590-8947fcc1b743', CAST(240.00 AS Decimal(18, 2)), N'2f6728d1-c993-4920-8972-a0e0d240221e')
GO
INSERT [dbo].[Wallets] ([Id], [UserId], [Balance], [CurrencyId]) VALUES (N'4938d894-51ee-4c43-8aa3-2a94c1fe6be1', N'90c72622-d910-49df-8698-de4f413ecab5', CAST(100.00 AS Decimal(18, 2)), N'f47fc901-b30b-4ecf-a11d-bb43641c3bc9')
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E41AB47741]    Script Date: 10/29/2024 4:22:09 PM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF__Transacti__Creat__2D27B809]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Wallets] ADD  DEFAULT ((0)) FOR [Balance]
GO
USE [master]
GO
ALTER DATABASE [DigitalWallet] SET  READ_WRITE 
GO

```
dotnet ef dbcontext scaffold "Server=.;Database=DigitalWallet;User Id=sa;Password=sasa@123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o WalletDbContextModels -c WalletDbContext -f
