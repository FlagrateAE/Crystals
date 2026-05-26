using Crystals.Core.Models;

namespace Crystals.Core.Middlewares;

public class VibranceMiddleware : IMiddleware
{
    public CrystalsColor Process(CrystalsColor color)
    {
        if (color.Equals(CrystalsColor.White))
        {
            return color;
        }

        color.S = 1f;
        color.V = 1f;
        return color;
    }
}