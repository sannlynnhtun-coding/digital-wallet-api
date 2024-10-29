using System;
using System.Collections.Generic;

namespace DigitalWallet.Database.WalletDbContextModels;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid FromWalletId { get; set; }

    public Guid ToWalletId { get; set; }

    public decimal Amount { get; set; }

    public string? TransactionType { get; set; }

    public DateTime CreatedAt { get; set; }
}
