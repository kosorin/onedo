using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using SimpleTasks.ViewModels;
using Microsoft.Phone.Shell;
using SimpleTasks.Resources;
using SimpleTasks.Controls;

namespace SimpleTasks.Views
{
    public partial class DueTimePickerPage : BasePickerPage
    {
        public DueTimePickerPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("DueTime"))
            {
                initTime = (PhoneApplicationService.Current.State["DueTime"] as DateTime?) ?? DateTime.Now;
                PhoneApplicationService.Current.State.Remove("DueTime");
            }
            else
            {
                initTime = DateTime.Now;
            }

            DataContext = this;
        }

        private DateTime initTime;

        protected override void Save()
        {
            PhoneApplicationService.Current.State["DueTime"] = new DateTime(
                initTime.Year,
                initTime.Month,
                initTime.Day,
                TimePicker.Hours,
                TimePicker.Minutes,
                0);
        }
    }
}