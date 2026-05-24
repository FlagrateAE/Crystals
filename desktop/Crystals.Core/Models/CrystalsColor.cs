using Crystals.Core.Utilities;

namespace Crystals.Core.Models;

public class CrystalsColor(float h, float s, float v)
{
    public float H = h;
    public float S = s;
    public float V = v;

    private float Vibrance => S * V;

    public bool IsVibrant()
    {
        const float vibranceMonochromeThreshold = 0.15f;
        return Vibrance is > vibranceMonochromeThreshold and < 1 - vibranceMonochromeThreshold;
    }

    public override string ToString()
    {
        var rgb = ColorConverter.HSVtoRGB(this);
        return $"\u001b[38;2;{rgb.R};{rgb.G};{rgb.B}m({H}, {S}, {V})\u001b[0m";
    }
}