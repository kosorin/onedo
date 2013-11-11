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
            // Dlaždici budeme aktualizovat každý den jen jednou
            if (DateTime.Today.Date - task.LastScheduledTime.Date < TimeSpan.FromDays(1))
            {
                NotifyComplete();
                return;
            }

            //{
            //    if (Debugger.IsAttached)
            //    {
            //        ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromMinutes(1));
            //    }

            //    ShellToast toast = new ShellToast
            //    {
            //        Content = task.LastScheduledTime.ToLongTimeString() + " " + DateTime.Now.ToLongTimeString(),
            //        Title = "Live Tile",
            //    };

            //    toast.Show();
            //}

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