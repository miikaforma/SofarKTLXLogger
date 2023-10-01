// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

[Flags]
public enum InverterAlertMessage : ushort
{
    NoError = 0,
    OverTempDerating = 1 << 0,
    OverFreqDerating = 1 << 1,
    RemoteDerating = 1 << 2,
    RemoteOff = 1 << 3,
    UnderFreqDerate = 1 << 4,
    reserved1 = 1 << 5,
    reserved2 = 1 << 6,
    reserved3 = 1 << 7,
    reserved4 = 1 << 8,
    reserved5 = 1 << 9,
    reserved6 = 1 << 10,
    reserved7 = 1 << 11,
    reserved8 = 1 << 12,
    reserved9 = 1 << 13,
    reserved10 = 1 << 14,
    reserved11 = 1 << 15,
}