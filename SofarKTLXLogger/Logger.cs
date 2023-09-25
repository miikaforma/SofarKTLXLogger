using System.Buffers;
using System.Net.Sockets;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;
using Microsoft.Extensions.Options;
using SofarKTLXLogger.ModbusRTU.ProductInformation;
using SofarKTLXLogger.ModbusRTU.RealtimeData;
using SofarKTLXLogger.Settings;
using SofarKTLXLogger.SolarmanV5;
using SofarKTLXLogger.SolarmanV5.Protocol;

namespace SofarKTLXLogger;

public class Logger
{
    private readonly ISolarmanV5Client _client;
    private readonly ProductInfoSettings _productInfoSettings;
    private readonly RealTimeDataSettings _realTimeDataSettings;
    private readonly InfluxDbSettings _influxDbSettings;

    public Logger(ISolarmanV5Client client, IOptions<ProductInfoSettings> productInfoSettings,
        IOptions<RealTimeDataSettings> realTimeDataSettings, IOptions<InfluxDbSettings> influxDbSettings)
    {
        _client = client;
        _productInfoSettings = productInfoSettings.Value;
        _realTimeDataSettings = realTimeDataSettings.Value;
        _influxDbSettings = influxDbSettings.Value;
    }

    public void Run()
    {
        var hwDataResponse =
            ProtocolResponse.FromReadonlySequence(
                new ReadOnlySequence<byte>(_client.GetHwData()));
        var productInformation = new ProductInformation(hwDataResponse.ModbusFrame);
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

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        // await PrintProductInformation(cancellationToken: cancellationToken);

        // await PrintRealTimeInformation(cancellationToken: cancellationToken);

        await LogRealTimeInformation(cancellationToken: cancellationToken);
    }

    private async Task PrintProductInformation(CancellationToken cancellationToken = default)
    {
        var modbusFrame = new ReadProductInformation(_productInfoSettings.StartRegister,
            _productInfoSettings.RegisterCount).GetFrameData();
        var response = await _client.SendAsync(modbusFrame, cancellationToken: cancellationToken);
        var productInformation = ProductInformation.FromProtocolResponse(response);

        Console.WriteLine(productInformation);
    }

    private async Task PrintRealTimeInformation(CancellationToken cancellationToken = default)
    {
        {
            var modbusFrame = new ReadRealtimeData(_realTimeDataSettings.InverterStartRegister,
                _realTimeDataSettings.InverterRegisterCount).GetFrameData();
            var response = await _client.SendAsync(modbusFrame, cancellationToken: cancellationToken);
            var inverterData = InverterData.FromProtocolResponse(response);
            Console.WriteLine(inverterData);
        }

        {
            var modbusFrame = new ReadRealtimeData(_realTimeDataSettings.PvStartRegister,
                _realTimeDataSettings.PvRegisterCount).GetFrameData();
            var response = await _client.SendAsync(modbusFrame, cancellationToken: cancellationToken);
            var inverterData = InverterData.FromProtocolResponse(response);
            Console.WriteLine(inverterData);
        }
    }

    private async Task LogRealTimeInformation(CancellationToken cancellationToken = default)
    {
        var metrics = new Dictionary<string, object>();
        try
        {
            var modbusFrame = new ReadRealtimeData(_realTimeDataSettings.InverterStartRegister,
                _realTimeDataSettings.InverterRegisterCount).GetFrameData();
            var response = await _client.SendAsync(modbusFrame, cancellationToken: cancellationToken);
            var inverterData = InverterData.FromProtocolResponse(response);
            inverterData.AddMetrics(ref metrics);
        }
        catch (SocketException)
        {
            // If couldn't connect, cancel
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"Response data: {BitConverter.ToString(_client.ResponseData.ToArray()).Replace("-", "")}");
        }

        if (_realTimeDataSettings.PvEnabled)
        {
            try
            {
                var modbusFrame = new ReadRealtimeData(_realTimeDataSettings.PvStartRegister,
                    _realTimeDataSettings.PvRegisterCount).GetFrameData();
                var response = await _client.SendAsync(modbusFrame, cancellationToken: cancellationToken);
                var pvData = PvData.FromProtocolResponse(response);
                pvData.AddMetrics(ref metrics);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Data: {BitConverter.ToString(_client.ResponseData.ToArray()).Replace("-", "")}");
            }
        }

        if (_influxDbSettings.Enabled)
        {
            var inverterData = new LineProtocolPoint(
                _influxDbSettings.MetricName,
                metrics,
                null,
                DateTime.UtcNow);

            var payload = new LineProtocolPayload();
            payload.Add(inverterData);
            
            var client = new LineProtocolClient(new Uri(_influxDbSettings.Address), _influxDbSettings.DbName);
            var influxResult = await client.WriteAsync(payload, cancellationToken);
            if (!influxResult.Success)
                await Console.Error.WriteLineAsync(influxResult.ErrorMessage);
        }
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
            ProtocolResponse.FromReadonlySequence(
                new ReadOnlySequence<byte>(File.ReadAllBytes("hwdata_sample.bin")));
        var productInformation = new ProductInformation(hwDataResponse.ModbusFrame);
        Console.WriteLine(productInformation);

        var data1Response = ProtocolResponse.FromReadonlySequence(
            new ReadOnlySequence<byte>(File.ReadAllBytes("inverter_response_faulty_test.bin")));
        var inverterData = new InverterData(data1Response.ModbusFrame);
        Console.WriteLine(inverterData);
        ;

        #endregion

        // var request = ProtocolRequest.Deserialize(new ReadOnlySequence<byte>(File.ReadAllBytes("hwdata_request.bin")));
        // Console.WriteLine("Request: " + BitConverter.ToString(request.ModbusFrame.ToArray()).Replace("-", ""));
        //
        // var response = ProtocolResponse.Deserialize(new ReadOnlySequence<byte>(File.ReadAllBytes("hwdata_sample.bin")));
        // Console.WriteLine("Response: " + BitConverter.ToString(response.ModbusFrame.ToArray()).Replace("-", ""));


        //var sofarSocket = new InverterClient();
        // var hwData = sofarSocket.GetHwData();
        // File.WriteAllBytes("hwdata.bin", hwData);

        //var rtData = sofarSocket.GetRealtimeData();
        //File.WriteAllBytes("rtData_1.bin", rtData.Part1);
        //File.WriteAllBytes("rtData_2.bin", rtData.Part2);
    }
}