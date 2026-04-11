using Crystals.Core.Models;
using Crystals.Core.Services;
using Crystals.Core.Utilities;

namespace Crystals.Core.Sources;

public class MusicSource(WebMediaService service) : ISource
{
    public int FocusPriority => 1;
    public event Action<CrystalsColor>? OnColorChanged;

    public void Start()
    {
        service.OnMediaChanged += OnMediaChanged;
    }

    private async void OnMediaChanged(Media media)
    {
        var palette = await ColorExtractionUtility.GetPaletteFromUrl(media.Thumbnail);

        Console.WriteLine($"{media.Title} by {media.Artist}");
        OnColorChanged?.Invoke(palette.GetVibrantColor());
    }
}