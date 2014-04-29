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

namespace SimpleTasks.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.Tasks;

            BuildAppBar();
            TasksSlideView.SelectedIndex = _tasksSlideViewIndex;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationService.RemoveBackEntry();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (!e.IsNavigationInitiator)
            {
                LiveTile.UpdateOrReset(App.Settings.EnableLiveTileSetting, App.Tasks.Tasks);
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TasksSlideView.SelectedIndex == _tagsSlideViewIndex)
            {
                TasksSlideView.SelectedIndex = _tasksSlideViewIndex;
                e.Cancel = true;
            }
        }

        #region AppBar

        private ApplicationBarIconButton appBarNewTaskButton;

        private ApplicationBarIconButton appBarSaveQuickButton;

        private ApplicationBarIconButton appBarCancelQuickButton;

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            #region Ikony

            // Přidat úkol
            appBarNewTaskButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative));
            appBarNewTaskButton.Text = AppResources.AppBarNew;
            appBarNewTaskButton.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/EditTaskPage.xaml", UriKind.Relative)); };
            ApplicationBar.Buttons.Add(appBarNewTaskButton);

            // Uložit rychlý úkol
            appBarSaveQuickButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.save.png", UriKind.Relative));
            appBarSaveQuickButton.Text = AppResources.AppBarSave;
            appBarSaveQuickButton.Click += (s, e) =>
            {
                string title = QuickAddTextBox.Text;
                if (!string.IsNullOrWhiteSpace(title))
                {
                    TaskModel task = new TaskModel() { Title = title };
                    App.Tasks.Add(task);
                    QuickAddTextBox.Text = "";
                    this.Focus();
                    TasksLongListSelector.ScrollTo(task);
                }
            };

            // Zrušit rychlý úkol
            appBarCancelQuickButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.close.png", UriKind.Relative));
            appBarCancelQuickButton.Text = AppResources.AppBarCancel;
            appBarCancelQuickButton.Click += (s, e) => { QuickAddTextBox.Text = ""; this.Focus(); };

            #endregion

            #region Menu

            // Smazat dokončené úkoly
            ApplicationBarMenuItem appBarDeleteCompletedItem = new ApplicationBarMenuItem(AppResources.AppBarDeleteCompleted);
            appBarDeleteCompletedItem.Click += (s, e) => { OverlayAction(App.Tasks.DeleteCompleted); };
            ApplicationBar.MenuItems.Add(appBarDeleteCompletedItem);

            // Smazat všechny úkoly
            ApplicationBarMenuItem appBarDeleteAllItem = new ApplicationBarMenuItem(AppResources.AppBarDeleteAll);
            appBarDeleteAllItem.Click += appBarDeleteAllItem_Click;
            ApplicationBar.MenuItems.Add(appBarDeleteAllItem);

            // Nastavení
            ApplicationBarMenuItem appBarSettingsMenuItem = new ApplicationBarMenuItem(AppResources.AppBarSettings);
            appBarSettingsMenuItem.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(appBarSettingsMenuItem);

            // O aplikaci
            ApplicationBarMenuItem appBarAboutMenuItem = new ApplicationBarMenuItem(AppResources.AppBarAbout);
            appBarAboutMenuItem.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/AboutPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(appBarAboutMenuItem);

#if DEBUG
            // Reset
            ApplicationBarMenuItem appBarResetMenuItem = new ApplicationBarMenuItem("resetovat data");
            appBarResetMenuItem.Click += appBarResetMenuItem_Click;
            ApplicationBar.MenuItems.Add(appBarResetMenuItem);

            // Clear
            ApplicationBarMenuItem appBarClearMenuItem = new ApplicationBarMenuItem("smazat data");
            appBarClearMenuItem.Click += (s, e) => { App.Tasks.DeleteAll(); };
            ApplicationBar.MenuItems.Add(appBarClearMenuItem);
