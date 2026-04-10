using Crystals.Core.Models;

namespace Crystals.Core.Devices;

public interface IDevice
{
    public void Start();
    
    public void SetColor(CrystalsColor crystalsColor);
    public void SetColorSmooth(CrystalsColor crystalsColor);

    public CrystalsColor GetColor();
}