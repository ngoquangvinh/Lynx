using System.Globalization;
using System.Windows.Data;

namespace LynxUI_Main.Converters
{
    public class GroupAvatarPositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] is not int index || values[1] is not int count)
                return 0.0;

            string coord = parameter?.ToString();

            // Layout logic: tam giác nếu 3, hình vuông nếu >= 4
            if (count == 3)
            {
                return coord switch
                {
                    "Left" => index switch
                    {
                        0 => 0.0,
                        1 => 24.0,
                        2 => 12.0,
                        _ => 0.0
                    },
                    "Top" => index switch
                    {
                        0 => 24.0,
                        1 => 24.0,
                        2 => 0.0,
                        _ => 0.0
                    },
                    _ => 0.0
                };
            }
            else // count >= 4
            {
                return coord switch
                {
                    "Left" => index switch
                    {
                        0 => 0.0,
                        1 => 24.0,
                        2 => 0.0,
                        3 => 24.0,
                        _ => 0.0
                    },
                    "Top" => index switch
                    {
                        0 => 0.0,
                        1 => 0.0,
                        2 => 24.0,
                        3 => 24.0,
                        _ => 0.0
                    },
                    _ => 0.0
                };
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}