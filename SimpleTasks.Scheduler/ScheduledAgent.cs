using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Linq;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using System.IO.IsolatedStorage;

namespace SimpleTasks.Scheduler
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
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        protected override void OnInvoke(ScheduledTask task)
        {
            if (Debugger.IsAttached)
            {
                int seconds = 60;
                Debug.WriteLine("> ScheduledActionService.LaunchForTest: Pro test za {0} vteřin", seconds);
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(seconds));

                ShellToast toast = new ShellToast
                {
                    Content = task.LastScheduledTime.ToLongTimeString() + " " + DateTime.Now.ToLongTimeString(),
                    Title = string.Format("Live Tile, další za {0} vteřin", seconds),
                };
                toast.Show();
            }


            // Dlaždici budeme aktualizovat každý den jen jednou
            if (DateTime.Today.Date - task.LastScheduledTime.Date < TimeSpan.FromDays(1))
            {
                NotifyComplete();
                return;
            }

            // Získání dat ze souboru
            TaskCollection tasks = TaskCollection.LoadFromXmlFile(TaskCollection.DefaultDataFileName);

            // Aktualizace dlaždice
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                // Tohle se musí provést v UI vklákně!
                LiveTile.Update(tasks);

                NotifyComplete();
            });
        }
    }
}