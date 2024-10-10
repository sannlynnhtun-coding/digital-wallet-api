namespace DigitalWallet.IntegrationTests;

public class GetAllCurrencies
{
    [Fact]
    public async Task GetAllCurrencies_ThereIsCurrencyInDatabase_ReturnListOfCurrencies()
    {
        // Arrange
        var expectedCurrencies = new GetCurrencyResponse()
        {
            Code = "rial",
            Name = "rial",
            Ratio = 1
        };

        const string inMemoryDatabaseName = "testDatabase";
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<WalletDbContext>();
        dbContextOptionsBuilder.UseInMemoryDatabase(inMemoryDatabaseName);
        var walletDbContextReadOnly = new WalletDbContext(dbContextOptionsBuilder.Options);

        var builder = new DbContextOptionsBuilder<WalletDbContext>();
        builder.UseInMemoryDatabase(inMemoryDatabaseName);
        var walletDbContext = new WalletDbContext(builder.Options);
        var currencyDto = CurrencyDto.Create("rial", "rial", 1);
        var currency = new Currency()
        {
            Id = currencyDto.Id.Value,
            Code = currencyDto.Code,
            Name = currencyDto.Name,
            Ratio = currencyDto.Ratio,
            ModifiedOnUtc = currencyDto.ModifiedOnUtc
        };
        walletDbContext.Currencies.Add(currency);
        await walletDbContext.SaveChangesAsync();

        // Act
        var sut = await Endpoint.GetCurrencies(walletDbContextReadOnly, CancellationToken.None);

        // Assert
        sut.Should().OnlyContain(currenciesDto =>
            currenciesDto.Ratio == expectedCurrencies.Ratio &&
            currenciesDto.Code == expectedCurrencies.Code &&
            currenciesDto.Name == expectedCurrencies.Name &&
            !string.IsNullOrWhiteSpace(currenciesDto.Id));
    }
}