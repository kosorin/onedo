using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    [Flags]
    public enum Repeats
    {
        None = 0,

        Monday = 1 << 0,
        Tuesday = 1 << 1,
        Wednesday = 1 << 2,
        Thursday = 1 << 3,
        Friday = 1 << 4,
        Saturday = 1 << 5,
        Sunday = 1 << 6,

        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        Weekends = Saturday | Sunday,

        Daily = Weekdays | Weekends,

        Weekly = 1 << 7,
        Monthly = 1 << 8,
    }

    public static class RepeatsExtensions
    {
        private const int _daysInWeek = 7;

        public static RecurrenceInterval Interval(this Repeats repeats)
        {
            switch (repeats)
            {
            case Repeats.Monthly: return RecurrenceInterval.Monthly;
            case Repeats.Weekly: return RecurrenceInterval.Weekly;
            case Repeats.Daily: return RecurrenceInterval.Daily;
            case Repeats.None: return RecurrenceInterval.None;
            default /* DaysOfWeek */: return RecurrenceInterval.Weekly;
            }
        }

        private static List<DayOfWeek> DaysOfWeek(this Repeats repeats)
        {
            List<DayOfWeek> daysOfWeek = new List<DayOfWeek>();
            if ((repeats & Repeats.Monday) != 0) daysOfWeek.Add(DayOfWeek.Monday);
            if ((repeats & Repeats.Tuesday) != 0) daysOfWeek.Add(DayOfWeek.Tuesday);
            if ((repeats & Repeats.Wednesday) != 0) daysOfWeek.Add(DayOfWeek.Wednesday);
            if ((repeats & Repeats.Thursday) != 0) daysOfWeek.Add(DayOfWeek.Thursday);
            if ((repeats & Repeats.Friday) != 0) daysOfWeek.Add(DayOfWeek.Friday);
            if ((repeats & Repeats.Saturday) != 0) daysOfWeek.Add(DayOfWeek.Saturday);
            if ((repeats & Repeats.Sunday) != 0) daysOfWeek.Add(DayOfWeek.Sunday);
            return daysOfWeek;
        }

        public static DateTime ActualDate(this Repeats repeats, DateTime startDate)
        {
            switch (repeats)
            {
            case Repeats.Monthly:
                while (startDate.Date < DateTime.Today)
                {
                    startDate = startDate.AddMonths(1);
                }
                return startDate;

            case Repeats.Weekly:
                while (startDate.Date < DateTime.Today)
                {
                    startDate = startDate.AddDays(_daysInWeek);
                }
                return startDate;

            case Repeats.Daily:
                return DateTime.Today.SetTime(startDate);

            case Repeats.None:
                return startDate;

            default /* DaysOfWeek */:
                if (startDate.Date < DateTime.Today)
                {
                    startDate = DateTime.Today.SetTime(startDate);
                }
                List<DayOfWeek> list = repeats.DaysOfWeek();
                for (int i = 0; i < _daysInWeek; i++)
                {
                    DateTime date = startDate.AddDays(i);
                    if (list.Contains(date.DayOfWeek))
                    {
                        return date;
                    }
                }
                return startDate;
            }
        }

        public static List<DateTime> Dates(this Repeats repeats, DateTime startDate, bool skipFirst = false)
        {
            List<DateTime> dates = new List<DateTime>();

            switch (repeats)
            {
            case Repeats.Monthly:
                dates.Add(skipFirst ? startDate.AddMonths(1) : startDate);
                return dates;

            case Repeats.Weekly:
                dates.Add(skipFirst ? startDate.AddDays(_daysInWeek) : startDate);
                return dates;

            case Repeats.Daily:
                dates.Add(skipFirst ? startDate.AddDays(1) : startDate);
                return dates;

            case Repeats.None:
                if (!skipFirst)
                {
                    dates.Add(startDate);
                }
                return dates;

            default /* DaysOfWeek */:
                List<DayOfWeek> list = repeats.DaysOfWeek();
                for (int i = 0; i < _daysInWeek; i++)
                {
                    DateTime date = startDate.AddDays(i);
                    if (list.Contains(date.DayOfWeek))
                    {
                        dates.Add(date);
                    }
                }

                if (skipFirst && dates.Count > 0)
                {
                    dates[0] = dates[0].AddDays(_daysInWeek);
                }

                return dates;
            }
        }
    }
}
