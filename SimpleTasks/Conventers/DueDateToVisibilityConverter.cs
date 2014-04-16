using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;
using System.Windows;

namespace SimpleTasks.Conventers
{
    public class DueDateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime?)
            {
                DateTime? dueDate = value as DateTime?;
                if (dueDate.HasValue)
                {
                    DateTime date = dueDate.Value;
                    if (date.Date < DateTimeExtensions.Today)
                    {
                        return Visibility.Collapsed;
                    }
                    else if (date.Date == DateTimeExtensions.Today || date.Date == DateTimeExtensions.Tomorrow)
                    {
                        return Visibility.Visible;
                    }
                    else if (date.Date <= DateTimeExtensions.LastDayOfWeek)
                    {
                        return Visibility.Visible;
                    }
                    else if ((DateTimeExtensions.LastDayOfWeek - DateTimeExtensions.Today).TotalDays <= 1 && date.Date <= DateTimeExtensions.LastDayOfNextWeek)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
