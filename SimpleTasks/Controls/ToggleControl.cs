using System.Windows;
using System.Windows.Controls;

namespace SimpleTasks.Controls
{
    [TemplateVisualState(Name = CheckedState, GroupName = CheckStates)]
    [TemplateVisualState(Name = UncheckedState, GroupName = CheckStates)]
    [TemplateVisualState(Name = IndeterminateState, GroupName = CheckStates)]
    public class ToggleControl : ContentControl
    {
        private const string CheckStates = "CheckStates";

        private const string CheckedState = "Checked";

        private const string UncheckedState = "Unchecked";

        private const string IndeterminateState = "Indeterminate";

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleControl),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsCheckedPropertyChanged)));

        public bool IsChecked
        {
            get { return (bool)base.GetValue(IsCheckedProperty); }
            set { base.SetValue(IsCheckedProperty, value); }
        }

        private static void OnIsCheckedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToggleControl control = sender as ToggleControl;
            if (control != null)
            {
                VisualStateManager.GoToState(control, control.IsChecked ? CheckedState : UncheckedState, true);
            }
        }

        public void ApplyStates()
        {
            VisualStateManager.GoToState(this, IsChecked ? CheckedState : UncheckedState, true);
        }

        public ToggleControl()
        {
            DefaultStyleKey = typeof(ToggleControl);
            VisualStateManager.GoToState(this, IndeterminateState, true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyStates();
        }
    }
}
