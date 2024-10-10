namespace DigitalWallet.Features.MultiCurrency.Common;

public class CurrencyConverter
{
    public decimal Convert(CurrencyDto sourceCurrency, CurrencyDto destinaitonCurrency, decimal amount)
        => sourceCurrency.Ratio / destinaitonCurrency.Ratio * amount;
}
