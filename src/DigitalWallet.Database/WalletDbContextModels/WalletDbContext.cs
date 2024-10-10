using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Database.WalletDbContextModels;

public partial class WalletDbContext : DbContext
{
    public WalletDbContext()
    {
    }

    public WalletDbContext(DbContextOptions<WalletDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=DigitalWallet;User Id=sa;Password=sasa@123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currencies", "wallet");

            entity.HasIndex(e => e.Code, "IX_Currencies_Code").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Ratio).HasColumnType("decimal(18, 6)");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions", "wallet");

            entity.HasIndex(e => e.WalletId, "IX_Transactions_WalletId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions).HasForeignKey(d => d.WalletId);
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallets", "wallet");

            entity.HasIndex(e => e.CurrencyId, "IX_Wallets_CurrencyId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Title).HasMaxLength(30);

            entity.HasOne(d => d.Currency).WithMany(p => p.Wallets).HasForeignKey(d => d.CurrencyId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
