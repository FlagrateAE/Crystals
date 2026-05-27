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
        int stride = width * 4;
        byte[] pixels = new byte[height * stride];

        icon.CopyPixels(pixels, stride, 0);

        var newRgb = ColorConverter.HSVtoRGB(newColor);
        byte newB = newRgb.B;
        byte newG = newRgb.G;
        byte newR = newRgb.R;

        for (int i = 0; i < pixels.Length; i += 4)
        {
            if (pixels[i] > 50 || pixels[i + 1] > 50 || pixels[i + 2] > 50)
            {
                pixels[i] = newB;
                pixels[i + 1] = newG;
                pixels[i + 2] = newR;
            }
        }

        return BitmapSource.Create(
            width, height,
            icon.DpiX, icon.DpiY,
            PixelFormats.Bgra32,
            null,
            pixels,
            stride);
    }
}