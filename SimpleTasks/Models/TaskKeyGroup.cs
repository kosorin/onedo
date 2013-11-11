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
            TaskKeyGroup todayGroup = new TaskKeyGroup(AppResources.DateTodayText);
            TaskKeyGroup tomorrowGroup = new TaskKeyGroup(AppResources.DateTomorrowText);
            TaskKeyGroup thisWeekGroup = new TaskKeyGroup(AppResources.DateThisWeekText);
            TaskKeyGroup nextWeekGroup = new TaskKeyGroup(AppResources.DateNextWeekText);
            TaskKeyGroup laterGroup = new TaskKeyGroup(AppResources.DateLaterText);
            TaskKeyGroup completedGroup = new TaskKeyGroup(AppResources.DateCompletedText);

            groups.Add(todayGroup);
            groups.Add(tomorrowGroup);
            groups.Add(thisWeekGroup);
            groups.Add(nextWeekGroup);
            groups.Add(laterGroup);
            groups.Add(completedGroup);

            // Přidání úkolů do jednotlivých skupin
            if (items != null)
            {
                foreach (TaskModel task in items)
                {
                    if (task.IsComplete)
                        completedGroup.Add(task);
                    else if (task.DueDate <= DateTimeExtensions.Today)
                        todayGroup.Add(task);
                    else if (task.DueDate == DateTimeExtensions.Tomorrow)
                        tomorrowGroup.Add(task);
                    else if (task.DueDate > DateTimeExtensions.Tomorrow && task.DueDate <= DateTimeExtensions.LastDayOfWeek)
                        thisWeekGroup.Add(task);
                    else if (task.DueDate > DateTimeExtensions.LastDayOfWeek && task.DueDate <= DateTimeExtensions.LastDayOfNextWeek)
                        nextWeekGroup.Add(task);
                    else
                        laterGroup.Add(task);
                }
            }

            // Seřazení úkolů ve skupinách
            Comparison<TaskModel> comparison = (t1, t2) =>
            {
                if (t1 == null && t2 != null)
                    return 1;
                else if (t1 != null && t2 == null)
                    return -1;
                else if (t1 == null && t2 == null)
                    return 0;
                else
                {
                    int dateCompare = DateTime.Compare(t1.ReminderDate ?? DateTime.Now, t2.ReminderDate ?? DateTime.Now);
                    if (dateCompare == 0)
                    {
                        // Nižší priorita bude vždy první, proto pro výsledek obrátíme znaménko.
                        return -(t1.Priority.CompareTo(t2.Priority));
                    }
                    else
                    {
                        return dateCompare;
                    }
                }
            };

            todayGroup.Sort(comparison);
            tomorrowGroup.Sort(comparison);
            thisWeekGroup.Sort(comparison);
            nextWeekGroup.Sort(comparison);
            laterGroup.Sort(comparison);
            completedGroup.Sort(comparison);
            completedGroup.Sort((t1, t2) =>
            {
                // Dokončené úkoly mají vždy nastavený datum dokončení,
                // proto můžeme přistupovat přímo k hodnotě
                return DateTime.Compare(t1.ReminderDate ?? DateTime.Now, t2.ReminderDate ?? DateTime.Now);
            });

            return groups;
        }
    }
}
