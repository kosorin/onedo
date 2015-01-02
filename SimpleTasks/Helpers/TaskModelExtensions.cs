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
    public static class TaskModelExtensions
    {
        private static string ToReminderName(string uid, DateTime date)
        {
            return string.Format("{0}#{1}", uid, (int)date.DayOfWeek);
        }

        public static void SetSystemReminder(this TaskModel task)
        {
            if (task.HasReminder)
            {
                List<DateTime> dates = task.ReminderDates();
                RecurrenceInterval interval = task.Repeats.Interval();

                foreach (DateTime date in dates)
                {
                    ReminderHelper.Add(
                        ToReminderName(task.Uid, date),
                        task.Title,
                        task.Detail,
                        date,
                        new Uri(string.Format("/Views/EditTaskPage.xaml?Task={0}", task.Uid), UriKind.Relative),
                        interval);
                }
            }
        }

        public static List<Reminder> GetSystemReminder(this TaskModel task)
        {
            return ReminderHelper.GetAll(task.Uid);
        }

        public static bool ExistsSystemReminder(this TaskModel task)
        {
            return ReminderHelper.GetAll(task.Uid).Count > 0;
        }

        public static void RemoveSystemReminder(this TaskModel task)
        {
            ReminderHelper.RemoveAll(task.Uid);
        }
    }
}
