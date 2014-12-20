using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Helpers;
using SimpleTasks.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SimpleTasks.ViewModels
{
    public class TasksViewModel : BindableBase
    {
        private TaskCollection _tasks = new TaskCollection();
        public TaskCollection Tasks
        {
            get { return _tasks; }
            private set { SetProperty(ref _tasks, value); }
        }

        public void Load()
        {
            Load(TaskCollection.LoadFromFile(App.TasksFileName));
        }

        public void Load(TaskCollection tasks)
        {
            Tasks = tasks;
            if (Settings.Current.Tasks.DeleteCompleted > 0)
            {
                DeleteCompleted(Settings.Current.Tasks.DeleteCompletedBefore);
            }

#if DEBUG
            Debug.WriteLine("> Nahrané úkoly ({0}):", Tasks.Count);
            foreach (TaskModel task in Tasks)
            {
                Reminder reminder = task.GetSystemReminder();
                Debug.WriteLine(": {0} [připomenutí: {1}]", task.Title, reminder != null ? reminder.Name : "<false>");
            }
#endif
        }

        public void Save()
        {
            if (Settings.Current.Tasks.DeleteCompleted == 0)
            {
                DeleteCompleted();
            }
            TaskCollection.SaveToFile(App.TasksFileName, Tasks);
        }

        public void Restore(TaskCollection tasks)
        {
            Debug.WriteLine("> Obnova úkolů");
            DeleteAll();

            foreach (TaskModel task in tasks)
            {
                Add(task);
            }

            if (Settings.Current.Tasks.DeleteCompleted > 0)
            {
                DeleteCompleted(Settings.Current.Tasks.DeleteCompletedBefore);
            }

#if DEBUG
            Debug.WriteLine(": Obnovené úkoly({0}):", Tasks.Count);
            foreach (TaskModel task in Tasks)
            {
                Reminder reminder = task.GetSystemReminder();
                Debug.WriteLine(": {0} [připomenutí: {1}]", task.Title, reminder != null ? reminder.Name : "<false>");
            }
#endif
        }

        public void Add(TaskModel task)
        {
            Tasks.Add(task);
            if (task.IsActive && task.HasReminder)
            {
                task.SetSystemReminder();
            }
        }

        public void Update(TaskModel task)
        {
            Reminder reminder = task.GetSystemReminder();
            if (reminder != null)
            {
                if (task.IsCompleted || !task.HasReminder || !reminder.IsScheduled || reminder.BeginTime != task.ReminderDate)
                {
                    task.RemoveSystemReminder();
                    reminder = null;
                }
            }

            if (reminder == null && task.IsActive && task.HasReminder)
            {
                task.SetSystemReminder();
            }

            TaskWrapper wrapper = task.GetWrapper();
            if (wrapper != null)
            {
                wrapper.UpdateIsScheduled();
            }
        }

        public void Delete(TaskModel task)
        {
            Tasks.Remove(task);

            task.RemoveSystemReminder();
            LiveTile.Unpin(task);
        }

        public void DeleteCompleted(DateTime beforeDate)
        {
            // Odstranění úkolů, které byly odstarněny před více jak 'days' dny.
            var completedTasks = Tasks.Where((t) =>
            {
                return t.IsCompleted && t.Completed.Value < beforeDate;
            }).ToList();
            foreach (TaskModel task in completedTasks)
            {
                Delete(task);
            }
        }

        public void DeleteCompleted()
        {
            // Odstranění dokončených úkolů
            foreach (TaskModel task in Tasks.Where(t => t.IsCompleted).ToList())
            {
                Delete(task);
            }
        }

        public void DeleteAll()
        {
            foreach (Reminder r in ScheduledActionService.GetActions<Reminder>())
            {
                ScheduledActionService.Remove(r.Name);
            }

            foreach (TaskModel task in Tasks)
            {
                LiveTile.Unpin(task);
            }
            Tasks.Clear();
        }
    }
}
