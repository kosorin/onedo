using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.ViewModels
{
    public class EditTaskViewModel : BindableBase
    {
        public EditTaskViewModel() : this(null) { }

        public EditTaskViewModel(TaskModel task)
        {
            OldTask = task;
            if (OldTask != null)
            {
                CurrentTask = OldTask.Clone();
                IsOldTask = true;
            }
            else
            {
                CurrentTask = new TaskModel();
                IsOldTask = false;
            }

            BuildDueDatePresetList();
        }

        public static Uri CreateEditTaskUri(TaskModel task)
        {
            return new Uri(string.Format("/Views/EditTaskPage.xaml?Task={0}", task.Uid), UriKind.Relative);
        }

        #region Tasks

        public bool IsOldTask { get; set; }

        public TaskModel OldTask { get; set; }

        public TaskModel CurrentTask { get; set; }

        public void CompleteTask()
        {
            CurrentTask.CompletedDate = DateTime.Now;
            CurrentTask.ReminderDate = null;
            SaveTask();
        }

        public void SaveTask()
        {
            if (IsOldTask)
            {
                App.Tasks.Update(OldTask, CurrentTask);
            }
            else
            {
                App.Tasks.Add(CurrentTask);
            }
        }

        public void DeleteTask()
        {
            App.Tasks.Delete(OldTask);
        }

        #endregion

        #region DueDatePresetList

        public List<KeyValuePair<string, DateTime>> DueDatePresetList { get; set; }

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

        #endregion
    }
}
