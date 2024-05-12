namespace SofarKTLXLogger.Settings;

public class TimescaleDbSettings
{
    public const string SectionName = "TimescaleDB";

    public bool Enabled { get; set; }
    public required string ConnectionString { get; set; }
    public required int PlantId { get; set; }
    public required int DeviceId { get; set; }
    public string TableName { get; set; } = "solarman_inverter_data";
}