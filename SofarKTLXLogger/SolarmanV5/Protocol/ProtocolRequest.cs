using System.Buffers;
using System.Buffers.Binary;
using SofarKTLXLogger.ModbusRTU;
using SofarKTLXLogger.SolarmanV5.Enums;

namespace SofarKTLXLogger.SolarmanV5.Protocol;

/// <summary>
/// https://pysolarmanv5.readthedocs.io/en/latest/solarmanv5_protocol.html
/// </summary>
public class ProtocolRequest : ProtocolBase
{
    public Header Header { get; private init; } = null!;
    public RequestPayload Payload { get; private init; } = null!;
    public Trailer Trailer { get; private init; } = null!;
    
    public static ProtocolRequest Deserialize(ReadOnlySequence<byte> data)
    {
        var reader = new SequenceReader<byte>(data);

        var header = DeserializeHeader(ref reader);
        var request = DeserializeRequestPayload(ref reader, header.Length - 0x0F);
        var trailer = DeserializeTrailer(ref reader);

        var protocolRequest = new ProtocolRequest
        {
            Header = header,
            Payload = request,
            Trailer = trailer,
        };

        return protocolRequest;
    }
    
    public static byte[] Serialize<TFunction>(uint serialNumber, BaseFrame<TFunction> frame)
    {
        var frameData = frame.GetFrameData();
        var header = new Header
        {
            Length = (ushort)(0x0F + frameData.Length),
            ControlCode = ControlCode.Request,
            Serial = 0x0000,
            LoggerSerial = serialNumber,
        };

        var payload = new RequestPayload
        {
            FrameType = FrameType.Inverter,
            SensorType = 0x0000,
            TotalWorkingTime = 0x00000000,
            PowerOnTime = 0x00000000,
            OffsetTime = 0x00000000,
            ModbusRtuFrame = frameData,
        };

        return ToBytes(header, payload);
    }

    public override string ToString()
    {
        return $@"Header: {Header}
---
Payload: {Payload}
---
Trailer: {Trailer}";
    }
    
    private static byte[] ToBytes(Header header, RequestPayload payload)
    {
        var modbusRtuFrameLength = payload.ModbusRtuFrame.Length;
        var totalLength = 11 /* Header size */ + 15 /* Request payload */ + 2 /* Trailer */ + modbusRtuFrameLength;
        var buffer = new byte[totalLength];

        buffer[0] = Header.Magic;
    
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(1, 2), header.Length);
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(3, 2), (ushort)header.ControlCode);
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(5, 2), header.Serial);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(7, 4), header.LoggerSerial);

        BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(11, 2), (ushort)payload.FrameType);
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(13, 2), payload.SensorType);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(15, 4), payload.TotalWorkingTime);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(19, 4), payload.PowerOnTime);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(23, 4), payload.OffsetTime);

        payload.ModbusRtuFrame.Span.CopyTo(buffer.AsSpan(26, modbusRtuFrameLength));

        var checksum = 0;
        for (var i = 1; i < buffer.Length - 2; i++)
        {
            checksum += buffer[i] & 255;
        }
        
        buffer[26 + modbusRtuFrameLength] = (byte)(checksum & 255);
        buffer[^1] = Trailer.End;

        return buffer;
    }
}