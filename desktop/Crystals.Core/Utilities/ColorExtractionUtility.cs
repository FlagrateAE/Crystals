using Crystals.Core.Models;
using SkiaSharp;
using NetPalette;

namespace Crystals.Core.Utilities;

public static class ColorExtractionUtility
{
    private static readonly HttpClient HttpClient = new();
    
    public static async Task<Palette> GetPaletteFromUrl(string url)
    {
        Console.WriteLine($"Downloading {url}");
        var data = await HttpClient.GetByteArrayAsync(url);
        Console.WriteLine($"Downloaded {data.Length} bytes from {url}");
        using var managedStream = new MemoryStream(data);
        using var bitmap = SKBitmap.Decode(managedStream);
        Console.WriteLine($"Decoded {bitmap.Width}x{bitmap.Height} bitmap");
        return Extract(bitmap);
    }
    
    private static Palette Extract(SKBitmap bitmap)
    {
        var gen = PaletteGenerator.FromBitmap(bitmap, maximumColorCount: 16);
        return new Palette(
            gen.DominantColor, 
            gen.VibrantColor, gen.LightVibrantColor, gen.DarkVibrantColor,
            gen.MutedColor, gen.LightMutedColor, gen.DarkVibrantColor
        );
    }
}