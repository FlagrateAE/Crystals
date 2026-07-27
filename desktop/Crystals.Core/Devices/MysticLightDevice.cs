using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Services;
using Crystals.Core.Utilities;

namespace Crystals.Core.Devices;

public class MysticLightDevice(MysticLightService service) : IDevice
{
    public string Name => "MSI Mystic Light";

    public BitmapSource Icon { get; } =
        BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Devices/Keyboard.png"));

    public bool Start()
    {
        SetColor(CrystalsColor.White);
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


        const double TargetGamma = 2.2;
        const double RedScale = 1;
        const double GreenScale = 0.62;
        const double BlueScale = 0.88;

        var rgb = color.ToRgb();

        double rNorm = rgb.R / 255.0;
        double gNorm = rgb.G / 255.0;
        double bNorm = rgb.B / 255.0;

        double rLinear = Math.Pow(rNorm, TargetGamma);
        double gLinear = Math.Pow(gNorm, TargetGamma);
        double bLinear = Math.Pow(bNorm, TargetGamma);

        rLinear *= RedScale;
        gLinear *= GreenScale;
        bLinear *= BlueScale;

        double rCorrected = Math.Pow(Math.Clamp(rLinear, 0.0, 1.0), 1.0 / TargetGamma);
        double gCorrected = Math.Pow(Math.Clamp(gLinear, 0.0, 1.0), 1.0 / TargetGamma);
        double bCorrected = Math.Pow(Math.Clamp(bLinear, 0.0, 1.0), 1.0 / TargetGamma);

        return (
            (byte)Math.Round(rCorrected * 255.0),
            (byte)Math.Round(gCorrected * 255.0),
            (byte)Math.Round(bCorrected * 255.0)
        );
    }

    public void Stop()
    {
    }
}