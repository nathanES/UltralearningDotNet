using AspNetCoreWorkerService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(hostConfig =>
    {
        // Setup host-level configuration
        hostConfig.SetBasePath(Directory.GetCurrentDirectory());
        hostConfig.AddJsonFile("hostsettings.json", optional: true, reloadOnChange: true);
        hostConfig.AddEnvironmentVariables(prefix: "DOTNET_");
        hostConfig.AddCommandLine(args);
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        // Setup app-level configuration
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
    })
    .ConfigureLogging(logging =>
    {
        // Configure logging
        logging.ClearProviders();
        logging.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        // Register services for DI
        services.AddHostedService<BackgroundService>();
    })
    .Build();

await host.RunAsync();