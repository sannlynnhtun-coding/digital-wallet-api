using DigitalWallet.Database.WalletDbContextModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq.EntityFrameworkCore;

namespace DigitalWallet.UnitTests
{
    public class TransactionServiceTests
    {
        private Mock<WalletDbContext> _dbContextMock;
        private TransactionService _transactionService;

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
            var transactions = new List<Transaction>();
            _dbContextMock.Setup(db => db.Transactions).ReturnsDbSet(transactions);
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
            _dbContextMock.Setup(db => db.Transactions).ReturnsDbSet(transactions);

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
            _dbContextMock.Setup(db => db.Transactions).ReturnsDbSet(new List<Transaction>());

            // Act
            var result = await _transactionService.GetTransactionsByWalletIdAsync(walletId);

            // Assert
            Assert.Empty(result);
        }
    }
}
