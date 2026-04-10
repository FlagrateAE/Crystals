using ColorThiefDotNet;

namespace Crystals.Core.Models;

public struct Palette
{
    private const int PaletteSize = 4;

    public List<CrystalsColor> Colors { get; }

    // public CrystalsColor GetVibrantColor()
    // {
    // }
    //
    // private bool IsMonochrome()
    // {
    // }

    public Palette(List<QuantizedColor> colors)
    {
        int countToTake = Math.Min(colors.Count, PaletteSize);
        Colors = colors.GetRange(0, countToTake)
            .Select(c => new CrystalsColor(System.Drawing.Color.FromArgb(c.Color.R, c.Color.G, c.Color.B)))
            .ToList();
    }

    public override string ToString() => string.Join(", ", Colors);
    
    public string ToStringHSL() => string.Join(", ", Colors.Select(c => c.ToStringHSL()));
    
    public string ToStringRGBandHSL() => string.Join("\n", Colors.Select(c => c.ToStringRGBandHSL()));
}