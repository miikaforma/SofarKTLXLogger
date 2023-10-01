using SofarKTLXLogger.SolarmanV5.Protocol;

namespace SofarKTLXLogger.SolarmanV5;

public interface ISolarmanV5Client
{
    Task<ProtocolResponse> SendAsync(Memory<byte> modbusFrame, CancellationToken cancellationToken = default);
    byte[] GetHwData();
    (byte[] Part1, byte[] Part2) GetRealtimeData();
    
    Memory<byte> ResponseData { get; }
}