using System.Buffers.Binary;

namespace SofarKTLXLogger.ModbusRTU.ProductInformation;

public class ReadProductInformation : BaseFrame<ReadProductInformation>
{
    private readonly ushort _startingAddress;
    private readonly ushort _numberOfRegisters;

    public ReadProductInformation(ushort startingAddress, ushort numberOfRegisters) 
        : base(0x01, FunctionCode.ReadProductInformation)
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