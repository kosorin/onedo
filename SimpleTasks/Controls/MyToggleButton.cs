using System.Windows;
using System.Windows.Controls;

namespace SimpleTasks.Controls
{
    [TemplateVisualState(Name = CheckedState, GroupName = CheckStates)]
    [TemplateVisualState(Name = UncheckedState, GroupName = CheckStates)]
    [TemplateVisualState(Name = IndeterminateState, GroupName = CheckStates)]
    public class MyToggleButton : Button
    {
        private const string CheckStates = "CheckStates";

        private const string CheckedState = "Checked";

        private const string UncheckedState = "Unchecked";

        private const string IndeterminateState = "Indeterminate";

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(MyToggleButton), 
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsCheckedPropertyChanged)));

        public bool IsChecked
        {
            get { return (bool)base.GetValue(IsCheckedProperty); }
            set { base.SetValue(IsCheckedProperty, value); }
        }

        private static void OnIsCheckedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MyToggleButton tb = sender as MyToggleButton;
            if (tb != null)
            {
                VisualStateManager.GoToState(tb, tb.IsChecked? CheckedState : UncheckedState, true);
            }
        }

        public void ApplyStates()
        {
            VisualStateManager.GoToState(this, IsChecked ? CheckedState : UncheckedState, true);
        }

        public MyToggleButton()
        {
            DefaultStyleKey = typeof(MyToggleButton);
            VisualStateManager.GoToState(this, IndeterminateState, true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyStates();
        }
    }
}
