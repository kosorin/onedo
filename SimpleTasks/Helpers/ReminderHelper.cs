using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
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
        public static void Add(string name, string title, string content, DateTime beginTime, Uri navigationUri)
        {
            Remove(name);
            if (beginTime <= DateTime.Now)
            {
                Debug.WriteLine("> Reminder Add: overdue: {0} <= {1}", beginTime, DateTime.Now);
                return;
            }

            try
            {
                const int titleMaxLength = 63;
                const int contentMaxLength = 255;

                // Není povolena nulová délka
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = AppResources.TitleUntitled;
                }

                // Maximum je 63 znaků
                if (title.Length > titleMaxLength)
                {
                    title = title.Substring(0, titleMaxLength - 3) + "...";
                }

                // Maximum je 255 znaků
                if (content.Length > contentMaxLength)
                {
                    content = content.Substring(0, contentMaxLength - 3) + "...";
                }

                ScheduledActionService.Add(new Reminder(name)
                {
                    BeginTime = beginTime,
                    Title = title,
                    Content = content,
                    NavigationUri = navigationUri,
                });
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine("> Add Reminder (InvalidOperationException): {0}", e.Message);
            }
            catch (SchedulerServiceException e)
            {
                Debug.WriteLine("> Add Reminder (SchedulerServiceException): {0}", e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine("> Add Reminder (Exception): {0}", e.Message);
            }
        }

        public static Reminder Get(string name)
        {
            return ScheduledActionService.Find(name) as Reminder;
        }

        public static bool Exists(string name)
        {
            return ScheduledActionService.Find(name) != null;
        }

        public static void Remove(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (InvalidOperationException) { }
        }
    }
}
