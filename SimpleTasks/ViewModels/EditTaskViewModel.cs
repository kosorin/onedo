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

        public EditTaskViewModel()
            : this(null)
        {
        }

        public EditTaskViewModel(TaskModel oldTask)
        {
            OldTask = oldTask;
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
        }

        public void ActivateTask()
        {
            CurrentTask.CompletedDate = null;
            SaveTask();
        }

        public void CompleteTask()
        {
            CurrentTask.CompletedDate = DateTime.Now;
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
