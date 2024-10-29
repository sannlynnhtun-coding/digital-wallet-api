using DigitalWallet.Database.WalletDbContextModels;
using Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Moq.EntityFrameworkCore;

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
            var wallets = new List<Wallet>();
            var mockSet = wallets.AsQueryable().BuildMockDbSet();
            _dbContextMock.Setup(db => db.Wallets).Returns(mockSet.Object);
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
            var wallets = new List<Wallet> { wallet };
            _dbContextMock.Setup(db => db.Wallets).ReturnsDbSet(wallets);
            _dbContextMock.Setup(db => db.Wallets.FindAsync(walletId)).ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetWalletAsync(walletId, wallet.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(walletId, result.Id);
        }

        [Fact]
        public async Task GetWallet_NonExistingWalletId_ReturnsNull()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var wallets = new List<Wallet>();
            _dbContextMock.Setup(db => db.Wallets).ReturnsDbSet(wallets);
            _dbContextMock.Setup(db => db.Wallets.FindAsync(walletId)).ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletService.GetWalletAsync(walletId, Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }

    public static class MockDbSetExtensions
    {
        public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> source) where T : class
        {
            var mock = new Mock<DbSet<T>>();
            mock.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(source.GetEnumerator()));
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(source.Provider));
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
            return mock;
        }
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
    }

    internal class TestAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IQueryable CreateQuery(Expression expression) => _inner.CreateQuery(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression) => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new TestAsyncEnumerable<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Execute<TResult>(expression);
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }

        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}
