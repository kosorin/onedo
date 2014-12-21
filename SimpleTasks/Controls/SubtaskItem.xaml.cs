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
    public class SubtaskEventArgs : RoutedEventArgs
    {
        public Subtask Subtask { get; set; }

        public TaskModel DeleteFrom { get; set; }

        public SubtaskEventArgs(Subtask subtask)
        {
            Subtask = subtask;
            DeleteFrom = null;
        }
    }

    [TemplateVisualState(Name = CompletedState, GroupName = CompleteStatesGroup)]
    [TemplateVisualState(Name = UncompletedState, GroupName = CompleteStatesGroup)]
    [TemplateVisualState(Name = DeletedState, GroupName = DeleteStatesGroup)]
    [TemplateVisualState(Name = NotDeletedState, GroupName = DeleteStatesGroup)]
    [TemplateVisualState(Name = GestureStartDragState, GroupName = GestureStatesGroup)]
    [TemplateVisualState(Name = GestureDragOkState, GroupName = GestureStatesGroup)]
    [TemplateVisualState(Name = GestureDragState, GroupName = GestureStatesGroup)]
    [TemplateVisualState(Name = GestureEndDragState, GroupName = GestureStatesGroup)]
    public partial class SubtaskItem : UserControl, INotifyPropertyChanged
    {
        #region Events
        private void OnSubtaskEvent(EventHandler<SubtaskEventArgs> handler)
        {
            if (handler != null)
            {
                SubtaskEventArgs args = new SubtaskEventArgs(Subtask);
                handler(this, args);
                DeleteFrom = args.DeleteFrom;
            }
        }

        public event EventHandler<SubtaskEventArgs> SwipeLeft;
        private void OnSwipeLeft()
        {
            OnSubtaskEvent(SwipeLeft);
        }

        public event EventHandler<SubtaskEventArgs> SwipeRight;
        private void OnSwipeRight()
        {
            OnSubtaskEvent(SwipeRight);
        }

        public event EventHandler<SubtaskEventArgs> Check;
        private void OnCheck()
        {
            OnSubtaskEvent(Check);
        }

        public event EventHandler<SubtaskEventArgs> Click;
        private void OnClick()
        {
            OnSubtaskEvent(Click);
        }
        #endregion // end of Events

        #region Dependency Properties
        public Subtask Subtask
        {
            get { return (Subtask)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Subtask", typeof(Subtask), typeof(SubtaskItem), new PropertyMetadata(null, TaskPropertyChanged));
        private static void TaskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubtaskItem item = d as SubtaskItem;
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
            DependencyProperty.Register("IsCompleted", typeof(bool), typeof(SubtaskItem), new PropertyMetadata(false, IsCompletedChanged));
        private static void IsCompletedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubtaskItem item = d as SubtaskItem;
            if (item != null)
            {
                item.UpdateVisualState((bool)e.NewValue ? CompletedState : UncompletedState);
            }
        }

        public double SwipeGestureTreshold
        {
            get { return (double)GetValue(SwipeGestureTresholdProperty); }
            set { SetValue(SwipeGestureTresholdProperty, value); }
        }
        public static readonly DependencyProperty SwipeGestureTresholdProperty =
            DependencyProperty.Register("SwipeGestureTreshold", typeof(double), typeof(SubtaskItem), new PropertyMetadata(105d));

        public TaskModel DeleteFrom
        {
            get { return (TaskModel)GetValue(DeleteFromProperty); }
            set { SetValue(DeleteFromProperty, value); }
        }
        public static readonly DependencyProperty DeleteFromProperty =
            DependencyProperty.Register("DeleteFrom", typeof(TaskModel), typeof(SubtaskItem), new PropertyMetadata(null, DeleteFromChanged));
        private static void DeleteFromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubtaskItem item = d as SubtaskItem;
            if (item != null)
            {
                item.UpdateVisualState(e.NewValue != null ? DeletedState : NotDeletedState);
            }
        }

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

        private const string DeleteStatesGroup = "DeleteStates";

        private const string DeletedState = "Deleted";

        private const string NotDeletedState = "NotDeleted";

        private const string GestureStatesGroup = "GestureStates";

        private const string GestureStartDragState = "GestureStartDrag";

        private const string GestureDragOkState = "GestureDragOk";

        private const string GestureDragState = "GestureDrag";

        private const string GestureEndDragState = "GestureEndDrag";

        private void UpdateVisualState(string state, bool useTransitions = true)
        {
            VisualStateManager.GoToState(this, state, useTransitions);
        }

        private void UpdateVisualStates(bool useTransitions = true)
        {
            UpdateVisualState((Subtask != null && Subtask.IsCompleted) ? CompletedState : UncompletedState, useTransitions);
            UpdateVisualState(DeleteFrom != null ? DeletedState : NotDeletedState);
            UpdateVisualState(GestureEndDragState, useTransitions);
        }
        #endregion // end of Visual States

        #region Constructor
        public SubtaskItem()
        {
            InitializeComponent();
            UpdateVisualStates(false);
        }
        #endregion // end of Constructor

        #region Event Handlers
        private void Subtask_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            UpdateVisualState(GestureStartDragState);

            SwipeLeftGestureIcon.Style = GestureActionHelper.IconStyle(Settings.Current.Tasks.SwipeLeftAction);
            SwipeRightGestureIcon.Style = GestureActionHelper.IconStyle(Settings.Current.Tasks.SwipeRightAction);
            SwipeLeftGestureIcon.Visibility = Visibility.Visible;
            SwipeRightGestureIcon.Visibility = Visibility.Visible;
            SwipeLeftGestureIcon.Opacity = 0;
            SwipeRightGestureIcon.Opacity = 0;
            BackgroundBorder.Opacity = 0;
            RootTransform.X = 0;
        }

        private void Subtask_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            double value = RootTransform.X;
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

        private void GestureEndStoryboard_Completed(object sender, EventArgs e)
        {
            SwipeLeftGestureIcon.Visibility = Visibility.Collapsed;
            SwipeRightGestureIcon.Visibility = Visibility.Collapsed;
            Debug.WriteLine("SUB GEST END");
        }

        private void Subtask_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            RootTransform.X += e.DeltaManipulation.Translation.X;
            if ((Settings.Current.Tasks.SwipeLeftAction != GestureAction.Complete && Settings.Current.Tasks.SwipeLeftAction != GestureAction.Delete) && RootTransform.X < 0)
            {
                RootTransform.X = 0;
            }
            else if ((Settings.Current.Tasks.SwipeRightAction != GestureAction.Complete && Settings.Current.Tasks.SwipeRightAction != GestureAction.Delete) && RootTransform.X > 0)
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
            SwipeLeftGestureIcon.Opacity = RootTransform.X < 0 ? opacity : 0;
            SwipeRightGestureIcon.Opacity = RootTransform.X > 0 ? opacity : 0;
        }

        private void Checkbox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            OnCheck();
        }

        private void Subtask_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            OnClick();
        }

        private void DeleteStoryboard_Completed(object sender, EventArgs e)
        {
            if (DeleteFrom != null)
            {
                DeleteFrom.Subtasks.Remove(Subtask);
            }
        }

        #endregion // end of Event Handlers
    }
}
