namespace SofarKTLXLogger.SolarmanV5;

public interface ISolarmanV5Client
{
    byte[] GetHwData();
    (byte[] Part1, byte[] Part2) GetRealtimeData();
}