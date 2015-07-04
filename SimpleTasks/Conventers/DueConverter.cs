using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;

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

            string dayOfWeek = date.ToString("ddd", CultureInfo.CurrentCulture);
            string monthDay = date.ToString("d", CultureInfo.CurrentCulture);
            string shortTime = date.ToShortTimeString();

            if (date.Date >= DateTimeExtensions.Today)
            {
                if (date.Date == DateTimeExtensions.Today || date.Date == DateTimeExtensions.Tomorrow)
                {
                    return shortTime;
                }
                else if (date.Date <= DateTimeExtensions.LastDayOfActualWeek || date.Date <= DateTimeExtensions.Today.AddDays(5).Date)
                {
                    return dayOfWeek;
                }
                else
                {
                    return monthDay;
                }
            }
            else
            {
                if (date.Date == DateTimeExtensions.Yesterday)
                {
                    return AppResources.DateYesterday;
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
