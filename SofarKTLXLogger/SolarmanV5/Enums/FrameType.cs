namespace SofarKTLXLogger.SolarmanV5.Enums;

public enum FrameType : byte
{
    Unknown = 0x00, // Solarman Cloud or keep alive?
    Logger = 0x01,
    Inverter = 0x02,
}