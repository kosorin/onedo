using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.LocalizedResources;
using SimpleTasks.ViewModels;
using Microsoft.Phone.Shell;
using SimpleTasks.Resources;
using SimpleTasks.Controls;
using System.Diagnostics;
using SimpleTasks.Models;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace SimpleTasks.Views
{
    public partial class ReminderPickerPage : BasePickerPage
    {
        public ReminderPickerPage()
        {
            initReminder = TimeSpan.Zero;
            if (PhoneApplicationService.Current.State.ContainsKey("Reminder"))
            {
                initReminder = (PhoneApplicationService.Current.State["Reminder"] as TimeSpan?) ?? initReminder;
                PhoneApplicationService.Current.State.Remove("Reminder");
            }

            InitializeComponent();

            SetTimeSpan(initReminder);
            DataContext = this;
        }

        private TimeSpan initReminder;

        protected override void Save()
        {
            PhoneApplicationService.Current.State["Reminder"] = GetTimeSpan();
        }

        private void QuickButton_Click(object sender, RoutedEventArgs e)
        {
            SetTimeSpan(TimeSpan.Parse((string)((Button)sender).Tag));
        }

        private TimeSpan GetTimeSpan()
        {
            switch (LengthType)
            {
            case 2: return new TimeSpan(DaysSlider.RoundValue, 0, 0, 0);
            case 1: return new TimeSpan(HoursSlider.RoundValue, 0, 0);
            case 0:
            default: return new TimeSpan(0, MinutesSlider.RoundValue, 0);
            }
        }

        private void SetTimeSpan(TimeSpan ts)
        {
            int type;
            if (ts.Days != 0)
            {
                type = 2;

                SetSliders(type);
                LengthType = type;
                DaysSlider.SetSliderValue(ts.Days);
            }
            else if (ts.Hours != 0)
            {
                type = 1;

                SetSliders(type);
                LengthType = type;
                HoursSlider.SetSliderValue(ts.Hours);
            }
            else
            {
                type = 0;

                SetSliders(type);
                LengthType = type;
                MinutesSlider.SetSliderValue(ts.Minutes);
            }
        }

        private void SetSliders(int type)
        {
            switch (type)
            {
            case 2:
                DaysShow.Begin();
                HoursHide.Begin();
                MinutesHide.Begin();
                break;
            case 1:
                DaysHide.Begin();
                HoursShow.Begin();
                MinutesHide.Begin();
                break;
            case 0:
            default:
                DaysHide.Begin();
                HoursHide.Begin();
                MinutesShow.Begin();
                break;
            }
            OnPropertyChanged("ReminderValue");
        }

        public TimeSpan ReminderValue
        {
            get
            {
                TimeSpan ts = GetTimeSpan();
                BeforeTextVisibility = ts == TimeSpan.Zero ? Visibility.Collapsed : Visibility.Visible;
                return ts;
            }
        }

        private int _lengthType = 0;
        public int LengthType
        {
            get { return _lengthType; }
            set { SetProperty(ref _lengthType, value); }
        }

        private Visibility _beforeTextVisibility = Visibility.Collapsed;
        public Visibility BeforeTextVisibility
        {
            get { return _beforeTextVisibility; }
            set { SetProperty(ref _beforeTextVisibility, value); }
        }

        private List<ReminderLengthType> _lengthTypes = null;
        public List<ReminderLengthType> LengthTypes
        {
            get
            {
                if (_lengthTypes == null)
                {
                    _lengthTypes = new List<ReminderLengthType>()
                    {
                        new ReminderLengthType(AppResources.MinutesLabel + " " + AppResources.ReminderBeforeDueDateText, 0.82),
                        new ReminderLengthType(AppResources.HoursLabel + " " + AppResources.ReminderBeforeDueDateText, 0.70),
                        new ReminderLengthType(AppResources.DaysLabel + " " + AppResources.ReminderBeforeDueDateText, 0.58)
                    };
                }
                return _lengthTypes;
            }
        }

        private void LengthTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReminderLengthType type = TypePicker.SelectedItem as ReminderLengthType;
            if (type != null)
            {
                SetSliders(TypePicker.SelectedIndex);
            }
        }

        private void MinutesSlider_RoundValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            OnPropertyChanged("ReminderValue");
        }

        private void HoursSlider_RoundValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            OnPropertyChanged("ReminderValue");
        }

        private void DaysSlider_RoundValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            OnPropertyChanged("ReminderValue");
        }
    }
}