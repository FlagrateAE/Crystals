using Crystals.Core.Models;

namespace Crystals.Core.Middlewares;

// Makes the white color slightly blueish for proper eye perception
public class WhiteBlueMiddleware : IMiddleware
{
    public CrystalsColor Process(CrystalsColor color)
    {
        if (!color.Equals(CrystalsColor.White)) return color;

        color.H = 240;
        color.S = 0.2f;
        color.V = 1f;
        return color;
    }
}