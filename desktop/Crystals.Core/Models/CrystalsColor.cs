using System.Drawing;
using Windows.ApplicationModel.Appointments.DataProvider;

namespace Crystals.Core.Models;

public class CrystalsColor(Color color)
{
    private Color RGB { get; } = color;
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
        return $"\u001b[38;2;{RGB.R};{RGB.G};{RGB.B}mRGB:({RGB.R}, {RGB.G}, {RGB.B}), HSV: ({HSV.H}, {HSV.S}, {HSV.V})\u001b[0m, vibrance {Vibrance} {IsVibrant()}";
    }
}