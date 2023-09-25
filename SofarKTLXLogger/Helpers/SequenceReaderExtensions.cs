using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace SofarKTLXLogger.Helpers;

public static class SequenceReaderExtensions
{
    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out ushort value)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadUInt16BigEndian(span, out value);

        if (result)
        {
            reader.Advance(sizeof(ushort));
        }

        return result;
    }

    public static bool TryReadLittleEndian(this ref SequenceReader<byte> reader, out ushort value)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadUInt16LittleEndian(span, out value);

        if (result)
        {
            reader.Advance(sizeof(ushort));
        }

        return result;
    }
    
    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out uint value)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadUInt32BigEndian(span, out value);

        if (result)
        {
            reader.Advance(sizeof(uint));
        }

        return result;
    }

    public static bool TryReadLittleEndian(this ref SequenceReader<byte> reader, out uint value)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadUInt32LittleEndian(span, out value);

        if (result)
        {
            reader.Advance(sizeof(uint));
        }

        return result;
    }
    
    public static string ReadString(this ref SequenceReader<byte> reader, int length)
    {
        if (reader.Remaining < length)
            throw new InvalidDataException("Not enough data to read");

        var slice = reader.Sequence.Slice(reader.Position, length);

        // Advance the reader past the string
        reader.Advance(length);

        // Convert the slice to a string using the appropriate encoding
        return Encoding.UTF8.GetString(slice.FirstSpan);
    }
    
    public static float ReadBigEndianUInt32ToFloat(this ref SequenceReader<byte> reader, float ratio = 1f)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadUInt32BigEndian(span, out var value);

        if (result)
        {
            reader.Advance(sizeof(uint));
        }

        return value * ratio;
    }
    
    public static float ReadLittleEndianUInt32ToFloat(this ref SequenceReader<byte> reader, float ratio = 1f)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadUInt32LittleEndian(span, out var value);

        if (result)
        {
            reader.Advance(sizeof(uint));
        }

        return value * ratio;
    }
    
    public static float ReadBigEndianInt32ToFloat(this ref SequenceReader<byte> reader, float ratio = 1f)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadInt32BigEndian(span, out var value);

        if (result)
        {
            reader.Advance(sizeof(int));
        }

        return value * ratio;
    }
    
    public static float ReadLittleEndianInt32ToFloat(this ref SequenceReader<byte> reader, float ratio = 1f)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadInt32LittleEndian(span, out var value);

        if (result)
        {
            reader.Advance(sizeof(int));
        }

        return value * ratio;
    }
    
    public static float ReadBigEndianUInt16ToFloat(this ref SequenceReader<byte> reader, float ratio = 1f)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadUInt16BigEndian(span, out var value);

        if (result)
        {
            reader.Advance(sizeof(ushort));
        }

        return value * ratio;
    }
    
    public static float ReadLittleEndianUInt16ToFloat(this ref SequenceReader<byte> reader, float ratio = 1f)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadUInt16LittleEndian(span, out var value);

        if (result)
        {
            reader.Advance(sizeof(ushort));
        }

        return value * ratio;
    }
    
    public static float ReadBigEndianInt16ToFloat(this ref SequenceReader<byte> reader, float ratio = 1f)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadInt16BigEndian(span, out var value);

        if (result)
        {
            reader.Advance(sizeof(short));
        }

        return value * ratio;
    }
    
    public static float ReadLittleEndianInt16ToFloat(this ref SequenceReader<byte> reader, float ratio = 1f)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadInt16LittleEndian(span, out var value);

        if (result)
        {
            reader.Advance(sizeof(short));
        }

        return value * ratio;
    }
    
    public static float ReadBigEndianTwelveBitsToFloat(this ref SequenceReader<byte> reader, float ratio = 1f)
    {
        var span = reader.UnreadSpan;
        var result = BinaryPrimitives.TryReadUInt16BigEndian(span, out var value);

        if (result)
        {
            reader.Advance(sizeof(ushort) - 1);
        }

        return value * ratio;
    }
}