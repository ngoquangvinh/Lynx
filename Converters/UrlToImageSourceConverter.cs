using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LynxUI_Main.Converters
{
    public class UrlToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string url = value as string;
            Debug.WriteLine($"[UrlToImageSourceConverter] Converting: {url}");
            if (string.IsNullOrEmpty(url))
                return null;

            try
            {
                if (string.IsNullOrEmpty(url))
                    return null;

                // Nếu là URL tuyệt đối
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    return new BitmapImage(new Uri(url));
                }

                // Nếu là đường dẫn cục bộ
                string localPath = Path.GetFullPath(url);
                if (File.Exists(localPath))
                    return new BitmapImage(new Uri(localPath));

                Debug.WriteLine($"[UrlToImageSourceConverter] File not found: {localPath}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UrlToImageSourceConverter] ERROR: {ex.Message}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

}
