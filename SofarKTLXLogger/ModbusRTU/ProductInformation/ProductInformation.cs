using System.Buffers;
using SofarKTLXLogger.Helpers;
using SofarKTLXLogger.SolarmanV5.Protocol;

namespace SofarKTLXLogger.ModbusRTU.ProductInformation;

public record ProductInformation
{
    public ProductCode ProductCode { get; init; }
    public string SerialNumber { get; init; }
    public string SoftwareVersion { get; init; }
    public string HardwareVersion { get; init; }
    public string DspVersion { get; init; }

    public ProductInformation(Memory<byte> data)
    {
        var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(data));
        reader.TryRead(out _); // Throw away the slave address
        if (!reader.TryRead(out var functionCode) || (FunctionCode)functionCode != FunctionCode.ReadProductInformation)
        {
            throw new InvalidOperationException("Not a valid response");
        }

        reader.TryRead(out var length);
        reader.TryRead(out var registerStart);
        if (reader.TryRead(out var pCode))
        {
            ProductCode = (ProductCode)pCode;
        }

        SerialNumber = reader.ReadString(0x0E);
        SoftwareVersion = reader.ReadString(0x04);
        HardwareVersion = reader.ReadString(0x04);
        DspVersion = reader.ReadString(0x04);
        
        reader.TryRead(out var registerEnd);
        reader.TryRead(out var crc16);
    }

    public static ProductInformation FromProtocolResponse(ProtocolResponse response) => new(response.ModbusFrame);
}