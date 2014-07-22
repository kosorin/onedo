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
            Debug.WriteLine("LT: {0}", LengthType.Label);
            switch (LengthTypes.FindIndex(t => t == LengthType))
            {
            case 2: return new TimeSpan(ReminderSlider.RoundValue, 0, 0, 0);
            case 1: return new TimeSpan(ReminderSlider.RoundValue, 0, 0);
            case 0:
            default: return new TimeSpan(0, ReminderSlider.RoundValue, 0);
            }
        }

        private void SetTimeSpan(TimeSpan ts)
        {
            int type;
            int value;
            if (ts.Days != 0)
            {
                type = 2;
                value = ts.Days;
            }
            else if (ts.Hours != 0)
            {
                type = 1;
                value = ts.Hours;
            }
            else
            {
                type = 0;
                value = ts.Minutes;
            }

            SetSliders(type);
            ReminderSlider.SetSliderValue(value);
        }

        private void SetSliders(int type)
        {
            LengthType = LengthTypes[type];
            ReminderSlider.SetMaximum(LengthTypes[type].Maximum);
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

        private ReminderLengthType _lengthType = null;
        public ReminderLengthType LengthType
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
                        (LengthType = new ReminderLengthType(60, AppResources.MinutesLabel, 0.82)),
                        new ReminderLengthType(24, AppResources.HoursLabel, 0.70),
                        new ReminderLengthType(30, AppResources.DaysLabel, 0.58)
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

        private void ReminderSlider_RoundValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
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