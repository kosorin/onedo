using SimpleTasks.Resources;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;
namespace SimpleTasks.Conventers
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultFormat = string.Format("d'{0} 'h'{1} 'mm'{2}'", AppResources.DateTimeDaysAbbr, AppResources.DateTimeHoursAbbr, AppResources.DateTimeMinutesAbbr);
            TimeSpan ts = value as TimeSpan? ?? TimeSpan.Zero;
            if (ts == TimeSpan.Zero)
            {
                return AppResources.TimeZero;
            }

            int a = Math.Sign(ts.Days) + Math.Sign(ts.Hours) + Math.Sign(ts.Minutes);
            StringBuilder sb = new StringBuilder();
            if (ts.Days != 0)
            {
                sb.Append(ts.Days);
                sb.Append("d ");
            }
            if (ts.Hours != 0 || a == 2)
            {
                sb.Append(ts.Hours);
                sb.Append("h ");
            }
            if (ts.Minutes != 0)
            {
                sb.Append(ts.Minutes);
                sb.Append("m ");
            }
            return sb.ToString();
            return Regex.Replace(ts.ToString(parameter as string ?? defaultFormat), @"\s?[0-9]{1,2}[^0-9]?", "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
