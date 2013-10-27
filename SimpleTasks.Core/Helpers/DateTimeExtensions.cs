using SimpleTasks.Core.Resources;
using System;
using System.Globalization;

namespace SimpleTasks.Core.Helpers
{
    public class DateTimeExtensions
    {
        public static DayOfWeek FirstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

        public static string Format { get { return "{0:dddd}, {0:d}"; } }

        public static DateTime Today { get { return DateTime.Today; } }

        public static DateTime Tomorrow { get { return Today.AddDays(1d); } }

        public static DateTime LastDayOfWeek { get { return DateTime.Today.AddDays((int)(DayOfWeek.Saturday - DateTime.Today.DayOfWeek + FirstDayOfWeek) % 7); } }

        public static DateTime LastDayOfNextWeek { get { return LastDayOfWeek.AddDays(7); } }

        public static DateTime LastDayOfMonth { get { return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)); } }

        public static DateTime LastDayOfNextMonth { get { DateTime nextMonth = LastDayOfMonth.AddDays(1); return new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month)); } }

        public static string ToRelativeString(DateTime? date, bool includeMonth = false)
        {
            if (!date.HasValue)
            {
                return AppResources.DateLaterText;
            }

            int days = (int)(date.Value - Today).TotalDays;
            int daysToEndOfWeek = (int)(LastDayOfWeek - Today).TotalDays;
            int daysToEndOfNextWeek = (int)(LastDayOfNextWeek - Today).TotalDays;
            int daysToEndOfMonth = (int)(LastDayOfMonth - Today).TotalDays;
            int daysToEndOfNextMonth = (int)(LastDayOfNextMonth - Today).TotalDays;
            int daysAfterTomorrow = 3;


            if (days < 0)
            {
                return AppResources.DateOverdueText;
            }
            else if (days == 0)
            {
                return AppResources.DateTodayText;
            }
            else if (days == 1)
            {
                return AppResources.DateTomorrowText;
            }
            else if (days > 1 && daysToEndOfWeek - (daysAfterTomorrow + 1) > 0)
            {
                return AppResources.DateThisWeekText;
            }
            else if (days > daysToEndOfWeek && days <= daysToEndOfNextWeek)
            {
                return AppResources.DateNextWeekText;
            }
            else if (!includeMonth && days > daysToEndOfNextWeek)
            {
                return AppResources.DateLaterText;
            }
            else if (includeMonth)
            {
                if (days > daysToEndOfNextWeek && days <= daysToEndOfMonth)
                {
                    return AppResources.DateThisMonthText;
                }
                else if (days > daysToEndOfMonth && days <= daysToEndOfNextMonth)
                {
                    return AppResources.DateNextMonthText;
                }
                else if (days > daysToEndOfNextMonth)
                {
                    return AppResources.DateLaterText;
                }
            }

            return date.Value.ToString("dddd", CultureInfo.CurrentCulture).ToLower();
        }
    
    }
}