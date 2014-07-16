using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;
using System.Windows;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Conventers
{
    public class DueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultString = parameter as string ?? "";


            DateTime date;

            if (value is DateTime)
            {
                date = (DateTime)value;
            }
            else if (value is DateTime?)
            {
                DateTime? nullableDate = value as DateTime?;
                if (nullableDate != null)
                {
                    date = nullableDate.Value;
                }
                else
                {
                    return defaultString;
                }
            }
            else
            {
                return defaultString;
            }

            int days = (int)(date.Date - DateTimeExtensions.Today).TotalDays;

            string dayOfWeek = date.ToString("dddd", CultureInfo.CurrentCulture);
            string monthDay = date.ToString("M", CultureInfo.CurrentCulture);
            string shortTime = date.ToShortTimeString();

            if (days >= 0)
            {
                int daysToEndOfActualWeek = (int)(DateTimeExtensions.LastDayOfActualWeek - DateTimeExtensions.Today).TotalDays;
                int daysToEndOfNextWeek = (int)(DateTimeExtensions.LastDayOfNextWeek - DateTimeExtensions.Today).TotalDays;
                int daysToEndOfActualMonth = (int)(DateTimeExtensions.LastDayOfActualMonth - DateTimeExtensions.Today).TotalDays;
                int daysToEndOfNextMonth = (int)(DateTimeExtensions.LastDayOfNextMonth - DateTimeExtensions.Today).TotalDays;

                if (days == 0)
                {
                    return string.Format("{0}, {1}", AppResources.DateToday, shortTime);
                }
                else if (days == 1)
                {
                    return string.Format("{0}, {1}", AppResources.DateTomorrow, shortTime);
                }
                else if (date.Date <= DateTimeExtensions.LastDayOfActualWeek || date.Date <= DateTimeExtensions.Today.AddDays(5).Date)
                {
                    return string.Format("{0}, {1}", dayOfWeek, shortTime);
                }
                else if (days > daysToEndOfActualWeek && days <= daysToEndOfNextWeek)
                {
                    return string.Format("{0}, {1}", AppResources.DateNextWeek, dayOfWeek);
                }
                else if (days > daysToEndOfNextWeek && days <= daysToEndOfActualMonth)
                {
                    return string.Format("{0}, {1}", AppResources.DateThisMonth, monthDay);
                }
                else if (days > daysToEndOfActualMonth && days <= daysToEndOfNextMonth)
                {
                    return string.Format("{0}, {1}", AppResources.DateNextMonth, monthDay);
                }
                else if (days > daysToEndOfNextMonth && date.Year == DateTime.Today.Year)
                {
                    return monthDay;
                }
            }
            else
            {
                if (days == -1)
                {
                    return string.Format("{0}, {1}", AppResources.DateYesterday, shortTime);
                }
                else if (date.Date >= DateTimeExtensions.FirstDayOfActualWeek)
                {
                    return string.Format("{0}, {1}", dayOfWeek, shortTime);
                }
                else if (date.Date >= DateTimeExtensions.FirstDayOfPreviousWeek)
                {
                    return string.Format("{0}, {1}", AppResources.DateLastWeek, dayOfWeek);
                }
                else if (date.Date >= DateTimeExtensions.FirstDayOfActualMonth)
                {
                    return string.Format("{0}, {1}", AppResources.DateThisMonth, monthDay);
                }
                else if (date.Date >= DateTimeExtensions.FirstDayOfPreviousMonth)
                {
                    return string.Format("{0}, {1}", AppResources.DateLastWeek, monthDay);
                }
                else if (date.Year == DateTime.Today.Year)
                {
                    return monthDay;
                }
            }

            return date.ToShortDateString();

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
