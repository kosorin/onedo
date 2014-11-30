using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleTasks.Helpers
{
    public static class GestureActionHelper
    {
        public static Style IconStyle(GestureAction action)
        {
            switch (action)
            {
            case GestureAction.Complete: return App.Current.Resources["CheckBoxIconStyle"] as Style;
            case GestureAction.Delete: return App.Current.Resources["DeleteIconStyle"] as Style;
            case GestureAction.Reminder: return App.Current.Resources["ReminderIconStyle"] as Style;
            case GestureAction.DueToday: return App.Current.Resources["CalendarIconStyle"] as Style;
            case GestureAction.DueTomorrow: return App.Current.Resources["CalendarIconStyle"] as Style;
            case GestureAction.None:
            default: return null;
            }
        }

        public static string Text(GestureAction action)
        {
            switch (action)
            {
            case GestureAction.Complete: return AppResources.GestureActionComplete;
            case GestureAction.Delete: return AppResources.GestureActionDelete;
            case GestureAction.Reminder: return AppResources.GestureActionReminder;
            case GestureAction.DueToday: return AppResources.GestureActionDueToday;
            case GestureAction.DueTomorrow: return AppResources.GestureActionDueTomorrow;
            case GestureAction.None:
            default: return AppResources.GestureActionNone;
            }
        }
    }
}
