using Crystals.Core.Models;

namespace Crystals.Core.Middlewares;

// Makes the white color slightly blueish for proper eye perception
public class BlueishPostprocessor : IPostprocessor
{
    public CrystalsColor Process(CrystalsColor color)
    {
        return color != CrystalsColor.White ? color : new CrystalsColor(240f, 0.2f, 1f);
    }
}