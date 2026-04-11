using Crystals.Core.Models;

namespace Crystals.Core.Sources;

public interface ISource
{
    public void Start();

    public int FocusPriority { get; }
    
    public event Action<CrystalsColor> OnColorChanged;
    
}