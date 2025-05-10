using LynxUI_Main.Helpers;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LynxUI_Main.Converters
{
    public class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    var uri = Uri.IsWellFormedUriString(path, UriKind.Absolute)
                        ? new Uri(path, UriKind.Absolute)
                        : new Uri(Path.GetFullPath(path), UriKind.Absolute);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    bitmap.UriSource = uri;
                    bitmap.EndInit();
                    return bitmap.SafeFreeze();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ImageSourceConverter Error] {ex.Message}");
                }
            }

            return BitmapImageFallback();
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

        private static BitmapImage BitmapImageFallback()
        {
            try
            {
                var fallbackPath = Path.GetFullPath("Assets/avatar_default.png");
                return new BitmapImage(new Uri(fallbackPath, UriKind.Absolute));
            }
            catch
            {
                return new BitmapImage();
            }
        }
    }
}