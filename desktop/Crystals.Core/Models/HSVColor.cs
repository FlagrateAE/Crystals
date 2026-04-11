using System.Drawing;

namespace Crystals.Core.Models;

public readonly struct HSVColor
{
    public float H { get; }
    public float S { get; }
    public float V { get; }
    
    public HSVColor(Color rgbColor)
    {
        var hslS = rgbColor.GetSaturation();
        var hslL = rgbColor.GetBrightness();

        H = rgbColor.GetHue();
        V = hslL + hslS * Math.Min(hslL, 1 - hslL);
        S = (V == 0) ? 0 : 2 * (1 - hslL / V);
    }
}