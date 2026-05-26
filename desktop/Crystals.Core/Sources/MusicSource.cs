using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Services;
using Crystals.Core.Utilities;

namespace Crystals.Core.Sources;

public class MusicSource(WebMediaService service) : ISource
{
    public int FocusPriority => 1;
    public BitmapSource Icon { get; } = BitmapFrame.Create(IconUri);
    public event EventHandler<CrystalsColor>? OnColorChanged;

    private static readonly Uri IconUri = new("pack://application:,,,/Resources/Sources/MusicSource.png");

    public void Start()
    {
        service.OnMediaChanged += OnMediaChanged;
        Console.WriteLine("[MusicSource] Source successfully started.");
    }

    private async void OnMediaChanged(Media media)
    {
        var palette = await ColorPaletteExtractor.GetPaletteFromUrl(media.Thumbnail);
        var color = palette.GetVibrantColor();
        Console.WriteLine($"[MusicSource] Now playing: {media.Title} by {media.Artist}");
        OnColorChanged?.Invoke(this, color);
    }
}