using System.Windows;
using Crystals.Core;
using Crystals.Core.Devices;
using Crystals.Core.Middlewares;
using Crystals.Core.Models;
using Crystals.Core.Services;
using Crystals.Core.Sources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Controls;
using Wpf.Ui.Tray.Controls;

namespace Crystals.App;

public class App : Application
{
    private const int WebMediaPort = 4030;

    private const string ArduinoPort = "COM3";
    private const int ArduinoBaudRate = 9600;

    private readonly IHost _host;
    private NotifyIcon? _trayIcon;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.Configure<ConsoleLifetimeOptions>(options => 
                {
                    options.SuppressStatusMessages = true;
                });
                
                services.AddHostedService<CrystalsEngine>();

                services.AddSingleton<IMiddleware, VibranceMiddleware>();

                services.AddSingleton(_ => new WebMediaService(WebMediaPort));
                services.AddHostedService(provider => provider.GetRequiredService<WebMediaService>());

                services.AddSingleton<ISource, MusicSource>();

                services.AddSingleton<IDevice, ArduinoDevice>(_ => new ArduinoDevice(ArduinoPort, ArduinoBaudRate));
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        InitializeTrayIcon();

        await _host.StartAsync();
    }

    private void InitializeTrayIcon()
    {
        Console.WriteLine("Initializing tray icon");
        _trayIcon = new NotifyIcon
        {
            TooltipText = "Flagrate Crystals",
            // Icon = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri("pack://application:,,,/Assets/icon.png",
            //     UriKind.Absolute))
        };

        var contextMenu = new System.Windows.Controls.ContextMenu();

        var overrideRedItem = new MenuItem
        {
            Header = "Override: Red",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Color24 }
        };
        overrideRedItem.Click += (s, args) =>
        {
            var engine = _host.Services.GetServices<IHostedService>()
                .OfType<CrystalsEngine>()
                .First();

            engine.ManualOverride(new CrystalsColor(0f, 1f, 1f));
        };

        var exitItem = new MenuItem
        {
            Header = "Exit",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Dismiss24 }
        };
        exitItem.Click += (s, args) => Current.Shutdown();

        contextMenu.Items.Add(overrideRedItem);
        contextMenu.Items.Add(new System.Windows.Controls.Separator());
        contextMenu.Items.Add(exitItem);

        _trayIcon.Menu = contextMenu;

        _trayIcon.Register();

        _trayIcon.LeftDoubleClick += (s, args) => { };
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_trayIcon != null)
        {
            _trayIcon.Unregister();
            _trayIcon.Dispose();
        }

        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}