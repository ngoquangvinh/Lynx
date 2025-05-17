using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LynxUI_Main.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public Brush OnlineBrush { get; set; } = Brushes.LimeGreen;
        public Brush OfflineBrush { get; set; } = Brushes.Gray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOnline)
            {
                return isOnline ? OnlineBrush : OfflineBrush;
            }
            return OfflineBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
