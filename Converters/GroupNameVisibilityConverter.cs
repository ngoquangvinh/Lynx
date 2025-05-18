using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LynxUI_Main.Converters
{
    public class GroupNameVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: IsGroup (bool), values[1]: IsOwnMessage (bool)
            if (values.Length == 2 &&
                values[0] is bool isGroup &&
                values[1] is bool isOwnMessage)
            {
                // Chỉ hiện tên nếu là nhóm và không phải tin nhắn của chính mình
                return (isGroup && !isOwnMessage) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}