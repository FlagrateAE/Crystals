using System.Drawing;
using ColorThiefDotNet;
using Color = System.Drawing.Color;

namespace Crystals.Core.Utilities;

public static class ColorExtractionUtility
{
    private static readonly ColorThief ColorThief = new();
    private static readonly HttpClient HttpClient = new();

    public static async Task<Color> GetMainColorFromUrl(string url)
    {
        var data = await HttpClient.GetByteArrayAsync(url);
        using var managedStream = new MemoryStream(data);
        using var bitmap = new Bitmap(managedStream);
        return Extract(bitmap);
    }

    private static Color Extract(Bitmap bitmap)
    {
        var dominantColor = ColorThief.GetColor(bitmap).Color;
        return Color.FromArgb(dominantColor.R, dominantColor.G, dominantColor.B);
    }
}