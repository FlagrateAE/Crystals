using System.Windows.Media.Imaging;
using Crystals.Core.Models;

namespace Crystals.Core.Devices;

public interface IDevice
{
    public string Name { get; }
    public BitmapSource Icon { get; }
    
    public void Start();

    public void SetColor(CrystalsColor color);
    public void SetColorSmooth(CrystalsColor color);

    public void Stop();
}