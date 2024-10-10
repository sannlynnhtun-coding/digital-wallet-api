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
}