using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
using MenuItem = System.Windows.Controls.MenuItem;
using ContextMenu = System.Windows.Controls.ContextMenu;

namespace Crystals.App;

public class App : Application
{
    private const int WebMediaPort = 4030;
    private const string ArduinoPort = "COM3";
    private const int ArduinoBaudRate = 9600;

    private readonly IHost _host;
    private NotifyIcon? _trayIcon;
    private bool _isExiting;

    public App()
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.Configure<ConsoleLifetimeOptions>(options => { options.SuppressStatusMessages = true; });

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
        var visual = new DrawingVisual();
        using (var context = visual.RenderOpen())
        {
            context.DrawRectangle(Brushes.Red, null, new Rect(0, 0, 32, 32));
        }

        var renderTarget = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
        renderTarget.Render(visual);
        
        renderTarget.Freeze();

        _trayIcon = new NotifyIcon
        {
            TooltipText = "Flagrate Crystals",
            Icon = renderTarget, 
            Visibility = Visibility.Visible
        };

        var contextMenu = new ContextMenu();

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
        exitItem.Click += (s, args) => 
        {
            _isExiting = true;
            Current.Shutdown();
        };

        contextMenu.Items.Add(overrideRedItem);
        contextMenu.Items.Add(new System.Windows.Controls.Separator());
        contextMenu.Items.Add(exitItem);

        _trayIcon.Menu = contextMenu;

        if (MainWindow != null)
        {
            if (MainWindow.Content is Panel rootPanel)
            {
                rootPanel.Children.Add(_trayIcon);
            }

            MainWindow.Closing += (s, e) =>
            {
                if (!_isExiting)
                {
                    e.Cancel = true;
                    MainWindow.Hide();
                }
            };

            _trayIcon.LeftDoubleClick += (s, args) => 
            {
                MainWindow.Show();
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            };
        }

        _trayIcon.Loaded += (s, e) => { Console.WriteLine("NotifyIcon loaded into visual tree."); };
        
        _trayIcon.Register();

        Console.WriteLine("Finished setting up tray icon");
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