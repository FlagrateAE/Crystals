using Windows.Storage.Streams;

namespace Crystals.Core.Models;

public record Media(
    string AppId,
    string Title,
    string Artist,
    IRandomAccessStreamReference? Thumbnail
);