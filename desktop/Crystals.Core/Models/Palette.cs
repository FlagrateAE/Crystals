using ColorThiefDotNet;
using Crystals.Core.Utilities;

namespace Crystals.Core.Models;

public readonly struct Palette(List<CrystalsColor> colors)
{
    public const int Size = 4;

    public List<CrystalsColor> Colors { get; } = colors;

    public CrystalsColor GetVibrantColor()
    {
        foreach (var color in Colors)
        {
            if (color.IsVibrant()) return color;
        }

        return CrystalsColor.White;
    }

    public override string ToString() => string.Join(", ", Colors);
}