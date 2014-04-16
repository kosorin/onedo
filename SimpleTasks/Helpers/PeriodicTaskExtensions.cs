using Microsoft.Phone.Scheduler;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Helpers
{
    public class PeriodicTaskExtensions
    {
        private const string PeriodicTaskName = "PeriodicTask";

        public static void StartOrStop(bool start)
        {
            if (start)
                Start();
            else
                Stop();
        }

        public static void Start()
        {
            PeriodicTask periodicTask = new PeriodicTask(PeriodicTaskName)
            {
                Description = AppResources.PeriodicTaskDescription
            };

            // Odstraním starou úlohu
            Stop();

            // Přidání úlohy.
            try
            {
                ScheduledActionService.Add(periodicTask);
                Debug.WriteLine("> Přidal jsem PeriodicTask: {0}", periodicTask.Name);
                if (Debugger.IsAttached)
                {
                    int seconds = 10;
                    Debug.WriteLine("> ScheduledActionService.LaunchForTest: Pro test za {0} vteřin", seconds);
                    ScheduledActionService.LaunchForTest(periodicTask.Name, TimeSpan.FromSeconds(seconds));
                }
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.Contains("BNS Error: The action is disabled"))
                    Debug.WriteLine("ScheduledActionService Start InvalidOperationException: Úloha byla zakázána uživatelem.");
                else if (e.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                    Debug.WriteLine("ScheduledActionService Start InvalidOperationException: Dosažen maximální limit úloh.");
                else
                    Debug.WriteLine("ScheduledActionService Start InvalidOperationException: " + e.Message);
            }
            catch (SchedulerServiceException e)
            {
                Debug.WriteLine("ScheduledActionService Start SchedulerServiceException: " + e.Message);
            }
        }

        public static void Stop()
        {
            foreach (PeriodicTask task in ScheduledActionService.GetActions<PeriodicTask>())
            {
                try
                {
                    ScheduledActionService.Remove(task.Name);
                    Debug.WriteLine("> Odstranil jsem PeriodicTask: {0}", task.Name);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("ScheduledActionService Remove Exception: " + e.Message);
                }
            }
        }
    }
}
