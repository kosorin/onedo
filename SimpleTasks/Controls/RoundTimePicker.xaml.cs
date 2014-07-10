using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Diagnostics;

namespace SimpleTasks.Controls
{
    /// <summary>
    /// Vlastnosti s možností zápisu: 
    ///     DefaultHoursAnimationDuration
    ///     DefaultMinutesAnimationDuration
    /// </summary>
    public partial class RoundTimePicker : UserControl, INotifyPropertyChanged
    {
        #region Quadrants
        private enum Quadrants { NorthWest = 2, NorthEast = 1, SouthWest = 4, SouthEast = 3 }

        private Quadrants lastHourQuadrant;
        #endregion

        #region Public Methods
        public RoundTimePicker()
        {
            InitializeComponent();

            DefaultHoursAnimationDuration = DurationFromSeconds(0.15);
            DefaultMinutesAnimationDuration = DurationFromSeconds(0.18);

            Time = DateTime.Now;
            lastHourQuadrant = QuadrantFromAngle((Time.Hour % 12) * 30);
            Animate(DurationFromSeconds(0.65));

            DataContext = this;
        }

        public void SetTime(DateTime time)
        {
            Time = time;
            Animate();
        }

        public void SetTime(int hours, int minutes)
        {
            Hours = hours;
            Minutes = minutes;
            Animate();
        }

        public void SetHours(int hours)
        {
            Hours = hours;
            Animate();
        }

