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
using SimpleTasks.Core.Models;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Collections.Generic;

namespace SimpleTasks.Views
{
    public partial class RepeatsPickerPage : BasePickerPage
    {
        #region Static
        private static readonly Repeats DefaultRepeats = Repeats.None;

        public static string[] DayNames { get; private set; }

        private static readonly int DayCount = 7;

        static RepeatsPickerPage()
        {
            DayNames = new string[DayCount];
            DayNames[(int)DayOfWeek.Monday] = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)DayOfWeek.Monday];
            DayNames[(int)DayOfWeek.Tuesday] = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)DayOfWeek.Tuesday];
            DayNames[(int)DayOfWeek.Wednesday] = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)DayOfWeek.Wednesday];
            DayNames[(int)DayOfWeek.Thursday] = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)DayOfWeek.Thursday];
            DayNames[(int)DayOfWeek.Friday] = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)DayOfWeek.Friday];
            DayNames[(int)DayOfWeek.Saturday] = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)DayOfWeek.Saturday];
            DayNames[(int)DayOfWeek.Sunday] = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)DayOfWeek.Sunday];
        }
        #endregion // end of Static

        public RepeatsPickerPage()
            : base("RepeatsPicker")
        {
            InitializeComponent();
            DataContext = this;

            Repeats repeats = NavigationParameter<Repeats>(_name, DefaultRepeats);
            switch (repeats)
            {
            case Repeats.Monthly:
                MonthlyRadioButton.IsChecked = true;
                break;

            case Repeats.Weekly:
                WeeklyRadioButton.IsChecked = true;
                break;

            case Repeats.Daily:
                DailyRadioButton.IsChecked = true;
                break;

            case Repeats.None:
                OnceRadioButton.IsChecked = true;
                break;

            default: // DaysOfWeek
                DaysOfWeekRadioButton.IsChecked = true;
                UpdateButtons(repeats);
                break;
            }
        }

        private bool IsSetDayOfWeek(DayOfWeek dayOfWeek)
        {
            return ((ToggleButton)ButtonPanel.Children[(int)dayOfWeek]).IsChecked ?? false;
        }

        private void SetDayOfWeek(DayOfWeek dayOfWeek, bool value)
        {
            ((ToggleButton)ButtonPanel.Children[(int)dayOfWeek]).IsChecked = value;
        }

        protected override object Save()
        {
            if (DailyRadioButton.IsChecked ?? false)
            {
                return Repeats.Daily;
            }
            else if (DaysOfWeekRadioButton.IsChecked ?? false)
            {
                Repeats repeats = Repeats.None;
                if (IsSetDayOfWeek(DayOfWeek.Monday)) repeats |= Repeats.Monday;
                if (IsSetDayOfWeek(DayOfWeek.Tuesday)) repeats |= Repeats.Tuesday;
                if (IsSetDayOfWeek(DayOfWeek.Wednesday)) repeats |= Repeats.Wednesday;
                if (IsSetDayOfWeek(DayOfWeek.Thursday)) repeats |= Repeats.Thursday;
                if (IsSetDayOfWeek(DayOfWeek.Friday)) repeats |= Repeats.Friday;
                if (IsSetDayOfWeek(DayOfWeek.Saturday)) repeats |= Repeats.Saturday;
                if (IsSetDayOfWeek(DayOfWeek.Sunday)) repeats |= Repeats.Sunday;
                return repeats;
            }
            else if (WeeklyRadioButton.IsChecked ?? false)
            {
                return Repeats.Weekly;
            }
            else if (MonthlyRadioButton.IsChecked ?? false)
            {
                return Repeats.Monthly;
            }

            return Repeats.None;
        }

        private void UpdateButtons(Repeats repeats)
        {
            SetDayOfWeek(DayOfWeek.Monday, (repeats & Repeats.Monday) != 0);
            SetDayOfWeek(DayOfWeek.Tuesday, (repeats & Repeats.Tuesday) != 0);
            SetDayOfWeek(DayOfWeek.Wednesday, (repeats & Repeats.Wednesday) != 0);
            SetDayOfWeek(DayOfWeek.Thursday, (repeats & Repeats.Thursday) != 0);
            SetDayOfWeek(DayOfWeek.Friday, (repeats & Repeats.Friday) != 0);
            SetDayOfWeek(DayOfWeek.Saturday, (repeats & Repeats.Saturday) != 0);
            SetDayOfWeek(DayOfWeek.Sunday, (repeats & Repeats.Sunday) != 0);
        }
    }
}