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
    public class IconButton : Button
    {
        public static readonly DependencyProperty IconStyleProperty =
            DependencyProperty.Register("IconStyle", typeof(Style), typeof(IconButton), null);

        public Style IconStyle
        {
            get { return (Style)base.GetValue(IconStyleProperty); }
            set { base.SetValue(IconStyleProperty, value); }
        }

        public IconButton()
        {
            DefaultStyleKey = typeof(IconButton);
        }
    }
}
