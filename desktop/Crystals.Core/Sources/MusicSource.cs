using Crystals.Core.Models;

namespace Crystals.Core.Sources;

public class MusicSource : ISource
{
    public event Action<Color>? OnColorChanged;
    public Color GetColor()
    {
        throw new NotImplementedException();
    }
}