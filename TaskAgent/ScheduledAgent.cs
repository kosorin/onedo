﻿using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using SimpleTasks.Models;
using System;
using SimpleTasks.Helpers;

namespace TaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            // Dlaždici budeme aktualizovat každý den jen jednou
            if (DateTimeExtensions.Today.Date - task.LastScheduledTime.Date < TimeSpan.FromDays(1))
            {
                NotifyComplete();
                return;
            }

            // Získání dat ze souboru
            TaskModelCollection tasks = TaskModelCollection.LoadTasksFromXmlFile();

            // Aktualizace dlaždice
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                // Tohle se musí provést v UI vklákně!
                LiveTile.UpdateTiles(tasks);

                NotifyComplete();
            });
        }
    }
}