using Crystals.Core.Models;

namespace Crystals.Core.Middlewares;

public interface IMiddleware
{
    public CrystalsColor Process(CrystalsColor color);
}