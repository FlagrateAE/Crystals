using Avalonia;
using System;
using System.Threading.Tasks;
using Crystals.Core;
using Crystals.Core.Devices;
using Crystals.Core.Services;
using Crystals.Core.Sources;

namespace Crystals.App;

class Program
{
    private const int WebMediaServicePort = 4030;

    private const string ArduinoPort = "COM3";
    private const int ArduinoBaudRate = 9600;

    [STAThread]
    public static async Task Main(string[] args)
    {
        var webMediaService = new WebMediaService(WebMediaServicePort);
        _ = Task.Run(webMediaService.Start);

        var engine = new Engine();

        engine.RegisterSource(new MusicSource(webMediaService));

        engine.RegisterDevice(new ArduinoDevice(ArduinoPort, ArduinoBaudRate));

        engine.Start();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}