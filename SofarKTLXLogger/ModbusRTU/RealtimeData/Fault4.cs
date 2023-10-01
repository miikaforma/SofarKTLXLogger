// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

[Flags]
public enum Fault4 : ushort
{
    NoError = 0,
    ConsistentFault_VGrid = 1 << 0,
    ConsistentFault_FGrid = 1 << 1,
    ConsistentFault_DCI = 1 << 2,
    ConsistentFault_GFCI = 1 << 3,
    SpiCommLose = 1 << 4,
    SciCommLose = 1 << 5,
    RelayTestFail = 1 << 6,
    PvIsoFault = 1 << 7,
    OverTempFault_Inv = 1 << 8,
    OverTempFault_Boost = 1 << 9,
    OverTempFault_Env = 1 << 10,
    PEConnectFault = 1 << 11,
    reserved1 = 1 << 12,
    reserved2 = 1 << 13,
    reserved3 = 1 << 14,
    reserved4 = 1 << 15,
}