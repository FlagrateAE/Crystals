using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Crystals.Core.Utilities;

public static class ImageUtilities
{
    private const float ThumbnailAspectRatioDeviation = 0.05f;
    
    public static async Task<bool> IsThumbnailMusic(IRandomAccessStreamReference thumbnail)
    {
        return Math.Abs(await GetThumbnailAspectRatio(thumbnail) - 1) < ThumbnailAspectRatioDeviation;
    }

    private static async Task<float> GetThumbnailAspectRatio(IRandomAccessStreamReference thumbnailRef)
    {
        try
        {
            using var stream = await thumbnailRef.OpenReadAsync();

            var decoder = await BitmapDecoder.CreateAsync(stream);

            var width = decoder.PixelWidth;
            var height = decoder.PixelHeight;

            if (height == 0) return 0;

            var ratio = (float)width / height;
        
            return ratio;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving thumbnail: {e.Message}");
            return 0;
        }
    }
}