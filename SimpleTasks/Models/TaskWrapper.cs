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

namespace SimpleTasks.Models
{
    public class TaskWrapper : BindableBase
    {
        public TaskWrapper(TaskModel task)
        {
            Task = task;
            UpdateReminderProperties();
        }

        public TaskModel Task { get; private set; }

        #region IsScheduled
        public void UpdateReminderProperties()
        {
            if (Task != null && Task.HasReminder)
            {
                Reminder reminder = ReminderHelper.Get(Task);
                if (reminder != null && reminder.IsScheduled)
                {
                    IsScheduled = true;
                    ReminderIconBrush = App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                    ReminderNotScheduledVisibility = Visibility.Collapsed;
                }
            }

            if (ReminderIconBrush == null)
            {
                IsScheduled = false;
                ReminderIconBrush = App.Current.Resources["SubtleBrush"] as SolidColorBrush;
                ReminderNotScheduledVisibility = Visibility.Visible;
            }
        }

        private bool _isScheduled = false;
        public bool IsScheduled
        {
            get { return _isScheduled; }
            set { SetProperty(ref _isScheduled, value); }
        }

        private SolidColorBrush _reminderIconBrush = null;
        public SolidColorBrush ReminderIconBrush
        {
            get { return _reminderIconBrush; }
            set { SetProperty(ref _reminderIconBrush, value); }
        }

        private Visibility _reminderNotScheduledVisibility = Visibility.Visible;
        public Visibility ReminderNotScheduledVisibility
        {
            get { return _reminderNotScheduledVisibility; }
            set { SetProperty(ref _reminderNotScheduledVisibility, value); }
        }
        #endregion
    }
}
