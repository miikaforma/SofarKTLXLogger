// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Buffers;
using SofarKTLXLogger.Helpers;
using SofarKTLXLogger.SolarmanV5.Protocol;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

public record PvData
{
    public FloatDescription PV1Voltage { get; init; }
    public FloatDescription PV1Current { get; init; }
    public FloatDescription PV2Voltage { get; init; }
    public FloatDescription PV2Current { get; init; }
    public FloatDescription PV3Voltage { get; init; }
    public FloatDescription PV3Current { get; init; }
    public FloatDescription PV4Voltage { get; init; }
    public FloatDescription PV4Current { get; init; }
    public FloatDescription PV5Voltage { get; init; }
    public FloatDescription PV5Current { get; init; }
    public FloatDescription PV6Voltage { get; init; }
    public FloatDescription PV6Current { get; init; }
    public FloatDescription PV7Voltage { get; init; }
    public FloatDescription PV7Current { get; init; }
    public FloatDescription PV8Voltage { get; init; }
    public FloatDescription PV8Current { get; init; }
    
    public PvData(Memory<byte> data)
    {
        var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(data));
        reader.TryRead(out _); // Throw away the slave address
        if (!reader.TryRead(out var functionCode) || (FunctionCode)functionCode != FunctionCode.ReadRealTimeData)
        {
            throw new InvalidOperationException("Not a valid response");
        }

        reader.TryRead(out var length);
        reader.TryRead(out var registerStart);
        PV1Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "DCInputVoltage_String1", "PV1 Voltage");
        PV1Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "DCInputCurrent_String1", "PV1 Current");
        PV2Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "DCInputVoltage_String2", "PV2 Voltage");
        PV2Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "DCInputCurrent_String2", "PV2 Current");
        PV3Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "DCInputVoltage_String3", "PV3 Voltage");
        PV3Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "DCInputCurrent_String3", "PV3 Current");
        PV4Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "DCInputVoltage_String4", "PV4 Voltage");
        PV4Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "DCInputCurrent_String4", "PV4 Current");
        PV5Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "DCInputVoltage_String5", "PV5 Voltage");
        PV5Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "DCInputCurrent_String5", "PV5 Current");
        PV6Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "DCInputVoltage_String6", "PV6 Voltage");
        PV6Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "DCInputCurrent_String6", "PV6 Current");
        PV7Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "DCInputVoltage_String7", "PV7 Voltage");
        PV7Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "DCInputCurrent_String7", "PV7 Current");
        PV8Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "DCInputVoltage_String8", "PV8 Voltage");
        PV8Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "DCInputCurrent_String8", "PV8 Current");
        reader.TryRead(out var registerEnd);
        reader.TryRead(out var crc16);
    }
    
    public void AddMetrics(ref Dictionary<string, object> dictionary)
    {
        PV1Voltage.AddMetric(dictionary);
        PV1Current.AddMetric(dictionary);
        PV2Voltage.AddMetric(dictionary);
        PV2Current.AddMetric(dictionary);
        PV3Voltage.AddMetric(dictionary);
        PV3Current.AddMetric(dictionary);
        PV4Voltage.AddMetric(dictionary);
        PV4Current.AddMetric(dictionary);
        PV5Voltage.AddMetric(dictionary);
        PV5Current.AddMetric(dictionary);
        PV6Voltage.AddMetric(dictionary);
        PV6Current.AddMetric(dictionary);
        PV7Voltage.AddMetric(dictionary);
        PV7Current.AddMetric(dictionary);
        PV8Voltage.AddMetric(dictionary);
        PV8Current.AddMetric(dictionary);
    }
    
    public static PvData FromProtocolResponse(ProtocolResponse response) => new(response.ModbusFrame);
}