using SofarKTLXLogger.SolarmanV5.Enums;

namespace SofarKTLXLogger.SolarmanV5.Protocol;

/// <summary>
/// A request payload is 15 bytes + the length of the Modbus RTU request frame
/// </summary>
public record RequestPayload
{
    /// <summary>
    /// Denotes the frame type.
    /// </summary>
    public FrameType FrameType;

    /// <summary>
    /// Denotes the sensor type. Set this to 0x0000 on outgoing requests.
    /// </summary>
    public ushort SensorType;

    /// <summary>
    /// Denotes the frame total working time. See corresponding response field of same name for further details.
    /// Set this to 0x00000000 on outgoing requests.
    /// </summary>
    public uint TotalWorkingTime;
    
    /// <summary>
    /// Denotes the frame power on time. See corresponding response field of same name for further details.
    /// Set this to 0x00000000 on outgoing requests.
    /// </summary>
    public uint PowerOnTime;
    
    /// <summary>
    /// Denotes the frame offset time. See corresponding response field of same name for further details.
    /// Set this to 0x00000000 on outgoing requests.
    /// </summary>
    public uint OffsetTime;

    /// <summary>
    /// Modbus RTU request frame.
    /// </summary>
    public Memory<byte> ModbusRtuFrame;
}