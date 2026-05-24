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
        return $"\u001b[38;2;{rgb.R};{rgb.G};{rgb.B}mHSV({H:F0}°, {S * 100}%, {V * 100}%)\u001b[0m";
    }

    public string ToStringRGB()
    {
        var rgb = ColorConverter.HSVtoRGB(this);
        return $"\u001b[38;2;{rgb.R};{rgb.G};{rgb.B}mRGB({rgb.R}, {rgb.G}, {rgb.B})\u001b[0m";
    }
}