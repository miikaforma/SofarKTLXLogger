using System.Buffers;
using System.Net.Sockets;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SofarKTLXLogger.Daytime;
using SofarKTLXLogger.Helpers;
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
    private readonly LoggerSettings _loggerSettings;
    private readonly AppSettings _appSettings;
    private readonly IDaytimeService _daytimeService;

    public Logger(ISolarmanV5Client client, IOptions<ProductInfoSettings> productInfoSettings,
        IOptions<RealTimeDataSettings> realTimeDataSettings, IOptions<InfluxDbSettings> influxDbSettings,
        ILogger<Logger> logger, IDaytimeService daytimeService, IOptions<AppSettings> appSettings,
        IOptions<LoggerSettings> loggerSettings)
    {
        _client = client;
        _logger = logger;
        _daytimeService = daytimeService;
        _loggerSettings = loggerSettings.Value;
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

        _logger.LogInformation("Starting Logger");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (await _daytimeService.IsDaytime())
                {
                    // Create a CancellationToken that gets canceled after a timeout period
                    using var cts =
                        cancellationToken.ExtendWithDelayedToken(TimeSpan.FromMilliseconds(_appSettings.Timeout));
                    _ = LogRealTimeInformation(cancellationToken: cts.Token).ConfigureAwait(false);
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
                await Task.Delay(_appSettings.LoggingInterval, cancellationToken);
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
        InverterData? inverterData = null;
        PvData? pvData = null;
        try
        {
            _logger.LogDebug("Logging real time information");
            var modbusFrame = new ReadRealtimeData(_realTimeDataSettings.InverterStartRegister,
                _realTimeDataSettings.InverterRegisterCount).GetFrameData();
            using var cts =
                cancellationToken.ExtendWithDelayedToken(TimeSpan.FromMilliseconds(_loggerSettings.Timeout));
            var response = await _client.SendAsync(modbusFrame, cancellationToken: cts.Token);
            inverterData = InverterData.FromProtocolResponse(response);

            _logger.LogDebug("{DataPreview}", inverterData.DataPreview);
        }
        catch (Exception ex) when (ex is OperationCanceledException or SocketException)
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
                using var cts =
                    cancellationToken.ExtendWithDelayedToken(TimeSpan.FromMilliseconds(_loggerSettings.Timeout));
                var response = await _client.SendAsync(modbusFrame, cancellationToken: cts.Token);
                pvData = PvData.FromProtocolResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unknown error retrieving real time pv data. {Response}",
                    BitConverter.ToString(_client.ResponseData.ToArray()).Replace("-", ""));
            }
        }

        // Write results to InfluxDB
        await WriteRealTimeInformationToInfluxDb(inverterData, pvData, cancellationToken);
    }

    private async Task WriteRealTimeInformationToInfluxDb(InverterData? inverterData, PvData? pvData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_influxDbSettings.Enabled && (inverterData != null || pvData != null))
            {
                var metrics = new Dictionary<string, object>();

                inverterData?.AddMetrics(ref metrics);
                pvData?.AddMetrics(ref metrics);

                var pointData = new LineProtocolPoint(
                    _influxDbSettings.MetricName,
                    metrics,
                    null,
                    DateTime.UtcNow);

                var payload = new LineProtocolPayload();
                payload.Add(pointData);

                var client = new LineProtocolClient(new Uri(_influxDbSettings.Address), _influxDbSettings.DbName,
                    _influxDbSettings.Username, _influxDbSettings.Password);
                using var cts =
                    cancellationToken.ExtendWithDelayedToken(TimeSpan.FromMilliseconds(_influxDbSettings.Timeout));
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
}