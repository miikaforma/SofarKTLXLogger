using System.Buffers;
using SofarKTLXLogger.Helpers;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

public record FloatDescription(float Value, float Ratio, Unit Unit, string Metric, string Description)
{
    public static FloatDescription FromUInt32(ref SequenceReader<byte> reader, float ratio, Unit unit, string metric,
        string description) => new(reader.ReadBigEndianUInt32ToFloat(ratio: ratio), ratio, unit,
        metric, description);

    public static FloatDescription FromInt32(ref SequenceReader<byte> reader, float ratio, Unit unit, string metric,
        string description) => new(reader.ReadBigEndianInt32ToFloat(ratio: ratio), ratio, unit,
        metric, description);

    public static FloatDescription FromUInt16(ref SequenceReader<byte> reader, float ratio, Unit unit, string metric,
        string description) => new(reader.ReadBigEndianUInt16ToFloat(ratio: ratio), ratio, unit,
        metric, description);

    public static FloatDescription FromInt16(ref SequenceReader<byte> reader, float ratio, Unit unit, string metric,
        string description) => new(reader.ReadBigEndianInt16ToFloat(ratio: ratio), ratio, unit,
        metric, description);

    public void AddMetric(Dictionary<string, object> dictionary)
    {
        dictionary.Add(Metric, Value);
    }
}