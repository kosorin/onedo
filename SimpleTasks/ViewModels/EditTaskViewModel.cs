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
        public bool IsOldTask { get; set; }

        public TaskModel OldTask { get; set; }

        public TaskModel CurrentTask { get; set; }

        public List<KeyValuePair<string, DateTime>> PresetDueDateList { get; set; }

        public EditTaskViewModel()
            : this(null)
        {
        }

        public EditTaskViewModel(TaskModel task)
        {
            OldTask = task;
            CurrentTask = new TaskModel();

            if (OldTask != null)
            {
                CurrentTask = OldTask.Clone();
                IsOldTask = true;
            }
            else
            {
                IsOldTask = false;
            }

            PresetDueDateList = BuildDueDatePresetList();
        }

        private List<KeyValuePair<string, DateTime>> BuildDueDatePresetList()
        {
            List<KeyValuePair<string, DateTime>> items = new List<KeyValuePair<string, DateTime>>();

            items.Add(new KeyValuePair<string, DateTime>(AppResources.DateTodayText, DateTimeExtensions.Today));
            items.Add(new KeyValuePair<string, DateTime>(AppResources.DateTomorrowText, DateTimeExtensions.Tomorrow));

            int daysAfterTomorrow = 4;
            for (int i = 1; i <= daysAfterTomorrow; i++)
            {
                DateTime date = DateTimeExtensions.Tomorrow.AddDays(i);
                items.Add(new KeyValuePair<string, DateTime>(date.ToString("dddd", CultureInfo.CurrentCulture).ToLower(), date));
            }

            items.Add(new KeyValuePair<string, DateTime>(AppResources.DateThisWeekText, DateTimeExtensions.LastDayOfWeek));
            items.Add(new KeyValuePair<string, DateTime>(AppResources.DateNextWeekText, DateTimeExtensions.LastDayOfNextWeek));
            items.Add(new KeyValuePair<string, DateTime>(AppResources.DateThisMonthText, DateTimeExtensions.LastDayOfMonth));
            items.Add(new KeyValuePair<string, DateTime>(AppResources.DateNextMonthText, DateTimeExtensions.LastDayOfNextMonth));

            return items;
        }

        public void ActivateTask()
        {
            CurrentTask.CompletedDate = null;
            SaveTask();
        }

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
                App.ViewModel.UpdateTask(OldTask, CurrentTask);
            }
            else
            {
                App.ViewModel.AddTask(CurrentTask);
            }
        }

        public void DeleteTask()
        {
            App.ViewModel.RemoveTask(OldTask);
        }
    }
}
