using Crystals.Core.Models;

namespace Crystals.Core.Middlewares;

public interface IPostprocessor
{
    public CrystalsColor Process(CrystalsColor color);
}