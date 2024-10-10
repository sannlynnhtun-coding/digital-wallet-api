namespace DigitalWallet.Features.Transactions.Common;

public class TransactionDto
{
    public TransactionId Id { get; private set; } = null!;

    public WalletId WalletId { get; private set; } = null!;

    public WalletDto Wallet { get; private set; } = null!;

    public string Description { get; private set; } = string.Empty;

    public decimal Amount { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public TransactionKind Kind { get; private set; }

    public TransactionType Type { get; private set; }

    public static TransactionDto CreateIncreaseUserTransaction(WalletId walletId, decimal amount, string description)
    {
        return new TransactionDto
        {
            Id = TransactionId.CreateUniqueId(),
            WalletId = walletId,
            Amount = amount,
            Kind = TransactionKind.Incremental,
            Type = TransactionType.User,
            Description = description,
            CreatedOnUtc = DateTime.UtcNow,
        };
    }

    public static TransactionDto CreateDecreaseUserTransaction(WalletId walletId, decimal amount, string description)
    {
        return new TransactionDto
        {
            Id = TransactionId.CreateUniqueId(),
            WalletId = walletId,
            Amount = amount,
            Kind = TransactionKind.Decremental,
            Type = TransactionType.User,
            Description = description,
            CreatedOnUtc = DateTime.UtcNow,
        };
    }

    public static TransactionDto CreateSourceFundsTransaction(WalletId sourceWalletId, decimal amount, string description, DateTime dateTime)
    {
        return new TransactionDto
        {
            Id = TransactionId.CreateUniqueId(),
            WalletId = sourceWalletId,
            Amount = amount,
            Kind = TransactionKind.Decremental,
            Type = TransactionType.Funds,
            Description = description,
            CreatedOnUtc = dateTime,
        };
    }

    public static TransactionDto CreateDestinationFundsTransaction(WalletId destinationWalletId, decimal amount, string description, DateTime dateTime)
    {
        return new TransactionDto
        {
            Id = TransactionId.CreateUniqueId(),
            WalletId = destinationWalletId,
            Amount = amount,
            Kind = TransactionKind.Incremental,
            Type = TransactionType.Funds,
            Description = description,
            CreatedOnUtc = dateTime,
        };
    }

    private TransactionDto()
    {
        //EF
    }
}