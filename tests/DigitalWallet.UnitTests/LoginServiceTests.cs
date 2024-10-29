using DigitalWallet.Database.WalletDbContextModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var user = new User { Id = Guid.NewGuid(), Username = "user1", PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=" };

            var users = new List<User> { user }.AsQueryable();
            _dbContextMock.Setup(db => db.Users).ReturnsDbSet(users);

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

            var users = new List<User>().AsQueryable(); // No users
            _dbContextMock.Setup(db => db.Users).ReturnsDbSet(users);

            // Act
            var token = await _loginService.LoginAsync(request);

            // Assert
            Assert.Null(token);
        }
    }
}
