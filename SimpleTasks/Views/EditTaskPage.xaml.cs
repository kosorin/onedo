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
        public EditTaskPage()
        {
            InitializeComponent();
            BuildDueDatePresetList();

            PageOverlayTransitionHide.Completed += (s2, e2) =>
            {
                PageOverlay.Visibility = Visibility.Collapsed;
            };

            DueDateGridShow.Completed += (s2, e2) =>
            {
                DueDatePicker.Visibility = Visibility.Visible;
                DueDatePresetPicker.Visibility = Visibility.Visible;
                DueDatePicker.IsEnabled = true;
                DueDatePresetPicker.IsEnabled = true;
                DueDateGrid.Height = 50;
            };
            DueDateGridHide.Completed += (s2, e2) =>
            {
                DueDatePicker.Visibility = Visibility.Collapsed;
                DueDatePresetPicker.Visibility = Visibility.Collapsed;
                DueDateGrid.Height = 0;
            };

            ReminderGridShow.Completed += (s2, e2) =>
            {
                ReminderDatePicker.IsEnabled = true;
                ReminderTimePicker.IsEnabled = true;
            };
            ReminderGridHide.Completed += (s2, e2) =>
            {
                ReminderGrid.Visibility = Visibility.Collapsed;
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

                OldTask = task;
                if (OldTask != null)
                {
                    CurrentTask = new TaskModel()
                    {
                        Title = OldTask.Title,
                        Detail = OldTask.Detail,
                        Priority = OldTask.Priority,
                        DueDate = OldTask.DueDate,
                        ReminderDate = OldTask.ReminderDate,
                        CompletedDate = OldTask.CompletedDate
                    };
                    IsOldTask = true;
                }
                else
                {
                    CurrentTask = new TaskModel();
                    IsOldTask = false;
                }
                DataContext = this;

                CreateAppBarItems();
                BuildAppBar();

                // Při prvním zobrazení stránky pro editaci úkolu se zobrází klávesnice a nastaví defaultní termín
                RoutedEventHandler firstTimeLoadHandler = null;
                firstTimeLoadHandler = (s, e2) =>
                {
                    if (!IsOldTask)
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

                    CurrentTask.ReminderDate = newReminderTime;
                }
            }
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
        public bool IsOldTask { get; set; }

        public TaskModel OldTask { get; set; }

        public TaskModel CurrentTask { get; set; }

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
                if (CurrentTask.ReminderDate <= DateTime.Now)
                {
                    CurrentTask.ReminderDate = DateTime.Now.AddMinutes(5);
                }
            }
            else
            {
                ReminderDatePicker.Value = null;
            }
        }

        private void AfterSave()
        {
            if (App.Settings.UnpinCompletedSetting && CurrentTask.IsComplete)
            {
                LiveTile.Unpin(CurrentTask);
            }
        }

        private void Save()
        {
            CurrentTask.ModifiedSinceStart = true;
            if (IsOldTask)
            {
                App.Tasks.Update(OldTask, CurrentTask);
            }
            else
            {
                App.Tasks.Add(CurrentTask);
            }
        }

        private void Delete()
        { App.Tasks.Delete(OldTask); }
        #endregion

        #region AppBar

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

            appBarAddBulletButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.list.add.below.png", UriKind.Relative));
            appBarAddBulletButton.Text = AppResources.AppBarAddBullet;
            appBarAddBulletButton.Click += appBarAddBulletButton_Click;
        }

        void appBarAddBulletButton_Click(object sender, EventArgs e)
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

        private void appBarPinButton_Click(object sender, EventArgs e)
        {
            if (CanSave())
            {
                BeforeSave();
                Save();
                AfterSave();

                LiveTile.PinEmpty(CurrentTask);
                ApplicationBar.Buttons.RemoveAt(0);
                ApplicationBar.Buttons.Insert(0, appBarUnpinButton);
            }
        }

        void appBarUnpinButton_Click(object sender, EventArgs e)
        {
            LiveTile.Unpin(CurrentTask);
            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBar.Buttons.Insert(0, appBarPinButton);
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            if (CurrentTask.IsActive)
            {
                if (LiveTile.IsPinned(CurrentTask))
                {
                    ApplicationBar.Buttons.Add(appBarUnpinButton);
                }
                else
                {
                    ApplicationBar.Buttons.Add(appBarPinButton);
                }
            }

            if (IsOldTask)
            {
                if (CurrentTask.IsComplete)
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

        private void BuildDetailTextAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // Ikony
            ApplicationBar.Buttons.Add(appBarAddBulletButton);
            ApplicationBar.Buttons.Add(appBarOkButton);
        }

        private void appBarActivateButton_Click(object sender, EventArgs e)
        {
            PageOverlayTransitionHide.Begin();

            CurrentTask.CompletedDate = null;
            BuildAppBar();
        }

        private void appBarCompleteButton_Click(object sender, EventArgs e)
        {
            if (CanSave())
            {
                BeforeSave();
                CurrentTask.CompletedDate = DateTime.Now;
                CurrentTask.ReminderDate = null;
                Save();
                AfterSave();

                GoBack();
            }
        }

        private void appBarSaveButton_Click(object sender, EventArgs e)
        {
            if (CanSave())
            {
                BeforeSave();
                Save();
                AfterSave();

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
                            + CurrentTask.Title,
                LeftButtonContent = AppResources.DeleteTaskYes,
                RightButtonContent = AppResources.DeleteTaskNo
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                case CustomMessageBoxResult.LeftButton:
                    Delete();
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
        public List<KeyValuePair<string, DateTime>> DueDatePresetList { get; set; }

        private void BuildDueDatePresetList()
        {
            DueDatePresetList = new List<KeyValuePair<string, DateTime>>();

            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateToday, DateTimeExtensions.Today));
            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateTomorrow, DateTimeExtensions.Tomorrow));

            int daysAfterTomorrow = 4;
            for (int i = 1; i <= daysAfterTomorrow; i++)
            {
                DateTime date = DateTimeExtensions.Tomorrow.AddDays(i);
                DueDatePresetList.Add(new KeyValuePair<string, DateTime>(date.ToString("dddd", CultureInfo.CurrentCulture).ToLower(), date));
            }

            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateThisWeek, DateTimeExtensions.LastDayOfWeek));
            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateNextWeek, DateTimeExtensions.LastDayOfNextWeek));
            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateThisMonth, DateTimeExtensions.LastDayOfMonth));
            DueDatePresetList.Add(new KeyValuePair<string, DateTime>(AppResources.DateNextMonth, DateTimeExtensions.LastDayOfNextMonth));
        }

        private void DueToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            DueDateGridHide.Pause();

            if (CurrentTask.DueDate == null)
            {
                if (App.Settings.DefaultDueDateSettingToDateTime != null)
                {
                    CurrentTask.DueDate = App.Settings.DefaultDueDateSettingToDateTime;
                }
                else
                {
                    CurrentTask.DueDate = DateTimeExtensions.Today;
                }
            }

            // Animace zobrazení
            DueDatePicker.Visibility = Visibility.Visible;
            DueDatePresetPicker.Visibility = Visibility.Visible;
            DueDateGridShow.Begin();
        }

        private void DueToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            DueDateGridShow.Pause();

            // Animace skrytí
            DueDatePicker.IsEnabled = false;
            DueDatePresetPicker.IsEnabled = false;
            DueDateGridHide.Begin();
        }

        private void DueDatePresetPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DueDatePresetPicker.SelectedItem != null)
            {
                KeyValuePair<string, DateTime> pair = (KeyValuePair<string, DateTime>)DueDatePresetPicker.SelectedItem;
                CurrentTask.DueDate = pair.Value;
            }
        }

        #endregion

        #region Reminder

        private void ReminderToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ReminderGridHide.Pause();

            if (CurrentTask.ReminderDate == null)
            {
                DateTime defaultReminderTime = App.Settings.DefaultReminderTimeSetting;
                if (CurrentTask.DueDate == null)
                {
                    CurrentTask.ReminderDate = DateTime.Today.Date.AddHours(defaultReminderTime.Hour)
                                                                            .AddMinutes(defaultReminderTime.Minute);
                }
                else
                {
                    CurrentTask.ReminderDate = CurrentTask.DueDate.Value.Date.AddHours(defaultReminderTime.Hour)
                                                                                                 .AddMinutes(defaultReminderTime.Minute);
                }
            }


            // Animace zobrazení
            ReminderGrid.Visibility = Visibility.Visible;
            ReminderGridShow.Begin();
        }

        private void ReminderToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ReminderGridShow.Pause();

            // Animace skrytí
            ReminderDatePicker.IsEnabled = false;
            ReminderTimePicker.IsEnabled = false;
            ReminderGridHide.Begin();
        }

        private void ReminderTimePicker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (phoneApplicationFrame != null)
            {
                PhoneApplicationService.Current.State["RadialTime"] = CurrentTask.ReminderDate.Value;
                phoneApplicationFrame.Navigate(new Uri("/Views/RadialTimePickerPage.xaml", UriKind.Relative));
            }
        }

        #endregion
    }
}