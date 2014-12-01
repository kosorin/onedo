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
            case GestureAction.Complete: return App.IconStyle("CheckBox");
            case GestureAction.Delete: return App.IconStyle("Delete");
            case GestureAction.DueToday: return App.IconStyle("Calendar");
            case GestureAction.DueTomorrow: return App.IconStyle("Calendar");
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
            case GestureAction.DueToday: return AppResources.GestureActionDueToday;
            case GestureAction.DueTomorrow: return AppResources.GestureActionDueTomorrow;
            case GestureAction.None:
            default: return AppResources.GestureActionNone;
            }
        }
    }
}
