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

        //#region Color
        //private Color? _color = null;
        //public Color Color
        //{
        //    get
        //    {
        //        if (_color == null)
        //        {
        //            if (Task.Color == Colors.Transparent)
        //            {
        //                _color = (Color)App.Current.Resources["PhoneAccentColor"];
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
