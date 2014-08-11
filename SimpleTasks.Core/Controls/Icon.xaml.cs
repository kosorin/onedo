using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.ComponentModel;

namespace SimpleTasks.Core.Controls
{
    public partial class Icon : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Icon()
        {
            InitializeComponent();

            Width = 50;
            Height = 50;

            Foreground = new SolidColorBrush(Colors.White);
        }

        public string Data
        {
            get { return (string)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(string), typeof(Icon), new PropertyMetadata(""));

        public Point Size
        {
            get { return (Point)GetValue(SizeProperty); }
            set
            {
                SetValue(SizeProperty, value);
                OnPropertyChanged("RealSize");
            }
        }
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Point), typeof(Icon), new PropertyMetadata(new Point(40, 40)));


        public Point Offset
        {
            get { return (Point)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Point), typeof(Icon), new PropertyMetadata(new Point()));

        public Point Scale
        {
            get { return (Point)GetValue(ScaleProperty); }
            set
            {
                SetValue(ScaleProperty, value);
                OnPropertyChanged("RealSize");
            }
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(Point), typeof(Icon), new PropertyMetadata(new Point(1, 1)));

        public Point RealSize
        {
            get
            {
                Point size = Size;
                Point scale = Scale;
                return new Point(size.X * scale.X, size.Y * scale.Y);
            }
        }
    }
}
