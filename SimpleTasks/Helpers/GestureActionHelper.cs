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
            case GestureAction.Complete: return App.IconStyle("CheckBoxIconStyle");
            case GestureAction.Delete: return App.IconStyle("DeleteIconStyle");
            case GestureAction.Reminder: return App.IconStyle("ReminderIconStyle");
            case GestureAction.DueToday: return App.IconStyle("CalendarIconStyle");
            case GestureAction.DueTomorrow: return App.IconStyle("CalendarIconStyle");
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
