using Crystals.Core.Models;
using Crystals.Core.Sources.Services;

namespace Crystals.Core.Sources;

public class MusicSource : ISource
{
    private readonly GSMTCService _gsmtcService = new();

    public async Task Start()
    {
        Console.WriteLine("Starting music source...");

        _gsmtcService.OnMediaChanged += (m) =>
        {
            Console.WriteLine("Media changed: " + m.Title);
        };
        
        await _gsmtcService.Start();
    }

    public event Action<Color>? OnColorChanged;
    public Color GetColor() => throw new NotImplementedException();
}