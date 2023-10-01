namespace SofarKTLXLogger.Settings;

public class RealTimeDataSettings
{
    public const string SectionName = "RealTimeData";

    public ushort InverterStartRegister { get; set; } = 0x0000;
    public ushort InverterRegisterCount { get; set; } = 0x0028;
    public bool PvEnabled { get; set; }
    public ushort PvStartRegister { get; set; } = 0x0105;
    public ushort PvRegisterCount { get; set; } = 0x0018;
}