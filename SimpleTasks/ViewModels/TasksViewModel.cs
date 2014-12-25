﻿using Microsoft.Phone.Scheduler;
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
            Tasks = tasks ?? new TaskCollection();
            if (Settings.Current.Tasks.DeleteCompleted > 0)
            {
                DeleteCompleted(Settings.Current.Tasks.DeleteCompletedBefore);
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
            if (task.IsActive && task.HasReminder)
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
            foreach (Reminder r in ScheduledActionService.GetActions<Reminder>())
            {
                ScheduledActionService.Remove(r.Name);
            }
        }
    }
}
