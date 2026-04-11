using ColorThiefDotNet;
using Color = System.Drawing.Color;

namespace Crystals.Core.Models;

public readonly struct Palette
{
    public const int Size = 4;

    public List<CrystalsColor> Colors { get; }

    public Palette(List<QuantizedColor> colors)
    {
        Colors = colors.Select(c => new CrystalsColor(Color.FromArgb(c.Color.R, c.Color.G, c.Color.B))).ToList();
    }

    public CrystalsColor GetVibrantColor()
    {
        foreach (var color in Colors){
            if (color.IsVibrant()) return color;
        }

        return new(Color.White);
    }

    public override string ToString() => string.Join(", ", Colors);

    public string ToStringHSV() => string.Join(", ", Colors.Select(c => c.ToStringHSV()));

    public string ToStringRGBandHSV() => string.Join("\n", Colors.Select(c => c.ToStringRGBandHSV()));
}