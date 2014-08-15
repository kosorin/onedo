using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.ViewModels;
using SimpleTasks.Resources;
using System.Windows;
using SimpleTasks.Models;
using SimpleTasks.Core.Helpers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Diagnostics;
using SimpleTasks.Core.Models;
using Microsoft.Phone.Scheduler;
using SimpleTasks.Helpers;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Threading;
using Microsoft.Devices;
using SimpleTasks.Controls;
using Microsoft.Phone.Tasks;
using System.Globalization;
using System.Collections.ObjectModel;

namespace SimpleTasks.Views
{
    public partial class MainPage : BasePage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.Tasks;

            CreateAppBarItems();
            BuildTasksdAppBar();

            if (App.IsFirstStart)
            {
                Loaded += FirstStart_Loaded;
            }
        }

        void FirstStart_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= FirstStart_Loaded;
            if (App.IsFirstStart)
            {
                App.IsFirstStart = false;
                ChangelogCategory changelog = AboutPage.LoadChangelog()[1];
                string text = string.Format("{0} ({1})\n\n", string.Format(AppResources.AboutVersion, changelog.Version), changelog.Date.ToShortDateString());
                foreach (ChangelogItem item in changelog)
                {
                    text += "  • " + item.Text + System.Environment.NewLine;
                }

                MessageBox.Show(text, AppResources.WhatsNew, MessageBoxButton.OK);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationService.RemoveBackEntry();

            App.Tasks.OnPropertyChanged(App.Tasks.GroupedTasksPropertyString);

            App.Tracker.SendView("MainPage");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (!e.IsNavigationInitiator)
            {
                App.UpdateAllLiveTiles();
            }
        }

        #region Other
        T FindFirstChild<T>(FrameworkElement element, string name = null) where T : FrameworkElement
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            var children = new FrameworkElement[childrenCount];

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                children[i] = child;
                if (child is T)
                {
                    if (name == null || child.Name == name)
                        return (T)child;
                }
            }

            for (int i = 0; i < childrenCount; i++)
            {
                if (children[i] != null)
                {
                    var subChild = FindFirstChild<T>(children[i]);
                    if (subChild != null)
                        return subChild;
                }
            }

            return null;
        }
        #endregion

        #region AppBar
        #region AppBar Create
        private ApplicationBarIconButton appBarNewTaskButton;

        private ApplicationBarIconButton appBarSaveQuickButton;

        private List<ApplicationBarMenuItem> appBarMenuItems;

        private void CreateAppBarItems()
        {
            #region Ikony
            appBarNewTaskButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative));
            appBarNewTaskButton.Text = AppResources.AppBarNew;
            appBarNewTaskButton.Click += AddNewTask_Click;

            appBarSaveQuickButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.save.png", UriKind.Relative));
            appBarSaveQuickButton.Text = AppResources.AppBarSave;
            appBarSaveQuickButton.Click += QuickAddSave;
            #endregion

            #region Menu
            appBarMenuItems = new List<ApplicationBarMenuItem>();

#if DEBUG
            // Reminders
            ApplicationBarMenuItem appBarRemindersMenuItem = new ApplicationBarMenuItem("seznam připomenutí");
            appBarRemindersMenuItem.Click += RemindersMenuItem_Click;
            appBarMenuItems.Add(appBarRemindersMenuItem);

            // Reset
            ApplicationBarMenuItem appBarResetMenuItem = new ApplicationBarMenuItem("resetovat data");
            appBarResetMenuItem.Click += ResetMenuItem_Click;
            appBarMenuItems.Add(appBarResetMenuItem);
