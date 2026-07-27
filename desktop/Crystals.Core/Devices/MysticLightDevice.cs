using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Services;

namespace Crystals.Core.Devices;

public class MysticLightDevice(MysticLightService service) : IDevice
{
    public string Name => "MSI Mystic Light";

    public BitmapSource Icon { get; } =
        BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Devices/Keyboard.png"));

    public bool Start()
    {
        return service.IsInitialized;
    }

    public void SetColor(CrystalsColor color)
    {
        var (r, g, b) = Adjust(color);
        service.SetStaticColor(Ms1565Zone.AllZones, r, g, b);
    }

    public void SetColorSmooth(CrystalsColor color) => SetColor(color);

    private (byte r, byte g, byte b) Adjust(CrystalsColor color)
    {
        if (color == CrystalsColor.White) return (160, 135, 255);

        const double MaxR = 160.0 / 255.0; // ~0.627
        const double MaxG = 135.0 / 255.0; // ~0.529
        const double MaxB = 255.0 / 255.0; // 1.000
        const double GreenYellowFactor = 100 / 255.0;
        const double GreenCyanFactor = 100 / 255.0;
        const double Gamma = 2.2;

        var rgb = color.ToRgb();
        double linR = Math.Pow(rgb.R / 255.0, Gamma);
        double linG = Math.Pow(rgb.G / 255.0, Gamma);
        double linB = Math.Pow(rgb.B / 255.0, Gamma);

        double greenScale = 1.0;

        if (linG > 0)
        {
            double redRatio = linR / (linR + linG + linB + 1e-6);
            double blueRatio = linB / (linR + linG + linB + 1e-6);

            double yellowMix = redRatio * GreenYellowFactor;
            double cyanMix = blueRatio * GreenCyanFactor;
            double pureGreenMix = (1.0 - redRatio - blueRatio);

            greenScale = yellowMix + cyanMix + Math.Max(0, pureGreenMix);
        }

        double targetR = linR * MaxR;
        double targetG = linG * greenScale * MaxG;
        double targetB = linB * MaxB;

        double outR = Math.Pow(targetR, 1.0 / Gamma) * 255.0;
        double outG = Math.Pow(targetG, 1.0 / Gamma) * 255.0;
        double outB = Math.Pow(targetB, 1.0 / Gamma) * 255.0;

        byte finalR = (byte)Math.Clamp((int)Math.Round(outR), 0, 255);
        byte finalG = (byte)Math.Clamp((int)Math.Round(outG), 0, 255);
        byte finalB = (byte)Math.Clamp((int)Math.Round(outB), 0, 255);

        return (finalR, finalG, finalB);
    }

    public void Stop()
    {
    }
}