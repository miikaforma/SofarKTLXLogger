using System.Buffers;
using SofarKTLXLogger.Helpers;
using SofarKTLXLogger.SolarmanV5.Enums;

namespace SofarKTLXLogger.SolarmanV5.Protocol;

/// <summary>
/// https://pysolarmanv5.readthedocs.io/en/latest/solarmanv5_protocol.html
/// </summary>
public class ProtocolBase
{
    protected static Header DeserializeHeader(ref SequenceReader<byte> reader)
    {
        if (!reader.TryRead(out var magic) || magic != Header.Magic)
        {
            throw new InvalidOperationException(
                $"Invalid magic in the data. Expected {Header.Magic} but got {magic}");
        }
        
        Header header;
        if (reader.TryReadLittleEndian(out ushort length) &&
            reader.TryReadLittleEndian(out ushort controlCode) &&
            reader.TryReadLittleEndian(out ushort serial) &&
            reader.TryReadLittleEndian(out uint loggerSerial))
        {
            header = new Header
            {
                Length = length,
                ControlCode = (ControlCode)controlCode,
                Serial = serial,
                LoggerSerial = loggerSerial,
            };
        }
        else
        {
            throw new InvalidDataException($"Insufficient data to read {nameof(Header)}");
        }

        return header;
    }
    
    protected static RequestPayload DeserializeRequestPayload(ref SequenceReader<byte> reader, int payloadLength)
    {
        RequestPayload payload;
        if (reader.TryRead(out var frameType) &&
            reader.TryReadLittleEndian(out ushort sensorType) &&
            reader.TryReadLittleEndian(out uint totalWorkingTime) &&
            reader.TryReadLittleEndian(out uint powerOnTime) &&
            reader.TryReadLittleEndian(out uint offsetTime))
        {
            payload = new RequestPayload
            {
                FrameType = (FrameType)frameType,
                SensorType = sensorType,
                TotalWorkingTime = totalWorkingTime,
                PowerOnTime = powerOnTime,
                OffsetTime = offsetTime,
                ModbusRtuFrame = reader.Sequence.Slice(reader.Position, payloadLength).ToArray(),
            };
            reader.Advance(payloadLength);
        }
        else
        {
            throw new InvalidDataException($"Insufficient data to read {nameof(RequestPayload)}");
        }

        return payload;
    }
    
    protected static ResponsePayload DeserializeResponsePayload(ref SequenceReader<byte> reader, int payloadLength)
    {
        ResponsePayload payload;
        if (reader.TryRead(out var frameType) &&
            reader.TryRead(out var status) &&
            reader.TryReadLittleEndian(out uint totalWorkingTime) &&
            reader.TryReadLittleEndian(out uint powerOnTime) &&
            reader.TryReadLittleEndian(out uint offsetTime))
        {
            payload = new ResponsePayload
            {
                FrameType = (FrameType)frameType,
                Status = status,
                TotalWorkingTime = totalWorkingTime,
                PowerOnTime = powerOnTime,
                OffsetTime = offsetTime,
                ModbusRtuFrame = reader.Sequence.Slice(reader.Position, payloadLength).ToArray(),
            };
            reader.Advance(payloadLength);
        }
        else
        {
            throw new InvalidDataException($"Insufficient data to read {nameof(RequestPayload)}");
        }

        return payload;
    }
    
    protected static Trailer DeserializeTrailer(ref SequenceReader<byte> reader)
    {
        Trailer trailer;
        if (reader.TryRead(out var checksum))
        {
            trailer = new Trailer
            {
                Checksum = checksum,
            };
        }
        else
        {
            throw new InvalidDataException($"Insufficient data to read {nameof(Trailer)}");
        }
        
        if (!reader.TryRead(out var end) || end != Trailer.End)
        {
            throw new InvalidOperationException(
                $"Invalid end in the data. Expected {Trailer.End} but got {end}");
        }

        return trailer;
    }
}