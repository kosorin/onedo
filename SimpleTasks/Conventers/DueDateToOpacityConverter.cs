using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;

namespace SimpleTasks.Conventers
{
    public class DueDateToOpacityConverter : IValueConverter
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
                        return 0.25;
                    }
                    else if (date.Date == DateTimeExtensions.Today || date.Date == DateTimeExtensions.Tomorrow)
                    {
                        return 1.0;
                    }
                    else if (date.Date <= DateTimeExtensions.LastDayOfWeek || date.Date <= DateTimeExtensions.Tomorrow.Date.AddDays(3))
                    {
                        return 0.4;
                    }
                    else if ((DateTimeExtensions.LastDayOfWeek - DateTimeExtensions.Today).TotalDays <= 1 && date.Date <= DateTimeExtensions.LastDayOfNextWeek)
                    {
                        return 0.4;
                    }
                    else
                    {
                        return 0.25;
                    }
                }
            }

            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
