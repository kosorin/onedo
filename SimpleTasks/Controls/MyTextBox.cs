using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleTasks.Controls
{
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplateVisualState(Name = ReadOnlyState, GroupName = CommonStates)]
    [TemplateVisualState(Name = FocusedState, GroupName = FocusStates)]
    [TemplateVisualState(Name = UnfocusedState, GroupName = FocusStates)]
    [TemplateVisualState(Name = EmptyState, GroupName = EmptyStates)]
    [TemplateVisualState(Name = NotEmptyState, GroupName = EmptyStates)]
    [TemplatePart(Name = TextBoxName, Type = typeof(TextBox))]
    [TemplatePart(Name = HintContentName, Type = typeof(ContentControl))]
    public class MyTextBox : TextBox
    {
        #region MyTextBox Properties & Variables
        private Grid _rootGrid;
        private TextBox _textBox;

        private Brush _foregroundBrushInactive = (Brush)Application.Current.Resources["PhoneTextBoxReadOnlyBrush"];
        private Brush _foregroundBrushEdit;

        // Hint Private Variables.
        private ContentControl _hintContent;

        //Temporarily ignore focus?
        private bool _ignoreFocus = false;

        /// <summary>
        /// Border for MyTextBox action icon
        /// </summary>
        protected Border ActionIconBorder { get; set; }

        #endregion

        #region Constants
        /// <summary>
        /// Root grid.
        /// </summary>
        private const string RootGridName = "RootGrid";

        /// <summary>
        /// Main text box.
        /// </summary>
        private const string TextBoxName = "Text";

        /// <summary>
        /// Hint Content.
        /// </summary>
        private const string HintContentName = "HintContent";

        /// <summary>
        /// Hint border.
        /// </summary>
        private const string HintBorderName = "HintBorder";
        #endregion

        #region Visual States
        /// <summary>
        /// Common States.
        /// </summary>
        private const string CommonStates = "CommonStates";

        /// <summary>
        /// Normal state.
        /// </summary>
        private const string NormalState = "Normal";

        /// <summary>
        /// Disabled state.
        /// </summary>
        private const string DisabledState = "Disabled";

        /// <summary>
        /// ReadOnly state.
        /// </summary>
        private const string ReadOnlyState = "ReadOnly";

        /// <summary>
        /// Focus states.
        /// </summary>
        private const string FocusStates = "FocusStates";

        /// <summary>
        /// Focused state.
        /// </summary>
        private const string FocusedState = "Focused";

        /// <summary>
        /// Unfocused state.
        /// </summary>
        private const string UnfocusedState = "Unfocused";

        private const string EmptyStates = "EmptyStates";

        private const string EmptyState = "EmptyState";

        private const string NotEmptyState = "NotEmptyState";
        #endregion

        #region Hint
        /// <summary>
        /// Identifies the Hint DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string), typeof(MyTextBox), new PropertyMetadata(
                    new PropertyChangedCallback(OnHintPropertyChanged)
                )
            );

        /// <summary>
        /// Gets or sets the Hint
        /// </summary>
        public string Hint
        {
            get { return base.GetValue(HintProperty) as string; }
            set { base.SetValue(HintProperty, value); }
        }

        /// <summary>
        /// Identifies the HintForeground DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HintForegroundProperty =
            DependencyProperty.Register("HintForeground", typeof(Brush), typeof(MyTextBox), null);

        /// <summary>
        /// Gets or sets the Hint style
        /// </summary>
        public Brush HintForeground
        {
            get { return base.GetValue(HintForegroundProperty) as Brush; }
            set { base.SetValue(HintForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the HintVisibility DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ActualHintVisibilityProperty =
            DependencyProperty.Register("ActualHintVisibility", typeof(Visibility), typeof(MyTextBox), null);

        /// <summary>
        /// Gets or sets whether the hint is actually visible.
        /// </summary>
        public Visibility ActualHintVisibility
        {
            get { return (Visibility)base.GetValue(ActualHintVisibilityProperty); }
            set { base.SetValue(ActualHintVisibilityProperty, value); }
        }


        /// <summary>
        /// When the Hint is changed, check if it needs to be hidden or shown.
        /// </summary>
        /// <param name="sender">Sending MyTextBox.</param>
        /// <param name="args">DependencyPropertyChangedEvent Arguments.</param>
        private static void OnHintPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MyTextBox phoneTextBox = sender as MyTextBox;

            if (phoneTextBox != null && phoneTextBox._hintContent != null)
            {
                phoneTextBox.UpdateHintVisibility();
            }
        }


        /// <summary>
        /// Determines if the Hint should be shown or not based on if there is content in the TextBox.
        /// </summary>
        private void UpdateHintVisibility()
        {
            if (_hintContent != null)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    ActualHintVisibility = Visibility.Visible;
                    Foreground = _foregroundBrushInactive;
                }
                else
                {
                    ActualHintVisibility = Visibility.Collapsed;
                    Foreground = _foregroundBrushEdit;
                }
            }

            VisualStateManager.GoToState(this, string.IsNullOrEmpty(Text) ? EmptyState : NotEmptyState, false);
        }

        /// <summary>
        /// Override the Blur/LostFocus event to show the Hint if needed.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            UpdateHintVisibility();
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Override the Focus event to hide the Hint.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (_ignoreFocus)
            {
                _ignoreFocus = false;

                var rootFrame = Application.Current.RootVisual as Frame;
                rootFrame.Focus();

                return;
            }

            Foreground = _foregroundBrushEdit;

            if (_hintContent != null)
            {
                ActualHintVisibility = Visibility.Collapsed;
            }

            base.OnGotFocus(e);
        }

        #endregion

        #region Icon
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ContentControl), typeof(MyTextBox), null);

        public ContentControl Icon
        {
            get { return base.GetValue(IconProperty) as ContentControl; }
            set { base.SetValue(IconProperty, value); }
        }
        #endregion

        #region Mask
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(Brush), typeof(MyTextBox), null);

        public Brush Mask
        {
            get { return base.GetValue(MaskProperty) as Brush; }
            set { base.SetValue(MaskProperty, value); }
        }
        #endregion

        public MyTextBox()
        {
            DefaultStyleKey = typeof(MyTextBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_textBox != null)
            {
                _textBox.TextChanged -= OnTextChanged;
            }

            _rootGrid = GetTemplateChild(RootGridName) as Grid;
            _textBox = GetTemplateChild(TextBoxName) as TextBox;

            // Getting the foreground color to save for later.
            _foregroundBrushEdit = Foreground;

            // Getting template children for the hint text.
            _hintContent = GetTemplateChild(HintContentName) as ContentControl;

            if (_hintContent != null)
            {
                UpdateHintVisibility();
            }

            if (_textBox != null)
            {
                _textBox.TextChanged += OnTextChanged;
            }
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            UpdateHintVisibility();
        }
    }
}
