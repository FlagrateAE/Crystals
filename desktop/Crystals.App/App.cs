using System.Windows;
using Crystals.App.Services;
using Crystals.App.Windows;
using Crystals.Core;
using Crystals.Core.Devices;
using Crystals.Core.Middlewares;
using Crystals.Core.Middlewares.Preprocessors;
using Crystals.Core.Services;
using Crystals.Core.Sources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Markup;

namespace Crystals.App;

public class App : Application
{
    private const int WebMediaPort = 5663;
    private const string ArduinoPort = "COM3";
    private const int ArduinoBaudRate = 9600;

    private readonly IHost _host;

    public App()
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        Resources.MergedDictionaries.Add(new ThemesDictionary { Theme = ApplicationTheme.Dark });
        Resources.MergedDictionaries.Add(new ControlsDictionary());

        System.Windows.Controls.TextBlock.FontWeightProperty.OverrideMetadata(
            typeof(TextBlock),
            new FrameworkPropertyMetadata(FontWeights.Thin)
        );

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.Configure<ConsoleLifetimeOptions>(options => { options.SuppressStatusMessages = true; });


                services.AddSingleton<CrystalsEngine>();
                services.AddHostedService(p => p.GetRequiredService<CrystalsEngine>());

                services.AddSingleton<MainWindow>();
                services.AddHostedService<TrayIconService>();
                services.AddHostedService<HotkeyService>();

                services.AddSingleton<IPreprocessor, VibrancePreprocessor>();

                services.AddSingleton<IPostprocessor, BlueishPostprocessor>();

                services.AddSingleton(_ => new WebMediaService(WebMediaPort));
                services.AddHostedService(p => p.GetRequiredService<WebMediaService>());

                services.AddSingleton<MediaExceptionService>();
                services.AddHostedService(p => p.GetRequiredService<MediaExceptionService>());

                services.AddSingleton<MysticLightService>();
                services.AddHostedService(p => p.GetRequiredService<MysticLightService>());

                services.AddSingleton<ISource, MusicSource>();

                services.AddSingleton<IDevice, ArduinoDevice>(_ => new ArduinoDevice(ArduinoPort, ArduinoBaudRate));
                services.AddSingleton<IDevice, MysticLightDevice>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);
        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        await _host.StartAsync();
    }


    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}