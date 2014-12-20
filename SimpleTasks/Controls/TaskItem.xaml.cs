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
using SimpleTasks.Models;

namespace SimpleTasks.Controls
{
    public class TaskEventArgs : RoutedEventArgs
    {
        public TaskModel Task { get; set; }

        public TaskEventArgs(TaskModel task)
        {
            Task = task;
        }
    }

    [TemplateVisualState(Name = CompletedState, GroupName = CompleteStatesGroup)]
    [TemplateVisualState(Name = UncompletedState, GroupName = CompleteStatesGroup)]
    [TemplateVisualState(Name = ScheduledState, GroupName = ScheduledStatesGroup)]
    [TemplateVisualState(Name = NotScheduledState, GroupName = ScheduledStatesGroup)]
    [TemplateVisualState(Name = GestureStartDragState, GroupName = GestureStatesGroup)]
    [TemplateVisualState(Name = GestureDragOkState, GroupName = GestureStatesGroup)]
    [TemplateVisualState(Name = GestureDragState, GroupName = GestureStatesGroup)]
    [TemplateVisualState(Name = GestureEndDragState, GroupName = GestureStatesGroup)]
    public partial class TaskItem : UserControl, INotifyPropertyChanged
    {
        #region Events
        private void OnTaskEvent(EventHandler<TaskEventArgs> handler)
        {
            if (handler != null)
            {
                handler(this, new TaskEventArgs(Task));
            }
        }

        public event EventHandler<TaskEventArgs> SwipeLeft;
        private void OnSwipeLeft()
        {
            OnTaskEvent(SwipeLeft);
        }

        public event EventHandler<TaskEventArgs> SwipeRight;
        private void OnSwipeRight()
        {
            OnTaskEvent(SwipeRight);
        }

        public event EventHandler<TaskEventArgs> Check;
        private void OnCheck()
        {
            OnTaskEvent(Check);
        }

        public event EventHandler<TaskEventArgs> Click;
        private void OnClick()
        {
            OnTaskEvent(Click);
        }
        #endregion // end of Events

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
                item.UpdateVisualStates(false);
            }
        }

        public bool IsCompleted
        {
            get { return (bool)GetValue(IsCompletedProperty); }
            set { SetValue(IsCompletedProperty, value); }
        }
        public static readonly DependencyProperty IsCompletedProperty =
            DependencyProperty.Register("IsCompleted", typeof(bool), typeof(TaskItem), new PropertyMetadata(false, IsCompletedChanged));
        private static void IsCompletedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskItem item = d as TaskItem;
            if (item != null)
            {
                item.UpdateVisualState((bool)e.NewValue ? CompletedState : UncompletedState);
            }
        }

        public bool IsScheduled
        {
            get { return (bool)GetValue(IsScheduledProperty); }
            set { SetValue(IsScheduledProperty, value); }
        }
        public static readonly DependencyProperty IsScheduledProperty =
            DependencyProperty.Register("IsScheduled", typeof(bool), typeof(TaskItem), new PropertyMetadata(true, IsScheduledChanged));
        private static void IsScheduledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TaskItem item = d as TaskItem;
            if (item != null)
            {
                item.UpdateVisualState((bool)e.NewValue ? ScheduledState : NotScheduledState);
            }
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

        private const string ScheduledStatesGroup = "ScheduledStates";

        private const string ScheduledState = "Scheduled";

        private const string NotScheduledState = "NotScheduled";

        private void UpdateVisualState(string state, bool useTransitions = true)
        {
            VisualStateManager.GoToState(this, state, useTransitions);
        }

        private void UpdateVisualStates(bool useTransitions = true)
        {
            UpdateVisualState((Task != null && Task.IsCompleted) ? CompletedState : UncompletedState, useTransitions);
            UpdateVisualState((Task != null && Task.GetWrapper() != null && Task.GetWrapper().IsScheduled) ? ScheduledState : NotScheduledState, useTransitions);
            UpdateVisualState(GestureEndDragState, useTransitions);
        }
        #endregion // end of Visual States

        #region Constructor
        public TaskItem()
        {
            InitializeComponent();
            UpdateVisualStates(false);
            Debug.WriteLine("CONSTRUCT");
        }
        #endregion // end of Constructor

        #region Event Handlers
        private void LOL_OMG(object a, object b)
        {

        }

        private void InfoGrid_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            UpdateVisualState(GestureStartDragState);

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
                    OnSwipeLeft();
                }
                else
                {
                    // Swipe Right
                    OnSwipeRight();
                }
            }
            UpdateVisualState(GestureEndDragState);
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
                UpdateVisualState(GestureDragOkState);
            }
            else
            {
                UpdateVisualState(GestureDragState);
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

        private void Checkbox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            OnCheck();
        }

        private void Task_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            OnClick();
        }
        #endregion // end of Event Handlers
    }
}
