using System.Drawing;
using ColorThiefDotNet;
using Crystals.Core.Models;

namespace Crystals.Core.Utilities;

public static class ColorPaletteExtractor
{
    private static readonly ColorThief ColorThief = new();

    public static Palette Extract(Bitmap bitmap)
    {
        var rawPalette = ColorThief.GetPalette(bitmap, Palette.Size, 20, false);
        var modelPalette = rawPalette.Select(c => new CrystalsColor(c.Color.R, c.Color.G, c.Color.B)).ToList();
        return new Palette(modelPalette);
    }
}