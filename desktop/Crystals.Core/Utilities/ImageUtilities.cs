using System.Diagnostics;
using Windows.Graphics.Imaging; using Windows.Storage;
using Windows.Storage.Streams;

namespace Crystals.Core.Utilities;

public static class ImageUtilities
{
    private const float ThumbnailAspectRatioDeviation = 0.05f;

    public static async Task<bool> IsThumbnailMusic(IRandomAccessStreamReference thumbnail)
    {
        await ShowThumbnail(thumbnail);
        
        var ratio = await GetThumbnailAspectRatio(thumbnail);
        return Math.Abs(ratio - 1) < ThumbnailAspectRatioDeviation;
    }

    private static async Task ShowThumbnail(IRandomAccessStreamReference thumbnailRef)
    {
        try
        {

            var tempPath = Path.Combine(Path.GetTempPath(), "temp_image_view.png");

            using var stream = await thumbnailRef.OpenReadAsync();

            using (var fileStream = File.Create(tempPath))
            {
                using (var dotNetStream = stream.AsStreamForRead())
                {
                    await dotNetStream.CopyToAsync(fileStream);
                }
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = tempPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to show image: {ex.Message}");
        }
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
            Console.WriteLine($"Thumbnail aspect ratio: {ratio}");
            return ratio;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving thumbnail: {e.Message}");
            return 0;
        }
    }
}