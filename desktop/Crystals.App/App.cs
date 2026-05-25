using System.Windows;
using Crystals.App.Services;
using Crystals.App.Windows;
using Crystals.Core;
using Crystals.Core.Devices;
using Crystals.Core.Middlewares;
using Crystals.Core.Services;
using Crystals.Core.Sources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Crystals.App;

public class App : Application
{
    private const int WebMediaPort = 4030;
    private const string ArduinoPort = "COM3";
    private const int ArduinoBaudRate = 9600;

    private readonly IHost _host;

    public App()
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.Configure<ConsoleLifetimeOptions>(options => { options.SuppressStatusMessages = true; });
                
                
                services.AddSingleton<CrystalsEngine>();
                services.AddHostedService(p => p.GetRequiredService<CrystalsEngine>());

                services.AddSingleton<MainWindow>();
                services.AddHostedService<TrayIconService>();
                
                services.AddSingleton<IMiddleware, VibranceMiddleware>();

                services.AddSingleton(_ => new WebMediaService(WebMediaPort));
                services.AddHostedService(p => p.GetRequiredService<WebMediaService>());

                services.AddSingleton<ISource, MusicSource>();

                services.AddSingleton<IDevice, ArduinoDevice>(_ => new ArduinoDevice(ArduinoPort, ArduinoBaudRate));
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await _host.StartAsync();
    }


    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}