using DigitalWallet.Database.WalletDbContextModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DigitalWallet.UnitTests
{
    public class TransactionServiceTests
    {
        private readonly Mock<WalletDbContext> _dbContextMock;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _dbContextMock = new Mock<WalletDbContext>();
            _transactionService = new TransactionService(_dbContextMock.Object);
        }

        [Fact]
        public async Task CreateTransaction_ValidTransaction_ReturnsTransaction()
        {
            // Arrange
            var transaction = new Transaction { Id = Guid.NewGuid(), FromWalletId = Guid.NewGuid(), ToWalletId = Guid.NewGuid(), Amount = 50 };

            var mockEntry = new Mock<EntityEntry<Transaction>>();
            mockEntry.Setup(x => x.Entity).Returns(transaction);

            _dbContextMock.Setup(db => db.Transactions.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockEntry.Object); // Use the mock entry here
            _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);

            // Assert
            Assert.NotNull(createdTransaction);
            Assert.Equal(transaction.Id, createdTransaction.Id);
        }

        [Fact]
        public async Task GetTransactionsByWalletId_ExistingWalletId_ReturnsTransactions()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), FromWalletId = walletId, ToWalletId = Guid.NewGuid(), Amount = 50 },
                new Transaction { Id = Guid.NewGuid(), FromWalletId = Guid.NewGuid(), ToWalletId = walletId, Amount = 20 }
            };

            var mockSet = new Mock<DbSet<Transaction>>();
            mockSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transactions.AsQueryable().Provider);
            mockSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transactions.AsQueryable().Expression);
            mockSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transactions.AsQueryable().ElementType);
            mockSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transactions.AsQueryable().GetEnumerator());

            _dbContextMock.Setup(m => m.Transactions).Returns(mockSet.Object);

            // Act
            var result = await _transactionService.GetTransactionsByWalletIdAsync(walletId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetTransactionsByWalletId_NonExistingWalletId_ReturnsEmptyList()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var transactions = new List<Transaction>();

            var mockSet = new Mock<DbSet<Transaction>>();
            mockSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transactions.AsQueryable().Provider);
            mockSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transactions.AsQueryable().Expression);
            mockSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transactions.AsQueryable().ElementType);
            mockSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transactions.AsQueryable().GetEnumerator());

            _dbContextMock.Setup(m => m.Transactions).Returns(mockSet.Object);

            // Act
            var result = await _transactionService.GetTransactionsByWalletIdAsync(walletId);

            // Assert
            Assert.Empty(result);
        }
    }
}
