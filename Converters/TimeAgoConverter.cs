using System.Globalization;
using System.Windows.Data;

namespace LynxUI_Main.Converters
{
    public class TimeAgoConverter : IValueConverter
    {
        private readonly DateTime _referenceTime;

        public TimeAgoConverter() : this(DateTime.UtcNow) { }

        public TimeAgoConverter(DateTime referenceTime)
        {
            _referenceTime = referenceTime;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                var timeSpan = _referenceTime - dateTime.ToUniversalTime();

                if (timeSpan.TotalSeconds < 60)
                    return "Vừa xong";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} phút trước";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} giờ trước";
                if (timeSpan.TotalDays < 2)
                    return "Hôm qua";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} ngày trước";
                if (timeSpan.TotalDays < 30)
                    return $"{(int)(timeSpan.TotalDays / 7)} tuần trước";
                if (timeSpan.TotalDays < 365)
                    return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
                else
                    return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
