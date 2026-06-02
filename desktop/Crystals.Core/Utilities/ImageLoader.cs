using System.Windows.Media.Imaging;

namespace Crystals.Core.Utilities;

public static class ImageLoader
{
    public static class IconUris
    {
        public static readonly Uri Ok = new("pack://application:,,,/Resources/Ok.png");
        public static readonly Uri Error = new("pack://application:,,,/Resources/Error.png");

        public static readonly Uri AE = new("pack://application:,,,/Crystals.App;component/Resources/Icons/AE.png");

        public static readonly Uri Crystals =
            new("pack://application:,,,/Crystals.App;component/Resources/Icons/Crystals.png");
    }

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