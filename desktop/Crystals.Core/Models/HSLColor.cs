using System.Drawing;

namespace Crystals.Core.Models;

public readonly struct HSLColor(Color rgbColor)
{
    public float H { get; } = rgbColor.GetHue();
    public float S { get; } = rgbColor.GetSaturation();
    public float L { get; } = rgbColor.GetBrightness();
}