using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;
using System.Windows;

namespace SimpleTasks.Conventers
{
    public class DueDateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultString = (parameter is string) ? parameter as string : "";

            if (value is DateTime?)
            {
                DateTime? dueDate = value as DateTime?;
                if (dueDate.HasValue)
                {
                    DateTime date = dueDate.Value;

                    string defaultDateTime = date.ToShortDateString();
                    if (date.Date < DateTimeExtensions.Today)
                    {
                        return defaultDateTime;
                    }
                    else if (date.Date == DateTimeExtensions.Today)
                    {
                        return AppResources.DateToday;
                    }
                    else if (date.Date == DateTimeExtensions.Tomorrow)
                    {
                        return AppResources.DateTomorrow;
                    }
                    else if (date.Date <= DateTimeExtensions.LastDayOfWeek || date.Date <= DateTimeExtensions.Today.AddDays(5).Date)
                    {
                        return date.ToString("dddd", CultureInfo.CurrentCulture);
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
