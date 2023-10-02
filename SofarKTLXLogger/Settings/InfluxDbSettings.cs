namespace SofarKTLXLogger.Settings;

public class InfluxDbSettings
{
    public const string SectionName = "InfluxDB";

    public bool Enabled { get; set; }
    public required string Address { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public required string DbName { get; set; }
    public string MetricName { get; set; } = "InverterData";
    public int Timeout { get; set; } = 10_000; // 10 seconds by default
}