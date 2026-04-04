using Crystals.Core.Models;

namespace Crystals.Core.Sources;

public interface ISource
{
    public void Start();
    
    public event Action<Color> OnColorChanged;
    
    public Color GetColor();
}