using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;
using System.Windows;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Conventers
{
    public class TaskToDueStringConverter : IValueConverter
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

                    string text = date.ToShortDateString();
                    if (days < 0)
                    {
                        if (date.Year == DateTime.Today.Year)
                        {
                            text = string.Format("{0}", date.ToString("M", CultureInfo.CurrentCulture).ToLower());
                        }
                        else
                        {
                            text = string.Format("{0}", date.ToShortDateString().ToLower());
                        }
                    }
                    else if (days == 0)
                    {
                        text = string.Format("{0}, {1}", AppResources.DateToday, date.ToShortTimeString());
                    }
                    else if (days == 1)
                    {
                        text = string.Format("{0}, {1}", AppResources.DateTomorrow, date.ToShortTimeString());
                    }
                    else if (date.Date <= DateTimeExtensions.LastDayOfWeek || date.Date <= DateTimeExtensions.Today.AddDays(5).Date)
                    {
                        text = string.Format("{0}, {1}", date.ToString("dddd", CultureInfo.CurrentCulture).ToLower(), date.ToShortTimeString());
                    }
                    else if (days > daysToEndOfWeek && days <= daysToEndOfNextWeek)
                    {
                        text = string.Format("{0}, {1}", AppResources.DateNextWeek, date.ToString("dddd", CultureInfo.CurrentCulture).ToLower());
                    }
                    else if (days > daysToEndOfNextWeek && days <= daysToEndOfMonth)
                    {
                        text = string.Format("{0}, {1}", AppResources.DateThisMonth, date.ToString("M", CultureInfo.CurrentCulture).ToLower());
                    }
                    else if (days > daysToEndOfMonth && days <= daysToEndOfNextMonth)
                    {
                        text = string.Format("{0}, {1}", AppResources.DateNextMonth, date.ToString("M", CultureInfo.CurrentCulture).ToLower());
                    }
                    else if (days > daysToEndOfNextMonth && date.Year == DateTime.Today.Year)
                    {
                        text = string.Format("{0}", date.ToString("M", CultureInfo.CurrentCulture).ToLower());
                    }
                    else 
                    {
                        text = string.Format("{0}", date.ToShortDateString().ToLower());
                    }


                    return text;
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
