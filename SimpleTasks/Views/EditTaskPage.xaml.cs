using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using SimpleTasks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

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

            BuildAppBar();

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
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                TitleTextBox.Text = "";
                TitleTextBox.Focus();
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

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            ApplicationBarIconButton appBarSaveButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.save.png", UriKind.Relative));
            appBarSaveButton.Text = AppResources.AppBarSave;
            appBarSaveButton.Click += appBarSaveButton_Click;
            ApplicationBar.Buttons.Add(appBarSaveButton);
            if (ViewModel.IsOldTask)
            {
                if (ViewModel.CurrentTask.IsComplete)
                {
                    ApplicationBarIconButton appBarActivateButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.undo.curve.png", UriKind.Relative));
                    appBarActivateButton.Text = AppResources.AppBarActivate;
                    appBarActivateButton.Click += appBarActivateButton_Click;
                    ApplicationBar.Buttons.Add(appBarActivateButton);
                }
                else
                {
                    ApplicationBarIconButton appBarCompleteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.checkmark.pencil.top.png", UriKind.Relative));
                    appBarCompleteButton.Text = AppResources.AppBarComplete;
                    appBarCompleteButton.Click += appBarCompleteButton_Click;
                    ApplicationBar.Buttons.Add(appBarCompleteButton);
                }

                ApplicationBarIconButton appBarDeleteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.delete.png", UriKind.Relative));
                appBarDeleteButton.Text = AppResources.AppBarDelete;
                appBarDeleteButton.Click += appBarDeleteButton_Click;
                ApplicationBar.Buttons.Add(appBarDeleteButton);
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

        private void appBarDeleteButton_Click(object sender, EventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = AppResources.DeleteTaskCaption,
                Message = AppResources.DeleteTask
                            + Environment.NewLine + Environment.NewLine
                            + ViewModel.CurrentTask.Title,
                LeftButtonContent = AppResources.DeleteTaskYes,
                RightButtonContent = AppResources.DeleteTaskNo
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

        private void BuildTitleTextAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            ApplicationBarIconButton appBarOkButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.check.png", UriKind.Relative));
            appBarOkButton.Text = AppResources.AppBarOk;
            appBarOkButton.Click += appBarOkButton_Click;
            ApplicationBar.Buttons.Add(appBarOkButton);
        }

        private void appBarOkButton_Click(object sender, EventArgs e)
        {
            // Při stisku tlačítka na AppBaru nezmizí focus z elementu, 
            // takže např. u TextBoxu se neaktivuje změna textu pro binding
            object focusedObject = FocusManager.GetFocusedElement();
            if (focusedObject != null && focusedObject is TextBox)
            {
                var binding = (focusedObject as TextBox).GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }

            this.Focus();
        }

        #endregion

        #region Task Title

        private void PhoneTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //this.Focus();
            }
        }

        private void TitleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BuildTitleTextAppBar();
            TitleTextBox.FontFamily = new FontFamily("Segoe WP");
            TitleTextBoxNoTextStoryboard.Stop();
            TitleTextBox.Opacity = 1;
        }

        private void TitleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            BuildAppBar();
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                TitleTextBox.FontFamily = new FontFamily("Segoe UI Symbol");
                TitleTextBoxNoTextStoryboard.Begin();
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

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GoBack();
        }
    }
}