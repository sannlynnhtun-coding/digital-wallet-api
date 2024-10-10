namespace DigitalWallet.Features.MultiCurrency.Common;

public class CurrencyDto
{
    public CurrencyId Id { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public decimal Ratio { get; private set; }

    public DateTime ModifiedOnUtc { get; private set; }

    public ICollection<WalletDto> Wallets { get; private set; } = default!;

    public static CurrencyDto Create(string code, string name, decimal ratio)
        => new()
        {
            Id = CurrencyId.CreateUniqueId(),
            Ratio = ratio,
            Code = code,
            Name = name,
            ModifiedOnUtc = DateTime.UtcNow
        };

    private CurrencyDto()
    {
        //EF
    }
}