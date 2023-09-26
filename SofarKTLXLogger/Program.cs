using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SofarKTLXLogger.Daytime;
using SofarKTLXLogger.Settings;
using SofarKTLXLogger.SolarmanV5;

namespace SofarKTLXLogger;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        // Create service collection
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        // Create service provider
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        // Entry to run app
        await serviceProvider.GetRequiredService<Logger>().RunAsync(cancellationTokenSource.Token);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // Add instances to DI
        services.AddSingleton<IConfiguration>(configuration);
        
        // Add Serilog
        services.AddLogging(config => 
        {
            config.ClearProviders();
            config.AddSerilog(dispose: true);
        });

        // Add logger
        services.AddTransient<Logger>();

        // Add settings
        services
            .AddOptions<LoggerSettings>()
            .Bind(configuration.GetSection(LoggerSettings.SectionName))
            .ValidateOnStart();
        services
            .AddOptions<AppSettings>()
            .Bind(configuration.GetSection(AppSettings.SectionName))
            .ValidateOnStart();
        services
            .AddOptions<ProductInfoSettings>()
            .Bind(configuration.GetSection(ProductInfoSettings.SectionName))
            .ValidateOnStart();
        services
            .AddOptions<RealTimeDataSettings>()
            .Bind(configuration.GetSection(RealTimeDataSettings.SectionName))
            .ValidateOnStart();
        services
            .AddOptions<InfluxDbSettings>()
            .Bind(configuration.GetSection(InfluxDbSettings.SectionName))
            .ValidateOnStart();

        // SolarmanV5Client
        services.AddTransient<ISolarmanV5Client, SolarmanV5Client>();
        
        // Other services
        services.AddSingleton<IDaytimeService, DaytimeService>();
    }
}