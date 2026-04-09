using Crystals.Core.Models;

namespace Crystals.Core.Sources;

public interface ISource
{
    public Task Start();

    public event Action RequestFocus;
    
    public event Action<Color> OnColorChanged;
    
}