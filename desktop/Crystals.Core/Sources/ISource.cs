using System.Windows.Media.Imaging;
using Crystals.Core.Models;
using Crystals.Core.Models.SourceModels;

namespace Crystals.Core.Sources;

public interface ISource
{
    public void Start();

    public int FocusPriority { get; }
    public BitmapSource SourceIcon { get; }
    public ISourceModel? CurrentSource { get; }
    public CrystalsColor CurrentColor { get; }
    public event EventHandler<CrystalsColor> OnColorChanged;
}