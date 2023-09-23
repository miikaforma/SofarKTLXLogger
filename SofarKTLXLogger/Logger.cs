using System.Buffers;
using SofarKTLXLogger.ModbusRTU.ProductInformation;
using SofarKTLXLogger.ModbusRTU.RealtimeData;
using SofarKTLXLogger.SolarmanV5;
using SofarKTLXLogger.SolarmanV5.Protocol;

namespace SofarKTLXLogger;

public class Logger
{
    private readonly ISolarmanV5Client _client;

    public Logger(ISolarmanV5Client client)
    {
        _client = client;
    }

    public void Run()
    {
        var hwDataResponse =
            ProtocolResponse.Deserialize(
                new ReadOnlySequence<byte>(_client.GetHwData()));
        var productInformation = new ProductInformation(hwDataResponse.Payload.ModbusRtuFrame);
        Console.WriteLine(productInformation);
        //
        // var data = _client.GetRealtimeData();
        // var data1Response = ProtocolResponse.Deserialize(
        //     new ReadOnlySequence<byte>(data.Part1));
        // var inverterData = new InverterData(data1Response.Payload.ModbusRtuFrame);
        // Console.WriteLine(inverterData);
        //
        // var data2Response = ProtocolResponse.Deserialize(
        //     new ReadOnlySequence<byte>(data.Part2));
        // var pvData = new InverterData(data2Response.Payload.ModbusRtuFrame);
        // Console.WriteLine(pvData);
        
        // Testing();
    }

    private void Testing()
    {
        // var fileBytes = File.ReadAllBytes("hwdata_request.bin");
        // var rom = new ReadOnlyMemory<byte>(fileBytes);
        // var sequence = new ReadOnlySequence<byte>(rom);
        // Console.WriteLine(Protocol.DeserializeRequest(sequence));

        #region Request creation

        // var productInfoRequest = ProtocolRequest.Serialize(new ReadProductInformation(0x2000, 0x0E));
        // Console.WriteLine(BitConverter.ToString(productInfoRequest.ToArray()).Replace("-", ""));
        //
        // var realtimeDataRequest = ProtocolRequest.Serialize(new ReadRealtimeData(0x0000, 0x0028));
        // Console.WriteLine(BitConverter.ToString(realtimeDataRequest.ToArray()).Replace("-", ""));
        //
        // var realtimeDataRequest2 = ProtocolRequest.Serialize(new ReadRealtimeData(0x0010, 0x0010));
        // Console.WriteLine(BitConverter.ToString(realtimeDataRequest2.ToArray()).Replace("-", ""));

        #endregion

        #region Response parsing

        var hwDataResponse =
            ProtocolResponse.Deserialize(
                new ReadOnlySequence<byte>(File.ReadAllBytes("hwdata_sample.bin")));
        var productInformation = new ProductInformation(hwDataResponse.Payload.ModbusRtuFrame);
        Console.WriteLine(productInformation);

        var data1Response = ProtocolResponse.Deserialize(
            new ReadOnlySequence<byte>(File.ReadAllBytes("inverter_response_faulty_test.bin")));
        var inverterData = new InverterData(data1Response.Payload.ModbusRtuFrame);
        Console.WriteLine(inverterData);
        ;

        #endregion

        // var request = ProtocolRequest.Deserialize(new ReadOnlySequence<byte>(File.ReadAllBytes("hwdata_request.bin")));
        // Console.WriteLine("Request: " + BitConverter.ToString(request.Payload.ModbusRtuFrame.ToArray()).Replace("-", ""));
        //
        // var response = ProtocolResponse.Deserialize(new ReadOnlySequence<byte>(File.ReadAllBytes("hwdata_sample.bin")));
        // Console.WriteLine("Response: " + BitConverter.ToString(response.Payload.ModbusRtuFrame.ToArray()).Replace("-", ""));


        //var sofarSocket = new InverterClient();
        // var hwData = sofarSocket.GetHwData();
        // File.WriteAllBytes("hwdata.bin", hwData);

        //var rtData = sofarSocket.GetRealtimeData();
        //File.WriteAllBytes("rtData_1.bin", rtData.Part1);
        //File.WriteAllBytes("rtData_2.bin", rtData.Part2);
    }
}