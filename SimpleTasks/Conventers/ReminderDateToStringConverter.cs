using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;

namespace SimpleTasks.Conventers
{
    public class ReminderDateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultString = (parameter is string) ? parameter as string : "";

            if (value is DateTime?)
            {
                DateTime? reminderDate = value as DateTime?;
                if (reminderDate.HasValue)
                {
                    DateTime date = reminderDate.Value;

                    string defaultDateTime = string.Format(CultureInfo.CurrentCulture, DateTimeExtensions.Format, reminderDate.Value);
                    if (date.Date < DateTimeExtensions.Today)
                    {
                        return defaultDateTime;
                    }
                    else if (date.Date == DateTimeExtensions.Today)
                    {
                        return string.Format("{0}, {1}", AppResources.DateToday, date.ToShortTimeString());
                    }
                    else if (date.Date == DateTimeExtensions.Tomorrow)
                    {
                        return string.Format("{0}, {1}", AppResources.DateTomorrow, date.ToShortTimeString());
                    }
                    else if (date.Date <= DateTimeExtensions.LastDayOfWeek || date.Date <= date.Date.AddDays(4))
                    {
                        return date.ToString("dddd, ", CultureInfo.CurrentCulture) + date.ToShortTimeString();
                    }
                    else
                    {
                        return defaultDateTime;
                    }
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
