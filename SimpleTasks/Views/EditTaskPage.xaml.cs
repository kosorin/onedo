using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.ViewModels;
using SimpleTasks.Resources;
using SimpleTasks.Models;
using System.Windows.Input;
using SimpleTasks.Helpers;

namespace SimpleTasks.Views
{
    public partial class EditTaskPage : PhoneApplicationPage
    {
        public EditTaskPage()
        {
            InitializeComponent(); 
            
            FirstTimeLoaded = true;
            EditingOldTask = (App.ViewModel.TaskToEdit != null);

            DataContext = ViewModel = new EditTaskViewModel(App.ViewModel.TaskToEdit);
            App.ViewModel.TaskToEdit = null;

            BuildLocalizedApplicationBar();
        }

        public EditTaskViewModel ViewModel { get; private set; }

        private bool FirstTimeLoaded { get; set; }

        private bool EditingOldTask { get; set; }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            if (EditingOldTask)
            {
                if (ViewModel.CurrentTask.IsComplete)
                {
                    ApplicationBarIconButton appBarActivateButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.undo.curve.png", UriKind.Relative));
                    appBarActivateButton.Text = AppResources.AppBarActivateText;
                    appBarActivateButton.Click += appBarActivateButton_Click;
                    ApplicationBar.Buttons.Add(appBarActivateButton);
                }
                else
                {
                    ApplicationBarIconButton appBarCompleteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.checkmark.pencil.top.png", UriKind.Relative));
                    appBarCompleteButton.Text = AppResources.AppBarCompleteText;
                    appBarCompleteButton.Click += appBarCompleteButton_Click;
                    ApplicationBar.Buttons.Add(appBarCompleteButton);
                }
            }

            ApplicationBarIconButton appBarSaveButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.save.png", UriKind.Relative));
            appBarSaveButton.Text = AppResources.AppBarSaveText;
            appBarSaveButton.Click += appBarSaveButton_Click;
            ApplicationBar.Buttons.Add(appBarSaveButton);


            // Menu
            if (EditingOldTask)
            {
                ApplicationBarMenuItem appBarDeleteItem = new ApplicationBarMenuItem();
                appBarDeleteItem.Text = AppResources.AppBarDeleteText;
                appBarDeleteItem.Click += appBarDeleteItem_Click;
                ApplicationBar.MenuItems.Add(appBarDeleteItem);
            }
        }

        private void appBarActivateButton_Click(object sender, EventArgs e)
        {
            ViewModel.ActivateTask();
            LiveTile.UpdateTiles(App.ViewModel.Tasks);
            NavigationService.GoBack();
        }

        private void appBarCompleteButton_Click(object sender, EventArgs e)
        {
            ViewModel.CompleteTask();
            LiveTile.UpdateTiles(App.ViewModel.Tasks);
            NavigationService.GoBack();
        }

        private void appBarSaveButton_Click(object sender, EventArgs e)
        {
            if (TitleTextBox.Text == "")
            {
                MessageBox.Show(AppResources.MissingTitleText);
                return;
            }

            ViewModel.SaveTask();
            LiveTile.UpdateTiles(App.ViewModel.Tasks);
            NavigationService.GoBack();
        }

        private void appBarDeleteItem_Click(object sender, EventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = AppResources.DeleteTaskCaptionText,
                Message = AppResources.DeleteTaskText
                            + Environment.NewLine + Environment.NewLine
                            + ViewModel.CurrentTask.Title,
                LeftButtonContent = AppResources.DeleteTaskYesText,
                RightButtonContent = AppResources.DeleteTaskNoText
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                case CustomMessageBoxResult.LeftButton:
                    ViewModel.DeleteTask();
                    LiveTile.UpdateTiles(App.ViewModel.Tasks);
                    NavigationService.GoBack();
                    break;
                case CustomMessageBoxResult.RightButton:
                case CustomMessageBoxResult.None:
                default:
                    break;
                }
            };

            messageBox.Show();
        }

        private void DueDatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker listPicker = sender as ListPicker;
            if (listPicker == null)
                return;

            DueDateModel dueDate = listPicker.SelectedItem as DueDateModel;
            if (dueDate == null)
                return;

            if (dueDate.Type == DueDateModel.DueDatePickerType.CustomDate)
            {
                CustomDueDatePicker.Visibility = System.Windows.Visibility.Visible;
                CustomDueDatePicker.Height = 0;

                DatePickerShow.Begin();
                DatePickerShow.Completed += (s2, e2) => { CustomDueDatePicker.IsEnabled = true; };
            }
            else
            {
                CustomDueDatePicker.IsEnabled = false;

                DatePickerHide.Begin();
                DatePickerHide.Completed += (s2, e2) => { CustomDueDatePicker.Visibility = System.Windows.Visibility.Collapsed; };
            }
        }

        private void PhoneTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Při první načtení stránky nastavíme focus na název úkolu.
            if (FirstTimeLoaded && !EditingOldTask)
            {
                FirstTimeLoaded = false;
                TitleTextBox.Focus();
            }
        }
    }
}