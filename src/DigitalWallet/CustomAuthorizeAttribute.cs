public class CustomAuthorizeAttribute
{
    private readonly WalletDbContext _dbContext;

    public CustomAuthorizeAttribute(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AuthorizeAsync(HttpContext context)
    {
        var tokenEncoded = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(tokenEncoded))
        {
            return false;
        }

        try
        {
            var tokenDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(tokenEncoded));
            var parts = tokenDecoded.Split('.');
            if (parts.Length != 3) return false;

            var userId = Guid.Parse(parts[0]);
            var tokenExpiry = DateTime.Parse(parts[2]);

            if (tokenExpiry < DateTime.UtcNow) return false;

            return await _dbContext.Users.AnyAsync(u => u.Id == userId);
        }
        catch
        {
            return false;
        }
    }

    public Guid? GetUserIdFromContext(HttpContext context)
    {
        var tokenEncoded = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(tokenEncoded))
        {
            return null; // No token found
        }

        try
        {
            var tokenDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(tokenEncoded));
            var parts = tokenDecoded.Split('.');
            if (parts.Length != 3) return null;

            var userId = Guid.Parse(parts[0]); // Extracting the user ID from the token
            return userId;
        }
        catch
        {
            return null; // Invalid token format
        }
    }

}
