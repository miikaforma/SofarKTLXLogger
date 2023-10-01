// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

[Flags]
public enum CommunicationBoardInnerMessage : ushort
{
    NoError = 0,
    Fan1Alarm = 1 << 0,
    Fan2Alarm = 1 << 1,
    LightningProtectionAlarm = 1 << 2,
    SoftwareVersionIsNotConsistent = 1 << 3,
    CommunicationBoardEEPROMFault = 1 << 4,
    RTCFatult = 1 << 5,
    InValidCountry = 1 << 6,
    SDfault = 1 << 7,
    Fan3Alarm = 1 << 8,
    WIFIFault = 1 << 9,
    Fan4Alarm = 1 << 10,
    Fan5Alarm = 1 << 11,
    reserved1 = 1 << 12,
    reserved2 = 1 << 13,
    reserved3 = 1 << 14,
    reserved4 = 1 << 15,
}