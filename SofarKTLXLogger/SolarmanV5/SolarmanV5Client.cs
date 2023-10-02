using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SofarKTLXLogger.Daytime;
using SofarKTLXLogger.ModbusRTU.ProductInformation;
using SofarKTLXLogger.ModbusRTU.RealtimeData;
using SofarKTLXLogger.Settings;
using SofarKTLXLogger.SolarmanV5.Protocol;

namespace SofarKTLXLogger.SolarmanV5;

public class SolarmanV5Client : ISolarmanV5Client
{
    private readonly ILogger<SolarmanV5Client> _logger;
    private readonly LoggerSettings _loggerSettings;
    private readonly AppSettings _appSettings;
    private readonly ProductInfoSettings _productInfoSettings;
    private readonly RealTimeDataSettings _realTimeDataSettings;
    private readonly IDaytimeService _daytimeService;

    public Memory<byte> ResponseData { get; private set; }

    public SolarmanV5Client(IOptions<LoggerSettings> loggerSettings, IOptions<AppSettings> inverterSettings,
        IOptions<ProductInfoSettings> productInfoSettings, IOptions<RealTimeDataSettings> realTimeDataSettings,
        ILogger<SolarmanV5Client> logger, IDaytimeService daytimeService)
    {
        _logger = logger;
        _daytimeService = daytimeService;
        _loggerSettings = loggerSettings.Value;
        _appSettings = inverterSettings.Value;
        _productInfoSettings = productInfoSettings.Value;
        _realTimeDataSettings = realTimeDataSettings.Value;
    }

    public async Task<ProtocolResponse> SendAsync(Memory<byte> modbusFrame,
        CancellationToken cancellationToken = default)
    {
        if (_appSettings.OfflineMode)
        {
            ResponseData = await File.ReadAllBytesAsync("inverter_response.bin", cancellationToken);
            _logger.LogTrace("Returning fake data: {Bytes}", ResponseData);
            return ProtocolResponse.FromMemory(ResponseData);
        }
        
        using var client = await GetClientAsync(cancellationToken);

        // Prepares the data to send.
        var dataBytes = ProtocolRequest.Serialize(_loggerSettings.SerialNumber, modbusFrame);

        // Send data to the server
        var stream = client.GetStream();
        var watch = Stopwatch.StartNew();
        await stream.WriteAsync(dataBytes, cancellationToken: cancellationToken);
        watch.Stop();
        
        _logger.LogTrace("Sent bytes (in {Ms}ms): {Bytes}", watch.Elapsed.TotalMilliseconds, dataBytes);

        // Receives the response back from the server.
        var buffer = new Memory<byte>(new byte[1024]);

        watch.Restart();
        var bytesRead = await stream.ReadAsync(buffer, cancellationToken);
        watch.Stop();
        
        ResponseData = buffer[..bytesRead];
        
        _logger.LogTrace("Received bytes (in {Ms}ms): {Bytes}", watch.Elapsed.TotalMilliseconds, ResponseData);

        return ProtocolResponse.FromMemory(ResponseData);
    }

    public byte[] GetHwData()
    {
        using var client = GetClientAsync().Result;

        // Prepares the data to send.
        var dataBytes = ProtocolRequest.Serialize(_loggerSettings.SerialNumber, new ReadProductInformation(
            _productInfoSettings.StartRegister,
            _productInfoSettings.RegisterCount));

        // Send data to the server
        var stream = client.GetStream();
        stream.Write(dataBytes, 0, dataBytes.Length);

        // Receives the response back from the server.
        var receivedBytes = new byte[1024];
        var bytesCount = stream.Read(receivedBytes, 0, receivedBytes.Length);
        var receivedData = Encoding.ASCII.GetString(receivedBytes, 0, bytesCount);

        return receivedBytes;
    }

    public (byte[] Part1, byte[] Part2) GetRealtimeData()
    {
        using var client = GetClientAsync().Result;

        // Prepares the data to send.
        var dataBytes = ProtocolRequest.Serialize(_loggerSettings.SerialNumber, new ReadRealtimeData(
            _realTimeDataSettings.InverterStartRegister,
            _realTimeDataSettings.InverterRegisterCount));

        // Send data to the server
        var stream = client.GetStream();
        stream.Write(dataBytes, 0, dataBytes.Length);

        // Receives the response back from the server.
        var receivedBytes = new byte[1024];
        var bytesCount = stream.Read(receivedBytes, 0, receivedBytes.Length);
        // var receivedData = Encoding.ASCII.GetString(receivedBytes, 0, bytesCount);

        var dataBytes2 = ProtocolRequest.Serialize(_loggerSettings.SerialNumber, new ReadRealtimeData(
            _realTimeDataSettings.PvStartRegister,
            _realTimeDataSettings.PvRegisterCount));

        // Send data to the server
        var stream2 = client.GetStream();
        stream2.Write(dataBytes2, 0, dataBytes2.Length);

        // Receives the response back from the server.
        var receivedBytes2 = new byte[1024];
        var bytesCount2 = stream2.Read(receivedBytes2, 0, receivedBytes2.Length);
        // var receivedData2 = Encoding.ASCII.GetString(receivedBytes2, 0, bytesCount2);

        return (receivedBytes, receivedBytes2);
    }

    private async Task<TcpClient> GetClientAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Parse the server's IP address.
            var ipAddress = IPAddress.Parse(_loggerSettings.Ip);

            // Create a TcpClient.
            var client = new TcpClient(AddressFamily.InterNetwork)
            {
                ReceiveTimeout = _loggerSettings.Timeout,
                SendTimeout = _loggerSettings.Timeout
            };

            // Establish a connection.
            await client.ConnectAsync(ipAddress, _loggerSettings.Port, cancellationToken);

            return client;
        }
        catch (SocketException ex)
        {
            if (await _daytimeService.IsDaytime())
            {
                _logger.LogWarning("Couldn't connect during daytime, is inverter/logger offline?");
                _logger.LogDebug(ex, "Couldn't connect, inverter/logger probably offline");
            }
            else
            {
                _logger.LogDebug(ex, "Couldn't connect, inverter/logger probably offline");
            }
            throw;
        }
    }
}