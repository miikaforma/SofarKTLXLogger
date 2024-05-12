// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using Npgsql;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

[Flags]
public enum Fault5 : ushort
{
    NoError = 0,
    unrecoverHwAcOCP = 1 << 0,
    unrecoverBusOVP = 1 << 1,
    unrecoverIacRmsUnbalance = 1 << 2,
    unrecoverIpvUnbalance = 1 << 3,
    unrecoverVbusUnbalance = 1 << 4,
    unrecoverOCPInstant = 1 << 5,
    unrecoverPvConfigSetWrong = 1 << 6,
    reserved1 = 1 << 7,
    reserved2 = 1 << 8,
    unrecoverIPVInstant = 1 << 9,
    unrecoverWRITEEEPROM = 1 << 10,
    unrecoverREADEEPROM = 1 << 11,
    unrecoverRelayFail = 1 << 12,
    reserved3 = 1 << 13,
    reserved4 = 1 << 14,
    reserved5 = 1 << 15,
}

public static class Fault5Extensions
{
    public static void AddMetric(this Fault5 fault, NpgsqlParameterCollection parameterCollection)
    {
        parameterCollection.AddWithValue("Fault5", (int)fault);
    }
}
