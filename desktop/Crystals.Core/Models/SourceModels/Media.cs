using System.Drawing;

namespace Crystals.Core.Models.SourceModels;

public class Media(string title, string artist, string album, Bitmap thumbnail) : ISourceModel
{
    public string Name { get; } = title;
    public string Description { get; } = artist;
    public string Album { get; } = album;
    public Bitmap Image { get; } = thumbnail;

    public override string ToString() => $"{Description} - {Name} (from {Album})";
}