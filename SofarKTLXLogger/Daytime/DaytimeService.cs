using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SofarKTLXLogger.Daytime.Models;
using SofarKTLXLogger.Settings;

namespace SofarKTLXLogger.Daytime;

public class DaytimeService : IDaytimeService
{
    private const string ApiEndpoint = "https://api.sunrise-sunset.org/json";
    private readonly ILogger<DaytimeService> _logger;
    private readonly AppSettings _appSettings;
    private KeyValuePair<DateTime, Result>? _cachedResult;

    public DaytimeService(IOptions<AppSettings> appSettings, ILogger<DaytimeService> logger)
    {
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    public async Task<bool> IsDaytime()
    {
        if (!_appSettings.EnableDaytimeChecking)
        {
            // If daytime checking is disabled, always return true
            return true;
        }

        var dateNow = DateNow().Date;
        if (_cachedResult.HasValue && _cachedResult.Value.Key.Equals(dateNow))
        {
            var result = _cachedResult.Value.Value;
            return IsDayTime(result);
        }

        var fetchedResult = await GetSunsetSunriseResult(dateNow);
        if (fetchedResult == null)
        {
            // On error, return true to continue fetching
            return true;
        }

        // Cache the new result
        _cachedResult = new KeyValuePair<DateTime, Result>(dateNow, fetchedResult);

        return IsDayTime(fetchedResult);
    }

    private bool IsDayTime(Result result)
    {
        var dateTimeNow = DateNow().ToUniversalTime();
        _logger.LogTrace("DateTimeNow: {DateTimeNow} | Sunrise: {Sunrise} | Sunset: {Sunset}", dateTimeNow,
            result.Sunrise, result.Sunset);
        return dateTimeNow >= result.Sunrise && dateTimeNow <= result.Sunset;
    }

    private async Task<Result?> GetSunsetSunriseResult(DateTime dateNow)
    {
        try
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10),
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri =
                    new Uri(
                        $"{ApiEndpoint}?lat={_appSettings.Latitude}&lng={_appSettings.Longitude}&formatted=0&date={dateNow:yyyy-MM-dd}"),
            };
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var resultResponse = JsonSerializer.Deserialize<Response>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (resultResponse == null)
            {
                _logger.LogWarning("Failed to parse daytime API result. {Body}", body);
                return null;
            }

            _logger.LogInformation("New daytime fetched with sunrise at {Sunrise} and sunset at {Sunset}",
                GetLocalDateTimeOffset(resultResponse.Result.Sunrise),
                GetLocalDateTimeOffset(resultResponse.Result.Sunset));

            return resultResponse?.Result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch new daytime data");
            return null;
        }
    }

    private DateTimeOffset DateNow()
    {
        var utcTime = DateTime.UtcNow;
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_appSettings.TimeZone);
        var localTime = TimeZoneInfo.ConvertTime(utcTime, timeZone);
        
        // Calculate offset
        var offset = timeZone.GetUtcOffset(localTime);
   
        // Return DateTimeOffset
        return new DateTimeOffset(localTime, offset);
    }

    private DateTimeOffset GetLocalDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_appSettings.TimeZone);
        return TimeZoneInfo.ConvertTime(dateTimeOffset, timeZone);
    }
}