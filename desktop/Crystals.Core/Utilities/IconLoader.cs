using System.Windows.Media.Imaging;

namespace Crystals.Core.Utilities;

public static class IconLoader
{
    public readonly static Uri IconUri = new("pack://application:,,,/Crystals.App;component/Resources/Icon.png");

    public static BitmapSource LoadDefaultIcon(int size) => LoadFromUri(IconUri, size);

    public static BitmapSource LoadFromUri(Uri uri, int size)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = uri;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.DecodePixelWidth = size;
        bitmap.DecodePixelHeight = size;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }
}