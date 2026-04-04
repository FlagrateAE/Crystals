using Crystals.Core.Models;

namespace Crystals.Core.Devices;

public interface IDevice
{
    public void Start();
    
    public void SetColor(Color color);
    public void SetColorSmooth(Color color);

    public Color GetColor();
}