using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using SimpleTasks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            if (FirstTimeNavigatedTo)
            {
                FirstTimeNavigatedTo = false;

                // Zjistíme, jaký úkol se bude upravovat (pokud se nepřidává nový úkol)
                TaskModel task = null;
                if (this.NavigationContext.QueryString.ContainsKey("Task"))
                {
                    task = App.Tasks.Tasks.FirstOrDefault((t) => { return t.Uid == this.NavigationContext.QueryString["Task"]; });
                }

                ViewModel = new EditTaskViewModel(task);
                DataContext = ViewModel;

                CreateAppBarItems();
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

                // Pokud je úkol dokončený, zobrazíme overlay
                if (task != null && task.IsComplete)
                {
                    PageOverlay.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (PhoneApplicationService.Current.State.ContainsKey("RadialTime"))
                {
                    DateTime newReminderTime = (DateTime)PhoneApplicationService.Current.State["RadialTime"];
                    PhoneApplicationService.Current.State.Remove("RadialTime");

                    ViewModel.CurrentTask.ReminderDate = newReminderTime;
                }
            }
        }

        private bool updateTile = false;

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (updateTile)
            {
                updateTile = false;
                LiveTile.Update(ViewModel.CurrentTask);
            }            
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GoBack();
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

        private void BeforeSave()
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

        private void AfterSave()
        {
            if (LiveTile.IsPinned(ViewModel.CurrentTask))
            {
                LiveTile.Update(ViewModel.CurrentTask);
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

        private ApplicationBarIconButton appBarSaveButton;

        private ApplicationBarIconButton appBarActivateButton;

        private ApplicationBarIconButton appBarCompleteButton;

        private ApplicationBarIconButton appBarDeleteButton;

        private ApplicationBarIconButton appBarOkButton;

        private ApplicationBarIconButton appBarPinButton;

        private ApplicationBarIconButton appBarUnpinButton;

        private void CreateAppBarItems()
        {
            appBarSaveButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.save.png", UriKind.Relative));
            appBarSaveButton.Text = AppResources.AppBarSave;
            appBarSaveButton.Click += appBarSaveButton_Click;

            appBarActivateButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.undo.curve.png", UriKind.Relative));
            appBarActivateButton.Text = AppResources.AppBarActivate;
            appBarActivateButton.Click += appBarActivateButton_Click;

            appBarCompleteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.checkmark.pencil.top.png", UriKind.Relative));
            appBarCompleteButton.Text = AppResources.AppBarComplete;
            appBarCompleteButton.Click += appBarCompleteButton_Click;

            appBarDeleteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.delete.png", UriKind.Relative));
            appBarDeleteButton.Text = AppResources.AppBarDelete;
            appBarDeleteButton.Click += appBarDeleteButton_Click;

            appBarOkButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.check.png", UriKind.Relative));
            appBarOkButton.Text = AppResources.AppBarOk;
            appBarOkButton.Click += appBarOkButton_Click;

            appBarPinButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.pin.png", UriKind.Relative));
            appBarPinButton.Text = AppResources.AppBarPin;
            appBarPinButton.Click += appBarPinButton_Click;

            appBarUnpinButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.pin.remove.png", UriKind.Relative));
            appBarUnpinButton.Text = AppResources.AppBarUnpin;
            appBarUnpinButton.Click += appBarUnpinButton_Click;
        }

        private void appBarPinButton_Click(object sender, EventArgs e)
        {
            if (CanSave())
            {
                BeforeSave();
                ViewModel.SaveTask();
                updateTile = true;
                LiveTile.PinEmpty(ViewModel.CurrentTask);
                ApplicationBar.Buttons.RemoveAt(0);
                ApplicationBar.Buttons.Insert(0, appBarUnpinButton);
            }
        }

        void appBarUnpinButton_Click(object sender, EventArgs e)
        {
            LiveTile.Unpin(ViewModel.CurrentTask);
            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBar.Buttons.Insert(0, appBarPinButton);
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            if (ViewModel.CurrentTask.IsActive)
            {
                if (LiveTile.IsPinned(ViewModel.CurrentTask))
                {
                    ApplicationBar.Buttons.Add(appBarUnpinButton);
                }
                else
                {
                    ApplicationBar.Buttons.Add(appBarPinButton);
                }
            }

            if (ViewModel.IsOldTask)
            {
                if (ViewModel.CurrentTask.IsComplete)
                {
                    ApplicationBar.Buttons.Add(appBarActivateButton);
                }
                else
                {
                    ApplicationBar.Buttons.Add(appBarSaveButton);
                    ApplicationBar.Buttons.Add(appBarCompleteButton);
                }
                ApplicationBar.Buttons.Add(appBarDeleteButton);
            }
            else
            {
                ApplicationBar.Buttons.Add(appBarSaveButton);
            }
        }

        private void BuildTitleTextAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            ApplicationBar.Buttons.Add(appBarOkButton);
        }

        private void appBarActivateButton_Click(object sender, EventArgs e)
        {
            PageOverlayTransitionHide.Completed += (s2, e2) =>
            {
                PageOverlay.Visibility = Visibility.Collapsed;
            };
            PageOverlayTransitionHide.Begin();

            ViewModel.CurrentTask.CompletedDate = null;
            BuildAppBar();
        }

        private void appBarCompleteButton_Click(object sender, EventArgs e)
        {
            if (CanSave())
            {
                BeforeSave();
                ViewModel.CompleteTask();
                AfterSave();
                updateTile = true;
                GoBack();
            }
        }

        private void appBarSaveButton_Click(object sender, EventArgs e)
        {
            if (CanSave())
            {
                BeforeSave();
                ViewModel.SaveTask();
                AfterSave();
                updateTile = true;
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

        #region Task Title a Detail TextBox

        private void TitleTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DetailTextBox.Focus();
            }
        }

        private void TitleAndDetailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BuildTitleTextAppBar();
            TitleTextBoxNoTextStoryboard.Stop();
            TitleTextBox.Opacity = 1;
        }

        private void TitleAndDetailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var focusedElement = FocusManager.GetFocusedElement();
            if (focusedElement != TitleTextBox && focusedElement != DetailTextBox)
            {
                BuildAppBar();
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    TitleTextBoxNoTextStoryboard.Begin();
                }
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

        private void ReminderTimePicker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (phoneApplicationFrame != null)
            {
                PhoneApplicationService.Current.State["RadialTime"] = ViewModel.CurrentTask.ReminderDate.Value;
                phoneApplicationFrame.Navigate(new Uri("/Views/RadialTimePickerPage.xaml", UriKind.Relative));
            }
        }

        #endregion
    }
}