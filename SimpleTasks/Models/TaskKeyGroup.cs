using SimpleTasks.Helpers;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleTasks.Models
{
    public class TaskKeyGroup : List<TaskModel>
    {
        public enum Types
        {
            Today,
            Tomorrow,
            ThisWeek,
            NextWeek,
            Later,
            Completed
        }

        public Types Type { get; private set; }

        public DateTime? Date { get; private set; }

        public string Key { get; private set; }

        public TaskKeyGroup(Types type, DateTime? date, string key)
        {
            Type = type;
            Date = date;
            Key = key;
        }

        public static TaskKeyGroupCollection CreateGroups(IEnumerable<TaskModel> items)
        {
            TaskKeyGroupCollection list = new TaskKeyGroupCollection();

            // Přidání úkolů do jednotlivých skupin
            if (items != null)
            {
                foreach (TaskModel item in items)
                {
                    list.NewTask(item);
                }
            }

            return list;
        }
    }

    public class TaskKeyGroupCollection : ObservableCollection<TaskKeyGroup>
    {
        public TaskKeyGroupCollection()
        {
            Add(new TaskKeyGroup(TaskKeyGroup.Types.Today, DateTimeExtensions.Today, AppResources.DateTodayText));
            Add(new TaskKeyGroup(TaskKeyGroup.Types.Tomorrow, DateTimeExtensions.Tomorrow, AppResources.DateTomorrowText));
            Add(new TaskKeyGroup(TaskKeyGroup.Types.ThisWeek, DateTimeExtensions.LastDayOfWeek, AppResources.DateThisWeekText));
            Add(new TaskKeyGroup(TaskKeyGroup.Types.NextWeek, DateTimeExtensions.LastDayOfNextWeek, AppResources.DateNextWeekText));
            Add(new TaskKeyGroup(TaskKeyGroup.Types.Later, null, AppResources.DateLaterText));
            Add(new TaskKeyGroup(TaskKeyGroup.Types.Completed, null, AppResources.DateCompletedText));
        }

        public void NewTask(TaskModel task)
        {
            TaskKeyGroup group = null;

            // Vyhledání příslušné skupiny, kde úkol patří
            if (task.IsComplete)
                group = this[(int)TaskKeyGroup.Types.Completed];

            else if (task.Date <= DateTimeExtensions.Today)
                group = this[(int)TaskKeyGroup.Types.Today];

            else if (task.Date == DateTimeExtensions.Tomorrow)
                group = this[(int)TaskKeyGroup.Types.Tomorrow];

            else if (task.Date > DateTimeExtensions.Tomorrow && task.Date <= DateTimeExtensions.LastDayOfWeek)
                group = this[(int)TaskKeyGroup.Types.ThisWeek];

            else if (task.Date > DateTimeExtensions.LastDayOfWeek && task.Date <= DateTimeExtensions.LastDayOfNextWeek)
                group = this[(int)TaskKeyGroup.Types.NextWeek];

            else
                group = this[(int)TaskKeyGroup.Types.Later];

            // Přidání úkolu
            group.Add(task);

            // Seřazení úkolů ve skupině podle data
            group.Sort((t0, t1) =>
            {
                return DateTime.Compare(t0.Date.HasValue ? t0.Date.Value : DateTimeExtensions.Today,
                                        t1.Date.HasValue ? t1.Date.Value : DateTimeExtensions.Today);
            });

            // Seřazení úkolů ve skupině podle stavu dokončení
            group.Sort((t0, t1) =>
            {
                return t0.IsComplete.CompareTo(t1.IsComplete);
            });
        }

        public void UpdateTask(TaskModel task)
        {
            // Odstranění úkolu ze skupin
            foreach (TaskKeyGroup group in this)
            {
                group.Remove(task);
            }

            // Nové vložení
            NewTask(task);
        }
    }
}