#endif

            // Smazat dokončené úkoly
            ApplicationBarMenuItem appBarDeleteCompletedItem = new ApplicationBarMenuItem(AppResources.AppBarDeleteCompleted);
            appBarDeleteCompletedItem.Click += (s, e) => { OverlayAction(App.Tasks.DeleteCompleted); };
            appBarMenuItems.Add(appBarDeleteCompletedItem);

            // Smazat všechny úkoly
            ApplicationBarMenuItem appBarDeleteAllItem = new ApplicationBarMenuItem(AppResources.AppBarDeleteAll);
            appBarDeleteAllItem.Click += DeleteAllItem_Click;
            appBarMenuItems.Add(appBarDeleteAllItem);

            // Nastavení
            ApplicationBarMenuItem appBarSettingsMenuItem = new ApplicationBarMenuItem(AppResources.AppBarSettings);
            appBarSettingsMenuItem.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative)); };
            appBarMenuItems.Add(appBarSettingsMenuItem);

            // O aplikaci
            ApplicationBarMenuItem appBarAboutMenuItem = new ApplicationBarMenuItem(AppResources.AppBarAbout);
            appBarAboutMenuItem.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/AboutPage.xaml", UriKind.Relative)); };
            appBarMenuItems.Add(appBarAboutMenuItem);
            #endregion
        }

        private void BuildTasksdAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Buttons.Add(appBarNewTaskButton);
            foreach (var item in appBarMenuItems)
            {
                ApplicationBar.MenuItems.Add(item);
            }
        }

        private void BuildQuickAddAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Buttons.Add(appBarSaveQuickButton);
        }
        #endregion

        private void AddNewTask_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/EditTaskPage.xaml", UriKind.Relative));
        }

        void QuickAddSave(object sender, EventArgs e)
        {
            string title = QuickAddTextBox.Text;
            if (!string.IsNullOrWhiteSpace(title))
            {
                TaskModel task = new TaskModel()
                {
                    Title = title,
                    DueDate = App.Settings.Tasks.DefaultDate
                };
                if (task.DueDate != null)
                {
                    DateTime defaultTime = App.Settings.Tasks.DefaultTime;
                    task.DueDate = task.DueDate.Value.AddHours(defaultTime.Hour).AddMinutes(defaultTime.Minute);
                }

                App.Tasks.Add(task);
                QuickAddTextBox.Text = "";
                this.Focus();
            }
        }

        void DeleteAllItem_Click(object sender, EventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = AppResources.DeleteAllTasksCaption,
                Message = AppResources.DeleteAllTasks,
                LeftButtonContent = AppResources.DeleteTaskYes,
                RightButtonContent = AppResources.DeleteTaskNo
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                case CustomMessageBoxResult.LeftButton:
                    OverlayAction(App.Tasks.DeleteAll);
                    break;
                case CustomMessageBoxResult.RightButton:
                case CustomMessageBoxResult.None:
                default:
                    break;
                }
            };


            messageBox.Show();
        }

        private void OverlayAction(Action action)
        {
            PageOverlayTransitionShow.Begin();
            EventHandler overlayHandler = null;
            overlayHandler = (s, e) =>
            {
                action();
                PageOverlayTransitionHide.Begin();
                PageOverlayTransitionShow.Completed -= overlayHandler;
            };
            PageOverlayTransitionShow.Completed += overlayHandler;
        }

