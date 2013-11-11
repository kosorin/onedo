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
using System.Collections.Generic;
using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace SimpleTasks.Views
{
    public partial class EditTaskPage : PhoneApplicationPage
    {
        public EditTaskViewModel ViewModel { get; private set; }

        private bool FirstTimeNavigatedTo = true;

        public EditTaskPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!FirstTimeNavigatedTo)
                return;
            FirstTimeNavigatedTo = false;

            // Zjistíme, jaký úkol se bude upravovat (pokud se nepřidává nový úkol)
            TaskModel task = null;
            if (this.NavigationContext.QueryString.ContainsKey("Task"))
            {
                task = App.ViewModel.Tasks.First((t) => { return t.Uid == this.NavigationContext.QueryString["Task"]; });
            }

            ViewModel = new EditTaskViewModel(task);
            DataContext = ViewModel;

            BuildLocalizedApplicationBar();

            // Při prvním zobrazení stránky pro editaci úkolu se zobrází klávesnice a nastaví defaultní termín
            RoutedEventHandler firstTimeLoadHandler = null;
            firstTimeLoadHandler = (s, e2) =>
            {
                if (!ViewModel.IsOldTask)
                {
                    // Zobrazení klávesnice
                    TitleTextBox.Focus();

                    // Nastavení defaultního termínu
                    if (App.Settings.DefaultDueDateSettingToDateTime != null)
                    {
                        DueDateToggleButton.IsChecked = true;
                    }
                }
                DueDatePresetPicker.SelectionChanged += DueDatePresetPicker_SelectionChanged;
                this.Loaded -= firstTimeLoadHandler;
            };
            this.Loaded += firstTimeLoadHandler;
        }

        private bool CanSave()
        {
            if (TitleTextBox.Text == "")
            {
                MessageBox.Show(AppResources.MissingTitleText);
                return false;
            }

            return true;
        }

        private void PrepareSave()
        {
            if (!DueDateToggleButton.IsChecked.Value)
            {
                DueDatePicker.Value = null;
            }

            if (ReminderToggleButton.IsChecked.Value)
            {
                if (ViewModel.CurrentTask.ReminderDate <= DateTime.Now)
                {
                    ViewModel.CurrentTask.ReminderDate = DateTime.Now.AddMinutes(5);
                }
            }
            else
            {
                ReminderDatePicker.Value = null;
            }
        }

        private void GoBack()
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            { 
                NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
            }
        }

        #region AppBar

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            if (ViewModel.IsOldTask)
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
            if (ViewModel.IsOldTask)
            {
                ApplicationBarMenuItem appBarDeleteItem = new ApplicationBarMenuItem();
                appBarDeleteItem.Text = AppResources.AppBarDeleteText;
                appBarDeleteItem.Click += appBarDeleteItem_Click;
                ApplicationBar.MenuItems.Add(appBarDeleteItem);
            }
        }

        private void appBarActivateButton_Click(object sender, EventArgs e)
        {
            if (CanSave())
            {
                PrepareSave();
                ViewModel.ActivateTask();
                GoBack();
            }
        }

        private void appBarCompleteButton_Click(object sender, EventArgs e)
        {
            if (CanSave())
            {
                PrepareSave();
                ViewModel.CompleteTask();
                GoBack();
            }
        }

        private void appBarSaveButton_Click(object sender, EventArgs e)
        {
            if (CanSave())
            {
                PrepareSave();
                ViewModel.SaveTask();
                GoBack();
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
                    GoBack();
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

        private void DueToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            DueDateGridHide.Pause();

            if (ViewModel.CurrentTask.DueDate == null)
            {
                if (App.Settings.DefaultDueDateSettingToDateTime != null)
                {
                    ViewModel.CurrentTask.DueDate = App.Settings.DefaultDueDateSettingToDateTime;
                }
                else
                {
                    ViewModel.CurrentTask.DueDate = DateTimeExtensions.Today;
                }
            }

            // Animace zobrazení
            DueDatePicker.Visibility = Visibility.Visible;
            DueDatePresetPicker.Visibility = Visibility.Visible;
            DueDateGridShow.Begin();
            DueDateGridShow.Completed += (s2, e2) =>
            {
                DueDatePicker.Visibility = Visibility.Visible;
                DueDatePresetPicker.Visibility = Visibility.Visible;
                DueDatePicker.IsEnabled = true;
                DueDatePresetPicker.IsEnabled = true;
            };
        }

        private void DueToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            DueDateGridShow.Pause();

            // Animace skrytí
            DueDatePicker.IsEnabled = false;
            DueDatePresetPicker.IsEnabled = false;
            DueDateGridHide.Begin();
            DueDateGridHide.Completed += (s2, e2) =>
            {
                DueDatePicker.Visibility = Visibility.Collapsed;
                DueDatePresetPicker.Visibility = Visibility.Collapsed;
            };
        }

        private void DueDatePresetPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DueDatePresetPicker.SelectedItem != null)
            {
                KeyValuePair<string, DateTime> pair = (KeyValuePair<string, DateTime>)DueDatePresetPicker.SelectedItem;
                ViewModel.CurrentTask.DueDate = pair.Value;
            }
        }

        #endregion

        #region Reminder

        private void ReminderToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ReminderGridHide.Pause();

            if (ViewModel.CurrentTask.ReminderDate == null)
            {
                DateTime defaultReminderTime = App.Settings.DefaultReminderTimeSetting;
                if (ViewModel.CurrentTask.DueDate == null)
                {
                    ViewModel.CurrentTask.ReminderDate = DateTime.Today.Date.AddHours(defaultReminderTime.Hour)
                                                                            .AddMinutes(defaultReminderTime.Minute);
                }
                else
                {
                    ViewModel.CurrentTask.ReminderDate = ViewModel.CurrentTask.DueDate.Value.Date.AddHours(defaultReminderTime.Hour)
                                                                                                 .AddMinutes(defaultReminderTime.Minute);
                }

                if (ViewModel.CurrentTask.ReminderDate <= DateTime.Now)
                {
                    ViewModel.CurrentTask.ReminderDate = DateTime.Now.AddHours(1);
                }
            }


            // Animace zobrazení
            ReminderGrid.Visibility = Visibility.Visible;
            ReminderGridShow.Begin();
            ReminderGridShow.Completed += (s2, e2) =>
            {
                ReminderDatePicker.IsEnabled = true;
                ReminderTimePicker.IsEnabled = true;
            };
        }

        private void ReminderToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ReminderGridShow.Pause();

            // Animace skrytí
            ReminderDatePicker.IsEnabled = false;
            ReminderTimePicker.IsEnabled = false;
            ReminderGridHide.Begin();
            ReminderGridHide.Completed += (s2, e2) =>
            {
                ReminderGrid.Visibility = Visibility.Collapsed;
            };
        }

        #endregion
    }
}