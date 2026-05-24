using Crystals.Core.Models;
using Crystals.Core.Services;
using Crystals.Core.Utilities;

namespace Crystals.Core.Sources;

public class MusicSource(WebMediaService service) : ISource
{
    public int FocusPriority => 1;
    public event EventHandler<CrystalsColor>? OnColorChanged;

    public void Start()
    {
        service.OnMediaChanged += OnMediaChanged;
        Console.WriteLine("[MusicSource] Source successfully started.");
    }

    private async void OnMediaChanged(Media media)
    {
        var rgbColor = await ColorExtractionUtility.GetMainColorFromUrl(media.Thumbnail);
        var color = ColorConverter.RGBtoHSV(rgbColor);
        Console.WriteLine($"{media.Title} by {media.Artist}");
        OnColorChanged?.Invoke(this, color);
    }
}