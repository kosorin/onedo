using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;

namespace SimpleTasks.Conventers
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultString = parameter as string ?? "";

            if (value is DateTime?)
            {
                DateTime? nullableDate = value as DateTime?;
                if (nullableDate.HasValue)
                {
                    DateTime date = nullableDate.Value;
                    if (date.Date == DateTimeExtensions.Today)
                    {
                        return AppResources.DateToday;
                    }
                    else if (date.Date == DateTimeExtensions.Tomorrow)
                    {
                        return AppResources.DateTomorrow;
                    }
                    else if (date.Date == DateTimeExtensions.Yesterday)
                    {
                        return AppResources.DateYesterday;
                    }
                    return string.Format(CultureInfo.CurrentCulture, DateTimeExtensions.DateFormat, date);
                }
            }

            return defaultString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
