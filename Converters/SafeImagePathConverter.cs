using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LynxUI_Main.Converters
{
    public class SafeImagePathConverter : IValueConverter
    {
        private static readonly string FallbackPath = Path.GetFullPath("Assets/avatar_default.png");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string path || string.IsNullOrWhiteSpace(path))
                return LoadImage(FallbackPath);

            try
            {
                Uri uri;

                if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                {
                    uri = new Uri(path, UriKind.Absolute);
                }
                else
                {
                    string fullPath = Path.GetFullPath(path);
                    if (!File.Exists(fullPath) || !IsSupportedImage(fullPath))
                    {
                        Console.WriteLine("[Skip Image] Invalid file type or missing: " + fullPath);
                        return LoadImage(FallbackPath);
                    }

                    uri = new Uri(fullPath, UriKind.Absolute);
                }

                return LoadImage(uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SafeImagePathConverter] Failed to load image: {ex.Message}");
                return LoadImage(FallbackPath);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

        private BitmapImage LoadImage(string path)
        {
            try
            {
                return LoadImage(new Uri(path, UriKind.Absolute));
            }
            catch
            {
                return new BitmapImage(); // fallback empty image if FallbackPath fails
            }
        }

        private BitmapImage LoadImage(Uri uri)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                bitmap.UriSource = uri;

                try
                {
                    bitmap.EndInit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BitmapImage EndInit ERROR] {ex.Message} (uri = {uri})");
                    return LoadFallback();
                }

                if (bitmap.CanFreeze)
                    bitmap.Freeze();

                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadImage(Uri) Outer ERROR] {ex.Message} (uri = {uri})");
                return LoadFallback();
            }
        }

        private BitmapImage LoadFallback()
        {
            try
            {
                var fallback = new BitmapImage();
                fallback.BeginInit();
                fallback.CacheOption = BitmapCacheOption.OnLoad;
                fallback.UriSource = new Uri(FallbackPath, UriKind.Absolute);
                fallback.EndInit();
                if (fallback.CanFreeze) fallback.Freeze();
                return fallback;
            }
            catch
            {
                return new BitmapImage(); // fallback rỗng nếu fallback ảnh hỏng
            }
        }



        private static readonly string[] SupportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };

        private bool IsSupportedImage(string path)
        {
            string ext = Path.GetExtension(path)?.ToLowerInvariant();
            return SupportedExtensions.Contains(ext);
        }

    }
}
