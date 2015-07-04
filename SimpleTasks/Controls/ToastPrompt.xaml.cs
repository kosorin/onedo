using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SimpleTasks.Controls
{
    public partial class ToastPrompt : UserControl
    {
        private enum States
        {
            Closed,
            Opened
        }

        #region Private variables
        private const double _defaultDuration = 4;
        private States _state = States.Closed;
        private TimeSpan _interval = TimeSpan.FromSeconds(_defaultDuration);
        private DispatcherTimer _timer = null;
        #endregion

        #region Event closed
        public delegate void ClosedEventHandler(object sender, VisualStateChangedEventArgs e);
        public event ClosedEventHandler ClosedHandler;
        public event ClosedEventHandler ClosingHandler;

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == ClosedState)
                if (ClosedHandler != null)
                    ClosedHandler(this, e);
        }

        private void OnCurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == ClosedState)
                if (ClosingHandler != null)
                    ClosingHandler(this, e);
        }
        #endregion

        #region ShowIcon
        /// <summary>
        /// Show the image bound to the control (default is True)
        /// </summary>
        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register("ShowIcon", typeof(bool), typeof(ToastPrompt), new PropertyMetadata(true));
        #endregion

        #region ShowTitle
        public bool ShowTitle
        {
            get { return (bool)GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register("ShowTitle", typeof(bool), typeof(ToastPrompt), new PropertyMetadata(false));
        #endregion // end of ShowTitle

        #region TextHAlignment
        public HorizontalAlignment TextHAlignment
        {
            get { return (HorizontalAlignment)GetValue(TextHAlignmentProperty); }
            set { SetValue(TextHAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TextHAlignmentProperty =
            DependencyProperty.Register("TextHAlignment", typeof(HorizontalAlignment), typeof(ToastPrompt), new PropertyMetadata(HorizontalAlignment.Stretch));
        #endregion

        #region Message
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ToastPrompt), new PropertyMetadata(String.Empty));

        #endregion

        #region Duration
        /// <summary>
        /// Displaying duration of the toast (in sec)
        /// </summary>
        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(double), typeof(ToastPrompt), new PropertyMetadata(_defaultDuration, OnDurationChanged));

        private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToastPrompt toast = d as ToastPrompt;
            double interval = _defaultDuration;
            Double.TryParse(e.NewValue.ToString(), out interval);
            if (toast != null) toast._interval = TimeSpan.FromSeconds(interval);
        }
        #endregion

        #region TextWrapping
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextWrapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(ToastPrompt), new PropertyMetadata(TextWrapping.NoWrap));
        #endregion

        #region Height
        public int InvertedHeight
        {
            get
            {
                return -(int)GetValue(HeightProperty);
            }
        }

        public new int Height
        {
            get { return (int)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public new static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(int), typeof(ToastPrompt), new PropertyMetadata(50));
        #endregion

        #region Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ToastPrompt), new PropertyMetadata("AppName"));
        #endregion

        #region Icon
        public Style Icon
        {
            get { return (Style)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(Style), typeof(ToastPrompt), new PropertyMetadata(null));
        #endregion

        #region Visual States
        private const string OpenedState = "Opened";
        private const string ClosedState = "Closed";

        private void UpdateVisualStates(States state, bool useTransitions)
        {
            _state = state;
            VisualStateManager.GoToState(this, state == States.Opened ? OpenedState : ClosedState, useTransitions);
        }
        #endregion

        #region Public methods
        public void Show(string message, Style icon = null, string title = "")
        {
            if (_state == States.Opened)
            {
                MessageChangedStoryboard.SkipToFill();
                MessageChangedStoryboard.Begin();
            }

            Message = message;
            Icon = icon;
            ShowIcon = Icon != null;
            Title = title;
            ShowTitle = Title != "";

            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            UpdateVisualStates(States.Opened, true);

            _timer = new DispatcherTimer { Interval = _interval };
            _timer.Tick += (s, t) =>
            {
                Hide();
            };
            _timer.Start();
        }

        public void Hide()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
            UpdateVisualStates(States.Closed, true);
        }
        #endregion // end of Public methods

        public ToastPrompt()
        {
            InitializeComponent();

            Foreground = App.Current.Resources["ButtonDarkForegroundBrush"] as SolidColorBrush;
            Background = App.Current.Resources["AccentBrush"] as SolidColorBrush;

            this.Loaded += (s, e) =>
            {
                UpdateVisualStates(States.Closed, false);
            };
        }

        private void CloseStoryboard_Completed(object sender, EventArgs e)
        {
            // Force the property to be changed even if the user don't change the message value
            // It's done in this callback to avoid text disappear (set to empty) before the closing animation is completed
            // Not sure it's a good way to do it (it was done with the CoerceValue in WPF)
            this.Message = String.Empty;
        }

        private void ToastRoot_Tap(object sender, GestureEventArgs e)
        {
            Hide();
        }
    }
}
