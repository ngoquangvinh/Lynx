using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LynxUI_Main.Converters
{
    public class BoolToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isOwn && isOwn)
                ? HorizontalAlignment.Right
                : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
