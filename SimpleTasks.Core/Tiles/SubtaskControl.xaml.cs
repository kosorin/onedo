using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Core.Controls;
using System.Windows;
using System.Windows.Shapes;
using SimpleTasks.Core.Models;
using System.Windows.Media;

namespace SimpleTasks.Core.Tiles
{
    [TemplatePart(Name = "Icon", Type = typeof(Canvas))]
    [TemplatePart(Name = "Text", Type = typeof(TextBlock))]
    [TemplatePart(Name = "Strike", Type = typeof(Border))]
    public partial class SubtaskControl : UserControl
    {
        public Subtask Subtask
        {
            get { return (Subtask)GetValue(SubtaskProperty); }
            set { SetValue(SubtaskProperty, value); }
        }
        public static readonly DependencyProperty SubtaskProperty =
            DependencyProperty.Register("Subtask", typeof(Subtask), typeof(SubtaskControl), null);

        public SubtaskControl()
        {
            InitializeComponent();
        }

        public SubtaskControl(Subtask subtask)
        {
            InitializeComponent();
            Subtask = subtask;
        }

        public void Refresh(double height)
        {
            Subtask subtask = Subtask;
            if (subtask == null)
                return;

            Icon.Visibility = subtask.IsCompleted ? Visibility.Visible : Visibility.Collapsed;
            Text.Text = subtask.Text;
            Strike.Visibility = subtask.IsCompleted ? Visibility.Visible : Visibility.Collapsed;
            if (subtask.IsCompleted)
            {
                Text.Opacity = 0.45;
            }

            Height = height;

            Root.Height = height;
            Root.Margin = new Thickness(0.125 * height, 0, 0, 0);

            IconBorder.Width = height / 1.2;
            IconBorder.Height = height / 1.2;
            IconBorder.BorderThickness = new Thickness(height / 24.0);

            Icon.Width = height / 1.2;
            Icon.Height = height / 1.2;

            IconPath.Width = 30.39984 * (height / 48.0);
            IconPath.Height = 25.3332 * (height / 48.0);
            Canvas.SetLeft(IconPath, height / 9.6);
            Canvas.SetTop(IconPath, height / 6.0);

            Text.FontSize = height * 0.6;
            Text.Margin = new Thickness(height, height / 12.0, 0, 0);

            Strike.Margin = new Thickness(height, height / 12.0, 0, 0);
            Strike.BorderThickness = new Thickness(0, 0, 0, height / 24.0);
            ((CompositeTransform)Strike.RenderTransform).TranslateY = height / 48.0;
        }
    }
}
