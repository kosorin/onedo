using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;
using System.Windows;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Conventers
{
    public class TaskToDueDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultString = (parameter is string) ? parameter as string : "";

            if (value is TaskModel)
            {
                TaskModel task = value as TaskModel;
                if (task.DueDate.HasValue)
                {
                    DateTime date = task.DueDate.Value;

                    int days = (int)(date - DateTimeExtensions.Today).TotalDays;
                    int daysToEndOfWeek = (int)(DateTimeExtensions.LastDayOfWeek - DateTimeExtensions.Today).TotalDays;
                    int daysToEndOfNextWeek = (int)(DateTimeExtensions.LastDayOfNextWeek - DateTimeExtensions.Today).TotalDays;
                    int daysToEndOfMonth = (int)(DateTimeExtensions.LastDayOfMonth - DateTimeExtensions.Today).TotalDays;
                    int daysToEndOfNextMonth = (int)(DateTimeExtensions.LastDayOfNextMonth - DateTimeExtensions.Today).TotalDays;

                    if (days < 0)
                    {
                        return date.ToShortDateString();
                    }
                    else if (days == 0)
                    {
                        return AppResources.DateToday;
                    }
                    else if (days == 1)
                    {
                        return AppResources.DateTomorrow;
                    }
                    else if (date.Date <= DateTimeExtensions.LastDayOfWeek || date.Date <= DateTimeExtensions.Today.AddDays(5).Date)
                    {
                        return date.ToString("dddd", CultureInfo.CurrentCulture).ToLower();
                    }
                    else if (days > daysToEndOfWeek && days <= daysToEndOfNextWeek)
                    {
                        return AppResources.DateNextWeek;
                    }
                    else if (days > daysToEndOfNextWeek && days <= daysToEndOfMonth)
                    {
                        return AppResources.DateThisMonth;
                    }
                    else if (days > daysToEndOfMonth && days <= daysToEndOfNextMonth)
                    {
                        return AppResources.DateNextMonth;
                    }
                    
                    return date.ToShortDateString();
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
