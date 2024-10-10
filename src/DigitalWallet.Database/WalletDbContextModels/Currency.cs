using System;
using System.Collections.Generic;

namespace DigitalWallet.Database.WalletDbContextModels;

public partial class Currency
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Ratio { get; set; }

    public DateTime ModifiedOnUtc { get; set; }

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
