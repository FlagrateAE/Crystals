using Crystals.Core.Models;

namespace Crystals.Core.Middlewares;

// Makes a color saturated and vibrant to be displayed by LED
public class VibranceMiddleware : IMiddleware
{
    public CrystalsColor Process(CrystalsColor color)
    {
        return color == CrystalsColor.White ? color : color.WithS(1f).WithV(1f);
    }
}