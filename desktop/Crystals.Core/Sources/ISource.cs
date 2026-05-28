using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Models.SourceModels;

namespace Crystals.Core.Sources;

public interface ISource
{
    public int FocusPriority { get; }
    public BitmapSource Icon { get; }
    public ISourceModel? CurrentSource { get; }
    public CrystalsColor CurrentColor { get; }
    public event EventHandler<CrystalsColor> OnColorChanged;

    public void Start();
}