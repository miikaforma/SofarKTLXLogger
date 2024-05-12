// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using Npgsql;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

[Flags]
public enum Fault3 : ushort
{
    NoError = 0,
    reserved1 = 1 << 0,
    reserved2 = 1 << 1,
    reserved3 = 1 << 2,
    reserved4 = 1 << 3,
    reserved5 = 1 << 4,
    reserved6 = 1 << 5,
    reserved7 = 1 << 6,
    reserved8 = 1 << 7,
    reserved9 = 1 << 8,
    reserved10 = 1 << 9,
    reserved11 = 1 << 10,
    reserved12 = 1 << 11,
    reserved13 = 1 << 12,
    reserved14 = 1 << 13,
    reserved15 = 1 << 14,
    reserved16 = 1 << 15,
}

public static class Fault3Extensions
{
    public static void AddMetric(this Fault3 fault, NpgsqlParameterCollection parameterCollection)
    {
        parameterCollection.AddWithValue("Fault3", (int)fault);
    }
}
