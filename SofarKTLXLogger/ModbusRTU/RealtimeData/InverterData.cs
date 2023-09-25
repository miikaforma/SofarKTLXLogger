// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Buffers;
using SofarKTLXLogger.Helpers;
using SofarKTLXLogger.SolarmanV5.Protocol;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

public record InverterData
{
    public OperatingState OperatingState { get; init; }
    public Fault1 Fault1 { get; init; }
    public Fault2 Fault2 { get; init; }
    public Fault3 Fault3 { get; init; }
    public Fault4 Fault4 { get; init; }
    public Fault5 Fault5 { get; init; }
    public FloatDescription PV1Voltage { get; init; }
    public FloatDescription PV1Current { get; init; }
    public FloatDescription PV2Voltage { get; init; }
    public FloatDescription PV2Current { get; init; }
    public FloatDescription PV1Power { get; init; }
    public FloatDescription PV2Power { get; init; }
    public FloatDescription OutputActivePower { get; init; }
    public FloatDescription OutputReactivePower { get; init; }
    public FloatDescription GridFrequency { get; init; }
    public FloatDescription L1Voltage { get; init; }
    public FloatDescription L1Current { get; init; }
    public FloatDescription L2Voltage { get; init; }
    public FloatDescription L2Current { get; init; }
    public FloatDescription L3Voltage { get; init; }
    public FloatDescription L3Current { get; init; }
    public FloatDescription TotalProduction { get; init; }
    public FloatDescription TotalGenerationTime { get; init; }
    public FloatDescription TodayProduction { get; init; }
    public FloatDescription TodayGenerationTime { get; init; }
    public FloatDescription InverterModuleTemperature { get; init; }
    public FloatDescription InverterInnerTemperature { get; init; }
    public FloatDescription InverterBusVoltage { get; init; }
    public FloatDescription PV1VoltageSample { get; init; }
    public FloatDescription PV1CurrentSample { get; init; }
    public FloatDescription CountdownTime { get; init; }
    public InverterAlertMessage InverterAlertMessage { get; init; }
    public InputMode InputMode { get; init; }
    public CommunicationBoardInnerMessage CommunicationBoardInnerMessage { get; init; }
    public FloatDescription PV1InsulationImpedance { get; init; }
    public FloatDescription PV2InsulationImpedance { get; init; }
    public FloatDescription CathodeInsulationImpedance { get; init; }
    public CountryCode CountryCode { get; init; }
    
    public InverterData(Memory<byte> data)
    {
        var reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(data));
        reader.TryRead(out _); // Throw away the slave address
        if (!reader.TryRead(out var functionCode) || (FunctionCode)functionCode != FunctionCode.ReadRealTimeData)
        {
            throw new InvalidOperationException("Not a valid response");
        }

        reader.TryRead(out var length);
        reader.TryRead(out var registerStart);
        if (reader.TryRead(out var operatingState))
        {
            OperatingState = (OperatingState)operatingState;
        }

