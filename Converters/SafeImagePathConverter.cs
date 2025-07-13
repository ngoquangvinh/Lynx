using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LynxUI_Main.Converters
{
    public class SafeImagePathConverter : IValueConverter
    {
        private static readonly string FallbackPath = Path.Combine(AppContext.BaseDirectory, "Assets", "avatar_default.png");
        private static readonly string[] SupportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == DependencyProperty.UnsetValue || value == null)
                    return LoadFallback("[Unset or Null]");

                string path = value.ToString();
                if (string.IsNullOrWhiteSpace(path))
                    return LoadFallback("[Empty String]");

                Debug.WriteLine($"[Debug] Try load: {path}");

                // Nếu là URL đầy đủ
                if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                {
                    Uri uri = new Uri(path, UriKind.Absolute);
                    return LoadImage(uri);
                }

                // Nếu là đường dẫn từ server (bắt đầu bằng "/")
                if (path.StartsWith("/") && !path.StartsWith("/Assets/", StringComparison.OrdinalIgnoreCase))
                {
                    string fullUrl = $"http://203.162.54.169:2090{path}";
                    Debug.WriteLine($"[Resolved Server Path] → {fullUrl}");
                    return LoadImage(new Uri(fullUrl, UriKind.Absolute));
                }

                // Nếu là ảnh trong thư mục Assets nội bộ
                if (path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/Assets/", StringComparison.OrdinalIgnoreCase))
                {
                    string relativePath = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                    string fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);
                    Debug.WriteLine($"[ResolvedPath] Asset → {fullPath}");
                    return LoadImageFromFile(fullPath);
                }

                // Nếu là đường dẫn tương đối hoặc tuyệt đối
                string resolved = Path.GetFullPath(path);
                return LoadImageFromFile(resolved);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SafeImagePathConverter] Error: {ex.Message}");
                return LoadFallback("[Exception]");
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

        private BitmapImage LoadImageFromFile(string path)
        {
            try
            {
                if (!File.Exists(path) || !IsSupportedImage(path))
                    return LoadFallback($"[File Missing or Unsupported] {path}");

                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoadImageFromFile ERROR] {ex.Message}");
                return LoadFallback("[Failed LoadImageFromFile]");
            }
        }

        private BitmapImage LoadImage(Uri uri)
        {
            try
            {
                Debug.WriteLine($"[Resolved URI] {uri}");

                if (uri.IsFile)
                    return LoadImageFromFile(uri.LocalPath);

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = uri;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoadImage ERROR] {ex.Message} (uri = {uri})");
                return LoadFallback("[Failed LoadImage]");
            }
        }

        private BitmapImage LoadFallback(string reason)
        {
            try
            {
                Debug.WriteLine($"[Fallback Avatar] Reason: {reason}");

                if (!File.Exists(FallbackPath))
                {
                    Debug.WriteLine($"[Fallback ERROR] Fallback file not found: {FallbackPath}");
                    return new BitmapImage();
                }

                using var stream = new FileStream(FallbackPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fallback = new BitmapImage();
                fallback.BeginInit();
                fallback.CacheOption = BitmapCacheOption.OnLoad;
                fallback.StreamSource = stream;
                fallback.EndInit();
                fallback.Freeze();
                return fallback;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Fallback Load Failed] {ex.Message}");
                return new BitmapImage();
            }
        }

        private bool IsSupportedImage(string path)
        {
            string ext = Path.GetExtension(path)?.ToLowerInvariant();
            return SupportedExtensions.Contains(ext);
        }
    }
}
