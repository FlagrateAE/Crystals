using System.Diagnostics;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Crystals.Core.Models;
using Crystals.Core.Sources.Services;

namespace Crystals.Core.Sources;

public class MusicSource : ISource
{
    private readonly GSMTCService _gsmtcService = new();

    public async Task Start()
    {
        Console.WriteLine("Starting music source...");

        _gsmtcService.OnMediaChanged += async void (m) =>
        {
            Console.WriteLine("Media changed: " + m.Title);
            Console.WriteLine("is music: " + await IsMusic(m));
        };
        
        await _gsmtcService.Start();
    }

    public event Action<Color>? OnColorChanged;
    public Color GetColor() => throw new NotImplementedException();

    private async Task<bool> IsMusic(Media media)
    {
        const float squareRatioDeviation = 0.05f;
        
        var ratio = await GetThumbnailAspectRatio(media.Thumbnail);
        return Math.Abs(ratio - 1) < squareRatioDeviation;
    }

    private async Task<double> GetThumbnailAspectRatio(IRandomAccessStreamReference thumbnailRef)
    {
        try
        {
            // 1. Open the native stream
            using var windowsStream = await thumbnailRef.OpenReadAsync();
        
            // 2. Immediately copy it to a managed MemoryStream
            // This prevents ObjectDisposedException if the OS closes the native stream
            var memoryStream = new MemoryStream();
            await windowsStream.AsStreamForRead().CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset position for the decoder

            // 3. Convert back to a WinRT stream for the BitmapDecoder
            var raStream = memoryStream.AsRandomAccessStream();

            // 4. Create the decoder
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(raStream);

            uint width = decoder.PixelWidth;
            uint height = decoder.PixelHeight;

            if (height == 0) return 0;

            return (double)width / height;
        }
        catch (Exception ex)
        {
            // Log the error but don't crash the app
            Console.WriteLine($"Thumbnail error: {ex.Message}");
            return 0;
        }
    }
}