using System.Windows.Media.Imaging;

namespace Crystals.Core.Utilities;

public static class IconLoader
{
    public static BitmapSource LoadFromUri(Uri uri)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = uri;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.DecodePixelWidth = 64;
        bitmap.DecodePixelHeight = 64;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }
}