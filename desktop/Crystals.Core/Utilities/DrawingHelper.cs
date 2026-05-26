using System.Windows.Media;
using System.Windows.Media.Imaging;
using Crystals.Core.Models;

namespace Crystals.Core.Utilities;

public static class DrawingHelper
{
    public static BitmapSource RecolorIcon(BitmapSource icon, CrystalsColor newColor)
    {
        int width = icon.PixelWidth;
        int height = icon.PixelHeight;
        int stride = width * 4; // 4 bytes per pixel for BGRA
        byte[] pixels = new byte[height * stride];

        // Copy source pixels into our buffer
        icon.CopyPixels(pixels, stride, 0);

        // Prepare target color components (WPF uses BGRA)
        var newRgb = ColorConverter.HSVtoRGB(newColor);
        byte newB = newRgb.B;
        byte newG = newRgb.G;
        byte newR = newRgb.R;

        for (int i = 0; i < pixels.Length; i += 4)
        {
            // pixels[i]   = Blue
            // pixels[i+1] = Green
            // pixels[i+2] = Red
            // pixels[i+3] = Alpha

            // Threshold: If the pixel isn't dark, change it
            // A simple average or max check works well for black
            if (pixels[i] > 50 || pixels[i + 1] > 50 || pixels[i + 2] > 50)
            {
                pixels[i] = newB;
                pixels[i + 1] = newG;
                pixels[i + 2] = newR;
                // Usually, you keep original Alpha (pixels[i+3]) to preserve anti-aliasing
            }
        }

        // Create a new BitmapSource from the modified buffer
        return BitmapSource.Create(
            width, height,
            icon.DpiX, icon.DpiY,
            PixelFormats.Bgra32,
            null,
            pixels,
            stride);
    }
}