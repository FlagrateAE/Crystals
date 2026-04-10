using Avalonia;
using System;
using System.Threading.Tasks;
using Crystals.Core;
using Crystals.Core.Services;
using Crystals.Core.Sources;

namespace Crystals.App;

class Program
{
    private const int WebMediaServicePort = 4030;
    
    
    [STAThread]
    public static async Task Main(string[] args)
    {
        var webMediaService = new WebMediaService(WebMediaServicePort);
        await webMediaService.Start();
        
        var engine = new Engine();
        engine.RegisterSource(new MusicSource(webMediaService));
        Console.WriteLine("Starting engine");
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