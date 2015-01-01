using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
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
        AllDays = Weekdays | Weekends
    }

    public static class RepeatsExtensions
    {
        public static List<DayOfWeek> DaysOfWeek(this Repeats repeats)
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
            const int daysInWeek = 7;
            List<DayOfWeek> list = repeats.DaysOfWeek();
            for (int i = 0; i < daysInWeek; i++)
            {
                DateTime date = startDate.AddDays(i);
                if (list.Contains(date.DayOfWeek))
                {
                    return date;
                }
            }
            return DateTime.MaxValue;
        }

        public static DateTime NextDate(this Repeats repeats, DateTime startDate)
        {
            const int daysInWeek = 7 * 2;
            bool next = false;
            List<DayOfWeek> list = repeats.DaysOfWeek();
            for (int i = 0; i < daysInWeek; i++)
            {
                DateTime date = startDate.AddDays(i);
                if (list.Contains(date.DayOfWeek))
                {
                    if (next)
                    {
                        return date;
                    }
                    else
                    {
                        next = true;
                    }
                }
            }
            return DateTime.MaxValue;
        }

        public static List<DateTime> WeekDates(this Repeats repeats, DateTime startDate, bool skipFirst = false)
        {
            List<DateTime> dates = new List<DateTime>();

            int daysInWeek = 7;
            List<DayOfWeek> list = repeats.DaysOfWeek();
            for (int i = 0; i < daysInWeek; i++)
            {
                DateTime date = startDate.AddDays(i);
                if (list.Contains(date.DayOfWeek))
                {
                    dates.Add(date);
                }
            }

            if (skipFirst && dates.Count > 0)
            {
                dates[0] = dates[0].AddDays(daysInWeek);
            }

            return dates;
        }
    }
}