        if (reader.TryReadBigEndian(out ushort fault1)) Fault1 = (Fault1)fault1;
        if (reader.TryReadBigEndian(out ushort fault2)) Fault2 = (Fault2)fault2;
        if (reader.TryReadBigEndian(out ushort fault3)) Fault3 = (Fault3)fault3;
        if (reader.TryReadBigEndian(out ushort fault4)) Fault4 = (Fault4)fault4;
        if (reader.TryReadBigEndian(out ushort fault5)) Fault5 = (Fault5)fault5;
        PV1Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "SolarVoltage_PV1", "PV1 Voltage");
        PV1Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "SolarCurrent_PV1", "PV1 Current");
        PV2Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "SolarVoltage_PV2", "PV2 Voltage");
        PV2Current = FloatDescription.FromInt16(ref reader, 0.01f, Unit.Ampere, "SolarCurrent_PV2", "PV2 Current");
        PV1Power = FloatDescription.FromUInt16(ref reader, 10f, Unit.Watt, "SolarPower_PV1", "PV1 Power");
        PV2Power = FloatDescription.FromUInt16(ref reader, 10f, Unit.Watt, "SolarPower_PV2", "PV2 Power");
        OutputActivePower = FloatDescription.FromUInt16(ref reader, 10f, Unit.Watt, "OutputPower_Active", "Output active power");
        OutputReactivePower = FloatDescription.FromInt16(ref reader, 0.01f, Unit.KilovoltAmpereReactive, "OutputPower_Reactive", "Output reactive power");
        GridFrequency = FloatDescription.FromUInt16(ref reader, 0.01f, Unit.Hertz, "OutputFreq_Frequency", "Grid frequency");
        L1Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "OutputVoltage_L1", "L1 Voltage");
        L1Current = FloatDescription.FromUInt16(ref reader, 0.01f, Unit.Ampere, "OutputCurrent_L1", "L1 Current");
        L2Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "OutputVoltage_L2", "L2 Voltage");
        L2Current = FloatDescription.FromUInt16(ref reader, 0.01f, Unit.Ampere, "OutputCurrent_L2", "L2 Current");
        L3Voltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "OutputVoltage_L3", "L3 Voltage");
        L3Current = FloatDescription.FromUInt16(ref reader, 0.01f, Unit.Ampere, "OutputCurrent_L3", "L3 Current");
        TotalProduction = FloatDescription.FromUInt32(ref reader, 1f, Unit.KilowattHour, "SolarProduction_Total", "Total production");
        TotalGenerationTime = FloatDescription.FromUInt32(ref reader, 1f, Unit.Hour, "SolarTime_Total", "Total generation time");
        TodayProduction = FloatDescription.FromUInt16(ref reader, 10f, Unit.WattHour, "SolarProduction_Today", "Today production");
        TodayGenerationTime = FloatDescription.FromUInt16(ref reader, 1f, Unit.Minute, "SolarTime_Today", "Today generation time");
        InverterModuleTemperature = FloatDescription.FromInt16(ref reader, 1f, Unit.Celsius, "InverterTemp_Module", "Inverter module temperature");
        InverterInnerTemperature = FloatDescription.FromInt16(ref reader, 1f, Unit.Celsius, "InverterTemp_Inner", "Inverter inner temperature");
        InverterBusVoltage = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "InverterVoltage_Bus", "Inverter bus voltage");
        PV1VoltageSample = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Volt, "InverterVoltage_PV1VoltageSample", "PV1 voltage sample by slave CPU");
        PV1CurrentSample = FloatDescription.FromUInt16(ref reader, 0.1f, Unit.Ampere, "InverterCurrent_PV1CurrentSample", "PV1 current sample by slave CPU");
        CountdownTime = FloatDescription.FromUInt16(ref reader, 1f, Unit.Second, "InverterCountdown_Time", "Countdown Time");
        if (reader.TryReadBigEndian(out ushort inverterAlertMessage)) InverterAlertMessage = (InverterAlertMessage)inverterAlertMessage;
        if (reader.TryReadBigEndian(out ushort inputMode)) InputMode = (InputMode)inputMode;
        if (reader.TryReadBigEndian(out ushort communicationBoardInnerMessage)) CommunicationBoardInnerMessage = (CommunicationBoardInnerMessage)communicationBoardInnerMessage;
        PV1InsulationImpedance = FloatDescription.FromUInt16(ref reader, 1f, Unit.KiloOhm, "InverterInsulation_PV1", "PV1 Insulation Impedance");
        PV2InsulationImpedance = FloatDescription.FromUInt16(ref reader, 1f, Unit.KiloOhm, "InverterInsulation_PV2", "PV2 Insulation Impedance");
        CathodeInsulationImpedance = FloatDescription.FromUInt16(ref reader, 1f, Unit.KiloOhm, "InverterInsulation_PV", "Insulation Impedance- Cathode to ground");
        if (reader.TryReadBigEndian(out ushort countryCode)) CountryCode = (CountryCode)countryCode;
        reader.TryRead(out var registerEnd);
        reader.TryRead(out var crc16);
    }

    public void AddMetrics(ref Dictionary<string, object> dictionary)
    {
        PV1Voltage.AddMetric(dictionary);
        PV1Current.AddMetric(dictionary);
        PV2Voltage.AddMetric(dictionary);
        PV2Current.AddMetric(dictionary);
        PV1Power.AddMetric(dictionary);
        PV2Power.AddMetric(dictionary);
        OutputActivePower.AddMetric(dictionary);
        OutputReactivePower.AddMetric(dictionary);
        GridFrequency.AddMetric(dictionary);
        L1Voltage.AddMetric(dictionary);
        L1Current.AddMetric(dictionary);
        L2Voltage.AddMetric(dictionary);
        L2Current.AddMetric(dictionary);
        L3Voltage.AddMetric(dictionary);
        L3Current.AddMetric(dictionary);
        TotalProduction.AddMetric(dictionary);
        TotalGenerationTime.AddMetric(dictionary);
        TodayProduction.AddMetric(dictionary);
        TodayGenerationTime.AddMetric(dictionary);
        InverterModuleTemperature.AddMetric(dictionary);
        InverterInnerTemperature.AddMetric(dictionary);
        InverterBusVoltage.AddMetric(dictionary);
        PV1VoltageSample.AddMetric(dictionary);
        PV1CurrentSample.AddMetric(dictionary);
        CountdownTime.AddMetric(dictionary);
        PV1InsulationImpedance.AddMetric(dictionary);
        PV2InsulationImpedance.AddMetric(dictionary);
        CathodeInsulationImpedance.AddMetric(dictionary);
    }

    public static InverterData FromProtocolResponse(ProtocolResponse response) => new(response.ModbusFrame);
}