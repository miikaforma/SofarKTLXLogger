namespace SofarKTLXLogger.Settings;

public class ProductInfoSettings
{
    public const string SectionName = "ProductInfo";

    public ushort StartRegister { get; set; } = 0x2000;
    public ushort RegisterCount { get; set; } = 0x0E;
}