#if DEBUG
        private void RemindersMenuItem_Click(object sender, EventArgs e)
        {
            string s = string.Format("> Reminders ({0})", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            foreach (var r in ScheduledActionService.GetActions<Microsoft.Phone.Scheduler.Reminder>())
            {
                s += string.Format("\n{0}\n  {1} -  {2}", r.Name, r.IsScheduled, r.BeginTime);
            }
            MessageBox.Show(s);
        }

        void ResetMenuItem_Click(object sender, EventArgs e)
        {
            App.Tasks.DeleteAll();

            // == Zátěžový test ==
            //Random r = new Random();
            //for (int i = 0; i < 500; i++)
            //{
            //    Debug.WriteLine(i);
            //    TaskModel task = new TaskModel();
            //    task.Title = System.IO.Path.GetRandomFileName();
            //    if (r.Next(2) == 1)
            //    {
            //        task.Detail = "";
            //        for (int j = 0; j < r.Next(10); j++)
            //        {
            //            task.Detail += System.IO.Path.GetRandomFileName();
            //        }
            //    }
            //    if (r.Next(4) == 1)
            //    {
            //        task.DueDate = new DateTime(2014, 7, 25).AddDays(r.Next(100)).AddHours(r.Next(24)).AddMinutes(r.Next(60));
            //    }
            //    if (task.HasDueDate && r.Next(3) == 1)
            //    {
            //        task.Reminder = TimeSpan.FromMinutes(r.Next(36000));
            //    }
            //    if (r.Next(8) == 1)
            //    {
            //        for (int j = 0; j < r.Next(10); j++)
            //        {
            //            task.Subtasks.Add(new Subtask(System.IO.Path.GetRandomFileName(), r.Next(3) == 1));
            //        }
            //    }
            //    int priority = r.Next(3);
            //    switch (priority)
            //    {
            //    case 0: task.Priority = TaskPriority.Low; break;
            //    case 1: task.Priority = TaskPriority.High; break;
            //    default:
            //    case 2: task.Priority = TaskPriority.Normal; break;
            //    }
            //    App.Tasks.Add(task);
            //}
            //return;

            App.Tasks.Add(new TaskModel()
            {
                Title = "Grocery list",
                DueDate = DateTimeExtensions.Tomorrow.AddHours(10).AddMinutes(30),
                Subtasks = new ObservableCollection<Subtask>
                { 
                    new Subtask("milk"), 
                    new Subtask("apples", true),
                    new Subtask("potatoes"),
                }
            });
            App.Tasks.Add(new TaskModel()
            {
                Title = "Math project",
                DueDate = DateTimeExtensions.Today.AddDays(9).AddHours(7).AddMinutes(30),
                Reminder = TimeSpan.FromDays(1),
                Priority = TaskPriority.High,
                Color = Color.FromArgb(255, 229, 20, 0)
            });
            App.Tasks.Add(new TaskModel()
            {
                Title = "Call Chuck",
                Completed = DateTime.Now
            });

            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "cs")
            {
                App.Tasks.Tasks[0].Title = "Seznam potravin";
                App.Tasks.Tasks[0].Subtasks[0].Text = "mléko";
                App.Tasks.Tasks[0].Subtasks[1].Text = "jablka";
                App.Tasks.Tasks[0].Subtasks[2].Text = "brambory";
                App.Tasks.Tasks[1].Title = "Projekt do matematiky";
                App.Tasks.Tasks[2].Title = "Zavolat Honzovi";
            }
            else if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "sk")
            {
                App.Tasks.Tasks[0].Title = "Zoznam potravín";
                App.Tasks.Tasks[0].Subtasks[0].Text = "mlieko";
                App.Tasks.Tasks[0].Subtasks[1].Text = "jablká";
                App.Tasks.Tasks[0].Subtasks[2].Text = "zemiaky";
                App.Tasks.Tasks[1].Title = "Projekt z matematiky";
                App.Tasks.Tasks[2].Title = "Zavolať Danovi";
            }

        }
