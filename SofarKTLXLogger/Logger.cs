using System.Buffers;
using System.Net.Sockets;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SofarKTLXLogger.Daytime;
using SofarKTLXLogger.ModbusRTU.ProductInformation;
using SofarKTLXLogger.ModbusRTU.RealtimeData;
using SofarKTLXLogger.Settings;
using SofarKTLXLogger.SolarmanV5;
using SofarKTLXLogger.SolarmanV5.Protocol;

namespace SofarKTLXLogger;

public class Logger
{
    private ILogger<Logger> _logger;
    private readonly ISolarmanV5Client _client;
    private readonly ProductInfoSettings _productInfoSettings;
    private readonly RealTimeDataSettings _realTimeDataSettings;
    private readonly InfluxDbSettings _influxDbSettings;
    private readonly AppSettings _appSettings;
    private readonly IDaytimeService _daytimeService;

    public Logger(ISolarmanV5Client client, IOptions<ProductInfoSettings> productInfoSettings,
        IOptions<RealTimeDataSettings> realTimeDataSettings, IOptions<InfluxDbSettings> influxDbSettings,
        ILogger<Logger> logger, IDaytimeService daytimeService, IOptions<AppSettings> appSettings)
    {
        _client = client;
        _logger = logger;
        _daytimeService = daytimeService;
        _appSettings = appSettings.Value;
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
        _logger.LogInformation("{ProductInformation}", productInformation);
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        // await PrintProductInformation(cancellationToken: cancellationToken);

        // await PrintRealTimeInformation(cancellationToken: cancellationToken);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (await _daytimeService.IsDaytime())
                {
                    await LogRealTimeInformation(cancellationToken: cancellationToken);
                }
                else
                {
                    _logger.LogDebug("Skipping logging because it's not daytime");
                }
                
                await Task.Delay(_appSettings.LoggingInterval, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error occurred executing {LogRealTimeInformation}", nameof(LogRealTimeInformation));
            }
        }
    }

    private async Task PrintProductInformation(CancellationToken cancellationToken = default)
    {
        var modbusFrame = new ReadProductInformation(_productInfoSettings.StartRegister,
            _productInfoSettings.RegisterCount).GetFrameData();
        var response = await _client.SendAsync(modbusFrame, cancellationToken: cancellationToken);
        var productInformation = ProductInformation.FromProtocolResponse(response);

        _logger.LogInformation("{ProductInformation}", productInformation);
    }

    private async Task PrintRealTimeInformation(CancellationToken cancellationToken = default)
    {
        {
            var modbusFrame = new ReadRealtimeData(_realTimeDataSettings.InverterStartRegister,
                _realTimeDataSettings.InverterRegisterCount).GetFrameData();
            var response = await _client.SendAsync(modbusFrame, cancellationToken: cancellationToken);
            var inverterData = InverterData.FromProtocolResponse(response);
            _logger.LogInformation("{InverterData}", inverterData);
        }

        {
            var modbusFrame = new ReadRealtimeData(_realTimeDataSettings.PvStartRegister,
                _realTimeDataSettings.PvRegisterCount).GetFrameData();
            var response = await _client.SendAsync(modbusFrame, cancellationToken: cancellationToken);
            var pvData = InverterData.FromProtocolResponse(response);
            _logger.LogInformation("{PVData}", pvData);
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
            _logger.LogWarning(ex, "Unknown error retrieving real time inverter data. {Response}",
                BitConverter.ToString(_client.ResponseData.ToArray()).Replace("-", ""));
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
                _logger.LogWarning(ex, "Unknown error retrieving real time pv data. {Response}",
                    BitConverter.ToString(_client.ResponseData.ToArray()).Replace("-", ""));
            }
        }

        try
        {
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
                    _logger.LogWarning("Error while saving data to InfluxDB. {ErrorMessage}",
                        influxResult.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unknown error while saving data to InfluxDB");
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