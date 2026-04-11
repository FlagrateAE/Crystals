using Crystals.Core.Models;

namespace Crystals.Core.Devices;

public interface IDevice
{
    public void Start();
    
    public void SetColor(CrystalsColor color);
    public void SetColorSmooth(CrystalsColor color);
}