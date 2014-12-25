using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Helpers
{
    public static class TaskModelExtensions
    {
        private static List<string> ReminderNames(string uid)
        {
            const int dayCount = 7;
            List<string> names = new List<string>();
            for (int i = 0; i < dayCount; i++)
            {
                names.Add(string.Format("{0}_{1}", uid, i));
            }
            return names;
        }

        private static string ToReminderName(string uid, DateTime date)
        {
            return string.Format("{0}_{1}", uid, (int)date.DayOfWeek);
        }

        public static void SetSystemReminder(this TaskModel task)
        {
            if (task.HasReminder)
            {
                bool weekly = (task.Repeats != Repeats.None);
                foreach (DateTime date in task.ReminderDates)
                {
                    ReminderHelper.Add(
                        ToReminderName(task.Uid, date),
                        task.Title,
                        task.Detail,
                        date,
                        new Uri(string.Format("/Views/EditTaskPage.xaml?Task={0}", task.Uid), UriKind.Relative),
                        weekly);
                }
            }
        }

        public static List<Reminder> GetSystemReminder(this TaskModel task)
        {
            List<Reminder> reminders = new List<Reminder>();
            foreach (string name in ReminderNames(task.Uid))
            {
                Reminder reminder = ReminderHelper.Get(name);
                if (reminder != null)
                {
                    reminders.Add(reminder);
                }
            }
            return reminders;
        }

        public static bool ExistsSystemReminder(this TaskModel task)
        {
            foreach (string name in ReminderNames(task.Uid))
            {
                if (ReminderHelper.Exists(name))
                {
                    return true;
                }
            }
            return false;
        }

        public static void RemoveSystemReminder(this TaskModel task)
        {
            foreach (string name in ReminderNames(task.Uid))
            {
                ReminderHelper.Remove(name);
            }
        }
    }
}
