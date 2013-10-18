using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleTasks.Controls
{
    public partial class Icon : UserControl
    {
        public Icon()
        {
            InitializeComponent();
            DataContext = this;
        }

        // Data
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data",
                                        typeof(string),
                                        typeof(Icon),
                                        new PropertyMetadata("F1 M 22,54L 22,22L 54,22L 54,54L 22,54 Z M 26,26L 26,50L 50,50L 50,26L 26,26 Z "));

        public string Data
        {
            get { return (string)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Barva
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color",
                                        typeof(SolidColorBrush),
                                        typeof(Icon),
                                        new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Offset
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset",
                                        typeof(Point),
                                        typeof(Icon),
                                        new PropertyMetadata(new Point(-8, -8)));

        public Point Offset
        {
            get { return (Point)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Velikost
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size",
                                        typeof(Size),
                                        typeof(Icon),
                                        new PropertyMetadata(new Size(76, 76)));

        public Size Size
        {
            get { return (Size)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }
    }
}
