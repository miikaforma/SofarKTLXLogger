namespace SofarKTLXLogger.SolarmanV5.Protocol;

/// <summary>
/// The Trailer is always 2 bytes
/// </summary>
public record Trailer
{
    /// <summary>
    /// Denotes the V5 frame checksum.
    /// The checksum is computed on the entire V5 frame except for Start, Checksum (obviously!) and End.
    /// Note, that this field is completely separate to the Modbus RTU checksum, which coincidentally,
    /// is the two bytes immediately preceding this field.
    /// </summary>
    public byte Checksum;

    /// <summary>
    /// Denotes the end of the V5 frame. Always 0x15.
    /// </summary>
    public const byte End = 0x15;
}