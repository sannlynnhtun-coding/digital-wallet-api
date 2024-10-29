using DigitalWallet.Database.WalletDbContextModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DigitalWallet.UnitTests
{
    public class LoginServiceTests
    {
        private readonly Mock<WalletDbContext> _dbContextMock;
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            _dbContextMock = new Mock<WalletDbContext>();
            _loginService = new LoginService(_dbContextMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var request = new LoginRequest { Username = "user1", Password = "admin" };
            var user = new User { Id = Guid.NewGuid(), Username = "user1", PasswordHash = "admin" };

            _dbContextMock.Setup(db => db.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), default))
                .ReturnsAsync(user);

            // Act
            var token = await _loginService.LoginAsync(request);

            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var request = new LoginRequest { Username = "invalidUser", Password = "wrongPassword" };

            _dbContextMock.Setup(db => db.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), default))
                .ReturnsAsync((User)null);

            // Act
            var token = await _loginService.LoginAsync(request);

            // Assert
            Assert.Null(token);
        }
    }
}
