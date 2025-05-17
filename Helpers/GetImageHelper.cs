using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LynxUI_Main.Helpers
{
    public static class ImageHelper
    {
        public static ImageSource GetAvatarImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return GetFallback();
            }

            if (File.Exists(imagePath))
            {
                try
                {
                    var image = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                    image.Freeze();
                    return image;
                }
                catch
                {
                    return GetFallback();
                }
            }
            return GetFallback();
        }

        private static ImageSource GetFallback()
        {
            try
            {
                var fallbackPath = Path.GetFullPath("Assets/avatar_default.png");
                var fallback = new BitmapImage(new Uri(fallbackPath, UriKind.Absolute));
                fallback.Freeze();
                return fallback;
            }
            catch
            {
                return null;
            }
        }
    }
}
