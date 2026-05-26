using System.Drawing;
using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Services;
using Crystals.Core.Utilities;
using ColorConverter = Crystals.Core.Utilities.ColorConverter;

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
        var rgbColor = await ColorExtractionUtility.GetMainColorFromUrl(media.Thumbnail);
        var color = ColorConverter.RGBtoHSV(rgbColor);
        Console.WriteLine($"[MusicSource] Now playing: {media.Title} by {media.Artist}");
        OnColorChanged?.Invoke(this, color);
    }
}