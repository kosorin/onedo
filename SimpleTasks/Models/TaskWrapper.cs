using SimpleTasks.Core.Helpers;
using SimpleTasks.Helpers;
using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Scheduler;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;

namespace SimpleTasks.Models
{
    public class TaskWrapper : BindableBase
    {
        public TaskWrapper(TaskModel task)
        {
            Task = task;
            UpdateIsScheduled();
        }

        public TaskModel Task { get; private set; }

        #region IsScheduled
        public void UpdateIsScheduled()
        {
            if (Task != null && Task.HasReminder)
            {
                Reminder reminder = Task.GetSystemReminder();
                IsScheduled = reminder != null && reminder.IsScheduled;
            }
            else
            {
                IsScheduled = false;
            }
        }

        private bool _isScheduled = false;
        public bool IsScheduled
        {
            get { return _isScheduled; }
            set { SetProperty(ref _isScheduled, value); }
        }
        #endregion

        #region CheckBoxVisibility
        public static Visibility CheckBoxVisibility
        {
            get { return Settings.Current.General.HideTaskCheckBox ? Visibility.Collapsed : Visibility.Visible; }
        }
        #endregion

        #region Swipe Icon Styles
        private Style GestureIconStyle(GestureAction action)
        {
            switch (action)
            {
            case GestureAction.Complete: return App.Current.Resources["CompleteIconStyle"] as Style;
            case GestureAction.Delete: return App.Current.Resources["DeleteIconStyle"] as Style;
            case GestureAction.Reminder: return App.Current.Resources["ClockIconStyle"] as Style;
            case GestureAction.DueToday: return App.Current.Resources["CompleteIconStyle"] as Style;
            case GestureAction.DueTomorrow: return App.Current.Resources["CompleteIconStyle"] as Style;
            case GestureAction.None:
            default: return App.Current.Resources["CompleteIconStyle"] as Style;
            }
        }

        public Style SwipeLeftIconStyle
        {
            get { return GestureIconStyle(Settings.Current.Tasks.SwipeLeftAction); }
        }

        public Style SwipeRightIconStyle
        {
            get { return GestureIconStyle(Settings.Current.Tasks.SwipeRightAction); }
        }
        #endregion // end of Swipe Icon Styles

        //#region Color
        //private Color? _color = null;
        //public Color Color
        //{
        //    get
        //    {
        //        if (_color == null)
        //        {
        //            if (Task.Color == TaskModel.DefaultColor)
        //            {
        //                _color = (Color)App.Current.Resources["AccentColor"];
        //            }
        //            else
        //            {
        //                _color = Task.Color;
        //            }
        //        }
        //        return _color.Value;
        //    }
        //}

        //private SolidColorBrush _colorBrush = null;
        //public SolidColorBrush ColorBrush
        //{
        //    get
        //    {
        //        if (_colorBrush == null)
        //        {
        //            _colorBrush = new SolidColorBrush(Color);
        //        }
        //        return _colorBrush;
        //    }
        //}
        //#endregion
    }
}
