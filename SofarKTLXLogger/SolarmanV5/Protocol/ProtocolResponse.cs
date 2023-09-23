using System.Buffers;

namespace SofarKTLXLogger.SolarmanV5.Protocol;

/// <summary>
/// https://pysolarmanv5.readthedocs.io/en/latest/solarmanv5_protocol.html
/// </summary>
public class ProtocolResponse : ProtocolBase
{
    public Header Header { get; private init; } = null!;
    public ResponsePayload Payload { get; private init; } = null!;
    public Trailer Trailer { get; private init; } = null!;

    public static ProtocolResponse Deserialize(ReadOnlySequence<byte> data)
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