using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LynxUI_Main.Helpers
{
    public static class ImageHelper
    {
        public static ImageSource GetAvatarImage(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return GetFallback();

            string resolvedPath = null;

            try
            {
                if (imagePath.StartsWith("/") || imagePath.StartsWith("\\"))
                {
                    imagePath = imagePath.TrimStart('/', '\\');
                }

                resolvedPath = Path.IsPathRooted(imagePath)
                    ? imagePath
                    : Path.Combine(AppContext.BaseDirectory, imagePath.Replace('/', Path.DirectorySeparatorChar));

                Debug.WriteLine($"[Debug] Try load: {resolvedPath}");
                Debug.WriteLine($"[Debug] BaseDirectory: {AppContext.BaseDirectory}");
                if (File.Exists(resolvedPath))
                {
                    var image = new BitmapImage(new Uri(resolvedPath, UriKind.Absolute));
                    image.Freeze();
                    return image;
                }
                else
                    Debug.WriteLine($"[Debug] Image not found: {resolvedPath}");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ImageHelper] Exception: {ex.Message}");
            }
            Debug.WriteLine($"[Fallback Avatar] Reason: [File Missing or Unsupported] {resolvedPath} ← raw: {imagePath}");

            return GetFallback();
        }

        private static ImageSource GetFallback()
        {
            try
            {
                string fallbackPath = Path.Combine(AppContext.BaseDirectory, "Assets", "avatar_default.png");
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
