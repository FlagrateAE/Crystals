using Crystals.Core.Utilities;
using NetPalette;

namespace Crystals.Core.Models;

public record Palette(
    PaletteColor DominantColor,
    PaletteColor VibrantColor,
    PaletteColor LightVibrantColor,
    PaletteColor DarkVibrantColor,
    PaletteColor MutedColor,
    PaletteColor LightMutedColor,
    PaletteColor MutterColor
)
{
    public override string ToString() => $"{DominantColor.ColoredForTerminal()} {VibrantColor.ColoredForTerminal()} {LightVibrantColor.ColoredForTerminal()} {DarkVibrantColor.ColoredForTerminal()} {MutedColor.ColoredForTerminal()} {LightMutedColor.ColoredForTerminal()} {MutterColor.ColoredForTerminal()}";
}