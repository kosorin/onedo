using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SimpleTasks.Controls
{
    public class MyToggleButton : ToggleButton
    {
        #region Icons
        public static readonly DependencyProperty IndeterminateIconProperty =
            DependencyProperty.Register("IndeterminateIcon", typeof(ContentControl), typeof(MyToggleButton), null);

        public ContentControl IndeterminateIcon
        {
            get { return base.GetValue(IndeterminateIconProperty) as ContentControl; }
            set { base.SetValue(IndeterminateIconProperty, value); }
        }

        public static readonly DependencyProperty UncheckedIconProperty =
            DependencyProperty.Register("UncheckedIcon", typeof(ContentControl), typeof(MyToggleButton), null);

        public ContentControl UncheckedIcon
        {
            get { return base.GetValue(UncheckedIconProperty) as ContentControl; }
            set { base.SetValue(UncheckedIconProperty, value); }
        }

        public static readonly DependencyProperty CheckedIconProperty =
            DependencyProperty.Register("CheckedIcon", typeof(ContentControl), typeof(MyToggleButton), null);

        public ContentControl CheckedIcon
        {
            get { return base.GetValue(CheckedIconProperty) as ContentControl; }
            set { base.SetValue(CheckedIconProperty, value); }
        }

        public static readonly DependencyProperty ActualIconProperty =
            DependencyProperty.Register("ActualIcon", typeof(ContentControl), typeof(MyToggleButton), null);

        public ContentControl ActualIcon
        {
            get { return base.GetValue(ActualIconProperty) as ContentControl; }
            private set { base.SetValue(ActualIconProperty, value); }
        }
        #endregion

        #region Texts
        public static readonly DependencyProperty IndeterminateTextProperty =
            DependencyProperty.Register("IndeterminateText", typeof(string), typeof(MyToggleButton), null);

        public string IndeterminateText
        {
            get { return base.GetValue(IndeterminateTextProperty) as string; }
            set { base.SetValue(IndeterminateTextProperty, value); }
        }

        public static readonly DependencyProperty UncheckedTextProperty =
            DependencyProperty.Register("UncheckedText", typeof(string), typeof(MyToggleButton), null);

        public string UncheckedText
        {
            get { return base.GetValue(UncheckedTextProperty) as string; }
            set { base.SetValue(UncheckedTextProperty, value); }
        }

        public static readonly DependencyProperty CheckedTextProperty =
            DependencyProperty.Register("CheckedText", typeof(string), typeof(MyToggleButton), null);

        public string CheckedText
        {
            get { return base.GetValue(CheckedTextProperty) as string; }
            set { base.SetValue(CheckedTextProperty, value); }
        }

        public static readonly DependencyProperty ActualTextProperty =
            DependencyProperty.Register("ActualText", typeof(string), typeof(MyToggleButton), null);

        public string ActualText
        {
            get { return base.GetValue(ActualTextProperty) as string; }
            private set { base.SetValue(ActualTextProperty, value); }
        }
        #endregion

        public MyToggleButton()
        {
            DefaultStyleKey = typeof(MyToggleButton);
        }
    }
}
