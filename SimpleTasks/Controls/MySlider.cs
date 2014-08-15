using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleTasks.Controls
{
    [TemplateVisualState(Name = StartedChangeState, GroupName = ChangeStates)]
    [TemplateVisualState(Name = CompletedChangeState, GroupName = ChangeStates)]
    [TemplateVisualState(Name = DeltaChangeState, GroupName = ChangeStates)]
    public class MySlider : Slider
    {
        #region Visual States
        private const string ChangeStates = "ChangeStates";

        private const string StartedChangeState = "StartedChange";

        private const string CompletedChangeState = "CompletedChange";

        private const string DeltaChangeState = "DeltaChange";
        #endregion

        #region Dependency Properties

        #region RoundValue
        public static readonly DependencyProperty RoundValueProperty = DependencyProperty.Register("RoundValue", typeof(int), typeof(MySlider), new PropertyMetadata(0, OnRoundValueChanged));
        public int RoundValue
        {
            get { return (int)base.GetValue(RoundValueProperty); }
            set { base.SetValue(RoundValueProperty, value); }
        }

        public static void OnRoundValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MySlider mySlider = (MySlider)d;
            if (mySlider.RoundValueChanged != null)
            {
                mySlider.RoundValueChanged(mySlider, new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue));
            }
        }

        public event EventHandler<RoutedPropertyChangedEventArgs<int>> RoundValueChanged;
        #endregion

        #region AnimationValue
        public static readonly DependencyProperty AnimationValueProperty = DependencyProperty.Register("AnimationValue", typeof(double), typeof(MySlider),
            new PropertyMetadata(0d, OnAnimationValuePropertyChanged));
        public double AnimationValue
        {
            get { return (double)base.GetValue(AnimationValueProperty); }
            set { base.SetValue(AnimationValueProperty, value); }
        }

        private static void OnAnimationValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MySlider slider = sender as MySlider;
            if (slider != null)
            {
                slider.UpdateThumbPosition((double)args.NewValue);
            }
        }
        #endregion

        #region Mask
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(Brush), typeof(MySlider), null);

        public Brush Mask
        {
            get { return base.GetValue(MaskProperty) as Brush; }
            set { base.SetValue(MaskProperty, value); }
        }
        #endregion

        #region ValueTextVisibility
        public Visibility ValueTextVisibility
        {
            get { return (Visibility)GetValue(ValueTextVisibilityProperty); }
            set { SetValue(ValueTextVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ValueTextVisibilityProperty =
            DependencyProperty.Register("ValueTextVisibility", typeof(Visibility), typeof(MySlider), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #endregion

        #region Events
        private void MySlider_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            VisualStateManager.GoToState(this, StartedChangeState, true);
        }

        private void MySlider_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            VisualStateManager.GoToState(this, CompletedChangeState, true);
        }

        private void MySlider_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            VisualStateManager.GoToState(this, DeltaChangeState, true);
            AnimationValue = Value;
            UpdateThumbPosition(Value);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            RoundValue = (int)Math.Round(newValue);
        }
        #endregion

        public MySlider()
        {
            DefaultStyleKey = typeof(MySlider);

            ManipulationStarted += MySlider_ManipulationStarted;
            ManipulationCompleted += MySlider_ManipulationCompleted;
            ManipulationDelta += MySlider_ManipulationDelta;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetSliderValue(RoundValue);
        }

        public void SetSliderValue(int value)
        {
            double dv = (double)value;
            if (dv < Minimum)
            {
                dv = Minimum;
            }
            if (dv > Maximum)
            {
                dv = Maximum;
            }

            Value = dv;
            MySlider_ManipulationStarted(null, null);
            MySlider_ManipulationCompleted(null, null);
        }

        public void SetMaximum(double maximum)
        {
            double ratio = maximum / Maximum;

            Maximum = maximum;
            AnimationValue = ratio * AnimationValue;
            Value = ratio * Value;

            MySlider_ManipulationStarted(null, null);
            MySlider_ManipulationCompleted(null, null);
        }

        private double ThumbPositionFromValue(double value)
        {
            Border trackBorder = (Border)GetTemplateChild("HorizontalTrack");
            double max = trackBorder.ActualWidth;
            return ((value - Minimum) / (Maximum - Minimum)) * max;
        }

        private void UpdateThumbPosition(double newValue)
        {
            // Nová pozice
            double position = ThumbPositionFromValue(newValue);

            // Popredi
            RectangleGeometry geometry = (RectangleGeometry)GetTemplateChild("HorizontalFillGeometry");
            Rect rect = geometry.Rect;
            try
            {
                geometry.Rect = new Rect(rect.X, rect.Y, position, rect.Height);
            }
            catch (ArgumentException)
            {
                return;
            }

            // Tahatko
            TranslateTransform thumb = (TranslateTransform)GetTemplateChild("HorizontalThumb");
            thumb.X = position;
        }
    }
}
