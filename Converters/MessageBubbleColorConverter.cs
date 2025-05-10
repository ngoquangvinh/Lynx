using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LynxUI_Main.Converters
{
    public class MessageBubbleColorConverter : IValueConverter
    {
        public Brush OwnBrush { get; set; } = new SolidColorBrush(Color.FromRgb(220, 248, 198)); // Light green
        public Brush OtherBrush { get; set; } = new SolidColorBrush(Color.FromRgb(240, 240, 240)); // Light gray

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOwn && isOwn)
                return OwnBrush;
            return OtherBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
