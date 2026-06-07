using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Models.SourceModels;
using Crystals.Core.Services;
using Crystals.Core.Utilities;

namespace Crystals.Core.Sources;

public class MusicSource(WebMediaService service) : ISource
{
    public int FocusPriority => 1;
    public BitmapSource Icon { get; } = BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Sources/Music.png"));
    public ISourceModel? CurrentSource => service.CurrentMedia;
    public CrystalsColor CurrentColor { get; private set; }
    public event EventHandler<CrystalsColor>? OnColorChanged;
    
    public void Start()
    {
        service.OnMediaChanged += OnMediaChanged;
        Console.WriteLine("[MusicSource] Source successfully started.");
    }

    private void OnMediaChanged(Media media)
    {
        var palette = ColorPaletteExtractor.Extract(media.Image);
        var color = palette.GetVibrantColor();
        CurrentColor = color;
        Console.WriteLine($"[MusicSource] Now playing: {media.Name} by {media.Description}");
        OnColorChanged?.Invoke(this, color);
    }
}