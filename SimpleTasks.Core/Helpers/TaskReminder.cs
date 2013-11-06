using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Helpers
{
    public class TaskReminder
    {
        public static void Add(TaskModel task)
        {
            Remove(task);

            if (task.ReminderDate != null)
            {
                Reminder reminder = new Reminder(task.Uid)
                {
                    BeginTime = task.ReminderDate.Value,
                    Title = "Připomenutí úkolu",
                    Content = task.Title,
                };

                try
                {
                    ScheduledActionService.Add(reminder);
                }
                catch (Exception)
                {
                    task.ReminderDate = null;
                }
            }
        }

        public static void Remove(TaskModel task)
        {
            ScheduledAction reminder = ScheduledActionService.Find(task.Uid);
            if (reminder != null)
            {
                ScheduledActionService.Remove(reminder.Name);
            }
        }
    }
}
