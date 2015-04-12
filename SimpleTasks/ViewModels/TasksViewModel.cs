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
            Load(TaskCollection.LoadFromFile(AppInfo.TasksFileName));
        }

        public void Load(TaskCollection tasks)
        {
            Tasks = tasks ?? new TaskCollection();
            if (Settings.Current.DeleteCompleted > 0)
            {
                DeleteCompleted(Settings.Current.DeleteCompletedBefore);
            }

#if DEBUG
            Debug.WriteLine("> Nahrané úkoly ({0}):", Tasks.Count);
            foreach (TaskModel task in Tasks)
            {
                List<Reminder> reminders = task.GetSystemReminder();
                Debug.WriteLine(": {0} [připomenutí: {1}]", task.Title, reminders.Count);
            }
#endif
        }

        public void Save()
        {
            if (Settings.Current.DeleteCompleted == 0)
            {
                DeleteCompleted();
            }
            TaskCollection.SaveToFile(AppInfo.TasksFileName, Tasks);
        }

        public void Restore(TaskCollection tasks)
        {
            Debug.WriteLine("> Obnova úkolů");
            DeleteAll();

            foreach (TaskModel task in tasks)
            {
                Add(task);
            }

            if (Settings.Current.DeleteCompleted > 0)
            {
                DeleteCompleted(Settings.Current.DeleteCompletedBefore);
            }

#if DEBUG
            Debug.WriteLine(": Obnovené úkoly({0}):", Tasks.Count);
            foreach (TaskModel task in Tasks)
            {
                List<Reminder> reminders = task.GetSystemReminder();
                Debug.WriteLine(": {0} [připomenutí: {1}]", task.Title, reminders.Count);
            }
#endif
        }

        public void Add(TaskModel task)
        {
            Tasks.Add(task);
            UpdateReminders(task);
        }

        private void UpdateReminders(TaskModel task)
        {
            task.RemoveSystemReminder();
            if (task.HasReminder)
            {
                task.SetSystemReminder();
            }
        }

        public void Update(TaskModel task)
        {
            UpdateReminders(task);

            TaskWrapper wrapper = task.GetWrapper();
            if (wrapper != null)
            {
                wrapper.UpdateIsScheduled();
            }

            task.ModifiedSinceStart = true;
        }

        public void Delete(TaskModel task)
        {
            Tasks.Remove(task);

            task.RemoveSystemReminder();
            LiveTile.Unpin(task);
        }

        public void DeleteCompleted(DateTime beforeDate)
        {
            // Odstranění úkolů, které byly dokončeny před beforeDate
            foreach (TaskModel task in Tasks.Where(t => (t.IsCompleted && t.Completed.Value < beforeDate)).ToList())
            {
                Delete(task);
            }
        }

        public void DeleteCompleted()
        {
            DeleteCompleted(DateTime.MaxValue);
        }

        public void DeleteAll()
        {
            foreach (TaskModel task in Tasks.ToList())
            {
                Delete(task);
            }

            // Odstranění "zapomenutých" připoměnutí
            ReminderHelper.RemoveAll();
        }

        public void Complete(TaskModel task)
        {
            if (task == null)
                return;

            task.Completed = DateTime.Now;
            task.ModifiedSinceStart = true;
            if (Settings.Current.CompleteSubtasks && task.HasSubtasks)
            {
                foreach (Subtask subtask in task.Subtasks)
                {
                    subtask.IsCompleted = true;
                }
            }
            if (Settings.Current.UnpinCompleted && !task.HasRepeats)
            {
                LiveTile.Unpin(task);
            }

            App.Tasks.Update(task);
        }

        public void Complete(TaskModel task, Subtask subtask)
        {
            if (task != null)
            {
                task.ModifiedSinceStart = true;

                if (subtask != null)
                {
                    subtask.IsCompleted = !subtask.IsCompleted;

                    if (Settings.Current.CompleteTask && !task.IsCompleted && subtask.IsCompleted)
                    {
                        if (task.Subtasks.All(s => s.IsCompleted))
                        {
                            Complete(task);
                        }
                    }
                }
            }
        }

        public void Activate(TaskModel task)
        {
            task.Completed = null;
            App.Tasks.Update(task);
        }
    }
}
