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
