// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using Npgsql;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

[Flags]
public enum Fault2 : ushort
{
    NoError = 0,
    HwADFaultIGrid = 1 << 0,
    HwADFaultDCI = 1 << 1,
    HwADFaultVGrid = 1 << 2,
    GFCIDeviceFault = 1 << 3,
    MChip_Fault = 1 << 4,
    HwAuxPowerFault = 1 << 5,
    BusVoltZeroFault = 1 << 6,
    IacRmsUnbalance = 1 << 7,
    BusUVP = 1 << 8,
    BusOVP = 1 << 9,
    VbusUnbalance = 1 << 10,
    DciOCP = 1 << 11,
    SwOCPInstant = 1 << 12,
    SwBOCPInstant = 1 << 13,
    reserved1 = 1 << 14,
    reserved2 = 1 << 15,
}

public static class Fault2Extensions
{
    public static void AddMetric(this Fault2 fault, NpgsqlParameterCollection parameterCollection)
    {
        parameterCollection.AddWithValue("Fault2", (int)fault);
    }
}
