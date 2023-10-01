using SofarKTLXLogger.ModbusRTU.ProductInformation;

namespace SofarKTLXLogger.Helpers;

public static class EnumExtensions
{
    public static string ProductCodeToString(this ProductCode productCode)
    {
        return productCode switch
        {
            ProductCode._5KW => "5KW",
            ProductCode._6KW => "6KW",
            ProductCode._8KW => "8KW",
            ProductCode._10KW => "10KW",
            ProductCode._12KW => "12KW",
            ProductCode._15KW => "15KW",
            ProductCode._17KW => "17KW",
            ProductCode._20KW => "20KW",
            ProductCode._25KW => "25KW",
            ProductCode._30KW => "30KW",
            _ => throw new ArgumentOutOfRangeException(nameof(productCode), productCode, null)
        };
    }
}