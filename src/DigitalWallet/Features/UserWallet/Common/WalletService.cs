using DigitalWallet.Common;

namespace DigitalWallet.Features.UserWallet.Common;

public class WalletService(CurrencyService currencyService, WalletDbContext dbContext)
{
    private readonly CurrencyService _currencyService = currencyService;
    private readonly WalletDbContext _dbContext = dbContext;

    public async Task<WalletId> CreateAsync(UserId userId, CurrencyId currencyId, string title,
        CancellationToken cancellationToken)
    {
        if (!await _currencyService.IsCurrencyIdValidAsync(currencyId, cancellationToken))
        {
            InvalidCurrencyException.Throw(currencyId);
        }

        if (await _dbContext.Wallets.AnyAsync(x => x.UserId == userId.Value &&
                                                   x.CurrencyId == currencyId.Value, cancellationToken))
        {
            WalletAlreadyExistsException.Throw(userId, currencyId);
        }

        var walletDto = WalletDto.Create(userId, currencyId, title);
        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = walletDto.UserId,
            CurrencyId = walletDto.CurrencyId,
            Title = title,
            Balance = 0,
            CreatedOnUtc = DateTime.UtcNow,
            Status = WalletStatus.Active.ToEnum2Int()
        };
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new WalletId(walletDto.Id);
    }

    private async Task<WalletDto> GetWalletAsync(WalletId walletId, CancellationToken cancellationToken)
    {
        var wallet = await _dbContext.Wallets
            .Include(x => x.Currency)
            .FirstOrDefaultAsync(x => x.Id == walletId.Value, cancellationToken);

        if (wallet is null)
        {
            WalletNotFoundException.Throw(walletId);
        }

        return new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            Title = wallet.Title!,
            Balance = wallet.Balance,
            CreatedOnUtc = wallet.CreatedOnUtc,
            CurrencyId = wallet.CurrencyId,
            Status = wallet.Status.ToInt2Enum<WalletStatus>()
        };
    }

    internal async Task SuspendAsync(WalletId walletId, CancellationToken cancellationToken)
    {
        var wallet = await GetWalletAsync(walletId, cancellationToken);

        wallet.Suspend();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    internal async Task ActiveAsync(WalletId walletId, CancellationToken cancellationToken)
    {
        var wallet = await GetWalletAsync(walletId, cancellationToken);
        wallet.Activate();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    internal async Task ChangeTitleAsync(WalletId walletId, string title, CancellationToken cancellationToken)
    {
        var wallet = await GetWalletAsync(walletId, cancellationToken);

        wallet.UpdateTitle(title);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    internal async Task<bool> IsWalletAvailableAsync(WalletId walletId, CancellationToken cancellationToken)
    {
        var wallet = await GetWalletAsync(walletId, cancellationToken);
        return wallet.Status == WalletStatus.Active;
    }

    internal async Task IncreaseBalanceAsync(WalletId walletId, decimal amount, CancellationToken cancellationToken)
    {
        var wallet = await GetWalletAsync(walletId, cancellationToken);

        wallet.IncreaseBalance(amount);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    internal async Task DecreaseBalanceAsync(WalletId walletId, decimal amount, CancellationToken cancellationToken)
    {
        var wallet = await GetWalletAsync(walletId, cancellationToken);

        if (wallet.Balance - amount < 0)
        {
            InsufficientBalanceException.Throw();
        }

        wallet.DecreaseBalance(amount);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    internal async Task<bool> IsUserOwnedAsync(List<WalletId> WalletIds, CancellationToken ct)
    {
        return (await _dbContext.Wallets.Where(x => WalletIds.Select(x => x.Value).Contains(x.Id))
                .ToListAsync(ct))
            .DistinctBy(x => x.UserId)
            .Count() == 1;
    }

    internal async Task<decimal> WalletFundsAsync(WalletId sourceWalletId, WalletId destinationWalletId, decimal amount,
        CancellationToken cancellationToken)
    {
        var walletSource = await GetWalletAsync(sourceWalletId, cancellationToken);
        var walletDestination = await GetWalletAsync(destinationWalletId, cancellationToken);

        walletSource.DecreaseBalance(amount);

        var destinationAmount = walletSource.Currency.Ratio / walletDestination.Currency.Ratio * amount;
        walletDestination.IncreaseBalance(destinationAmount);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return destinationAmount;
    }
}