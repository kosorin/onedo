using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using System;

namespace SimpleTasks.BackgroundAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        static ScheduledAgent()
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
        protected override void OnInvoke(ScheduledTask scheduledTask)
        {
            Debug.WriteLine(">>> BACKGROUND AGENT <<<");
#if DEBUG
            ScheduledActionService.LaunchForTest(scheduledTask.Name, TimeSpan.FromSeconds(65));
            Debug.WriteLine("> LAUNCH FOR TEST");
#endif

            // Aktualizace každý den 1x
            if (scheduledTask.LastScheduledTime.Date != DateTime.Today)
            {
                Debug.WriteLine(">>> BACKGROUND AGENT: UPDATE <<<");

                Settings.Current = Settings.LoadFromFile("Settings.json");
                TaskCollection tasks = TaskCollection.LoadFromFile("Tasks.json");
                if (tasks != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        LiveTile.UpdateOrReset(Settings.Current.Tiles.Enable, tasks);
                        foreach (TaskModel task in tasks)
                        {
                            if (task.Repeats != Repeats.None)
                            {
                                LiveTile.Update(task);
                            }
                        }
                        Debug.WriteLine(">>> BACKGROUND AGENT: DONE <<<");
                        NotifyComplete();
                    });
                }
                else
                {
                    Debug.WriteLine(">>> BACKGROUND AGENT: no tasks <<<");
                    NotifyComplete();
                }
            }
            else
            {
                Debug.WriteLine(">>> BACKGROUND AGENT: PASS <<<");
                NotifyComplete();
            }
        }
    }
}