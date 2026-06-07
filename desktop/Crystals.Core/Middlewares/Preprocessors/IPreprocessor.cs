using Crystals.Core.Models;

namespace Crystals.Core.Middlewares.Preprocessors;

public interface IPreprocessor
{
    public CrystalsColor Process(CrystalsColor color);
}