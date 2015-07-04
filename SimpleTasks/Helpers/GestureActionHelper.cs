using System.Windows;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;

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
            case GestureAction.DueToday: return App.IconStyle("DueToday");
            case GestureAction.DueTomorrow: return App.IconStyle("DueTomorrow");
            case GestureAction.PostponeDay: return App.IconStyle("PostponeOneDay");
            case GestureAction.PostponeWeek: return App.IconStyle("PostponeOneWeek");
            case GestureAction.None:
            default: return App.IconStyle("BoldClose");
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
            case GestureAction.PostponeDay: return AppResources.GestureActionPostponeDay;
            case GestureAction.PostponeWeek: return AppResources.GestureActionPostponeWeek;
            case GestureAction.None:
            default: return AppResources.GestureActionNone;
            }
        }
    }
}
