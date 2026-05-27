using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Models.SourceModels;
using Crystals.Core.Services;
using Crystals.Core.Utilities;

namespace Crystals.Core.Sources;

public class MusicSource(WebMediaService service) : ISource
{
    public int FocusPriority => 1;
    public BitmapSource SourceIcon { get; } = BitmapFrame.Create(IconUri);
    public ISourceModel? CurrentSource => service.CurrentMedia;
    public event EventHandler<CrystalsColor>? OnColorChanged;

    private static readonly Uri IconUri = new("pack://application:,,,/Resources/Sources/MusicSource.png");

    public void Start()
    {
        service.OnMediaChanged += OnMediaChanged;
        Console.WriteLine("[MusicSource] Source successfully started.");
    }

    private void OnMediaChanged(Media media)
    {
        var palette = ColorPaletteExtractor.Extract(media.Image);
        var color = palette.GetVibrantColor();
        Console.WriteLine($"[MusicSource] Now playing: {media.Name} by {media.Description}");
        OnColorChanged?.Invoke(this, color);
    }
}