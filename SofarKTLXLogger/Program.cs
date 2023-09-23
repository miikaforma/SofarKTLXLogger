using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SofarKTLXLogger.Settings;
using SofarKTLXLogger.SolarmanV5;

namespace SofarKTLXLogger;

internal class Program
{
    private static void Main(string[] args)
    {
        // Create service collection
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        // Create service provider
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        // Entry to run app
        serviceProvider.GetService<Logger>()?.Run();
    }

    private static void ConfigureServices(IServiceCollection serviceCollection)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();

        // Add instances to DI
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        // Add logger
        serviceCollection.AddTransient<Logger>();

        // Add settings
        serviceCollection
            .AddOptions<LoggerSettings>()
            .Bind(configuration.GetSection(LoggerSettings.SectionName))
            .ValidateOnStart();
        serviceCollection
            .AddOptions<AppSettings>()
            .Bind(configuration.GetSection(AppSettings.SectionName))
            .ValidateOnStart();
        serviceCollection
            .AddOptions<ProductInfoSettings>()
            .Bind(configuration.GetSection(ProductInfoSettings.SectionName))
            .ValidateOnStart();
        serviceCollection
            .AddOptions<RealTimeDataSettings>()
            .Bind(configuration.GetSection(RealTimeDataSettings.SectionName))
            .ValidateOnStart();

        // SolarmanV5Client
        serviceCollection.AddTransient<ISolarmanV5Client, SolarmanV5Client>();
    }
}