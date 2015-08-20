using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Info;
using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;

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

        private static void ShowMemoryUsage()
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                Debug.WriteLine("> MEM. LIMIT: {0:0.00} MB", DeviceStatus.ApplicationMemoryUsageLimit / 1000000.0);
                Debug.WriteLine("> MEM. USAGE: {0:0.00} MB", DeviceStatus.ApplicationCurrentMemoryUsage / 1000000.0);
            }
#endif
        }

        protected override void OnInvoke(ScheduledTask scheduledTask)
        {
            Debug.WriteLine(">>> BACKGROUND AGENT <<<");
#if DEBUG
            if (Debugger.IsAttached)
            {
                ScheduledActionService.LaunchForTest(scheduledTask.Name, TimeSpan.FromSeconds(65));
                Debug.WriteLine("> LAUNCH FOR TEST");
            }
#endif

            // Aktualizace každý den 1x
            if (scheduledTask.LastScheduledTime.Date != DateTime.Today)
            {
                Debug.WriteLine(">>> BACKGROUND AGENT: UPDATE <<<");

                Settings.Current = Settings.LoadFromFile(AppInfo.SettingsFileName);
                if (Settings.Current.EnableTile)
                {
                    TaskCollection tasks = TaskCollection.LoadFromFile(AppInfo.TasksFileName);
                    if (tasks != null)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            LiveTile.Update(tasks);
                            foreach (TaskModel task in tasks)
                            {
                                if (task.HasDueDate)
                                {
                                    DateTime date = task.ActualDueDate.Value.Date;
                                    if (date == DateTimeExtensions.Today)
                                    //if (date >= DateTimeExtensions.Yesterday && date <= DateTimeExtensions.Tomorrow)
                                    {
                                        LiveTile.Update(task);
                                    }
                                }
                            }
                            Debug.WriteLine(">>> BACKGROUND AGENT: DONE <<<");
                            ShowMemoryUsage();
                            NotifyComplete();
                        });
                    }
                    else
                    {
                        Debug.WriteLine(">>> BACKGROUND AGENT: no tasks <<<");
                        ShowMemoryUsage();
                        NotifyComplete();
                    }
                }
            }
            else
            {
                Debug.WriteLine(">>> BACKGROUND AGENT: PASS <<<");
                ShowMemoryUsage();
                NotifyComplete();
            }
        }
    }
}