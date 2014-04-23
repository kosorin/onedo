using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleTasks.Models
{
    public class TaskKeyGroup : List<TaskModel>
    {
        public string Title { get; private set; }

        public TaskKeyGroup(string title)
        {
            Title = title;
        }

        public static List<TaskKeyGroup> CreateGroups(IEnumerable<TaskModel> items)
        {
            List<TaskKeyGroup> groups = new List<TaskKeyGroup>();
            TaskKeyGroup overdueGroup = new TaskKeyGroup(AppResources.DateOverdue);
            TaskKeyGroup todayGroup = new TaskKeyGroup(AppResources.DateToday);
            TaskKeyGroup tomorrowGroup = new TaskKeyGroup(AppResources.DateTomorrow);
            TaskKeyGroup upcomingGroup = new TaskKeyGroup(AppResources.DateUpcoming);
            TaskKeyGroup somedayGroup = new TaskKeyGroup(AppResources.DateSomeday);
            TaskKeyGroup completedGroup = new TaskKeyGroup(AppResources.DateCompleted);

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
                    if (task.IsComplete)
                        completedGroup.Add(task);
                    else if (!task.DueDate.HasValue)
                        somedayGroup.Add(task);
                    else if (task.DueDate < DateTimeExtensions.Today)
                        overdueGroup.Add(task);
                    else if (task.DueDate == DateTimeExtensions.Today)
                        todayGroup.Add(task);
                    else if (task.DueDate == DateTimeExtensions.Tomorrow)
                        tomorrowGroup.Add(task);
                    else
                        upcomingGroup.Add(task);
                }
            }

            // Seřazení úkolů ve skupinách
            overdueGroup.Sort((t1, t2) =>
            {
                return DateTime.Compare(t1.DueDate.Value, t2.DueDate.Value);
            });

            todayGroup.Sort((t1, t2) =>
            {
                return t2.Priority.CompareTo(t1.Priority);
            });

            tomorrowGroup.Sort((t1, t2) =>
            {
                return t2.Priority.CompareTo(t1.Priority);
            });

            upcomingGroup.Sort((t1, t2) =>
            {
                return DateTime.Compare(t1.DueDate.Value, t2.DueDate.Value);
            });

            somedayGroup.Sort((t1, t2) =>
            {
                return t2.Priority.CompareTo(t1.Priority);
            });

            completedGroup.Sort((t1, t2) =>
            {
                return DateTime.Compare(t2.CompletedDate.Value, t1.CompletedDate.Value);
            });

            return groups;
        }
    }
}
