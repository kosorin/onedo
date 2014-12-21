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

namespace SimpleTasks.Views
{
    public partial class RepeatsPickerPage : BasePickerPage
    {
        #region Static
        private static readonly Repeats DefaultRepeats = Repeats.None;

        public static string[] DayNames { get; private set; }

        private static readonly int DayCount = 7;

        private static readonly DayOfWeek FirstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

        static RepeatsPickerPage()
        {
            DayNames = new string[DayCount];
            DayNames[GetIndex(DayOfWeek.Monday)] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)DayOfWeek.Monday];
            DayNames[GetIndex(DayOfWeek.Tuesday)] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)DayOfWeek.Tuesday];
            DayNames[GetIndex(DayOfWeek.Wednesday)] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)DayOfWeek.Wednesday];
            DayNames[GetIndex(DayOfWeek.Thursday)] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)DayOfWeek.Thursday];
            DayNames[GetIndex(DayOfWeek.Friday)] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)DayOfWeek.Friday];
            DayNames[GetIndex(DayOfWeek.Saturday)] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)DayOfWeek.Saturday];
            DayNames[GetIndex(DayOfWeek.Sunday)] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)DayOfWeek.Sunday];
        }

        private static int GetIndex(DayOfWeek dayOfWeek)
        {
            return ((int)(dayOfWeek - FirstDayOfWeek) + 7) % DayCount;
        }
        #endregion // end of Static

        public RepeatsPickerPage()
            : base("RepeatsPicker")
        {
            InitializeComponent();
            DataContext = this;

            UpdateButtons(NavigationParameter<Repeats>(_name, DefaultRepeats));
        }

        private bool IsSetDayOfWeek(DayOfWeek dayOfWeek)
        {
            return ((ToggleButton)ButtonPanel.Children[GetIndex(dayOfWeek)]).IsChecked ?? false;
        }

        private void SetDayOfWeek(DayOfWeek dayOfWeek, bool value)
        {
            ((ToggleButton)ButtonPanel.Children[GetIndex(dayOfWeek)]).IsChecked = value;
        }

        protected override object Save()
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