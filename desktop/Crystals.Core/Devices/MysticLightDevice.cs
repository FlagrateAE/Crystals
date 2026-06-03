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
        return service.IsInitialized;
    }

    public void SetColor(CrystalsColor color)
    {
        var rgb = ColorConverter.HSVtoRGB(color);
        service.SetStaticColor(Ms1565Zone.AllZones, rgb.R, rgb.G, rgb.B);
    }

    public void SetColorSmooth(CrystalsColor color) => SetColor(color);

    public void Stop()
    {
    }
}