using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.ViewModels;
using SimpleTasks.Resources;
using System.Windows;
using SimpleTasks.Models;
using SimpleTasks.Helpers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Diagnostics;

namespace SimpleTasks.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainViewModel ViewModel { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            DataContext = ViewModel = App.ViewModel;

            LiveTile.UpdateTiles(ViewModel.Tasks);
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            #region Ikony

            // Přidat úkol
            ApplicationBarIconButton appBarNewTaskButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative));
            appBarNewTaskButton.Text = AppResources.AppBarNewText;
            appBarNewTaskButton.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/EditTaskPage.xaml", UriKind.Relative)); };
            ApplicationBar.Buttons.Add(appBarNewTaskButton);

            #endregion

            #region Menu

            // Živá dlaždice
            ApplicationBarMenuItem appBarLiveTileItem = new ApplicationBarMenuItem();
            SetAppBarLiveTileItem(appBarLiveTileItem);
            ApplicationBar.MenuItems.Add(appBarLiveTileItem);

            // Smazat dokončené úkoly
            ApplicationBarMenuItem appBarDeleteCompletedItem = new ApplicationBarMenuItem(AppResources.AppBarDeleteCompletedText);
            appBarDeleteCompletedItem.Click += (s, e) => { ViewModel.DeleteCompletedTasks(); };
            ApplicationBar.MenuItems.Add(appBarDeleteCompletedItem);

            // Nastavení
            ApplicationBarMenuItem appBarSettingsMenuItem = new ApplicationBarMenuItem(AppResources.AppBarSettingsText);
            appBarSettingsMenuItem.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(appBarSettingsMenuItem);

            // O aplikaci
            ApplicationBarMenuItem appBarAboutMenuItem = new ApplicationBarMenuItem(AppResources.AppBarAboutText);
            appBarAboutMenuItem.Click += (s, e) => { NavigationService.Navigate(new Uri("/Views/AboutPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(appBarAboutMenuItem);

            // Reset
            ApplicationBarMenuItem appBarResetMenuItem = new ApplicationBarMenuItem("resetovat data");
            appBarResetMenuItem.Click += appBarResetMenuItem_Click;
            ApplicationBar.MenuItems.Add(appBarResetMenuItem);

            // Clear
            ApplicationBarMenuItem appBarClearMenuItem = new ApplicationBarMenuItem("smazat data");
            appBarClearMenuItem.Click += (s, e) => { ViewModel.Tasks.Clear(); LiveTile.UpdateTilesUI(ViewModel.Tasks); };
            ApplicationBar.MenuItems.Add(appBarClearMenuItem);

            #endregion
        }

        private void SetAppBarLiveTileItem(ApplicationBarMenuItem appBarItem)
        {
            if (LiveTile.HasSecondaryTile)
            {
                // Je připnuta i sekundární dlaždice
                appBarItem.Text = AppResources.AppBarUnpinTileText;
                appBarItem.Click += (s, e) => { App.RemoveSecondaryTile(); App.StopPeriodicTask(); SetAppBarLiveTileItem(appBarItem); };
            }
            else
            {
                // Pouze primární dlaždice
                appBarItem.Text = AppResources.AppBarPinTileText;
                appBarItem.Click += (s, e) => { App.AddSecondaryTile(); App.StartPeriodicTask(); SetAppBarLiveTileItem(appBarItem); };
            }
        }

        void appBarHelpMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Zatím bez nápovědy");
        }

        void appBarResetMenuItem_Click(object sender, EventArgs e)
        {
            ViewModel.Tasks.Clear();
            ViewModel.Tasks.Add(new TaskModel() { Title = "Buy milk", Date = DateTimeExtensions.Today.AddDays(-7) });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Book flight to London", Date = DateTimeExtensions.Today.AddDays(-2), IsImportant = true });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Call mom", Date = DateTimeExtensions.Today.AddDays(0), IsImportant = true });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Go to the dentist", Date = DateTimeExtensions.Today.AddDays(1) });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Release new version", Date = DateTimeExtensions.Today.AddDays(1) });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Math project", Date = DateTimeExtensions.Today.AddDays(2), IsImportant = true });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Pay the rent", Date = DateTimeExtensions.Today.AddDays(4) });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Call Chuck", Date = DateTimeExtensions.Today.AddDays(6) });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Break bank", Date = DateTimeExtensions.Today.AddDays(7) });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Clone a dinosaur", Date = DateTimeExtensions.Today.AddDays(10), IsImportant = true });
            ViewModel.Tasks.Add(new TaskModel() { Title = "Go to cinema", Date = DateTimeExtensions.Today.AddDays(35) });

            LiveTile.UpdateTilesUI(ViewModel.Tasks);
        }

        private void TasksLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            if (selector == null)
                return;

            TaskModel task = selector.SelectedItem as TaskModel;
            if (task == null)
                return;

            ViewModel.TaskToEdit = task;
            selector.SelectedItem = null;

            NavigationService.Navigate(new Uri("/Views/EditTaskPage.xaml", UriKind.Relative));
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