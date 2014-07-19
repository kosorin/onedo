using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleTasks.Models
{
    public class TaskGroup : List<TaskWrapper>
    {
        public string Title { get; private set; }

        public TaskGroup(string title)
        {
            Title = title;
        }

        public static List<TaskGroup> CreateOrderByDate(IEnumerable<TaskModel> items)
        {
            List<TaskGroup> groups = new List<TaskGroup>();
            TaskGroup overdueGroup = new TaskGroup(AppResources.DateOverdue);
            TaskGroup todayGroup = new TaskGroup(AppResources.DateToday);
            TaskGroup tomorrowGroup = new TaskGroup(AppResources.DateTomorrow);
            TaskGroup upcomingGroup = new TaskGroup(AppResources.DateUpcoming);
            TaskGroup somedayGroup = new TaskGroup(AppResources.DateSomeday);
            TaskGroup completedGroup = new TaskGroup(AppResources.DateCompleted);

            groups.Add(overdueGroup);
            groups.Add(todayGroup);
            groups.Add(tomorrowGroup);
            groups.Add(upcomingGroup);
            groups.Add(somedayGroup);
            groups.Add(completedGroup);

            // Přidání úkolů do jednotlivých skupin
            if (items != null)
            {
                foreach (TaskModel task in items)
                {
                    TaskWrapper taskWrapper = new TaskWrapper(task);
                    if (task.IsComplete)
                        completedGroup.Add(taskWrapper);
                    else if (!task.DueDate.HasValue)
                        somedayGroup.Add(taskWrapper);
                    else if (task.DueDate.Value.Date < DateTimeExtensions.Today)
                        overdueGroup.Add(taskWrapper);
                    else if (task.DueDate.Value.Date == DateTimeExtensions.Today)
                        todayGroup.Add(taskWrapper);
                    else if (task.DueDate.Value.Date == DateTimeExtensions.Tomorrow)
                        tomorrowGroup.Add(taskWrapper);
                    else
                        upcomingGroup.Add(taskWrapper);
                }
            }

            // Seřazení úkolů ve skupinách
            overdueGroup.Sort((t1, t2) =>
            {
                return DateTime.Compare(t1.Task.DueDate.Value.Date, t2.Task.DueDate.Value.Date);
            });

            todayGroup.Sort((t1, t2) =>
            {
                return t2.Task.Priority.CompareTo(t1.Task.Priority);
            });

            tomorrowGroup.Sort((t1, t2) =>
            {
                return t2.Task.Priority.CompareTo(t1.Task.Priority);
            });

            upcomingGroup.Sort((t1, t2) =>
            {
                return DateTime.Compare(t1.Task.DueDate.Value.Date, t2.Task.DueDate.Value.Date);
            });

            somedayGroup.Sort((t1, t2) =>
            {
                return t2.Task.Priority.CompareTo(t1.Task.Priority);
            });

            completedGroup.Sort((t1, t2) =>
            {
                return DateTime.Compare(t2.Task.CompletedDate.Value.Date, t1.Task.CompletedDate.Value.Date);
            });

            return groups;
        }
        public static List<TaskGroup> CreateOrderByPriority(IEnumerable<TaskModel> items)
        {
            List<TaskGroup> groups = new List<TaskGroup>();
            TaskGroup lowGroup = new TaskGroup(AppResources.PriorityLow);
            TaskGroup normalGroup = new TaskGroup(AppResources.PriorityNormal);
            TaskGroup highGroup = new TaskGroup(AppResources.PriorityHigh);

            groups.Add(highGroup);
            groups.Add(normalGroup);
            groups.Add(lowGroup);

            // Přidání úkolů do jednotlivých skupin
            if (items != null)
            {
                foreach (TaskModel task in items)
                {
                    TaskWrapper taskWrapper = new TaskWrapper(task);
                    if (task.IsHighPriority)
                        highGroup.Add(taskWrapper);
                    else if (task.IsLowPriority)
                        lowGroup.Add(taskWrapper);
                    else
                        normalGroup.Add(taskWrapper);
                }
            }

            // Seřazení úkolů ve skupinách
            Comparison<TaskWrapper> dateComparison = (t1, t2) =>
            {
                if (t1.Task.IsComplete || t2.Task.IsComplete)
                {
                    return t1.Task.IsComplete && t2.Task.IsComplete ? 0 : (t1.Task.IsComplete ? 1 : -1);
                }
                if (!t1.Task.HasDueDate || !t2.Task.HasDueDate)
                {
                    return !t1.Task.HasDueDate && !t2.Task.HasDueDate ? 0 : (t1.Task.HasDueDate ? -1 : 1);
                }
                return DateTime.Compare(t1.Task.DueDate.Value.Date, t2.Task.DueDate.Value.Date);
            };

            lowGroup.Sort(dateComparison);
            normalGroup.Sort(dateComparison);
            highGroup.Sort(dateComparison);

            return groups;
        }

    }
}
