using System.Windows.Media.Imaging;
using Crystals.Core.Models;

namespace Crystals.Core.Sources;

public interface ISource
{
    public void Start();

    public int FocusPriority { get; }

    public event EventHandler<CrystalsColor> OnColorChanged;

    public BitmapSource SourceIcon { get; }
}