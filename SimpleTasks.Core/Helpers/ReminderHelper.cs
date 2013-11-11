using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Helpers
{
    public class ReminderHelper
    {
        public static void Add(string name, string title, string content, DateTime beginTime)
        {
            Remove(name);

            Reminder reminder = new Reminder(name)
            {
                BeginTime = beginTime,
                Title = title,
                Content = content,
            };

            try
            {
                ScheduledActionService.Add(reminder);
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
    }
}
