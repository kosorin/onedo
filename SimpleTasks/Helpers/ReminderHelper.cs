using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Phone.Scheduler;
using SimpleTasks.Resources;

namespace SimpleTasks.Helpers
{
    public class ReminderHelper
    {
        public static void Add(string name, string title, string content, DateTime beginTime, Uri navigationUri, RecurrenceInterval interval = RecurrenceInterval.None)
        {
            Remove(name);
            if (interval == RecurrenceInterval.None && beginTime <= DateTime.Now)
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
                    RecurrenceType = interval
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
            if (Exists(name))
            {
                ScheduledActionService.Remove(name);
            }
        }

        public static List<Reminder> GetAll()
        {
            return ScheduledActionService.GetActions<Reminder>().ToList();
        }

        public static List<Reminder> GetAll(string name)
        {
            List<Reminder> reminders = new List<Reminder>();
            foreach (Reminder reminder in ScheduledActionService.GetActions<Reminder>())
            {
                try
                {
                    if (reminder.Name.StartsWith(name))
                    {
                        reminders.Add(reminder);
                    }
                }
                catch { }
            }
            return reminders;
        }

        public static void RemoveAll()
        {
            foreach (Reminder reminder in ScheduledActionService.GetActions<Reminder>())
            {
                try
                {
                    ScheduledActionService.Remove(reminder.Name);
                }
                catch { }
            }
        }

        public static void RemoveAll(string name)
        {
            foreach (Reminder reminder in ScheduledActionService.GetActions<Reminder>())
            {
                try
                {
                    if (reminder.Name.StartsWith(name))
                    {
                        ScheduledActionService.Remove(reminder.Name);
                    }
                }
                catch { }
            }
        }
    }
}
