using System.Drawing;
using ColorThiefDotNet;
using Crystals.Core.Models;

namespace Crystals.Core.Utilities;

public static class ColorExtractionUtility
{
    private static readonly ColorThief ColorThief = new();
    private static readonly HttpClient HttpClient = new();
    
    public static async Task<Palette> GetPaletteFromUrl(string url)
    {
        var data = await HttpClient.GetByteArrayAsync(url);
        using var managedStream = new MemoryStream(data);
        using var bitmap = new Bitmap(managedStream);
        return Extract(bitmap);
    }
    
    private static Palette Extract(Bitmap bitmap)
    {
        var rawPalette = ColorThief.GetPalette(bitmap, Palette.Size);
        return new Palette(rawPalette);
    }
}