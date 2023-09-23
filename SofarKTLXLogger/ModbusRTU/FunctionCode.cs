namespace SofarKTLXLogger.ModbusRTU;

public enum FunctionCode : byte
{
    PowerControl = 0x01,
    AutomaticTimeCorrection = 0x02,
    ReadRealTimeData = 0x03,
    ReadProductInformation = 0x04,
    RealTimePowerControlWrite = 0x06,
    ReadCurrentTime = 0x10,
    FactoryReset = 0x30,
    ClearTodayEnergy = 0x31,
    ResetProtectionValueOfCurrentCountryCode = 0x32,
    ClearTotalGeneration = 0x33,
    ClearHistoricalEventRecord = 0x34,
    SetInverterModbusAddress = 0x39,
    SetCountryCode = 0x40,
    SetInputMode = 0x41,
    ReadEepromData = 0x50,
    ReadSdCrdData = 0x60,
}