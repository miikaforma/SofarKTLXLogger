namespace SofarKTLXLogger.Settings;

public class RealTimeDataSettings
{
    public const string SectionName = "RealTimeData";

    public ushort InverterStartRegister { get; set; } = 0x0000;
    public ushort InverterRegisterCount { get; set; } = 0x0028;
    public ushort PvStartRegister { get; set; } = 0x0010;
    public ushort PvRegisterCount { get; set; } = 0x0010;
}