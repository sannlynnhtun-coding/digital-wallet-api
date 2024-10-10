namespace DigitalWallet.Common;

public static class DevCode
{
    public static int ToEnum2Int<T>(this T enumValue) where T : Enum
    {
        return Convert.ToInt32(enumValue);
    }
    
    public static T ToInt2Enum<T>(this int value) where T : Enum
    {
        return (T)Enum.ToObject(typeof(T), value);
    }

    public static RouteHandlerBuilder Validator<T>(this RouteHandlerBuilder handlerBuilder)
        where T : class
    {
        handlerBuilder.AddEndpointFilter<EndpointValidatorFilter<T>>();
        return handlerBuilder;
    }

    public static IServiceCollection ConfigureValidator(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

        return services;
    }
}