using DigitalWallet.Database.WalletDbContextModels;
using Moq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DigitalWallet.UnitTests
{
    public class WalletServiceTests
    {
        private readonly Mock<WalletDbContext> _dbContextMock;
        private readonly WalletService _walletService;

        public WalletServiceTests()
        {
            _dbContextMock = new Mock<WalletDbContext>();
            _walletService = new WalletService(_dbContextMock.Object);
        }

        [Fact]
        public async Task CreateWallet_ValidWallet_ReturnsWallet()
        {
            // Arrange
            var wallet = new Wallet { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Balance = 100 };

            var mockEntry = new Mock<EntityEntry<Wallet>>();
            mockEntry.Setup(x => x.Entity).Returns(wallet);

            _dbContextMock.Setup(db => db.Wallets.AddAsync(It.IsAny<Wallet>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockEntry.Object); // Correct return type
            _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var createdWallet = await _walletService.CreateWalletAsync(wallet);

            // Assert
            Assert.NotNull(createdWallet);
            Assert.Equal(wallet.Id, createdWallet.Id);
        }

        [Fact]
        public async Task GetWallet_ExistingWalletId_ReturnsWallet()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var wallet = new Wallet { Id = walletId, UserId = Guid.NewGuid(), Balance = 100 };
            _dbContextMock.Setup(db => db.Wallets.FindAsync(walletId)).ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetWalletAsync(walletId, wallet.UserId); // Provide UserId if needed

            // Assert
            Assert.NotNull(result);
            Assert.Equal(walletId, result.Id);
        }

        [Fact]
        public async Task GetWallet_NonExistingWalletId_ReturnsNull()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            _dbContextMock.Setup(db => db.Wallets.FindAsync(walletId)).ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletService.GetWalletAsync(walletId, Guid.NewGuid()); // Provide UserId if needed

            // Assert
            Assert.Null(result);
        }
    }
}
