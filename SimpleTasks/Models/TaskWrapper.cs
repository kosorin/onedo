using System.Collections.Generic;
using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Helpers;

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
                List<Reminder> reminders = Task.GetSystemReminder();
                bool isScheduled = reminders.Count > 0;
                foreach (Reminder reminder in reminders)
                {
                    if (!reminder.IsScheduled)
                    {
                        isScheduled = false;
                        break;
                    }
                }
                IsScheduled = isScheduled;
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

    public static class TaskWrapperExtensions
    {
        public static TaskWrapper GetWrapper(this TaskModel task)
        {
            if (task != null && task.Wrapper != null)
            {
                return task.Wrapper as TaskWrapper;
            }
            return null;
        }
    }
}
