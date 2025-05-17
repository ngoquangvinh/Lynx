using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LynxUI_Main.Converters
{
    public class MessageStatusToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status && status.Equals("Sent", StringComparison.OrdinalIgnoreCase))
                return HorizontalAlignment.Right;

            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class MessageStatusToBackgroundConverter : IValueConverter
    {
        public Brush SentBrush { get; set; }
        public Brush ReceivedBrush { get; set; } = new SolidColorBrush(Color.FromRgb(240, 240, 240));

        public MessageStatusToBackgroundConverter()
        {
            // Default gradient for Sent messages
            LinearGradientBrush purpleGradient = new LinearGradientBrush();
            purpleGradient.StartPoint = new Point(0, 0);
            purpleGradient.EndPoint = new Point(1, 0);
            purpleGradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#667eea"), 0.0));
            purpleGradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#764ba2"), 1.0));

            SentBrush = purpleGradient;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status && status.Equals("Sent", StringComparison.OrdinalIgnoreCase))
                return SentBrush;

            return ReceivedBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
