using System.Buffers.Binary;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

public class ReadRealtimeData : BaseFrame<ReadRealtimeData>
{
    private readonly ushort _startingAddress;
    private readonly ushort _numberOfRegisters;

    public ReadRealtimeData(ushort startingAddress, ushort numberOfRegisters) 
        : base(0x01, FunctionCode.ReadRealTimeData)
    {
        _startingAddress = startingAddress;
        _numberOfRegisters = numberOfRegisters;
    }
    
    protected override byte[] GetData()
    {
        var result = new byte[sizeof(ushort) * 2];

        BinaryPrimitives.WriteUInt16BigEndian(result.AsSpan(0, sizeof(ushort)), _startingAddress);
        BinaryPrimitives.WriteUInt16BigEndian(result.AsSpan(sizeof(ushort), sizeof(ushort)), _numberOfRegisters);

        return result;
    }
}