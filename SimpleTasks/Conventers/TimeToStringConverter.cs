using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;

namespace SimpleTasks.Conventers
{
    public class TimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultString = (parameter is string) ? parameter as string : "";

            if (value is DateTime?)
            {
                DateTime? date = value as DateTime?;
                if (date.HasValue)
                    return string.Format(CultureInfo.CurrentCulture, DateTimeExtensions.TimeFormat, date.Value);
            }

            return defaultString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
