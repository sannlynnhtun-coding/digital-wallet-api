using System;
using System.Collections.Generic;

namespace DigitalWallet.Database.WalletDbContextModels;

public partial class Wallet
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Title { get; set; }

    public decimal Balance { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public Guid CurrencyId { get; set; }

    public int Status { get; set; }

    public virtual Currency Currency { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
