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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repeats"></param>
        /// <param name="startDate"></param>
        /// <returns>Vrací datum dalšího dne (může být stejný jako startDate) nebo DateTime.MaxValue, pokud</returns>
        public static DateTime NextDate(this Repeats repeats, DateTime startDate)
        {
            const int daysInWeek = 7;
            List<DayOfWeek> list = new List<DayOfWeek>();
            if ((repeats & Repeats.Monday) != 0) list.Add(DayOfWeek.Monday);
            if ((repeats & Repeats.Tuesday) != 0) list.Add(DayOfWeek.Tuesday);
            if ((repeats & Repeats.Wednesday) != 0) list.Add(DayOfWeek.Wednesday);
            if ((repeats & Repeats.Thursday) != 0) list.Add(DayOfWeek.Thursday);
            if ((repeats & Repeats.Friday) != 0) list.Add(DayOfWeek.Friday);
            if ((repeats & Repeats.Saturday) != 0) list.Add(DayOfWeek.Saturday);
            if ((repeats & Repeats.Sunday) != 0) list.Add(DayOfWeek.Sunday);

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

        public static List<DateTime> NextWeekDates(this Repeats repeats, DateTime startDate)
        {
            List<DateTime> dates = new List<DateTime>();

            const int daysInWeek = 7;
            List<DayOfWeek> list = new List<DayOfWeek>();
            if ((repeats & Repeats.Monday) != 0) list.Add(DayOfWeek.Monday);
            if ((repeats & Repeats.Tuesday) != 0) list.Add(DayOfWeek.Tuesday);
            if ((repeats & Repeats.Wednesday) != 0) list.Add(DayOfWeek.Wednesday);
            if ((repeats & Repeats.Thursday) != 0) list.Add(DayOfWeek.Thursday);
            if ((repeats & Repeats.Friday) != 0) list.Add(DayOfWeek.Friday);
            if ((repeats & Repeats.Saturday) != 0) list.Add(DayOfWeek.Saturday);
            if ((repeats & Repeats.Sunday) != 0) list.Add(DayOfWeek.Sunday);

            for (int i = 0; i < daysInWeek; i++)
            {
                DateTime date = startDate.AddDays(i);
                if (list.Contains(date.DayOfWeek))
                {
                    dates.Add(date);
                }
            }
            return dates;
        }
    }
}
