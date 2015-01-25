using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using SimpleTasks.Resources;
using SimpleTasks.Models;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using SimpleTasks.Controls;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Helpers;
using Microsoft.Phone.Scheduler;

namespace SimpleTasks.Views
{
    public partial class AboutBAPage : BasePage
    {
        public AboutBAPage()
        {
            InitializeComponent();
            DataContext = this;

            PeriodicTask task = ScheduledActionService.Find(App.BackgroundAgentName) as PeriodicTask;
            if (task != null)
            {
                Description = task.Description;
                IsScheduled = task.IsScheduled;
                ExpirationTime = task.ExpirationTime;
                LastExitReason = task.LastExitReason;
                LastScheduledTime = task.LastScheduledTime;
            }
        }

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }


        private bool _isScheduled = false;
        public bool IsScheduled
        {
            get { return _isScheduled; }
            set { SetProperty(ref _isScheduled, value); }
        }

        private DateTime _expirationTime = DateTime.MinValue;
        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
            set { SetProperty(ref _expirationTime, value); }
        }

        private AgentExitReason _lastExitReason = AgentExitReason.None;
        public AgentExitReason LastExitReason
        {
            get { return _lastExitReason; }
            set { SetProperty(ref _lastExitReason, value); }
        }

        private DateTime _lastScheduledTime = DateTime.MinValue;
        public DateTime LastScheduledTime
        {
            get { return _lastScheduledTime; }
            set { SetProperty(ref _lastScheduledTime, value); }
        }
    }
}