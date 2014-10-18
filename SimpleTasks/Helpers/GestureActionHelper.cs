using SimpleTasks.Core.Models;
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
            case GestureAction.DueToday: return App.Current.Resources["CompleteIconStyle"] as Style;
            case GestureAction.DueTomorrow: return App.Current.Resources["CompleteIconStyle"] as Style;
            case GestureAction.None:
            default: return null;
            }
        }

        public static string Text(GestureAction action)
        {
            switch (action)
            {
            case GestureAction.Complete: return "dokoncit";
            case GestureAction.Delete: return "smazat";
            case GestureAction.Reminder: return "pripomenuti";
            case GestureAction.DueToday: return "dnes";
            case GestureAction.DueTomorrow: return "zitra";
            case GestureAction.None:
            default: return "zadna akce";
            }
        }
    }
}
