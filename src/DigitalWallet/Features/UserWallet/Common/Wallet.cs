namespace DigitalWallet.Features.UserWallet.Common;

public class WalletDto
{
    public Guid Id { get; set; } 

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public decimal Balance { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public Guid CurrencyId { get; set; }

    public CurrencyDto Currency { get; set; } = null!;

    public WalletStatus Status { get; set; }

    public ICollection<TransactionDto> Transactions { get; set; } = default!;

    public static WalletDto Create(UserId userId, CurrencyId currencyId, string title)
    {
        return new WalletDto
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            CurrencyId = currencyId.Value,
            Title = title,
            Balance = 0,
            CreatedOnUtc = DateTime.UtcNow,
            Status = WalletStatus.Active
        };
    }

    internal void IncreaseBalance(decimal amount)
    {
        Balance += amount;
    }

    internal void DecreaseBalance(decimal amount)
    {
        Balance -= amount;
    }

    internal void UpdateTitle(string title)
    {
        Title = title;
    }

    internal void Activate()
    {
        Status = WalletStatus.Active;
    }

    internal void Suspend()
    {
        Status = WalletStatus.Suspend;
    }

    public WalletDto()
    {
        //EF
    }
}