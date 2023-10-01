namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

public enum OperatingState : byte
{
    Wait = 0x00,
    Check = 0x01,
    Normal = 0x02,
    Fault = 0x03,
    Permanent = 0x04,
}