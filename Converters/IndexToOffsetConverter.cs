using System.Globalization;
using System.Windows.Data;

namespace LynxUI_Main.Converters
{
    public class IndexToOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
                return index * 2; // khoảng cách giữa các avatar
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
