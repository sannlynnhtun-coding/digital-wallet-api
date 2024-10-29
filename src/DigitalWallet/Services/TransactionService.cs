public class TransactionService
{
    private readonly WalletDbContext _dbContext;

    public TransactionService(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Transaction>> ProcessTransactionAsync(Transaction transaction, Guid userId)
    {
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

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();
        return transaction;
    }

    public async Task<List<Transaction>> GetTransactionsByWalletIdAsync(Guid walletId)
    {
        return await _dbContext.Transactions
            .Where(t => t.FromWalletId == walletId || t.ToWalletId == walletId)
            .ToListAsync();
    }
}
