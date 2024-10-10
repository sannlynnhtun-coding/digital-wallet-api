using System;
using System.Collections.Generic;

namespace DigitalWallet.Database.WalletDbContextModels;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public string Description { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public int Kind { get; set; }

    public int Type { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
