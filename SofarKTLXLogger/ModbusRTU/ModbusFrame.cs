using System.Buffers.Binary;

namespace SofarKTLXLogger.ModbusRTU;

public abstract class ModbusFrame
{
    private readonly byte _deviceAddress;
    private readonly FunctionCode _functionCode;

    protected ModbusFrame(byte deviceAddress, FunctionCode functionCode)
    {
        _deviceAddress = deviceAddress;
        _functionCode = functionCode;
    }

    public Memory<byte> GetFrameData()
    {
        var dataBytes = GetData();
        var totalLength = sizeof(byte) * 2 /*DeviceAddress and FunctionCode*/ +
                          sizeof(ushort) /*CRC*/ + 
                          dataBytes.Length /*Data bytes*/;

        var frameData = new byte[totalLength];

        // Write DeviceAddress.
        frameData[0] = _deviceAddress;

        // Write FunctionCode.
        frameData[1] = (byte)_functionCode;

        // Write Data.
        Array.Copy(dataBytes, 0, frameData, 2, dataBytes.Length);

        // Write Crc.
        var crc = CalculateCrc16(frameData.AsSpan(0, totalLength - sizeof(ushort)).ToArray());
        BinaryPrimitives.WriteUInt16LittleEndian(frameData.AsSpan(dataBytes.Length + 2), crc);

        return new Memory<byte>(frameData);
    }

    protected abstract byte[] GetData();
    
    private static ushort CalculateCrc16(IReadOnlyList<byte> data)
    {
        ushort crc = 0xFFFF;

        for (var i = 0; i < data.Count; i++)
        {
            crc ^= data[i + 0];

            for (var j = 0; j < 8; j++)
            {
                if ((crc & 1) != 0)
                {
                    crc >>= 1;
                    crc ^= 0xA001;
                }
                else
                {
                    crc >>= 1;
                }
            }
        }

        return crc;
    }
}