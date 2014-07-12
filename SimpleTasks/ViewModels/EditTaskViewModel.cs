using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.ViewModels
{
    public class EditTaskViewModel : BindableBase
    {
        private TaskModel Original { get; set; }

        public bool IsNew { get; private set; }

        public EditTaskViewModel(TaskModel originalTask)
        {
            Original = originalTask;
            IsNew = (Original == null);
            if (IsNew)
            {
                Original = new TaskModel();

                IsSetDueDate = (App.Settings.DefaultDueDateSettingToDateTime != null);
                DueDate = App.Settings.DefaultDueDateSettingToDateTime ?? DateTime.Now;
                IsSetReminderDate = false;
                IsComplete = false;
            }
            else
            {
                Title = originalTask.Title;
                Detail = originalTask.Detail;
                Priority = originalTask.Priority;
                IsSetDueDate = (originalTask.DueDate != null);
                DueDate = originalTask.DueDate ?? DateTime.Now;
                IsSetReminderDate = (originalTask.ReminderDate != null);
                Reminder = originalTask.Reminder;
                ReminderDate = originalTask.ReminderDate ?? DateTime.Now;
                IsComplete = originalTask.IsComplete;
            }

            BuildDueDatePresetList();
            BuildReminderPresetList();
        }

        #region Data
        private string _title = "";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _detail = "";
        public string Detail
        {
            get { return _detail; }
            set { SetProperty(ref _detail, value); }
        }

        private TaskPriority _priority = TaskPriority.Normal;
        public TaskPriority Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }

        private DateTime _dueDate;
        public DateTime DueDate
        {
            get { return _dueDate; }
            set { SetProperty(ref _dueDate, value); }
        }

        private bool _isSetDueDate = false;
        public bool IsSetDueDate
        {
            get { return _isSetDueDate; }
            set
            {
                if (value)
                {
                    DateTime? newDueDate = App.Settings.DefaultDueDateSettingToDateTime;
                    if (newDueDate == null)
                    {
                        newDueDate = DateTimeExtensions.Today;
                    }
                    DueDate = newDueDate.Value;
                }
                SetProperty(ref _isSetDueDate, value);
            }
        }

        private DateTime _reminderDate;
        public DateTime ReminderDate
        {
            get { return _reminderDate; }
            set { SetProperty(ref _reminderDate, value); }
        }

        private TimeSpan? _reminder = null;
        public TimeSpan? Reminder
        {
            get { return _reminder; }
            set { SetProperty(ref _reminder, value); }
        }

        private bool _isSetReminderDate = false;
        public bool IsSetReminderDate
        {
            get { return _isSetReminderDate; }
            set
            {
                if (value)
                {
                    DateTime defaultReminderTime = App.Settings.DefaultReminderTimeSetting;
                    if (IsSetDueDate)
                    {
                        ReminderDate = DueDate.Date.AddHours(defaultReminderTime.Hour)
                                                   .AddMinutes(defaultReminderTime.Minute);
                    }
                    else
                    {
                        ReminderDate = DateTime.Today.Date.AddHours(defaultReminderTime.Hour)
                                                          .AddMinutes(defaultReminderTime.Minute);
                    }
                }
                SetProperty(ref _isSetReminderDate, value);
            }
        }

        private bool _isComplete;
        public bool IsComplete
        {
            get { return _isComplete; }
            set { SetProperty(ref _isComplete, value); }
        }
        #endregion

        #region Task methods
        public void Save()
        {
            // Title
            Original.Title = Title;

            // Detail
            Original.Detail = Detail;

            // Priority
            Original.Priority = Priority;

            // Due Date
            if (IsSetDueDate)
                Original.DueDate = DueDate;
            else
                Original.DueDate = null;

            // Reminder Date
            if (ReminderDate <= DateTime.Now)
            {
                ReminderDate = DateTime.Now.AddMinutes(2);
            }
            if (IsSetReminderDate)
                Original.ReminderDate = ReminderDate;
            else
                Original.ReminderDate = null;

            // Completed Date
            if (IsComplete)
                Original.CompletedDate = DateTime.Now;
            else
                Original.CompletedDate = null;

            // ULOŽENÍ
            Original.ModifiedSinceStart = true;
            if (IsNew)
            {
                App.Tasks.Add(Original);
            }
            else
            {
                App.Tasks.Update(Original);
            }

            IsNew = false;
        }

        public void Activate()
        {
            if (!IsNew)
            {
                IsComplete = false;
            }
        }

        public void Complete()
        {
            if (!IsNew)
            {
                IsComplete = true;
                Save();

                if (App.Settings.UnpinCompletedSetting)
                {
                    Unpin();
                }
            }
        }

        public void Delete()
        {
            App.Tasks.Delete(Original);
        }

        public void Pin()
        {
            Save();
            LiveTile.PinEmpty(Original);
        }

        public void Unpin()
        {
            if (!IsNew)
            {
                LiveTile.Unpin(Original);
            }
        }

        public bool IsPinned()
        {
            return LiveTile.IsPinned(Original);
        }
        #endregion

        #region PresetLists
        public List<KeyValuePair<string, DateTime>> DueDatePresetList { get; set; }

        public List<TimeSpan?> ReminderPresetList { get; set; }

        private void BuildDueDatePresetList()
        {
            DueDatePresetList = new List<KeyValuePair<string, DateTime>>();

            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateToday, DateTimeExtensions.Today));
            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateTomorrow, DateTimeExtensions.Tomorrow));

            int daysAfterTomorrow = 4;
            for (int i = 1; i <= daysAfterTomorrow; i++)
            {
                DateTime date = DateTimeExtensions.Tomorrow.AddDays(i);
                DueDatePresetList.Add(new KeyValuePair<string, DateTime>(date.ToString("dddd", CultureInfo.CurrentCulture).ToLower(), date));
            }

            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateThisWeek, DateTimeExtensions.LastDayOfWeek));
            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateNextWeek, DateTimeExtensions.LastDayOfNextWeek));
            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateThisMonth, DateTimeExtensions.LastDayOfMonth));
            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateNextMonth, DateTimeExtensions.LastDayOfNextMonth));
        }

        private void BuildReminderPresetList()
        {
            ReminderPresetList = new List<TimeSpan?>();
            ReminderPresetList.Add(null);
            ReminderPresetList.Add(TimeSpan.FromMinutes(5));
            ReminderPresetList.Add(TimeSpan.FromMinutes(10));
            ReminderPresetList.Add(TimeSpan.FromMinutes(15));
            ReminderPresetList.Add(TimeSpan.FromMinutes(20));
            ReminderPresetList.Add(TimeSpan.FromMinutes(30));
            ReminderPresetList.Add(TimeSpan.FromMinutes(45));
            ReminderPresetList.Add(TimeSpan.FromHours(1));
            ReminderPresetList.Add(TimeSpan.FromHours(1.5));
            ReminderPresetList.Add(TimeSpan.FromHours(2));
            ReminderPresetList.Add(TimeSpan.FromHours(3));
            ReminderPresetList.Add(TimeSpan.FromHours(4));
            ReminderPresetList.Add(TimeSpan.FromHours(10));
            ReminderPresetList.Add(TimeSpan.FromHours(18));
            ReminderPresetList.Add(TimeSpan.FromDays(1));
            ReminderPresetList.Add(TimeSpan.FromDays(2));
            ReminderPresetList.Add(TimeSpan.FromDays(4));
            ReminderPresetList.Add(TimeSpan.FromDays(7));
            ReminderPresetList.Add(TimeSpan.FromDays(14));
        }
        #endregion
    }
}
