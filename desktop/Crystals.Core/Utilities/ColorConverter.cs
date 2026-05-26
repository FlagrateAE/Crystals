using Crystals.Core.Models;
using Wacton.Unicolour;
using Color = System.Drawing.Color;

namespace Crystals.Core.Utilities;

public static class ColorConverter
{
    public static Color HSVtoRGB(CrystalsColor color)
    {
        var hue = color.H;
        var saturation = color.S;
        var value = color.V;

        var sector = hue / 60.0;
        var sectorIndex = (int)Math.Floor(sector);
        var sectorFraction = sector - sectorIndex;

        var p = value * (1.0 - saturation);
        var q = value * (1.0 - (saturation * sectorFraction));
        var t = value * (1.0 - (saturation * (1.0 - sectorFraction)));

        double r = 0, g = 0, b = 0;

        switch (sectorIndex)
        {
            case 0:
            case 6:
                r = value;
                g = t;
                b = p;
                break;
            case 1:
                r = q;
                g = value;
                b = p;
                break;
            case 2:
                r = p;
                g = value;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = value;
                break;
            case 4:
                r = t;
                g = p;
                b = value;
                break;
            case 5:
                r = value;
                g = p;
                b = q;
                break;
        }

        return Color.FromArgb(
            255,
            (int)Math.Round(r * 255),
            (int)Math.Round(g * 255),
            (int)Math.Round(b * 255)
        );
    }

    public static CrystalsColor RGBtoHSV(ColorThiefDotNet.Color color)
    {
        var uniColor = new Unicolour(ColourSpace.Rgb255, color.R, color.G, color.B).Hsb;
        return new CrystalsColor((float)uniColor.H, (float)uniColor.S, (float)uniColor.B);
    }
}