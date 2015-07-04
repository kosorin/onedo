using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SimpleTasks.Controls;
using SimpleTasks.Resources;
using ReminderLengthType = SimpleTasks.Controls.ListPickerItem<int, double>;

namespace SimpleTasks.Views
{
    public partial class ReminderPickerPage : BasePickerPage
    {
        private readonly TimeSpan _defaultReminder = TimeSpan.Zero;
        private TimeSpan _reminder;

        public ReminderPickerPage()
            : base("ReminderPicker")
        {
            _reminder = NavigationParameter<TimeSpan?>(_name, null) ?? _defaultReminder;

            InitializeComponent();
            SetTimeSpan(_reminder);
            DataContext = this;
        }

        protected override object Save()
        {
            return GetTimeSpan();
        }

        #region Private methods
        private TimeSpan GetTimeSpan()
        {
            int value = (int)Math.Round(ReminderSlider.Value);
            switch (LengthTypes.FindIndex(t => t == LengthType))
            {
            case 2: return new TimeSpan(ConvertBackDays(value), 0, 0, 0);
            case 1: return new TimeSpan(value, 0, 0);
            case 0:
            default: return new TimeSpan(0, value, 0);
            }
        }

        private void SetTimeSpan(TimeSpan ts)
        {
            int type;
            int value;
            if (ts.Days != 0)
            {
                type = 2;
                value = ConvertDays(ts.Days);
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
            ReminderSlider.Value = (double)value;
        }

        private int ConvertDays(int value)
        {
            if (value == 0) return 0;
            if (value == 1) return 2;
            if (value == 2) return 4;
            if (value == 3) return 6;
            if (value == 4) return 7;
            if (value == 5) return 8;
            if (value == 6) return 9;

            if (value >= 7 && value < 14) return 10;
            if (value >= 14 && value < 21) return 12;
            if (value >= 21 && value < 28) return 14;
            if (value >= 28 && value < 35) return 16;
            return 0;
        }

        private int ConvertBackDays(int value)
        {
            if (value == 0) return 0;
            if (value >= 1 && value <= 3) return 1;
            if (value == 4 || value == 5) return 2;
            if (value == 6) return 3;
            if (value == 7) return 4;
            if (value == 8) return 5;
            if (value == 9) return 6;

            if (value == 10 || value == 11) return 7;
            if (value == 12 || value == 13) return 14;
            if (value == 14 || value == 15) return 21;
            if (value == 16 || value == 17) return 30;
            return 0;
        }

        private void SetSliders(int type)
        {
            LengthType = LengthTypes[type];
            ReminderSlider.Maximum = (double)LengthTypes[type].Value1;
            OnPropertyChanged("ReminderValue");
        }
        #endregion

        public TimeSpan ReminderValue
        {
            get
            {
                TimeSpan ts = GetTimeSpan();
                BeforeTextVisibility = ts == TimeSpan.Zero ? Visibility.Collapsed : Visibility.Visible;
                return ts;
            }
        }

        private Visibility _beforeTextVisibility = Visibility.Collapsed;
        public Visibility BeforeTextVisibility
        {
            get { return _beforeTextVisibility; }
            set { SetProperty(ref _beforeTextVisibility, value); }
        }

        #region LengthType
        private ReminderLengthType _lengthType = null;
        public ReminderLengthType LengthType
        {
            get { return _lengthType; }
            set { SetProperty(ref _lengthType, value); }
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
                        (LengthType = new ReminderLengthType(AppResources.MinutesLabel, 60, 0.14)),
                        new ReminderLengthType(AppResources.HoursLabel, 24, 0.22),
                        new ReminderLengthType(AppResources.DaysLabel, 17, 0.28)
                    };
                }
                return _lengthTypes;
            }
        }
        #endregion

        #region Event handlers
        private void LengthTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReminderLengthType type = TypePicker.SelectedItem as ReminderLengthType;
            if (type != null)
            {
                SetSliders(TypePicker.SelectedIndex);
            }
        }

        private void QuickButton_Click(object sender, RoutedEventArgs e)
        {
            SetTimeSpan(TimeSpan.Parse((string)((Button)sender).Tag));
        }

        private void ReminderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnPropertyChanged("ReminderValue");
        }
        #endregion
    }
}