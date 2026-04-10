using NetPalette;
using SkiaSharp;

namespace Crystals.Core.Utilities;

public static class ColorPrintingUtility
{
    public static string ColoredForTerminal(this PaletteColor color)
    {
        return $"\u001b[38;2;{color.Color.Red};{color.Color.Green};{color.Color.Blue}m";
    }
}