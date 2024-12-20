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
    var wallet = await walletService.GetWalletAsync(id, userId!.Value);
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
        var transactions = await walletService.ProcessTransactionAsync(transaction, userId!.Value);
        return Results.Created($"/transactions/{transactions[0].Id}", transactions);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/wallets/{walletId}/transactions", async (Guid walletId, int page, WalletDbContext db, HttpContext context) =>
{
    var auth = context.RequestServices.GetRequiredService<CustomAuthorizeAttribute>();
    if (!await auth.AuthorizeAsync(context))
        return Results.Unauthorized();

    // Get the user ID from the context
    var userId = auth.GetUserIdFromContext(context);
    if (userId == null)
        return Results.Unauthorized();

    // Fetch the wallet to ensure ownership
    var wallet = await db.Wallets.FindAsync(walletId);
    if (wallet == null || wallet.UserId != userId)
        return Results.BadRequest("Invalid account number");

    // Pagination
    const int pageSize = 20;
    var transactions = await db.Transactions
        .Where(t => t.FromWalletId == walletId || t.ToWalletId == walletId)
        .OrderByDescending(t => t.CreatedAt) // Most recent first
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(t => new
        {
            t.Id,
            Amount = t.FromWalletId == walletId ? -t.Amount : t.Amount, // Debit as negative, credit as positive
            t.CreatedAt,
            t.TransactionType
        })
        .ToListAsync();

    return Results.Ok(transactions);
});

app.Run();
