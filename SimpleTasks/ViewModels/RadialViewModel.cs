using Microsoft.Phone.Shell;
using SimpleTasks.Controls;
using SimpleTasks.Views;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SimpleTasks.ViewModels
{
    public enum Quadrants { Nw = 2, Ne = 1, Sw = 4, Se = 3 }

    public class RadialViewModel : INotifyPropertyChanged
    {
        #region Fields
        private DateTime _initTime;
        private double _angleHours = default(double);
        private double _angleHoursAnimateFrom = default(double);
        private double _angleHoursAnimateTo = default(double);
        private double _angleMinutes = default(double);
        private double _angleMinutesAnimateFrom = default(double);
        private double _angleMinutesAnimateTo = default(double);
        private int _hoursSimple = default(int);
        private int _minutes = default(int);
        private bool _isAm;
        private readonly RadialTimePickerPage _picker;
        private bool? _is24Format;
        private string _hoursPattern;
        #endregion

        #region Properties

        public bool Is24Format
        {
            get { return _is24Format.HasValue ? _is24Format.Value : (_is24Format = !CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.Contains("tt")).Value; }
        }

        public bool? ShowAmPm
        {
            get
            {
                if (Is24Format)
                    return null;
                else
                    return IsAm;
            }
        }

        public double AngleMinutes
        {
            get { return _angleMinutes; }
            set
            {
                SetProperty(ref _angleMinutes, value);
                var minutes = (int)Math.Round(value / 6d);
                Minutes = minutes != 60 ? minutes : 0;
            }
        }

        public double AngleMinutesAnimateFrom
        {
            get { return _angleMinutesAnimateFrom; }
            set { SetProperty(ref _angleMinutesAnimateFrom, value); }
        }

        public double AngleMinutesAnimateTo
        {
            get { return _angleMinutesAnimateTo; }
            set { SetProperty(ref _angleMinutesAnimateTo, value); }
        }

        public double AngleHours
        {
            get { return _angleHours; }
            set
            {
                SetProperty(ref _angleHours, value);
                HoursSimple = (int)Math.Round(value / 30d);
            }
        }

        public double AngleHoursAnimateFrom
        {
            get { return _angleHoursAnimateFrom; }
            set { SetProperty(ref _angleHoursAnimateFrom, value); }
        }

        public double AngleHoursAnimateTo
        {
            get { return _angleHoursAnimateTo; }
            set { SetProperty(ref _angleHoursAnimateTo, value); }
        }

        public int Minutes { get { return _minutes; } private set { SetProperty(ref _minutes, value); } }

        public int HoursSimple
        {
            get { return _hoursSimple; }
            private set
            {
                SetProperty(ref _hoursSimple, value);
                RaisePropertyChanged("HoursFormated");
            }
        }

        public string HoursFormated
        {
            get
            {
                var hours = HoursSimple + (IsAm ? 0 : 12);
                return new DateTime(1, 1, 1, hours > 23 ? 0 : hours, 0, 0).ToString(HoursPattern);
            }
        }

        public bool IsAm
        {
            get { return _isAm; }
            set
            {
                SetProperty(ref _isAm, value);
                RaisePropertyChanged("HoursFormated");
                RaisePropertyChanged("ShowAmPm");
            }
        }

        public string HoursPattern
        {
            get
            {
                if (_hoursPattern == null)
                {
                    _hoursPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Replace(":mm", "").Replace(" tt", "");
                    // need to handle special case if format is only 'h'
                    // see this link: http://stackoverflow.com/questions/3459677/datetime-tostringh-causes-exception
                    if (_hoursPattern.Length == 1)
                        _hoursPattern = _hoursPattern.Insert(0, "%");
                }

                return _hoursPattern;
            }
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

        #region Constructor
        private void AnimationDuration(object s, object e)
        {
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 350));
            foreach (var c in _picker.AngleHoursAnimation.Children)
                c.Duration = duration;
            foreach (var c in _picker.AngleMinutesAnimation.Children)
                c.Duration = duration;
            _picker.AngleMinutesAnimation.Completed -= AnimationDuration;
        }

        public RadialViewModel(RadialTimePickerPage picker)
        {
            _picker = picker;

            // read values from selected alarm and set values
            _initTime = (DateTime)PhoneApplicationService.Current.State["RadialTime"]; // TODO: initialize your time
            PhoneApplicationService.Current.State.Remove("RadialTime");
            HoursSimple = _initTime.Hour % 12;
            Minutes = _initTime.Minute;
            IsAm = _initTime.Hour < 12;

            // set angles
            _picker.AngleMinutesAnimation.Completed += AnimationDuration;
            RoundAngleHours();
            RoundAngleMinutes();

            // set _lastQuadrant to init value
            _picker.SetLastQuadrantTo(GetQuadrant(AngleHours));
        }
        #endregion

        #region Public Methods
        public void ToggleAmPm()
        {
            IsAm = !IsAm;
        }

        /// <summary>
        /// Counts AngleHourse from HoursSimple.
        /// If called when manipulation is complete then rounds AngleHours to HoursSimple value (int).
        /// May be called also to set angle manually. Definitely should not be called during manipulation delta.
        /// </summary>
        public void RoundAngleHours()
        {
            var oldQ = GetQuadrant(AngleHours);
            AngleHoursAnimateFrom = AngleHours;
            AngleHours = HoursSimple * 30;
            AngleHoursAnimateTo = AngleHours;
            var newQ = GetQuadrant(AngleHours);
            _picker.AngleHoursAnimation.Begin();

            // when going from 
            if (oldQ == Quadrants.Nw && newQ == Quadrants.Ne)
            {
                HoursSimple = 0;
                ToggleAmPm();
                _picker.SetLastQuadrantTo(Quadrants.Ne);
            }
        }

        /// <summary>
        /// Counts AngleMinutes from Minutes.
        /// </summary>
        public void RoundAngleMinutes()
        {
            AngleMinutesAnimateFrom = AngleMinutes;
            AngleMinutes = Minutes * 6;
            AngleMinutesAnimateTo = AngleMinutes;
            _picker.AngleMinutesAnimation.Begin();
        }

        public void OnClosing(bool canSave)
        {
            if (canSave)
                PhoneApplicationService.Current.State["RadialTime"] = new DateTime(_initTime.Year, _initTime.Month, _initTime.Day, IsAm ? HoursSimple : HoursSimple + 12, Minutes, 0);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculates quadrant for given angle.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private Quadrants GetQuadrant(double angle)
        {
            if (angle >= 0 && angle < 90 || angle == 360) return Quadrants.Ne;
            if (angle >= 90 && angle < 180) return Quadrants.Se;
            if (angle >= 180 && angle < 270) return Quadrants.Sw;
            return Quadrants.Nw;
        }
        #endregion


    }
}
