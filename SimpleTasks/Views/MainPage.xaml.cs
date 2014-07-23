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

        #region AppBar create
        private ApplicationBarIconButton appBarNewTaskButton;

        private ApplicationBarIconButton appBarSaveQuickButton;

        private ApplicationBarIconButton appBarCancelQuickButton;

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

            appBarCancelQuickButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.close.png", UriKind.Relative));
            appBarCancelQuickButton.Text = AppResources.AppBarCancel;
            appBarCancelQuickButton.Click += QuickAddCancel;
            #endregion

            #region Menu

            appBarMenuItems = new List<ApplicationBarMenuItem>();
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

#if DEBUG
            // Reminders
            ApplicationBarMenuItem appBarRemindersMenuItem = new ApplicationBarMenuItem("seznam připomenutí");
            appBarRemindersMenuItem.Click += RemindersMenuItem_Click;
            appBarMenuItems.Add(appBarRemindersMenuItem);

            // Reset
            ApplicationBarMenuItem appBarResetMenuItem = new ApplicationBarMenuItem("resetovat data");
            appBarResetMenuItem.Click += ResetMenuItem_Click;
            appBarMenuItems.Add(appBarResetMenuItem);

            // Clear
            ApplicationBarMenuItem appBarClearMenuItem = new ApplicationBarMenuItem("smazat data");
            appBarClearMenuItem.Click += (s, e) => { App.Tasks.DeleteAll(); };
            appBarMenuItems.Add(appBarClearMenuItem);
#endif
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
            ApplicationBar.Buttons.Add(appBarCancelQuickButton);
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

        void QuickAddCancel(object sender, EventArgs e)
        {
            QuickAddTextBox.Text = "";
            this.Focus();
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

            App.Tasks.Add(new TaskModel()
            {
                Title = "Buy milk",
                Completed = DateTime.Now
            });
            App.Tasks.Add(new TaskModel()
            {
                Title = "Call Chuck",
                DueDate = DateTimeExtensions.Today.AddHours(13).AddMinutes(00),
                Reminder = TimeSpan.FromHours(4),
                Priority = TaskPriority.High
            });
            App.Tasks.Add(new TaskModel()
            {
                Title = "Walk the dog",
                DueDate = DateTimeExtensions.Today.AddHours(18).AddMinutes(45)
            });
            App.Tasks.Add(new TaskModel()
            {
                Title = "Math project",
                DueDate = DateTimeExtensions.Today.AddDays(9).AddHours(7).AddMinutes(30)
            });

            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "cs")
            {
                App.Tasks.Tasks[0].Title = "Koupit mléko";
                App.Tasks.Tasks[1].Title = "Zavolat Honzovi";
                App.Tasks.Tasks[2].Title = "Vyvenčit psa";
                App.Tasks.Tasks[3].Title = "Projekt do matematiky";
            }
            else if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "sk")
            {
                App.Tasks.Tasks[0].Title = "Kúpiť mlieko";
                App.Tasks.Tasks[1].Title = "Zavolať Danovi";
                App.Tasks.Tasks[2].Title = "Vyvenčiť psa";
                App.Tasks.Tasks[3].Title = "Projekt z matematiky";
            }
        }
#endif

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

        #endregion

        #region TasksList
        private void CompleteButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.Focus();

            ToggleComplete((sender as MyToggleButton).DataContext as TaskWrapper);
        }

        private void ToggleComplete(TaskWrapper wrapper)
        {
            if (wrapper == null)
                return;
            TaskModel task = wrapper.Task;
            if (task == null)
                return;

            if (task.IsActive)
            {
                // DOKONČENÍ
                task.Completed = DateTime.Now;
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

        private void TasksLongListSelector_Loaded(object sender, RoutedEventArgs e)
        {
            // Změnění margin scrollbaru. 
            // Při větším počtu úkolů se automaticky měnila šířka celého LLS.
            ScrollBar scrollBar = ((FrameworkElement)VisualTreeHelper.GetChild(TasksLongListSelector, 0)).FindName("VerticalScrollBar") as ScrollBar;
            if (scrollBar != null)
                scrollBar.Margin = new Thickness(-10, 0, 0, 0);
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

        private void RootBorder_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
        }

        private void RootBorder_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Border border = (Border)sender;
            Storyboard storyboard = border.Resources["ResetTranslate"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
            border.Background = null;

            double value = e.TotalManipulation.Translation.X;
            if (_canUseGestures && value < _completeGestureTreshold)
            {
                VibrateController.Default.Start(TimeSpan.FromSeconds(0.05));
                ToggleComplete(border.DataContext as TaskWrapper);
            }
        }

        private void RootBorder_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Border border = (Border)sender;
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
        #endregion

        #region Context Menu
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            _canUseGestures = false;
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            _canUseGestures = true;
        }

        private void TodayMenuItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TaskModel task = (TaskModel)((TaskWrapper)((TaskListMenuItem)sender).DataContext).Task;
            task.DueDate = DateTimeExtensions.Tomorrow.AddMinutes(-1);
            App.Tasks.Update(task);
        }

        private void TomorrowMenuItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TaskModel task = (TaskModel)((TaskWrapper)((TaskListMenuItem)sender).DataContext).Task;
            task.DueDate = DateTimeExtensions.Tomorrow.AddDays(1).AddMinutes(-1);
            App.Tasks.Update(task);
        }

        private void NextWeekMenuItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TaskModel task = (TaskModel)((TaskWrapper)((TaskListMenuItem)sender).DataContext).Task;
            task.DueDate = DateTimeExtensions.LastDayOfNextWeek.AddDays(1).AddMinutes(-1);
            App.Tasks.Update(task);
        }

        private void SomedayMenuItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TaskModel task = (TaskModel)((TaskWrapper)((TaskListMenuItem)sender).DataContext).Task;
            task.DueDate = null;
            App.Tasks.Update(task);
        }
        #endregion

    }
}