using Crystals.Core.Models;
using Crystals.Core.Services;
using Crystals.Core.Utilities;

namespace Crystals.Core.Sources;

public class MusicSource(WebMediaService service) : ISource
{
    public void Start()
    {
        service.OnMediaChanged += OnMediaChanged;
        Console.WriteLine("Music source started");
    }

    public event Action? RequestFocus;
    public event Action<Color>? OnColorChanged;

    private async void OnMediaChanged(Media media)
    {
        Console.WriteLine("Media changed");
        var palette = await ColorExtractionUtility.GetPaletteFromUrl(media.Thumbnail);
        
        Console.WriteLine($"{media.Title} by {media.Artist}");
        Console.WriteLine(palette);
    }
}