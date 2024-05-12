using Npgsql;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

public enum InputMode
{
    Parallel = 0x00,
    Independent = 0x01,
}

public static class InputModeExtensions
{
    public static void AddMetric(this InputMode inputMode, NpgsqlParameterCollection parameterCollection)
    {
        parameterCollection.AddWithValue("InputMode", (int)inputMode);
    }
}
