using SofarKTLXLogger.SolarmanV5.Enums;

namespace SofarKTLXLogger.SolarmanV5.Protocol;

/// <summary>
/// The Header is always 11 bytes
/// </summary>
public record Header
{
    /// <summary>
    /// Denotes the start of the V5 frame. Always 0xA5.
    /// </summary>
    public const byte Magic = 0xA5;

    /// <summary>
    /// Payload length
    /// </summary>
    public ushort Length;

    /// <summary>
    /// Describes the type of V5 frame.
    /// 0x4510 For Modbus RTU requests
    /// 0x1510 For Modbus RTU responses
    /// </summary>
    public ControlCode ControlCode;

    /// <summary>
    /// This field acts as a two-way sequence number. On outgoing requests, the first
    /// byte of this field is echoed back in the same position on incoming responses.
    /// pysolarmanv5 exploits this property to detect invalid responses. This is done by initialising this byte
    /// to a random value, and incrementing for each subsequent request. The second byte is incremented
    /// by the data logging stick for every response sent (either to Solarman Cloud or local requests).
    /// </summary>
    public ushort Serial;

    /// <summary>
    /// Serial number of Solarman data logging stick
    /// </summary>
    public uint LoggerSerial;
}