#endif
            #endregion
        }

        void appBarDeleteAllItem_Click(object sender, EventArgs e)
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

        void appBarResetMenuItem_Click(object sender, EventArgs e)
        {
            App.Tasks.DeleteAll();

            if (App.ForceDebugCulture == "en-US")
            {
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Go to the dentist",
                    DueDate = DateTimeExtensions.Today.AddDays(2)
                });
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Call Chuck",
                    DueDate = DateTimeExtensions.Today.AddDays(0),
                    ReminderDate = DateTimeExtensions.Today.AddHours(21).AddMinutes(13),
                    Priority = TaskPriority.Low
                });
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Go to cinema",
                    Detail = "Amazing Spider-Man 2 or X-Men: Days of Future Past",
                    ReminderDate = DateTimeExtensions.Today.AddHours(65).AddMinutes(27),
                });
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Math project",
                    DueDate = DateTimeExtensions.Today.AddDays(9),
                    ReminderDate = DateTimeExtensions.Today.AddDays(4).AddHours(13).AddMinutes(42),
                    Priority = TaskPriority.High
                });
            }
            else if (App.ForceDebugCulture == "cs-CZ")
            {
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Jít k zubaři",
                    DueDate = DateTimeExtensions.Today.AddDays(2)
                });
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Zavolat Honzovi",
                    DueDate = DateTimeExtensions.Today.AddDays(0),
                    ReminderDate = DateTimeExtensions.Today.AddHours(21).AddMinutes(13),
                    Priority = TaskPriority.Low
                });
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Jít do kina",
                    Detail = "Amazing Spider-Man 2 nebo X-Men: Days of Future Past",
                    ReminderDate = DateTimeExtensions.Today.AddHours(65).AddMinutes(27),
                });
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Projekt do matematiky",
                    DueDate = DateTimeExtensions.Today.AddDays(9),
                    ReminderDate = DateTimeExtensions.Today.AddDays(4).AddHours(13).AddMinutes(42),
                    Priority = TaskPriority.High
                });
            }
            else if (App.ForceDebugCulture == "sk-SK")
            {
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Ísť k zubárovi",
                    DueDate = DateTimeExtensions.Today.AddDays(2)
                });
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Kúpiť mlieko",
                    DueDate = DateTimeExtensions.Today.AddDays(0),
                    ReminderDate = DateTimeExtensions.Today.AddHours(21).AddMinutes(13),
                    Priority = TaskPriority.Low
                });
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Ísť do kina",
                    Detail = "Amazing Spider-Man 2 alebo X-Men: Days of Future Past",
                    ReminderDate = DateTimeExtensions.Today.AddHours(65).AddMinutes(27),
                });
                App.Tasks.Add(new TaskModel()
                {
                    Title = "Projekt z matematiky",
                    DueDate = DateTimeExtensions.Today.AddDays(9),
                    ReminderDate = DateTimeExtensions.Today.AddDays(4).AddHours(13).AddMinutes(42),
                    Priority = TaskPriority.High
                });
            }
        }

        private void OverlayAction(Action action)
        {
            PageOverlay.Visibility = Visibility.Visible;
            PageOverlayTransitionShow.Completed += (s2, e2) =>
            {
                action();

                PageOverlayTransitionHide.Completed += (s3, e3) =>
                {
                    PageOverlay.Visibility = Visibility.Collapsed;
                };
                PageOverlayTransitionHide.Begin();
            };
            PageOverlayTransitionShow.Begin();
        }

        #endregion

        #region TasksList

        private void TasksLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            if (selector == null)
                return;

            TaskModel task = selector.SelectedItem as TaskModel;
            if (task == null)
                return;

            selector.SelectedItem = null;

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

        #region TagList

        private void TagListControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region QuickAddTextBox

        private void QuickAddTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PageOverlayTransitionQuickHide.Completed += (s2, e2) =>
            {
                PageOverlay.Visibility = Visibility.Collapsed;
            };
            PageOverlayTransitionQuickHide.Begin();

            ApplicationBar.Buttons.Remove(appBarSaveQuickButton);
            ApplicationBar.Buttons.Remove(appBarCancelQuickButton);
            ApplicationBar.Buttons.Add(appBarNewTaskButton);
        }

        private void QuickAddTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PageOverlay.Visibility = Visibility.Visible;
            PageOverlayTransitionQuickShow.Begin();

            ApplicationBar.Buttons.Remove(appBarNewTaskButton);
            ApplicationBar.Buttons.Add(appBarSaveQuickButton);
            ApplicationBar.Buttons.Add(appBarCancelQuickButton);
        }

        #endregion

        #region SlideView

        private int _tagsSlideViewIndex = 1;

        private int _tasksSlideViewIndex = 0;

        private void SlideView_SelectionChanged(object sender, EventArgs e)
        {
            this.Focus();
            if (TasksSlideView.SelectedIndex == _tagsSlideViewIndex)
            {
                TasksPageOverlay.Visibility = Visibility.Visible;
                TasksPageOverlayTransitionShow.Begin();
            }
            else
            {
                TasksPageOverlayTransitionHide.Completed += (s2, e2) =>
                {
                    TasksPageOverlay.Visibility = Visibility.Collapsed;
                };
                TasksPageOverlayTransitionHide.Begin();
            }
        }

        #endregion

    }
}