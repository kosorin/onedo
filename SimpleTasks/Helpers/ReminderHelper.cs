using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Helpers
{
    public class ReminderHelper
    {
        public static void Add(TaskModel task)
        {
            if (task.ReminderDate.HasValue)
            {
                Add(task.Uid,
                    task.Title,
                    task.Detail,
                    task.ReminderDate.Value,
                    new Uri(string.Format("/Views/EditTaskPage.xaml?Task={0}", task.Uid), UriKind.Relative));
            }
        }

        public static void Add(string name, string title, string content, DateTime beginTime, Uri navigationUri)
        {
            Remove(name);
            try
            {
                ScheduledActionService.Add(new Reminder(name)
                {
                    BeginTime = beginTime,
                    Title = title,
                    Content = content,
                    NavigationUri = navigationUri,
                });
            }
            catch (Exception)
            {
                Debug.WriteLine("Chyba při přidání připomínky.");
            }
        }

        public static bool Exists(string name)
        {
            return ScheduledActionService.Find(name) != null;
        }

        public static void Remove(string name)
        {
            if (Exists(name))
            {
                ScheduledActionService.Remove(name);
            }
        }

        public static void RemoveAll()
        {
            var remiders = ScheduledActionService.GetActions<Reminder>();
            foreach (Reminder reminder in remiders)
            {
                Debug.WriteLine("  mažu ---- " + reminder.Name);
                ScheduledActionService.Remove(reminder.Name);
            }
        }
    }
}
