using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;
namespace SimpleTasks.Conventers
{
    public class RepeatsConverter : IValueConverter
    {
        private string FormattedText(int value, string format1, string format2To4, string format5AndMore)
        {
            if (value == 1)
            {
                return string.Format(format1, value);
            }
            else if (value >= 2 && value <= 4)
            {
                return string.Format(format2To4, value);
            }
            else
            {
                return string.Format(format5AndMore, value);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Repeats)
            {
                Repeats repeats = (Repeats)value;

                if (repeats == Repeats.Weekends)
                {
                    return AppResources.RepeatsWeekends;
                }
                else if (repeats == Repeats.Weekdays)
                {
                    return AppResources.RepeatsWeekdays;
                }
                else if (repeats == Repeats.AllDays)
                {
                    return AppResources.RepeatsEveryDay;
                }
                else
                {
                    string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
                    switch (repeats)
                    {
                    case Repeats.Monday: return dayNames[(int)DayOfWeek.Monday];
                    case Repeats.Tuesday: return dayNames[(int)DayOfWeek.Tuesday];
                    case Repeats.Wednesday: return dayNames[(int)DayOfWeek.Wednesday];
                    case Repeats.Thursday: return dayNames[(int)DayOfWeek.Thursday];
                    case Repeats.Friday: return dayNames[(int)DayOfWeek.Friday];
                    case Repeats.Saturday: return dayNames[(int)DayOfWeek.Saturday];
                    case Repeats.Sunday: return dayNames[(int)DayOfWeek.Sunday];
                    }

                    dayNames = CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames;
                    List<string> list = new List<string>();
                    if ((repeats & Repeats.Weekdays) == Repeats.Weekdays)
                    {
                        list.Add(AppResources.RepeatsWeekdays);
                    }
                    else
                    {
                        if ((repeats & Repeats.Monday) != 0) list.Add(dayNames[(int)DayOfWeek.Monday]);
                        if ((repeats & Repeats.Tuesday) != 0) list.Add(dayNames[(int)DayOfWeek.Tuesday]);
                        if ((repeats & Repeats.Wednesday) != 0) list.Add(dayNames[(int)DayOfWeek.Wednesday]);
                        if ((repeats & Repeats.Thursday) != 0) list.Add(dayNames[(int)DayOfWeek.Thursday]);
                        if ((repeats & Repeats.Friday) != 0) list.Add(dayNames[(int)DayOfWeek.Friday]);
                    }
                    if ((repeats & Repeats.Weekends) == Repeats.Weekends)
                    {
                        list.Add(AppResources.RepeatsWeekends);
                    }
                    else
                    {
                        if ((repeats & Repeats.Saturday) != 0) list.Add(dayNames[(int)DayOfWeek.Saturday]);
                        if ((repeats & Repeats.Sunday) != 0) list.Add(dayNames[(int)DayOfWeek.Sunday]);
                    }

                    if (list.Count > 0)
                    {
                        return string.Join(", ", list);
                    }
                }
            }
            return AppResources.RepeatsOnce;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
