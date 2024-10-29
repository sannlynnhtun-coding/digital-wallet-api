public class LoginService
{
    private readonly WalletDbContext _dbContext;

    public LoginService(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return null;

        return GenerateToken(user.Id);
    }

    private string GenerateToken(Guid userId)
    {
        var token = $"{userId}.{Guid.NewGuid()}.{DateTime.UtcNow.AddHours(1)}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        using (var sha256 = SHA256.Create())
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(passwordBytes);
            var hashString = Convert.ToBase64String(hashBytes);
            return hashString == storedHash;
        }
    }
}
