using System.Drawing;

namespace Crystals.Core.Models.SourceModels;

public interface ISourceModel
{
    public string Name { get; }
    public string Description { get; }
    public Bitmap Image { get; }
}