#endif
        #endregion

        #region TasksList
        private void CompleteButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.Focus();

            ToggleComplete(sender as FrameworkElement);
        }

        private void ToggleComplete(FrameworkElement element)
        {
            TaskWrapper wrapper = element.DataContext as TaskWrapper;
            if (wrapper == null)
                return;
            TaskModel task = wrapper.Task;
            if (task == null)
                return;

            if (task.IsActive)
            {
                // DOKONČENÍ
                task.Completed = DateTime.Now;
                if (App.Settings.Tasks.CompleteSubtasks)
                {
                    foreach (Subtask subtask in task.Subtasks)
                    {
                        subtask.IsCompleted = true;
                    }
                }
                if (App.Settings.Tiles.UnpinCompleted)
                {
                    LiveTile.Unpin(task);
                }
            }
            else
            {
                // AKTIVOVÁNÍ
                task.Completed = null;
            }
            App.Tasks.Update(task);

            wrapper.UpdateIsScheduled();
        }

        private void ToggleSubtaskComplete(FrameworkElement element)
        {
            if (element != null)
            {
                Subtask subtask = element.DataContext as Subtask;
                if (subtask != null)
                {
                    subtask.IsCompleted = !subtask.IsCompleted;
                }
            }
        }

        private void TaskListItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid grid = (Grid)sender;
            TaskWrapper wrapper = (TaskWrapper)grid.DataContext;
            TaskModel task = wrapper.Task;
            if (task == null)
                return;

            App.Tracker.SendEvent("EvCategory", "EvAction", "task edit", task.GetHashCode());
            NavigationService.Navigate(new Uri(string.Format("/Views/EditTaskPage.xaml?Task={0}", task.Uid), UriKind.Relative));
        }

        private void SubtaskCheckbox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ToggleSubtaskComplete(sender as FrameworkElement);
        }

        private void SubtaskText_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Subtask subtask = element.DataContext as Subtask;
            while (element != null)
            {
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
                if (element is ItemsControl)
                    break;
            }
            ItemsControl itemsControl = element as ItemsControl;
            TaskWrapper wrapper = (TaskWrapper)itemsControl.DataContext;
            TaskModel task = wrapper.Task;
            if (task == null)
                return;

            BasePickerPage.Navigate("Subtasks", task.Subtasks);
        }
        #endregion

        #region QuickAddTextBox
        SupportedPageOrientation orientation;

        private void QuickAddTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PageOverlayTransitionQuickHide.Begin();
            BuildTasksdAppBar();

            SupportedOrientations = orientation;
        }

        private void QuickAddTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PageOverlayTransitionQuickShow.Begin();
            BuildQuickAddAppBar();

            orientation = SupportedOrientations;
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
        }
        #endregion

        #region Gestures
        private double _completeGestureTreshold = -105;

        private bool _canUseGestures = true;

        private void InfoGrid_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
        }

        private void InfoGrid_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Grid infoGrid = (Grid)sender;
            Border border = (Border)infoGrid.FindName("RootBorder");
            Storyboard storyboard = border.Resources["ResetTranslate"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
            border.Background = null;

            double value = e.TotalManipulation.Translation.X;
            if (_canUseGestures && value < _completeGestureTreshold)
            {
                VibrateHelper.Short();
                ToggleComplete(border);
            }
        }

        private void InfoGrid_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Grid infoGrid = (Grid)sender;
            Border border = (Border)infoGrid.FindName("RootBorder");
            ContentControl icon = (ContentControl)border.FindName("CompleteGestureIcon");
            TranslateTransform t = (TranslateTransform)border.RenderTransform;

            t.X += e.DeltaManipulation.Translation.X;
            if (t.X > 0)
            {
                t.X = 0;
            }

            if (t.X < _completeGestureTreshold)
            {
                border.Background = new SolidColorBrush((Color)CurrentApp.Resources["SubtleColor"]) { Opacity = 0.30 };
                icon.Foreground = (Brush)CurrentApp.Resources["PhoneAccentBrush"];
            }
            else
            {
                border.Background = null;
                icon.Foreground = (Brush)CurrentApp.Resources["SubtleBrush"];
            }
        }

        private void SubtaskBorder_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
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
            if (_canUseGestures && value < _completeGestureTreshold)
            {
                VibrateHelper.Short();
                ToggleSubtaskComplete(border);
            }
        }

        private void SubtaskBorder_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Border border = (Border)sender;
            TranslateTransform t = (TranslateTransform)border.RenderTransform;

            t.X += e.DeltaManipulation.Translation.X;
            if (t.X > 0)
            {
                t.X = 0;
            }

            if (t.X < _completeGestureTreshold)
            {
                border.Background = new SolidColorBrush((Color)CurrentApp.Resources["SubtleColor"]) { Opacity = 0.30 };
            }
            else
            {
                border.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
        #endregion
    }
}