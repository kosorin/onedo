using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Helpers
{
    public class TaskReminder
    {
        public static void Add(string name, string content, DateTime? reminderDateTime)
        {
            Remove(name);

            if (reminderDateTime != null)
            {
                Reminder reminder = new Reminder(name)
                {
                    BeginTime = reminderDateTime.Value,
                    Title = "Připomenutí úkolu",
                    Content = content
                };
                ScheduledActionService.Add(reminder);
            }
        }

        public static void Remove(string name)
        {
            ScheduledAction reminder = ScheduledActionService.Find(name);
            if (reminder != null)
            {
                ScheduledActionService.Remove(reminder.Name);
            }
        }
    }
}
