using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Models.SourceModels;
using Crystals.Core.Services;
using Crystals.Core.Utilities;

namespace Crystals.Core.Sources;

public class MusicSource(WebMediaService webService, MediaExceptionService exceptionService) : ISource
{
    public int FocusPriority => 1;

    public BitmapSource Icon { get; } =
        BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Sources/Music.png"));

    public ISourceModel? CurrentSource => webService.CurrentMedia;
    public CrystalsColor CurrentColor { get; private set; }
    public event EventHandler<CrystalsColor>? OnColorChanged;

    private Media? _currentMedia;

    public void Start()
    {
        webService.OnMediaChanged += OnMediaChanged;
        Console.WriteLine("[MusicSource] Source successfully started.");
    }

    public void AddException(CrystalsColor color)
    {
        exceptionService.AddException(_currentMedia!, color);
    }

    private void OnMediaChanged(Media media)
    {
        _currentMedia = media;

        if (!exceptionService.IsInExceptions(media, out var color))
        {
            var palette = ColorPaletteExtractor.Extract(media.Image);
            color = palette.GetVibrantColor();
        }

        CurrentColor = color;
        Console.WriteLine($"[MusicSource] Now playing: {media.Name} by {media.Description}");
        OnColorChanged?.Invoke(this, color);
    }
}