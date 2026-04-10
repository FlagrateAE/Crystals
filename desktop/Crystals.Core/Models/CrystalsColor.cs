using System.Drawing;

namespace Crystals.Core.Models;

public class CrystalsColor
{
    public Color RGB { get; }
    public HSLColor HSL { get; }

    public CrystalsColor(Color color)
    {
        RGB = color;
        HSL = new HSLColor(color);
    }

    public override string ToString()
    {
        return $"\u001b[38;2;{RGB.R};{RGB.G};{RGB.B}m({RGB.R}, {RGB.G}, {RGB.B})\u001b[0m";
    }

    public string ToStringHSL()
    {
        return $"\u001b[38;2;{RGB.R};{RGB.G};{RGB.B}m({HSL.H}, {HSL.S}, {HSL.L})\u001b[0m";
    }

    public string ToStringRGBandHSL()
    {
        return $"\u001b[38;2;{RGB.R};{RGB.G};{RGB.B}mRGB:({RGB.R}, {RGB.G}, {RGB.B}), HSL: ({HSL.H}, {HSL.S}, {HSL.L})\u001b[0m";
    }
}