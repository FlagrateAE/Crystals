using System.Drawing;

namespace Crystals.Core.Models;

public class CrystalsColor(Color color)
{
    public Color RGB { get; } = color;
    public HSVColor HSV { get; } = new(color);

    private float Vibrance => HSV.S * HSV.V;

    public bool IsVibrant()
    {
        const float vibranceMonochromeThreshold = 0.15f;
        return Vibrance is > vibranceMonochromeThreshold and < 1 - vibranceMonochromeThreshold;
    }

    public override string ToString()
    {
        return $"\u001b[38;2;{RGB.R};{RGB.G};{RGB.B}m({RGB.R}, {RGB.G}, {RGB.B})\u001b[0m";
    }

    public string ToStringHSV()
    {
        return $"\u001b[38;2;{RGB.R};{RGB.G};{RGB.B}m({HSV.H}, {HSV.S}, {HSV.V})\u001b[0m";
    }

    public string ToStringRGBandHSV()
    {
        return
            $"\u001b[38;2;{RGB.R};{RGB.G};{RGB.B}mRGB:({RGB.R}, {RGB.G}, {RGB.B}), HSV: ({HSV.H}, {HSV.S}, {HSV.V})\u001b[0m, vibrance {Vibrance} {IsVibrant()}";
    }

    public static CrystalsColor FromHue(double hue)
    {
        hue = Math.Clamp(hue, 0.0, 360.0);
        const double saturation = 1;
        const double value = 1;

        // Sector of the color wheel (0 to 5)
        double sector = hue / 60.0;
        int sectorIndex = (int)Math.Floor(sector);
        double sectorFraction = sector - sectorIndex;

        // Calculate intermediate values for the formula
        double p = value * (1.0 - saturation);
        double q = value * (1.0 - (saturation * sectorFraction));
        double t = value * (1.0 - (saturation * (1.0 - sectorFraction)));

        // Map the results to RGB based on the sector index
        double r = 0, g = 0, b = 0;

        switch (sectorIndex)
        {
            case 0:
            case 6: // 360 degrees wraps around to 0
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

        // Convert normalized [0.0, 1.0] values back to standard [0, 255] RGB bounds
        return new CrystalsColor(Color.FromArgb(
            255,
            (int)Math.Round(r * 255),
            (int)Math.Round(g * 255),
            (int)Math.Round(b * 255)
        ));
    }
}