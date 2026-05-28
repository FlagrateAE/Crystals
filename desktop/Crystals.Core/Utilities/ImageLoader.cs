using System.Windows.Media.Imaging;

namespace Crystals.Core.Utilities;

public static class ImageLoader
{
    public static class IconUris
    {
        public static readonly Uri AE = new("pack://application:,,,/Crystals.App;component/Resources/Icons/AE.png");
        public static readonly Uri Crystals = new("pack://application:,,,/Crystals.App;component/Resources/Icons/Crystals.png");
        public static readonly Uri Crystals80 = new("pack://application:,,,/Crystals.App;component/Resources/Icons/Crystals80.png");
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