// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using Npgsql;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

[Flags]
public enum Fault1 : ushort
{
    NoError = 0,
    GridOVP = 1 << 0,
    GridUVP = 1 << 1,
    GridOFP = 1 << 2,
    GridUFP = 1 << 3,
    PVUVP = 1 << 4,
    GridLVRT = 1 << 5,
    reserve1 = 1 << 6,
    reserve2 = 1 << 7,
    PVOVP = 1 << 8,
    IpvUnbalance = 1 << 9,
    PvConfigSetWrong = 1 << 10,
    GFCIFault = 1 << 11,
    PhaseSequenceFault = 1 << 12,
    HwBoostOCP = 1 << 13,
    HwAcOCP = 1 << 14,
    AcRmsOCP = 1 << 15,
}

public static class Fault1Extensions
{
    public static void AddMetric(this Fault1 fault, NpgsqlParameterCollection parameterCollection)
    {
        parameterCollection.AddWithValue("Fault1", (int)fault);
    }
}
