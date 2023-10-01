using SofarKTLXLogger.SolarmanV5.Enums;

namespace SofarKTLXLogger.SolarmanV5.Protocol;

/// <summary>
/// A response payload is 14 bytes + the length of the Modbus RTU response frame
/// </summary>
public record ResponsePayload
{
    /// <summary>
    /// Denotes the frame type.
    /// 0x02: Solar Inverter
    /// 0x01: Data Logging Stick
    /// 0x00: Solarman Cloud (or keep alive?)
    /// </summary>
    public FrameType FrameType;

    /// <summary>
    /// Denotes the request status. 0x01 appears to denote real-time data.
    /// </summary>
    public byte Status;

    /// <summary>
    /// Denotes the number of seconds that data logging stick has been operating.
    /// Other implementations have this field named TimeOutOfFactory.
    /// </summary>
    public uint TotalWorkingTime;
    
    /// <summary>
    /// Denotes the current uptime of the data logging stick in seconds.
    /// </summary>
    public uint PowerOnTime;
    
    /// <summary>
    /// Denotes offset timestamp, in seconds.
    /// This is defined as current data logging stick timestamp minus Total Working Time.
    /// </summary>
    public uint OffsetTime;

    /// <summary>
    /// Modbus RTU response frame.
    /// </summary>
    public Memory<byte> ModbusRtuFrame;
}