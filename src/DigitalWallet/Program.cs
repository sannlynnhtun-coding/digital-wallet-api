using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database context setup
builder.Services.AddDbContext<WalletDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// Register the custom authorize attribute as a service
builder.Services.AddScoped<CustomAuthorizeAttribute>();

builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<TransactionService>();

// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Digital Wallet API", Version = "v1" });

    // Add security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    // Add security requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Use Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital Wallet API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.MapPost("/login", async (LoginRequest request, LoginService loginService) =>
{
    var token = await loginService.LoginAsync(request);
    return token != null ? Results.Ok(new { token }) : Results.Unauthorized();
});

app.MapPost("/wallets", async (Wallet wallet, WalletService walletService, HttpContext context) =>
{
    var auth = context.RequestServices.GetRequiredService<CustomAuthorizeAttribute>();
    if (!await auth.AuthorizeAsync(context))
        return Results.Unauthorized();

    var createdWallet = await walletService.CreateWalletAsync(wallet);
    return Results.Created($"/wallets/{createdWallet.Id}", createdWallet);
});

app.MapGet("/wallets/{id}", async (Guid id, WalletService walletService, HttpContext context) =>
{
    var auth = context.RequestServices.GetRequiredService<CustomAuthorizeAttribute>();
    if (!await auth.AuthorizeAsync(context))
        return Results.Unauthorized();

    var userId = auth.GetUserIdFromContext(context);
    var wallet = await walletService.GetWalletAsync(id, userId.Value);
    return wallet is not null ? Results.Ok(wallet) : Results.NotFound();
});

app.MapPost("/transactions", async (Transaction transaction, WalletService walletService, HttpContext context) =>
{
    var auth = context.RequestServices.GetRequiredService<CustomAuthorizeAttribute>();
    if (!await auth.AuthorizeAsync(context))
        return Results.Unauthorized();

    var userId = auth.GetUserIdFromContext(context);
    try
    {
        var transactions = await walletService.ProcessTransactionAsync(transaction, userId.Value);
        return Results.Created($"/transactions/{transactions[0].Id}", transactions);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/transactions", async (Transaction transaction, TransactionService transactionService, HttpContext context) =>
{
    var auth = context.RequestServices.GetRequiredService<CustomAuthorizeAttribute>();
    if (!await auth.AuthorizeAsync(context))
        return Results.Unauthorized();

    var userId = auth.GetUserIdFromContext(context);
    try
    {
        var transactions = await transactionService.ProcessTransactionAsync(transaction, userId.Value);
        return Results.Created($"/transactions/{transactions[0].Id}", transactions);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

// Token Generation Method
string GenerateToken(Guid userId)
{
    var token = $"{userId}.{Guid.NewGuid()}.{DateTime.UtcNow.AddHours(1)}";
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
}

// Password Verification Method
bool VerifyPassword(string password, string storedHash)
{
    // Hash the provided password
    using (var sha256 = SHA256.Create())
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(passwordBytes);
        var hashString = Convert.ToBase64String(hashBytes);

        // Compare the hashed password with the stored hash
        return hashString == storedHash;
    }
}

// Run the application
app.Run();

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

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

public class WalletService
{
    private readonly WalletDbContext _dbContext;

    public WalletService(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Wallet> CreateWalletAsync(Wallet wallet)
    {
        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync();
        return wallet;
    }

    public async Task<Wallet?> GetWalletAsync(Guid walletId, Guid userId)
    {
        return await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);
    }

    public async Task<List<Transaction>> ProcessTransactionAsync(Transaction transaction, Guid userId)
    {
        // Validate wallet ownership and balances
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
}

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
