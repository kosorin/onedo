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
        public static void SetSystemReminder(this TaskModel task)
        {
            if (task.HasReminder)
            {
                ReminderHelper.Add(
                    task.Uid,
                    task.Title,
                    task.Detail,
                    task.ReminderDate,
                    new Uri(string.Format("/Views/EditTaskPage.xaml?Task={0}", task.Uid), UriKind.Relative));
            }
        }

        public static Reminder GetSystemReminder(this TaskModel task)
        {
            return ReminderHelper.Get(task.Uid);
        }

        public static bool ExistsSystemReminder(this TaskModel task)
        {
            return ReminderHelper.Exists(task.Uid);
        }

        public static void RemoveSystemReminder(this TaskModel task)
        {
            ReminderHelper.Remove(task.Uid);
        }
    }
}
