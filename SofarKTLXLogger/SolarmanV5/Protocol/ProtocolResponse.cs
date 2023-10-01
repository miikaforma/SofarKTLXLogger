using System.Buffers;
using SofarKTLXLogger.ModbusRTU;

namespace SofarKTLXLogger.SolarmanV5.Protocol;

/// <summary>
/// https://pysolarmanv5.readthedocs.io/en/latest/solarmanv5_protocol.html
/// </summary>
public class ProtocolResponse : ProtocolBase
{
    private Header Header { get; init; } = null!;
    private ResponsePayload Payload { get; init; } = null!;
    private Trailer Trailer { get; init; } = null!;

    public Memory<byte> ModbusFrame => Payload.ModbusRtuFrame;

    public Memory<byte> GetSpecificFunctionFrame(FunctionCode functionCode)
    {
        var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(ModbusFrame));

        while (!reader.End)
        {
            reader.TryRead(out _); // Throw away the slave address
            if (reader.TryRead(out var fCode) && (FunctionCode)fCode == functionCode)
            {
                reader.TryRead(out var length);
                return ModbusFrame[(reader.Position.GetInteger() - 3)..(reader.Position.GetInteger() + length + 2)];
            }

            {
                reader.TryRead(out var length);
                reader.Advance(length + 2);
            }
        }

        throw new Exception($"ModbusFrame doesn't contain the wanted FunctionCode: {functionCode}");
    }

    public static ProtocolResponse FromMemory(Memory<byte> data)
    {
        return FromReadonlySequence(new ReadOnlySequence<byte>(data));
    }
    
    public static ProtocolResponse FromReadonlySequence(ReadOnlySequence<byte> data)
    {
        var reader = new SequenceReader<byte>(data);

        var header = DeserializeHeader(ref reader);
        var request = DeserializeResponsePayload(ref reader, header.Length - 0x0E);
        var trailer = DeserializeTrailer(ref reader);

        var protocolResponse = new ProtocolResponse
        {
            Header = header,
            Payload = request,
            Trailer = trailer,
        };

        return protocolResponse;
    }

    public override string ToString()
    {
        return $@"Header: {Header}
---
Payload: {Payload}
---
Trailer: {Trailer}";
    }
}