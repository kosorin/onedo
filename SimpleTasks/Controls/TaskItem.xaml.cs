using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Core.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;
using SimpleTasks.Helpers;

namespace SimpleTasks.Controls
{
    [TemplateVisualState(Name = CompletedState, GroupName = CompleteStatesGroup)]
    [TemplateVisualState(Name = UncompletedState, GroupName = CompleteStatesGroup)]
    [TemplateVisualState(Name = GestureStartDragState, GroupName = GestureStatesGroup)]
    [TemplateVisualState(Name = GestureDragOkState, GroupName = GestureStatesGroup)]
    [TemplateVisualState(Name = GestureDragState, GroupName = GestureStatesGroup)]
    [TemplateVisualState(Name = GestureEndDragState, GroupName = GestureStatesGroup)]
    public partial class TaskItem : UserControl, INotifyPropertyChanged
    {
        #region Dependency Properties
        public TaskModel Task
        {
            get { return (TaskModel)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(TaskModel), typeof(TaskItem), new PropertyMetadata(null, TaskPropertyChanged));

        private static void TaskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskItem item = d as TaskItem;
            if (item != null)
            {
                item.UpdateVisualStates(CompletedState, false);
                item.UpdateVisualStates(GestureEndDragState, false);
            }
            //TaskModel task = e.NewValue as TaskModel;
            //if (task != null)
            //{
            //    task.PropertyChanged -= Task_PropertyChanged;
            //    task.PropertyChanged += Task_PropertyChanged;
            //}

            //TaskModel oldTask = e.OldValue as TaskModel;
            //if (oldTask != null)
            //{
            //    oldTask.PropertyChanged -= Task_PropertyChanged;
            //}
        }

        public double SwipeGestureTreshold
        {
            get { return (double)GetValue(SwipeGestureTresholdProperty); }
            set { SetValue(SwipeGestureTresholdProperty, value); }
        }
        public static readonly DependencyProperty SwipeGestureTresholdProperty =
            DependencyProperty.Register("SwipeGestureTreshold", typeof(double), typeof(TaskItem), new PropertyMetadata(105d));
        #endregion // end of Dependency Properties

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;

            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion // end of INotifyPropertyChanged

        #region Visual States
        private const string CompleteStatesGroup = "CompleteStates";

        private const string CompletedState = "Completed";

        private const string UncompletedState = "Uncompleted";

        private const string GestureStatesGroup = "GestureStates";

        private const string GestureStartDragState = "GestureStartDrag";

        private const string GestureDragOkState = "GestureDragOk";

        private const string GestureDragState = "GestureDrag";

        private const string GestureEndDragState = "GestureEndDrag";

        private void UpdateVisualStates(string state, bool useTransitions = true)
        {
            VisualStateManager.GoToState(this, state, useTransitions);
        }
        #endregion // end of Visual States

        #region Constructor
        public TaskItem()
        {
            InitializeComponent();
            UpdateVisualStates(CompletedState, false);
            UpdateVisualStates(GestureEndDragState, false);
        }
        #endregion // end of Constructor

        private void LOL_OMG(object a, object b)
        {

        }

        private void InfoGrid_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            UpdateVisualStates(GestureStartDragState);

            SwipeLeftGestureIcon.Style = GestureActionHelper.IconStyle(Settings.Current.Tasks.SwipeLeftAction);
            SwipeRightGestureIcon.Style = GestureActionHelper.IconStyle(Settings.Current.Tasks.SwipeRightAction);
        }