        public void SetMinutes(int minutes)
        {
            Minutes = minutes;
            Animate();
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        public DateTime Time
        {
            get { return new DateTime(1, 1, 1, Hours, Minutes, 0); }
            private set
            {
                Hours = value.Hour;
                Minutes = value.Minute;
                RaisePropertyChanged("Time");
            }
        }

        public int Hours
        {
            get
            {
                return (IsAm ? Hours12 : Hours12 + 12) % 24;
            }
            private set
            {
                Hours12 = value % 12;
                IsAm = value < 12;
                RaisePropertyChanged("Time");
            }
        }

        private int _minutes = default(int);
        public int Minutes
        {
            get { return _minutes % 60; }
            private set
            {
                SetProperty(ref _minutes, value % 60);
                RaisePropertyChanged("Time");
            }
        }

        private bool? _is24Format = null;
        public bool Is24Format
        {
            get
            { 
                return _is24Format.HasValue ? _is24Format.Value : (_is24Format = !CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.Contains("tt")).Value;
            }
            set
            {
                SetProperty(ref _is24Format, value);
                RaisePropertyChanged("Time");
            }
        }
        #endregion

        #region Properties for UI

        #region Hours
        private double _hoursAngle = default(double);
        public double HoursAngle
        {
            get { return _hoursAngle; }
            private set
            {
                SetProperty(ref _hoursAngle, value);
                Hours12 = HoursFromAngle(value);
            }
        }

        private double _hoursAngleAnimateFrom = default(double);
        public double HoursAngleAnimateFrom
        {
            get { return _hoursAngleAnimateFrom; }
            private set { SetProperty(ref _hoursAngleAnimateFrom, value); }
        }

        private double _hoursAngleAnimateTo = default(double);
        public double HoursAngleAnimateTo
        {
            get { return _hoursAngleAnimateTo; }
            private set { SetProperty(ref _hoursAngleAnimateTo, value); }
        }

        private Duration _defaultHoursAnimationDuration;
        public Duration DefaultHoursAnimationDuration
        {
            get { return _defaultHoursAnimationDuration; }
            set { SetProperty(ref _defaultHoursAnimationDuration, value); }
        }

        private Duration _currentHoursAnimationDuration;
        public Duration CurrentHoursAnimationDuration
        {
            get { return _currentHoursAnimationDuration; }
            private set { SetProperty(ref _currentHoursAnimationDuration, value); }
        }
        #endregion

        #region Minutes
        private double _minutesAngle = default(double);
        public double MinutesAngle
        {
            get { return _minutesAngle; }
            private set
            {
                SetProperty(ref _minutesAngle, value);
                Minutes = MinutesFromAngle(value);
            }
        }

        private double _minutesAngleAnimateFrom = default(double);
        public double MinutesAngleAnimateFrom
        {
            get { return _minutesAngleAnimateFrom; }
            private set { SetProperty(ref _minutesAngleAnimateFrom, value); }
        }

        private double _minutesAngleAnimateTo = default(double);
        public double MinutesAngleAnimateTo
        {
            get { return _minutesAngleAnimateTo; }
            private set { SetProperty(ref _minutesAngleAnimateTo, value); }
        }

        private Duration _defaultMinutesAnimationDuration;
        public Duration DefaultMinutesAnimationDuration
        {
            get { return _defaultMinutesAnimationDuration; }
            set { SetProperty(ref _defaultMinutesAnimationDuration, value); }
        }

        private Duration _currentMinutesAnimationDuration;
        public Duration CurrentMinutesAnimationDuration
        {
            get { return _currentMinutesAnimationDuration; }
            private set { SetProperty(ref _currentMinutesAnimationDuration, value); }
        }
        #endregion

        private bool _isAm;
        public bool IsAm
        {
            get { return _isAm; }
            private set
            {
                SetProperty(ref _isAm, value);
                RaisePropertyChanged("Hours");
            }
        }

        private int _hours12 = default(int);
        public int Hours12
        {
            get { return _hours12; }
            private set
            {
                SetProperty(ref _hours12, value);
                RaisePropertyChanged("Hours");
                RaisePropertyChanged("Time");
                RaisePropertyChanged("HoursForeground");
                RaisePropertyChanged("HoursBackground");
            }
        }

        public SolidColorBrush HoursForeground
        {
            get
            {
                bool isZeroAngle = HoursAngle == 0 || HoursAngle == 360;
                if ((Hours > 0 && Hours < 12)
                    || (Hours == 0 && (isZeroAngle || HoursAngle < 180))
                    || (Hours == 12 && (!isZeroAngle && HoursAngle > 180)))
                {
                    AmOrPm.Text = "AM";
                    return Resources["PhoneAccentBrush07"] as SolidColorBrush;
                }
                else
                {
                    AmOrPm.Text = "PM";
                    return Resources["PhoneAccentBrush"] as SolidColorBrush;
                }
            }
        }

        public SolidColorBrush HoursBackground
        {
            get
            {
                bool isZeroAngle = HoursAngle == 0 || HoursAngle == 360;
                if ((Hours > 0 && Hours < 12)
                    || (Hours == 0 && (isZeroAngle || HoursAngle < 180))
                    || (Hours == 12 && (!isZeroAngle && HoursAngle > 180)))
                    return Resources["PhoneAccentBrush03"] as SolidColorBrush;
                else
                    return Resources["PhoneAccentBrush08"] as SolidColorBrush;
            }
        }
        #endregion

        #region Event Handlers

        #region Hours
        private void OnManipulationStartedHours(object sender, ManipulationStartedEventArgs e)
        {
            HoursAngleAnimation.SkipToFill();
            HoursAngleAnimation.Stop();
            GrabberHours.Fill = Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
        }

        private void OnManipulationDeltaHours(object sender, ManipulationDeltaEventArgs e)
        {
            HoursAngle = GetAngle(e.ManipulationOrigin, ((Grid)sender).RenderSize);
            Quadrants quadrant = QuadrantFromAngle(HoursAngle);
            if (lastHourQuadrant == Quadrants.NorthWest && quadrant == Quadrants.NorthEast
             || lastHourQuadrant == Quadrants.NorthEast && quadrant == Quadrants.NorthWest)
            {
                IsAm = !IsAm;
                HoursAngle = HoursAngle; // toto je kvůli notifikaci potřebných properties
            }
            lastHourQuadrant = quadrant;
        }

        private void OnManipulationCompletedHours(object sender, ManipulationCompletedEventArgs e)
        {
            AnimateHours();
            GrabberHours.Fill = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
        }
        #endregion

        #region Minutes
        private void OnManipulationStartedMinutes(object sender, ManipulationStartedEventArgs e)
        {
            MinutesAngleAnimation.SkipToFill();
            MinutesAngleAnimation.Stop();
            GrabberMinutes.Fill = Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
        }

        private void OnManipulationDeltaMinutes(object sender, ManipulationDeltaEventArgs e)
        {
            MinutesAngle = GetAngle(e.ManipulationOrigin, ((Grid)sender).RenderSize);
        }

        private void OnManipulationCompletedMinutes(object sender, ManipulationCompletedEventArgs e)
        {
            AnimateMinutes();
            GrabberMinutes.Fill = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
        }
        #endregion

        private void HoursAngleAnimation_Completed(object sender, EventArgs e)
        {
            RaisePropertyChanged("HoursForeground");
            RaisePropertyChanged("HoursBackground");
            HoursAngle = Hours12 * 30;
        }

        private void MinutesAngleAnimation_Completed(object sender, EventArgs e)
        {
            MinutesAngle = Minutes * 6;
        }
        #endregion

        #region Private Methods
        private double GetAngle(Point touchPoint, Size circleSize)
        {
            var x = touchPoint.X - (circleSize.Width / 2d);
            var y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
            var hypot = Math.Sqrt(x * x + y * y);
            var value = Math.Asin(y / hypot) * 180 / Math.PI;
            var quadrant = (x >= 0) ?
                (y >= 0) ? Quadrants.NorthEast : Quadrants.SouthEast :
                (y >= 0) ? Quadrants.NorthWest : Quadrants.SouthWest;

            switch (quadrant)
            {
            case Quadrants.NorthEast:
            case Quadrants.SouthEast: value = 90 - value; break;
            default: value = 270 + value; break;
            }

            return value;
        }

        private Quadrants QuadrantFromAngle(double angle)
        {
            if (angle >= 0 && angle < 90) return Quadrants.NorthEast;
            if (angle >= 90 && angle < 180) return Quadrants.SouthEast;
            if (angle >= 180 && angle < 270) return Quadrants.SouthWest;
            if (angle >= 270 && angle <= 360) return Quadrants.NorthWest;

            return Quadrants.NorthEast;
        }

        private int HoursFromAngle(double angle, bool format24 = false)
        {
            return (int)Math.Round(angle / (format24 ? 15d : 30d));
        }

        private int MinutesFromAngle(double angle)
        {
            return (int)Math.Round(angle / 6d);
        }

        private Duration DurationFromSeconds(double seconds)
        {
            return new Duration(TimeSpan.FromSeconds(seconds));
        }

        private void Animate(Duration? duration = null)
        {
            AnimateHours(duration ?? DefaultHoursAnimationDuration);
            AnimateMinutes(duration ?? DefaultMinutesAnimationDuration);
        }

        private void AnimateHours(Duration? duration = null)
        {
            HoursAngleAnimateFrom = HoursAngle;
            HoursAngleAnimateTo = Hours12 * 30;

            if (HoursAngleAnimateFrom > 30 && HoursAngleAnimateTo == 0)
                HoursAngleAnimateFrom = HoursAngleAnimateFrom - 360;

            CurrentHoursAnimationDuration = duration ?? DefaultHoursAnimationDuration;
            Debug.WriteLine("dur: {0}", duration);
            Debug.WriteLine("DEF: {0}", DefaultHoursAnimationDuration);
            Debug.WriteLine("DURATION: {0}", CurrentHoursAnimationDuration);
            HoursAngleAnimation.Begin();
        }

        private void AnimateMinutes(Duration? duration = null)
        {
            MinutesAngleAnimateFrom = MinutesAngle;
            MinutesAngleAnimateTo = Minutes * 6;

            if (MinutesAngleAnimateFrom > 6 && MinutesAngleAnimateTo == 0)
                MinutesAngleAnimateFrom = MinutesAngleAnimateFrom - 360;

            CurrentMinutesAnimationDuration = duration ?? DefaultMinutesAnimationDuration;
            MinutesAngleAnimation.Begin();
        }
        #endregion
    }
}
