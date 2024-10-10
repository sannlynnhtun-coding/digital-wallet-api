namespace DigitalWallet.Features.MultiCurrency.Common;

public class CurrencyService(WalletDbContext dbContext)
{
    private readonly WalletDbContext _dbContext = dbContext;

    public async Task<CurrencyId> CreateAsync(string code, string name, decimal ratio,
        CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Currencies.AnyAsync(x => x.Code == code, cancellationToken))
        {
            DuplicateCurrencyException.Throw(code);
        }

        if (ratio == 0)
        {
            InvalidCurrencyRatioException.Throw();
        }

        var currencyDto = CurrencyDto.Create(code, name, ratio);
        var currency = new Currency()
        {
            Id = Guid.NewGuid(), // Assign a new GUID or use an existing one if necessary.
            Code = currencyDto.Code,
            Name = currencyDto.Name,
            Ratio = currencyDto.Ratio,
            ModifiedOnUtc = DateTime.UtcNow // Set the current UTC time for modification.
        };

        _dbContext.Currencies.Add(currency);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CurrencyId(currency.Id);
    }

    public async Task UpdateRationAsync(CurrencyId currencyId, decimal ratio, CancellationToken cancellationToken)
    {
        if (ratio == 0)
        {
            InvalidCurrencyRatioException.Throw();
        }

        var currency = await _dbContext.Currencies.FirstOrDefaultAsync(x => x.Id == currencyId.Value, cancellationToken);
        if (currency is null)
        {
            CurrencyNotFoundException.Throw(currencyId);
        }

        currency.Ratio = ratio;
        currency.ModifiedOnUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsCurrencyIdValidAsync(CurrencyId currencyId, CancellationToken ct)
    {
        return await _dbContext.Currencies.AnyAsync(x => x.Id == currencyId.Value, ct);
    }
}