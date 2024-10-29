using System;
using System.Collections.Generic;

namespace DigitalWallet.Database.WalletDbContextModels;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
}
