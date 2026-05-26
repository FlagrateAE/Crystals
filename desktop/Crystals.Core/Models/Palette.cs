using ColorThiefDotNet;
using Crystals.Core.Utilities;

namespace Crystals.Core.Models;

public readonly struct Palette
{
    public const int Size = 4;

    public List<CrystalsColor> Colors { get; }

    public Palette(List<QuantizedColor> rawPalette)
    {
        Colors = rawPalette.Select(c => ColorConverter.RGBtoHSV(c.Color)).ToList();

        foreach (var color in Colors)
        {
            Console.WriteLine($"[Palette] {color}");
        }
    }

    public CrystalsColor GetVibrantColor()
    {
        // foreach (var color in Colors)
        // {
        //     if (color.IsVibrant()) return color;
        // }

        return CrystalsColor.White;
    }

    public override string ToString() => string.Join(", ", Colors);
}