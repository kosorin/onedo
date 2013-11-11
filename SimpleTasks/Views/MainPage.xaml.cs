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
        public MainViewModel ViewModel { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel = App.ViewModel;

            LiveTile.Update(ViewModel.Tasks);
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationService.RemoveBackEntry();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            #region Ikony

            // Přidat úkol
            ApplicationBarIconButton appBarNewTaskButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative));
            appBarNewTaskButton.Text = AppResources.AppBarNew;
            appBarNewTaskButton.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/EditTaskPage.xaml", UriKind.Relative)); };
            ApplicationBar.Buttons.Add(appBarNewTaskButton);

            #endregion

            #region Menu

            // Smazat dokončené úkoly
            ApplicationBarMenuItem appBarDeleteCompletedItem = new ApplicationBarMenuItem(AppResources.AppBarDeleteCompleted);
            appBarDeleteCompletedItem.Click += (s, e) => { ViewModel.DeleteCompletedTasks(); };
            //appBarDeleteCompletedItem.IsEnabled = ViewModel.Tasks.ActiveTaskCount > 0;
            ApplicationBar.MenuItems.Add(appBarDeleteCompletedItem);

            // Živá dlaždice
            ApplicationBarMenuItem appBarLiveTileItem = new ApplicationBarMenuItem();
            SetAppBarLiveTileItem(appBarLiveTileItem);
            ApplicationBar.MenuItems.Add(appBarLiveTileItem);

            // Nastavení
            ApplicationBarMenuItem appBarSettingsMenuItem = new ApplicationBarMenuItem(AppResources.AppBarSettings);
            appBarSettingsMenuItem.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(appBarSettingsMenuItem);

            // O aplikaci
            ApplicationBarMenuItem appBarAboutMenuItem = new ApplicationBarMenuItem(AppResources.AppBarAbout);
            appBarAboutMenuItem.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/AboutPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(appBarAboutMenuItem);

            //// Reset
            //ApplicationBarMenuItem appBarResetMenuItem = new ApplicationBarMenuItem("resetovat data");
            //appBarResetMenuItem.Click += appBarResetMenuItem_Click;
            //ApplicationBar.MenuItems.Add(appBarResetMenuItem);

            ////// Clear
            //ApplicationBarMenuItem appBarClearMenuItem = new ApplicationBarMenuItem("smazat data");
            //appBarClearMenuItem.Click += (s, e) => { ViewModel.Tasks.Clear(); LiveTile.UpdateUI(ViewModel.Tasks); };
            //ApplicationBar.MenuItems.Add(appBarClearMenuItem);

            #endregion
        }

        private void SetAppBarLiveTileItem(ApplicationBarMenuItem appBarItem)
        {
            if (LiveTile.HasSecondaryTile)
            {
                // Je připnuta i sekundární dlaždice
                appBarItem.Text = AppResources.AppBarUnpinTile;
                appBarItem.Click += (s, e) => { LiveTileExtensions.RemoveSecondaryTile(); PeriodicTaskExtensions.Stop(); SetAppBarLiveTileItem(appBarItem); };
            }
            else
            {
                // Pouze primární dlaždice
                appBarItem.Text = AppResources.AppBarPinTile;
                appBarItem.Click += (s, e) => { LiveTileExtensions.AddSecondaryTile(ViewModel.Tasks); PeriodicTaskExtensions.Start(); SetAppBarLiveTileItem(appBarItem); };
            }
        }

        void appBarResetMenuItem_Click(object sender, EventArgs e)
        {
            ViewModel.Tasks.Clear();
            ViewModel.Tasks.Add(new TaskModel() { Title = "Buy milk", DueDate = DateTimeExtensions.Today.AddDays(-7), Priority = TaskPriority.Low });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Book flight to London", DueDate = DateTimeExtensions.Today.AddDays(-2), Priority = TaskPriority.High, ReminderDate = DateTime.Now });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Go to the dentist", DueDate = DateTimeExtensions.Today.AddDays(1) });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Release new version", DueDate = DateTimeExtensions.Today.AddDays(1), ReminderDate = DateTime.Now });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Math project", DueDate = DateTimeExtensions.Today.AddDays(2), Priority = TaskPriority.High });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Pay the rent", DueDate = DateTimeExtensions.Today.AddDays(4) });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Call Chuck", DueDate = DateTimeExtensions.Today.AddDays(6) });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Clone a dinosaur", DueDate = DateTimeExtensions.Today.AddDays(10), Priority = TaskPriority.High });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Go to cinema", DueDate = DateTimeExtensions.Today.AddDays(35), Priority = TaskPriority.Low });

            LiveTile.UpdateUI(ViewModel.Tasks);
        }

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
    }
}