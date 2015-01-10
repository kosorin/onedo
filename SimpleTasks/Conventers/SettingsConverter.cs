using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;
using DefaultDateTypes = SimpleTasks.Core.Models.Settings.DefaultDateTypes;
using SimpleTasks.Core.Models;
using System.Windows;
using SimpleTasks.Helpers;

namespace SimpleTasks.Conventers
{
    public class SettingsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string settings = parameter as string ?? "";

            if (settings == "DeleteCompleted" && value is int)
            {
                switch ((int)value)
                {
                case 0: return AppResources.SettingsDeleteWhenStarts;
                case 1: return AppResources.SettingsDeleteAfterOneDay;
                case 2: return AppResources.SettingsDeleteAfterTwoDays;
                case 3: return AppResources.SettingsDeleteAfterThreeDays;
                case 7: return AppResources.SettingsDeleteAfterOneWeek;
                case 14: return AppResources.SettingsDeleteAfterTwoWeeks;
                case -1:
                default: return AppResources.SettingsDeleteNever;
                }
            }
            else if (settings == "DefaultDateType" && value is DefaultDateTypes)
            {
                switch ((DefaultDateTypes)value)
                {
                case DefaultDateTypes.Today: return AppResources.DateToday;
                case DefaultDateTypes.Tomorrow: return AppResources.DateTomorrow;
                case DefaultDateTypes.ThisWeek: return AppResources.DateThisWeek;
                case DefaultDateTypes.NextWeek: return AppResources.DateNextWeek;
                case DefaultDateTypes.NoDueDate:
                default: return AppResources.DateNoDue;
                }
            }
            else if (settings == "GestureActionIconStyle" && value is GestureAction)
            {
                return GestureActionHelper.IconStyle((GestureAction)value);
            }
            else if (settings == "GestureActionText" && value is GestureAction)
            {
                return GestureActionHelper.Text((GestureAction)value);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
