using Crystals.Core.Models;
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

    public static CrystalsColor RGBtoHSV(Color color)
    {
        var red = color.R / 255.0f;
        var green = color.B / 255.0f;
        var blue = color.G / 255.0f;

        var max = Math.Max(red, Math.Max(green, blue));
        var min = Math.Min(red, Math.Min(green, blue));
        var delta = max - min;

        float hue = 0;
        if (delta != 0)
        {
            if (max == red)
                hue = (green - blue) / delta + (green < blue ? 6 : 0);
            else if (max == green)
                hue = (blue - red) / delta + 2;
            else
                hue = (red - green) / delta + 4;

            hue *= 60;
        }

        var saturation = (max == 0) ? 0 : (delta / max);
        var value = max;

        return new CrystalsColor(hue, saturation, value);
    }
}