        private void InfoGrid_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            double value = e.TotalManipulation.Translation.X;
            if (Math.Abs(value) > SwipeGestureTreshold)
            {
                if (value < 0)
                {
                    // Swipe Left
                    //ExecuteGesture(Settings.Current.Tasks.SwipeLeftAction, task);
                }
                else
                {
                    // Swipe Right
                    //ExecuteGesture(Settings.Current.Tasks.SwipeRightAction, task);
                }
            }
            UpdateVisualStates(GestureEndDragState);
        }

        private void InfoGrid_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            RootTransform.X += e.DeltaManipulation.Translation.X;
            if (Settings.Current.Tasks.SwipeLeftAction == GestureAction.None && RootTransform.X < 0)
            {
                RootTransform.X = 0;
            }
            else if (Settings.Current.Tasks.SwipeRightAction == GestureAction.None && RootTransform.X > 0)
            {
                RootTransform.X = 0;
            }

            double value = Math.Abs(RootTransform.X);
            if (value > SwipeGestureTreshold)
            {
                UpdateVisualStates(GestureDragOkState);
            }
            else
            {
                UpdateVisualStates(GestureDragState);
            }

            double opacity = Math.Min(value / SwipeGestureTreshold, 1.0);
            SwipeLeftGestureIcon.Opacity = opacity;
            SwipeRightGestureIcon.Opacity = opacity;
        }

        private void SubtaskBorder_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Border border = (Border)sender;
            ContentControl swipeLeftIcon = (ContentControl)border.FindName("SwipeLeftGestureIcon");
            ContentControl swipeRightIcon = (ContentControl)border.FindName("SwipeRightGestureIcon");
            swipeLeftIcon.Style = GestureActionHelper.IconStyle(Settings.Current.Tasks.SwipeLeftAction);
            swipeRightIcon.Style = GestureActionHelper.IconStyle(Settings.Current.Tasks.SwipeRightAction);
        }

        private void SubtaskBorder_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Border border = (Border)sender;
            Storyboard storyboard = border.Resources["ResetTranslate"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
            border.Background = new SolidColorBrush(Colors.Transparent);

            double value = e.TotalManipulation.Translation.X;

            Subtask subtask = border.DataContext as Subtask;
            if (value < 0 && Math.Abs(value) > SwipeGestureTreshold)
            {
                // Swipe Left
                //ExecuteSubtaskGesture(Settings.Current.Tasks.SwipeLeftAction, subtask);
            }
            else if (value > 0 && Math.Abs(value) > SwipeGestureTreshold)
            {
                // Swipe Right
                //ExecuteSubtaskGesture(Settings.Current.Tasks.SwipeRightAction, subtask);
            }
        }

        private void SubtaskBorder_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Border border = (Border)sender;
            ContentControl swipeLeftIcon = (ContentControl)border.FindName("SwipeLeftGestureIcon");
            ContentControl swipeRightIcon = (ContentControl)border.FindName("SwipeRightGestureIcon");
            TranslateTransform t = (TranslateTransform)border.RenderTransform;

            t.X += e.DeltaManipulation.Translation.X;
            if (t.X < 0 && (Settings.Current.Tasks.SwipeLeftAction != GestureAction.Complete && Settings.Current.Tasks.SwipeLeftAction != GestureAction.Delete))
            {
                t.X = 0;
            }
            else if (t.X > 0 && (Settings.Current.Tasks.SwipeRightAction != GestureAction.Complete && Settings.Current.Tasks.SwipeRightAction != GestureAction.Delete))
            {
                t.X = 0;
            }

            double value = Math.Abs(t.X);
            double opacity = Math.Min(value / SwipeGestureTreshold, 1.0);
            Brush brush;
            if (value > SwipeGestureTreshold)
            {
                border.Background = new SolidColorBrush((Color)App.Current.Resources["SubtleColor"]) { Opacity = 0.30 };
                brush = (Brush)App.Current.Resources["AccentBrush"];
            }
            else
            {
                border.Background = null;
                brush = (Brush)App.Current.Resources["SubtleBrush"];
            }
            swipeLeftIcon.Opacity = opacity;
            swipeLeftIcon.Foreground = brush;
            swipeRightIcon.Opacity = opacity;
            swipeRightIcon.Foreground = brush;
        }
    }
}
