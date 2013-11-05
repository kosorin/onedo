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
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Views
{
    public partial class EditTaskPage : PhoneApplicationPage
    {
        public EditTaskViewModel ViewModel { get; private set; }

        public EditTaskPage()
        {
            InitializeComponent();

            // Zjistíme, jaký úkol se bude upravovat (pokud se nepřidává nový úkol)
            TaskModel taskToEdit = null;
            if (PhoneApplicationService.Current.State.ContainsKey("TaskToEdit"))
            {
                taskToEdit = (TaskModel)PhoneApplicationService.Current.State["TaskToEdit"];
                PhoneApplicationService.Current.State["TaskToEdit"] = null;
            }
            EditingOldTask = taskToEdit != null;

            ViewModel = new EditTaskViewModel(taskToEdit);
            DataContext = ViewModel;

            BuildLocalizedApplicationBar();

            // Při přidání nového úkolu se zobrází klávesnice
            RoutedEventHandler firstTimeLoadHandler = null;
            firstTimeLoadHandler = (s, e) =>
            {
                if (!EditingOldTask)
                {
                    TitleTextBox.Focus();
                }
                this.Loaded -= firstTimeLoadHandler;
            };
            this.Loaded += firstTimeLoadHandler;

            WasReminderSet = false;
        }

        private bool EditingOldTask { get; set; }

        #region AppBar

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
            NavigationService.GoBack();
        }

        private void appBarCompleteButton_Click(object sender, EventArgs e)
        {
            ViewModel.CompleteTask();
            NavigationService.GoBack();
        }

        private void appBarSaveButton_Click(object sender, EventArgs e)
        {
            if (TitleTextBox.Text == "")
            {
                MessageBox.Show(AppResources.MissingTitleText);
            }
            else if (ViewModel.CurrentTask.ReminderDate <= DateTime.Now)
            {
                MessageBox.Show("Datum a čas připomenutí musí být v budoucnosti.");
            }
            else
            {
                ViewModel.SaveTask();
                NavigationService.GoBack();
            }
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

        #endregion

        #region Task Title

        private void PhoneTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }

        #endregion

        #region Due Date

        private void DueDatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DueDateModel dueDate = DueDatePicker.SelectedItem as DueDateModel;
            if (dueDate == null)
                return;

            if (dueDate.Type == DueDateModel.DueDatePickerType.CustomDate)
            {
                CustomDueDatePicker.Visibility = Visibility.Visible;
                CustomDueDatePicker.Height = 0;

                DatePickerShow.Begin();
                DatePickerShow.Completed += (s2, e2) => { CustomDueDatePicker.IsEnabled = true; };
            }
            else
            {
                CustomDueDatePicker.IsEnabled = false;

                DatePickerHide.Begin();
                DatePickerHide.Completed += (s2, e2) => { CustomDueDatePicker.Visibility = Visibility.Collapsed; };
            }
        }

        #endregion

        #region Reminder

        private bool WasReminderSet { get; set; }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!WasReminderSet)
            {
                ReminderDatePicker.Value = ViewModel.CurrentDueDate.Date;
                if (ReminderDatePicker.Value == null)
                {
                    ReminderDatePicker.Value = DateTimeExtensions.Today;
                }
                WasReminderSet = true;
            }

            ReminderGrid.Visibility = Visibility.Visible;
            ReminderGrid.Height = 0;

            ReminderPickerShow.Begin();
            ReminderPickerShow.Completed += (s2, e2) =>
            {
                ReminderDatePicker.IsEnabled = true;
                ReminderTimePicker.IsEnabled = true;
            };
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ReminderDatePicker.IsEnabled = false;
            ReminderTimePicker.IsEnabled = false;

            ReminderPickerHide.Begin();
            ReminderPickerHide.Completed += (s2, e2) => { ReminderGrid.Visibility = Visibility.Collapsed; };

        }

        #endregion
    }
}