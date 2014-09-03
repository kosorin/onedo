using System;
using System.Globalization;

namespace SimpleTasks.Core.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime SetTime(this DateTime dt, DateTime time)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, time.Hour, time.Minute, time.Second);
        }

        public static DateTime SetDate(this DateTime dt, DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, dt.Hour, dt.Minute, dt.Second);
        }

        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1);
            return (long)(dateTime - unixEpoch).TotalSeconds;
        }

        public static DateTime FromUnixTimestamp(long timestamp)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1);
            return unixEpoch.AddSeconds(timestamp);
        }

        private const string _fileNameFormat = "yyyy'-'MM'-'dd HH'-'mm'-'ss";

        public static string ToFileName(this DateTime dateTime)
        {
            return dateTime.ToString(_fileNameFormat, CultureInfo.InvariantCulture);
        }

        public static DateTime FromFileName(string fileName)
        {
            return DateTime.ParseExact(fileName ?? "", _fileNameFormat, CultureInfo.InvariantCulture);
        }

        public static DayOfWeek FirstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

        public static string DateFormat { get { return "{0:ddd}, {0:d}"; } }

        public static string TimeFormat { get { return "{0:t}"; } }

        public static DateTime Now { get { return DateTime.Now; } }

        public static DateTime Today { get { return DateTime.Today; } }

        public static DateTime Yesterday { get { return Today.AddDays(-1); } }

        public static DateTime Tomorrow { get { return Today.AddDays(1); } }

        public static DateTime LastDayOfActualWeek { get { return DateTime.Today.AddDays((int)(DayOfWeek.Saturday - DateTime.Today.DayOfWeek + FirstDayOfWeek) % 7); } }

        public static DateTime LastDayOfNextWeek { get { return LastDayOfActualWeek.AddDays(7); } }

        public static DateTime LastDayOfActualMonth { get { return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)); } }

        public static DateTime LastDayOfNextMonth { get { DateTime nextMonth = LastDayOfActualMonth.AddDays(1); return new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month)); } }

        public static DateTime FirstDayOfActualWeek { get { return DateTime.Today.AddDays((int)(-((int)DateTime.Today.DayOfWeek) + FirstDayOfWeek) % 7); } }

        public static DateTime FirstDayOfPreviousWeek { get { return FirstDayOfActualWeek.AddDays(-7); } }

        public static DateTime FirstDayOfActualMonth { get { return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); } }

        public static DateTime FirstDayOfPreviousMonth { get { return FirstDayOfActualMonth.AddMonths(-1); } }
    }
}