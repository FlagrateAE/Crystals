using System.Drawing;
using Crystals.Core.Models.SourceModels;

namespace Crystals.Core.Models;

public class Media(string title, string artist, Bitmap thumbnail) : ISourceModel
{
    public string Name { get; } = title;
    public string Description { get; } = artist;
    public Bitmap Image { get; } = thumbnail;
}