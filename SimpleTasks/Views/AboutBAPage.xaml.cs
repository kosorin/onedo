using System;
using Microsoft.Phone.Scheduler;
using SimpleTasks.Controls;

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