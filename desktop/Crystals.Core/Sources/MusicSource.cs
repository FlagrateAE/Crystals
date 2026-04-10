using Windows.Media.Control;
using Crystals.Core.Models;
using Crystals.Core.Services;
using Crystals.Core.Utilities;

namespace Crystals.Core.Sources;

public class MusicSource(WebMediaService service) : ISource
{
    public void Start()
    {
        
    }

    public event Action? RequestFocus;
    public event Action<Color>? OnColorChanged;
}