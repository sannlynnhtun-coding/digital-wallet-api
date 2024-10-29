public class WalletService
{
    private readonly WalletDbContext _dbContext;

    public WalletService(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Wallet> CreateWalletAsync(Wallet wallet)
    {
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();
        return wallet;
    }

    public async Task<Wallet?> GetWalletAsync(Guid walletId, Guid userId)
    {
        return await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);
    }

    public async Task<List<Transaction>> ProcessTransactionAsync(Transaction transaction, Guid userId)
    {
        // Validate wallet ownership and balances
        var fromWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == transaction.FromWalletId && w.UserId == userId);
        var toWallet = await _dbContext.Wallets.FindAsync(transaction.ToWalletId);

        if (fromWallet == null || toWallet == null || fromWallet.Balance < transaction.Amount)
        {
            throw new ArgumentException("Invalid transaction.");
        }

        // Create debit and credit records
        fromWallet.Balance -= transaction.Amount;
        toWallet.Balance += transaction.Amount;

        var debitTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            FromWalletId = fromWallet.Id,
            ToWalletId = fromWallet.Id,
            Amount = transaction.Amount,
            CreatedAt = DateTime.UtcNow,
            TransactionType = "Debit"
        };

        var creditTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            FromWalletId = toWallet.Id,
            ToWalletId = toWallet.Id,
            Amount = transaction.Amount,
            CreatedAt = DateTime.UtcNow,
            TransactionType = "Credit"
        };

        _dbContext.Transactions.Add(debitTransaction);
        _dbContext.Transactions.Add(creditTransaction);
        await _dbContext.SaveChangesAsync();

        return new List<Transaction> { debitTransaction, creditTransaction };
    }
}
