using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using SimpleTasks.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public EditTaskViewModel ViewModel { get; set; }

        public EditTaskPage()
        {
            InitializeComponent();

            PageOverlayTransitionHide.Completed += (s2, e2) =>
            {
                PageOverlay.Visibility = Visibility.Collapsed;
            };

            DueDateGridShow.Completed += (s2, e2) =>
            {
                DueDatePicker.IsEnabled = true;
                DueDatePresetPicker.IsEnabled = true;

                DueDateGrid.Height = 50;
                DueDatePresetPickerBorder.Width = 60;
            };
            DueDateGridHide.Completed += (s2, e2) =>
            {
                DueDateGrid.Visibility = Visibility.Collapsed;
                DueDateGrid.Height = 0;
                DueDatePresetPickerBorder.Width = 0;
            };

            ReminderGridShow.Completed += (s2, e2) =>
            {
                ReminderDatePicker.IsEnabled = true;
                ReminderTimePicker.IsEnabled = true;
                ReminderDatePresetPicker.IsEnabled = true;

                ReminderGrid.Height = 100;
                ReminderDatePresetPickerBorder.Width = 60;
            };
            ReminderGridHide.Completed += (s2, e2) =>
            {
                ReminderGrid.Visibility = Visibility.Collapsed;
                ReminderGrid.Height = 0;
                ReminderDatePresetPickerBorder.Width = 0;
            };
        }

        #region Page
        private bool _firstTimeNavigatedTo = true;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_firstTimeNavigatedTo)
            {
                _firstTimeNavigatedTo = false;

                // Zjistíme, jaký úkol se bude upravovat (pokud se nepřidává nový úkol)
                TaskModel task = null;
                if (this.NavigationContext.QueryString.ContainsKey("Task"))
                {
                    task = App.Tasks.Tasks.FirstOrDefault((t) => { return t.Uid == this.NavigationContext.QueryString["Task"]; });
                }

                ViewModel = new EditTaskViewModel(task);
                DataContext = ViewModel;

                CreateAppBarItems();

                // Při prvním zobrazení stránky pro editaci úkolu se zobrází klávesnice a nastaví defaultní termín
                RoutedEventHandler firstTimeLoadHandler = null;
                firstTimeLoadHandler = (s, e2) =>
                {
                    if (ViewModel.IsNew)
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
                    ReminderDatePresetPicker.SelectionChanged += ReminderDatePresetPicker_SelectionChanged;
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

                    ViewModel.ReminderDate = newReminderTime;
                }
            }

            BuildAppBar();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (!e.IsNavigationInitiator)
            {
                App.UpdateAllLiveTiles();
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GoBack();
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
        #endregion

        #region Task
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
        #endregion

        #region AppBar

        #region AppBar Create
        private ApplicationBarIconButton appBarSaveButton;

        private ApplicationBarIconButton appBarActivateButton;

        private ApplicationBarIconButton appBarCompleteButton;

        private ApplicationBarIconButton appBarDeleteButton;

        private ApplicationBarIconButton appBarOkButton;

        private ApplicationBarIconButton appBarPinButton;

        private ApplicationBarIconButton appBarUnpinButton;

        private ApplicationBarIconButton appBarAddBulletButton;

        private void CreateAppBarItems()
        {
            appBarSaveButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.save.png", UriKind.Relative));
            appBarSaveButton.Text = AppResources.AppBarSave;
            appBarSaveButton.Click += SaveButton;

            appBarActivateButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.undo.curve.png", UriKind.Relative));
            appBarActivateButton.Text = AppResources.AppBarActivate;
            appBarActivateButton.Click += ActivateButton;

            appBarCompleteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.checkmark.pencil.top.png", UriKind.Relative));
            appBarCompleteButton.Text = AppResources.AppBarComplete;
            appBarCompleteButton.Click += CompleteButton;

            appBarDeleteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.delete.png", UriKind.Relative));
            appBarDeleteButton.Text = AppResources.AppBarDelete;
            appBarDeleteButton.Click += DeleteButton;

            appBarOkButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.check.png", UriKind.Relative));
            appBarOkButton.Text = AppResources.AppBarOk;
            appBarOkButton.Click += OkButton;

            appBarPinButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.pin.png", UriKind.Relative));
            appBarPinButton.Text = AppResources.AppBarPin;
            appBarPinButton.Click += PinButton;

            appBarUnpinButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.pin.remove.png", UriKind.Relative));
            appBarUnpinButton.Text = AppResources.AppBarUnpin;
            appBarUnpinButton.Click += UnpinButton;

            appBarAddBulletButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.list.add.below.png", UriKind.Relative));
            appBarAddBulletButton.Text = AppResources.AppBarAddBullet;
            appBarAddBulletButton.Click += AddBulletButton;
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            if (ViewModel.IsNew)
            {
                ApplicationBar.Buttons.Add(appBarPinButton);
                ApplicationBar.Buttons.Add(appBarSaveButton);
            }
            else
            {
                if (ViewModel.IsComplete)
                {
                    ApplicationBar.Buttons.Add(appBarActivateButton);
                }
                else
                {
                    if (ViewModel.IsPinned())
                    {
                        ApplicationBar.Buttons.Add(appBarUnpinButton);
                    }
                    else
                    {
                        ApplicationBar.Buttons.Add(appBarPinButton);
                    }
                    ApplicationBar.Buttons.Add(appBarSaveButton);
                    ApplicationBar.Buttons.Add(appBarCompleteButton);
                }

                ApplicationBar.Buttons.Add(appBarDeleteButton);
            }
        }

        private void BuildTitleTextAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            ApplicationBar.Buttons.Add(appBarOkButton);
        }

        private void BuildDetailTextAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            ApplicationBar.Buttons.Add(appBarAddBulletButton);
            ApplicationBar.Buttons.Add(appBarOkButton);
        }
        #endregion

        private void OkButton(object sender, EventArgs e)
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

        private void AddBulletButton(object sender, EventArgs e)
        {
            int lineNumber = DetailTextBox.Text.LineNumberAtPosition(DetailTextBox.SelectionStart);
            List<string> lines = new List<string>(DetailTextBox.Text.Lines());

            string bullet = string.Format(" {0} ", '\u2022');
            string bulletText = AppResources.BulletText;
            string newLineText = string.Format("{0}{1}", bullet, bulletText);

            if (string.IsNullOrWhiteSpace(lines[lineNumber]))
            {
                lines[lineNumber] = newLineText;
            }
            else
            {
                lines.Insert(++lineNumber, newLineText);
            }

            string newLine = "\r\n";
            int newLineLength = newLine.Length;
            int newSelectionStart = 0;
            for (int i = 0; i < lineNumber; ++i)
            {
                newSelectionStart += lines[i].Length + newLineLength;
            }

            DetailTextBox.Text = string.Join(newLine, lines);
            DetailTextBox.SelectionStart = newSelectionStart + bullet.Length;
            DetailTextBox.SelectionLength = bulletText.Length;
        }

        private void PinButton(object sender, EventArgs e)
        {
            if (CanSave())
            {
                ViewModel.Pin();

                ApplicationBar.Buttons.RemoveAt(0);
                ApplicationBar.Buttons.Insert(0, appBarUnpinButton);
            }
        }

        private void UnpinButton(object sender, EventArgs e)
        {
            ViewModel.Unpin();

            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBar.Buttons.Insert(0, appBarPinButton);
        }

        private void ActivateButton(object sender, EventArgs e)
        {
            ViewModel.Activate();

            PageOverlayTransitionHide.Begin();
            BuildAppBar();
        }

        private void CompleteButton(object sender, EventArgs e)
        {
            if (CanSave())
            {
                ViewModel.Complete();
                GoBack();
            }
        }

        private void SaveButton(object sender, EventArgs e)
        {
            if (CanSave())
            {
                ViewModel.Save();
                GoBack();
            }
        }

        private void DeleteButton(object sender, EventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = AppResources.DeleteTaskCaption,
                Message = AppResources.DeleteTask
                            + Environment.NewLine + Environment.NewLine
                            + TitleTextBox.Text,
                LeftButtonContent = AppResources.DeleteTaskYes,
                RightButtonContent = AppResources.DeleteTaskNo
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                if (e1.Result == CustomMessageBoxResult.LeftButton)
                {
                    ViewModel.Delete();
                    GoBack();
                }
            };
            messageBox.Show();
        }
        #endregion

        #region Title a Detail
        private void TitleTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DetailTextBox.Focus();
            }
        }

        private void TitleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BuildTitleTextAppBar();
            TitleTextBoxNoTextStoryboard.Stop();
            TitleTextBox.Opacity = 1;
        }

        private void DetailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BuildDetailTextAppBar();
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
            // Animace zobrazení
            DueDateGrid.Visibility = Visibility.Visible;
            DueDateGridHide.Pause();
            DueDateGridShow.Begin();
            DueDatePresetPickerHide.Pause();
            DueDatePresetPickerShow.Begin();
        }

        private void DueToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Animace skrytí
            DueDatePicker.IsEnabled = false;
            DueDatePresetPicker.IsEnabled = false;

            DueDateGridShow.Pause();
            DueDateGridHide.Begin();
            DueDatePresetPickerShow.Pause();
            DueDatePresetPickerHide.Begin();
        }

        private void DueDatePresetPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DueDatePresetPicker.SelectedItem != null)
            {
                KeyValuePair<string, DateTime> pair = (KeyValuePair<string, DateTime>)DueDatePresetPicker.SelectedItem;
                ViewModel.DueDate = pair.Value;
            }
        }

        #endregion

        #region Reminder
        private void ReminderToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            // Animace zobrazení
            ReminderGrid.Visibility = Visibility.Visible;
            ReminderGridHide.Pause();
            ReminderGridShow.Begin();
            ReminderDatePresetPickerHide.Pause();
            ReminderDatePresetPickerShow.Begin();
        }

        private void ReminderToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Animace skrytí
            ReminderDatePicker.IsEnabled = false;
            ReminderTimePicker.IsEnabled = false;
            ReminderDatePresetPicker.IsEnabled = false;

            ReminderGridShow.Pause();
            ReminderGridHide.Begin();
            ReminderDatePresetPickerShow.Pause();
            ReminderDatePresetPickerHide.Begin();
        }

        private void ReminderDatePresetPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReminderDatePresetPicker.SelectedItem != null)
            {
                KeyValuePair<string, DateTime> pair = (KeyValuePair<string, DateTime>)ReminderDatePresetPicker.SelectedItem;
                ViewModel.ReminderDate = pair.Value.Date.AddHours(ViewModel.ReminderDate.Hour)
                                                        .AddMinutes(ViewModel.ReminderDate.Minute);
            }
        }

        private void ReminderTimePicker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (phoneApplicationFrame != null)
            {
                PhoneApplicationService.Current.State["RadialTime"] = ViewModel.ReminderDate;
                phoneApplicationFrame.Navigate(new Uri("/Views/RadialTimePickerPage.xaml", UriKind.Relative));
            }
        }
        #endregion
    }
}