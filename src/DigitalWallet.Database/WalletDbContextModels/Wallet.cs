﻿using System;
using System.Collections.Generic;

namespace DigitalWallet.Database.WalletDbContextModels;

public partial class Wallet
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public decimal Balance { get; set; }

    public Guid CurrencyId { get; set; }
}
