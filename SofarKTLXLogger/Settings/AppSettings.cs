namespace SofarKTLXLogger.Settings;

public class AppSettings
{
    public const string SectionName = "App";
    
    public int Timeout { get; set; } = 15_000; // 15 seconds by default
}