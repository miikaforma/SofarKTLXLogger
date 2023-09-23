namespace SofarKTLXLogger.Settings;

public class LoggerSettings
{
    public const string SectionName = "Logger";
    
    public required string Ip { get; set; }
    public required int Port { get; set; }
    public required uint SerialNumber { get; set; }
}