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
    public partial class RoundTimePicker : UserControl, INotifyPropertyChanged
    {
        #region Quadrants
        private enum Quadrants { NorthWest = 2, NorthEast = 1, SouthWest = 4, SouthEast = 3 }

        private Quadrants lastQuadrant;
        #endregion

        #region Public Methods
        public RoundTimePicker()
        {
            InitializeComponent();

            Time = new DateTime(1, 1, 1, 21, 52, 0);
            Animate(0.65);

            DataContext = this;
        }

        public void SetTime(DateTime time)
        {
            Time = time;
            Animate();
        }

        public void SetTime(int hours, int minutes, double duration = 0)
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
        #endregion

        #region Properties for UI
        private Duration _defaultAnimationDuration = new Duration(TimeSpan.FromSeconds(0.18));
        public Duration DefaultAnimationDuration
        {
            get { return _defaultAnimationDuration; }
            private set { SetProperty(ref _defaultAnimationDuration, value); }
        }

        private Duration _animationDuration;
        public Duration AnimationDuration
        {
            get { return _animationDuration; }
            private set { SetProperty(ref _animationDuration, value); }
        }

        private double _angleMinutes = default(double);
        public double AngleMinutes
        {
            get { return _angleMinutes; }
            private set
            {
                SetProperty(ref _angleMinutes, value);
                Minutes = MinutesFromAngle(value);
            }
        }

        private double _angleMinutesAnimateFrom = default(double);
        public double AngleMinutesAnimateFrom
        {
            get { return _angleMinutesAnimateFrom; }
            private set { SetProperty(ref _angleMinutesAnimateFrom, value); }
        }

        private double _angleMinutesAnimateTo = default(double);
        public double AngleMinutesAnimateTo
        {
            get { return _angleMinutesAnimateTo; }
            private set { SetProperty(ref _angleMinutesAnimateTo, value); }
        }

        private double _angleHours = default(double);
        public double AngleHours
        {
            get { return _angleHours; }
            private set
            {
                SetProperty(ref _angleHours, value);
                Hours12 = HoursFromAngle(value);
            }
        }

        private double _angleHoursAnimateFrom = default(double);
        public double AngleHoursAnimateFrom
        {
            get { return _angleHoursAnimateFrom; }
            private set { SetProperty(ref _angleHoursAnimateFrom, value); }
        }

        private double _angleHoursAnimateTo = default(double);
        public double AngleHoursAnimateTo
        {
            get { return _angleHoursAnimateTo; }
            private set { SetProperty(ref _angleHoursAnimateTo, value); }
        }

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
                bool isZeroAngle = AngleHours == 0 || AngleHours == 360;
                if ((Hours > 0 && Hours < 12)
                    || (Hours == 0 && (isZeroAngle || AngleHours < 180))
                    || (Hours == 12 && (!isZeroAngle && AngleHours > 180)))
                    return Resources["PhoneAccentBrush07"] as SolidColorBrush;
                else
                    return Resources["PhoneAccentBrush"] as SolidColorBrush;
            }
        }

        public SolidColorBrush HoursBackground
        {
            get
            {
                bool isZeroAngle = AngleHours == 0 || AngleHours == 360;
                if ((Hours > 0 && Hours < 12)
                    || (Hours == 0 && (isZeroAngle || AngleHours < 180))
                    || (Hours == 12 && (!isZeroAngle && AngleHours > 180)))
                    return Resources["PhoneAccentBrush03"] as SolidColorBrush;
                else
                    return Resources["PhoneAccentBrush08"] as SolidColorBrush;
            }
        }
        #endregion

        #region Event Handlers
        private void OnManipulationStartedMinutes(object sender, ManipulationStartedEventArgs e)
        {
            AngleMinutesAnimation.SkipToFill();
            AngleMinutesAnimation.Stop();
            GrabberMinutes.Fill = Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
        }

        private void OnManipulationStartedHours(object sender, ManipulationStartedEventArgs e)
        {
            AngleHoursAnimation.SkipToFill();
            AngleHoursAnimation.Stop();
            GrabberHours.Fill = Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
        }

        private void OnManipulationDeltaMinutes(object sender, ManipulationDeltaEventArgs e)
        {
            AngleMinutes = GetAngle(e.ManipulationOrigin, ((Grid)sender).RenderSize);
        }

        private void OnManipulationDeltaHours(object sender, ManipulationDeltaEventArgs e)
        {
            AngleHours = GetAngle(e.ManipulationOrigin, ((Grid)sender).RenderSize);
            Quadrants quadrant = QuadrantFromAngle(AngleHours);
            if (lastQuadrant == Quadrants.NorthWest && quadrant == Quadrants.NorthEast
             || lastQuadrant == Quadrants.NorthEast && quadrant == Quadrants.NorthWest)
            {
                IsAm = !IsAm;
                AngleHours = AngleHours; // toto je kvůli notifikaci potřebných properties
            }
            lastQuadrant = quadrant;
        }

        private void OnManipulationCompletedMinutes(object sender, ManipulationCompletedEventArgs e)
        {
            AnimateMinutes();
            GrabberMinutes.Fill = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
        }

        private void OnManipulationCompletedHours(object sender, ManipulationCompletedEventArgs e)
        {
            AnimateHours();
            GrabberHours.Fill = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
        }

        private void AngleHoursAnimation_Completed(object sender, EventArgs e)
        {
            RaisePropertyChanged("HoursForeground");
            RaisePropertyChanged("HoursBackground");
            AngleHours = Hours12 * 30;
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

        private void Animate()
        {
            AnimationDuration = DefaultAnimationDuration;
            AnimateHours();
            AnimateMinutes();
        }

        private void Animate(double duration)
        {
            AnimationDuration = new Duration(TimeSpan.FromSeconds(duration));
            AnimateHours();
            AnimateMinutes();
            AnimationDuration = DefaultAnimationDuration;
        }

        private void AnimateHours()
        {
            lastQuadrant = QuadrantFromAngle((Time.Hour % 12) * 30);
            AngleHoursAnimateFrom = AngleHours;
            AngleHoursAnimateTo = Hours12 * 30;

            if (AngleHoursAnimateFrom > 30 && AngleHoursAnimateTo == 0)
                AngleHoursAnimateFrom = AngleHoursAnimateFrom - 360;
            AngleHoursAnimation.Begin();
        }

        private void AnimateMinutes()
        {
            AngleMinutesAnimateFrom = AngleMinutes;
            AngleMinutes = (Minutes * 6) % 360;
            AngleMinutesAnimateTo = AngleMinutes;

            if (AngleMinutesAnimateFrom > 6 && AngleMinutesAnimateTo == 0)
                AngleMinutesAnimateFrom = AngleMinutesAnimateFrom - 360;
            AngleMinutesAnimation.Begin();
        }
        #endregion
    }
}
