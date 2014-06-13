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

namespace SimpleTasks.Views
{
    public partial class RadialTimePickerPage : PhoneApplicationPage
    {
        #region Fields
        private readonly RadialViewModel _radialVM;
        private Quadrants? _lastQ;
        private readonly CultureInfo _ci = CultureInfo.CurrentCulture;
        private bool _save = false;
        #endregion

        #region Constructor
        public RadialTimePickerPage()
        {
            InitializeComponent();

            //AmTextBlock.Text = _ci.DateTimeFormat.AMDesignator;
            //PmTextBlock.Text = _ci.DateTimeFormat.PMDesignator;

            _radialVM = new RadialViewModel(this);
            DataContext = _radialVM;

            BuildAppBar();
        }
        #endregion

        #region Private Methods
        private void OnManipulationStartedMinutes(object sender, ManipulationStartedEventArgs e)
        {
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
            var grid = (Grid)sender;
            var angle = GetAngle(e.ManipulationOrigin, grid.RenderSize, false);
            _radialVM.AngleMinutes = angle;
        }

        private void OnManipulationDeltaHours(object sender, ManipulationDeltaEventArgs e)
        {
            var grid = (Grid)sender;
            var angle = GetAngle(e.ManipulationOrigin, grid.RenderSize, true);
            _radialVM.AngleHours = angle;
        }

        private void OnManipulationCompletedMinutes(object sender, ManipulationCompletedEventArgs e)
        {
            _radialVM.RoundAngleMinutes();
            GrabberMinutes.Fill = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
        }

        private void OnManipulationCompletedHours(object sender, ManipulationCompletedEventArgs e)
        {
            _radialVM.RoundAngleHours();
            GrabberHours.Fill = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
        }

        private void AmPmTextBlock_Click(object sender, RoutedEventArgs e)
        {
            _radialVM.IsAm = !_radialVM.IsAm;
        }

        private double GetAngle(Point touchPoint, Size circleSize, bool hours)
        {
            var x = touchPoint.X - (circleSize.Width / 2d);
            var y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
            var hypot = Math.Sqrt(x * x + y * y);
            var value = Math.Asin(y / hypot) * 180 / Math.PI;
            var quadrant = (x >= 0) ?
                (y >= 0) ? Quadrants.Ne : Quadrants.Se :
                (y >= 0) ? Quadrants.Nw : Quadrants.Sw;
            switch (quadrant)
            {
                case Quadrants.Ne:
                case Quadrants.Se: value = 090 - value; break;
                default: value = 270 + value; break;
            }

            if (hours && _lastQ.HasValue
                && (_lastQ.Value == Quadrants.Nw && quadrant == Quadrants.Ne
                    || _lastQ.Value == Quadrants.Ne && quadrant == Quadrants.Nw))
            {
                _radialVM.ToggleAmPm();
            }

            _lastQ = quadrant;

            return value;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
                _radialVM.OnClosing(_save);
        }
        #endregion

        #region Public Methods
        public void SetLastQuadrantTo(Quadrants quadrant)
        {
            _lastQ = quadrant;
        }
        #endregion

        #region AppBar

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarDoneButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative));
            appBarDoneButton.Text = AppResources.AppBarDone;
            appBarDoneButton.Click += appBarDoneButton_Click;
            ApplicationBar.Buttons.Add(appBarDoneButton);


            ApplicationBarIconButton appBarCancelButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            appBarCancelButton.Text = AppResources.AppBarCancel;
            appBarCancelButton.Click += appBarCancelButton_Click;
            ApplicationBar.Buttons.Add(appBarCancelButton);
        }

        void appBarDoneButton_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                _save = true;
                NavigationService.GoBack();
            }
        }

        void appBarCancelButton_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                _save = false;
                NavigationService.GoBack();
            }
        }

        #endregion
    }
}