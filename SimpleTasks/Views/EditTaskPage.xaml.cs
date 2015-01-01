using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Controls;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Helpers;
using SimpleTasks.Resources;
using SimpleTasks.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SimpleTasks.Views
{
    public partial class EditTaskPage : BasePage
    {
        public TaskModel Task { get; private set; }

        private bool IsNew { get; set; }

        public EditTaskPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Page
        private bool _firstTimeNavigatedTo = true;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_firstTimeNavigatedTo)
            {
                _firstTimeNavigatedTo = false;

                TaskModel task = null;
                if (this.NavigationContext.QueryString.ContainsKey("Task"))
                {
                    task = App.Tasks.Tasks.FirstOrDefault((t) => { return t.Uid == this.NavigationContext.QueryString["Task"]; });
                }
                Load(task);

                CreateAppBarItems();

                FirstTimeLoaded();
            }
            else
            {
                // Příchod ze stránky výběru datumu.
                if (IsSetNavigationParameter("DatePicker"))
                {
                    DueDate = NavigationParameter<DateTime>("DatePicker");
                }

                // Příchod ze stránky výběru času.
                if (IsSetNavigationParameter("TimePicker"))
                {
                    DueDate = NavigationParameter<DateTime>("TimePicker");
                }

                // Příchod ze stránky výběru připomenutí.
                if (IsSetNavigationParameter("ReminderPicker"))
                {
                    Reminder = NavigationParameter<TimeSpan>("ReminderPicker");
                }

                // Příchod ze stránky výběru opakování.
                if (IsSetNavigationParameter("RepeatsPicker"))
                {
                    Repeats = NavigationParameter<Repeats>("RepeatsPicker");
                    IsSetRepeats = (Repeats != Core.Models.Repeats.None);
                }
            }

            if (Task != null)
            {
                Task.ModifiedSinceStart = true;
            }
            BuildAppBar();

            // Ošetření chyby, kdy po návratu na stránku byl zobrazen hint, 
            // ikdyž byl vložen text
            StopNoTextAnimation();
            TitleTextBox.UpdateHintVisibility();

            UpdateSubtasksAngle();
        }

        private void FirstTimeLoaded()
        {
            RoutedEventHandler firstTimeLoadHandler = null;
            firstTimeLoadHandler = (s, e2) =>
            {
                if (IsNew)
                {
                    // Zobrazení klávesnice
                    TitleTextBox.Focus();
                }

                // Bez tohoto se to jaksi nechce samo změnit
                DueDatePicker.ApplyStates();
                ReminderPicker.ApplyStates();

                // Aby nebylo vidět případné skrytí
                HideReminderStoryboard.SkipToFill();
                HideDueStoryboard.SkipToFill();

                this.Loaded -= firstTimeLoadHandler;
            };
            this.Loaded += firstTimeLoadHandler;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            Save();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GoBack();
            e.Cancel = true;
        }

        private void GoBack()
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                Navigate(typeof(MainPage));
            }
        }
        #endregion

        #region Task properties
        private DateTime _dueDate;
        public DateTime DueDate
        {
            get { return _dueDate; }
            set
            {
                if (SetProperty(ref _dueDate, value))
                {
                    OnPropertyChanged("ReminderDate");
                }
            }
        }

        private bool _isSetDueDate = false;
        public bool IsSetDueDate
        {
            get { return _isSetDueDate; }
            set
            {
                if (value)
                {
                    ShowDue();
                }
                else
                {
                    HideDue();
                }
                SetProperty(ref _isSetDueDate, value);
            }
        }

        private TimeSpan _reminder = TimeSpan.Zero;
        public TimeSpan Reminder
        {
            get { return _reminder; }
            set
            {
                if (SetProperty(ref _reminder, value))
                {
                    OnPropertyChanged("ReminderDate");
                }
            }
        }

        public DateTime? ReminderDate
        {
            get { return DueDate - Reminder; }
        }

        private bool _isSetReminderDate = false;
        public bool IsSetReminder
        {
            get { return _isSetReminderDate; }
            set
            {
                if (value)
                {
                    ShowReminder();
                }
                else
                {
                    HideReminder();
                }
                SetProperty(ref _isSetReminderDate, value);
            }
        }

        private Repeats _repeats = Repeats.None;
        public Repeats Repeats
        {
            get { return _repeats; }
            set { SetProperty(ref _repeats, value); }
        }

        private bool _isSetRepeats = false;
        public bool IsSetRepeats
        {
            get { return _isSetRepeats; }
            set
            {
                if (value)
                {
                    ShowRepeats();
                }
                else
                {
                    HideRepeats();
                }
                SetProperty(ref _isSetRepeats, value);
            }
        }
        #endregion

        #region Task methods
        private void Load(TaskModel task)
        {
            if (task != null)
            {
                Task = task;
                IsNew = false;
            }
            else
            {
                Task = new TaskModel();
                IsNew = true;
                App.Tasks.Add(Task);
            }

            // Výchozí hodnoty
            DateTime? defaultDate = Settings.Current.Tasks.DefaultDate;
            DateTime defaultTime = Settings.Current.Tasks.DefaultTime;

            // Datum & čas
            IsSetDueDate = Task.HasDueDate;
            if (IsSetDueDate)
            {
                DueDate = Task.DueDate.Value;
            }
            else
            {
                DueDate = (defaultDate ?? DateTime.Today);
                DueDate = DueDate.AddHours(defaultTime.Hour).AddMinutes(defaultTime.Minute);

                if (IsNew)
                {
                    IsSetDueDate = defaultDate != null;
                }
            }

            // Připomenutí
            IsSetReminder = Task.HasReminder;
            if (IsSetReminder)
            {
                Reminder = Task.Reminder.Value;
            }

            // Opakování
            IsSetRepeats = Task.Repeats != Core.Models.Repeats.None;
            Repeats = Task.Repeats;
        }

        public void Save()
        {
            // Název
            try
            {
                TitleTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
            catch { }
            if (string.IsNullOrWhiteSpace(Task.Title))
                Task.Title = AppResources.TitleUntitled;
            else
                Task.Title = Task.Title.Trim();

            // Detail
            try
            {
                DetailTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
            catch { }
            Task.Detail = Task.Detail.Trim();

            // Datum & čas
            if (IsSetDueDate)
                Task.DueDate = DueDate;
            else
                Task.DueDate = null;

            // Připomenutí
            if (IsSetDueDate && IsSetReminder)
                Task.Reminder = Reminder;
            else
                Task.Reminder = null;

            // Opakování
            if (IsSetRepeats)
                Task.Repeats = Repeats;
            else
                Task.Repeats = Core.Models.Repeats.None;

            // Nastavení dlaždice
            if (Task.TileSettings == null)
            {
                Task.TileSettings = Settings.Current.Tiles.DefaultTaskTileSettings.Clone();
            }

            // ULOŽENÍ
            App.Tasks.Update(Task);
        }

        public void Activate()
        {
            Task.Completed = null;
        }

        public void Complete()
        {
            Task.Completed = DateTime.Now;

            if (Settings.Current.Tasks.CompleteSubtasks)
            {
                foreach (Subtask subtask in Task.Subtasks)
                {
                    subtask.IsCompleted = true;
                }
            }
            if (Settings.Current.Tiles.UnpinCompleted && Repeats == Core.Models.Repeats.None)
            {
                Unpin();
            }
        }

        public void Delete()
        {
            App.Tasks.Delete(Task);
        }

        public void Pin()
        {
            LiveTile.PinEmpty(Task);
        }

        public void Unpin()
        {
            LiveTile.Unpin(Task);
        }

        public bool IsPinned()
        {
            return LiveTile.IsPinned(Task);
        }
        #endregion

        #region AppBar

        #region AppBar Create
        private ApplicationBarIconButton appBarActivateButton;

        private ApplicationBarIconButton appBarCompleteButton;

        private ApplicationBarIconButton appBarDeleteButton;

        private ApplicationBarIconButton appBarOkButton;

        private ApplicationBarIconButton appBarTileSettingsButton;

        private ApplicationBarIconButton appBarPinButton;

        private ApplicationBarIconButton appBarUnpinButton;

        private ApplicationBarIconButton appBarAddBulletButton;

        private void CreateAppBarItems()
        {
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


            appBarTileSettingsButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.settings.png", UriKind.Relative));
            appBarTileSettingsButton.Text = AppResources.AppBarTileSettings;
            appBarTileSettingsButton.Click += TileSettingsButton_Click;

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
            ApplicationBar = ThemeHelper.CreateApplicationBar();

            if (IsPinned())
                ApplicationBar.Buttons.Add(appBarUnpinButton);
            else
                ApplicationBar.Buttons.Add(appBarPinButton);

            if (Task.IsActive)
                ApplicationBar.Buttons.Add(appBarCompleteButton);
            else
                ApplicationBar.Buttons.Add(appBarActivateButton);

            ApplicationBar.Buttons.Add(appBarDeleteButton);

            ApplicationBar.Buttons.Add(appBarTileSettingsButton);
        }

        private void BuildTitleTextAppBar()
        {
            ApplicationBar = ThemeHelper.CreateApplicationBar();

            // Ikony
            ApplicationBar.Buttons.Add(appBarOkButton);
        }

        private void BuildDetailTextAppBar()
        {
            ApplicationBar = ThemeHelper.CreateApplicationBar();

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

        private void TileSettingsButton_Click(object sender, EventArgs e)
        {
            Navigate(typeof(EditTaskTilePage), Task);
        }

        private void PinButton(object sender, EventArgs e)
        {
            Pin();

            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBar.Buttons.Insert(0, appBarUnpinButton);
        }

        private void UnpinButton(object sender, EventArgs e)
        {
            Unpin();

            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBar.Buttons.Insert(0, appBarPinButton);
        }

        private void ActivateButton(object sender, EventArgs e)
        {
            Activate();

            ApplicationBar.Buttons.RemoveAt(1);
            ApplicationBar.Buttons.Insert(1, appBarCompleteButton);
        }

        private void CompleteButton(object sender, EventArgs e)
        {
            Complete();

            ApplicationBar.Buttons.RemoveAt(1);
            ApplicationBar.Buttons.Insert(1, appBarActivateButton);
            GoBack();
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
                    Delete();
                    GoBack();
                }
            };
            messageBox.Show();
        }
        #endregion

        #region Název
        SupportedPageOrientation orientation;

        private void StopNoTextAnimation()
        {
            TitleTextBoxNoTextStoryboard.Stop();
            TitleTextBox.Opacity = 1;
        }

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
            StopNoTextAnimation();

            orientation = SupportedOrientations;
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
        }

        private void TitleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DetailTextBox_LostFocus(sender, e);

            if (e.OriginalSource == TitleTextBox)
            {
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    TitleTextBoxNoTextStoryboard.Begin();
                }
            }
            SupportedOrientations = orientation;
        }
        #endregion

        #region Detail
        private void DetailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BuildDetailTextAppBar();

            orientation = SupportedOrientations;
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
        }

        private void DetailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var focusedElement = FocusManager.GetFocusedElement();
            if (focusedElement != TitleTextBox && focusedElement != DetailTextBox)
            {
                BuildAppBar();
            }
            SupportedOrientations = orientation;
        }
        #endregion

        #region Termín (datum+čas+připomenutí)
        private void ShowDue()
        {
            DueDatePicker.ApplyStates();
            HideDueStoryboard.Pause();
            ShowDueStoryboard.Begin();
        }

        private void HideDue()
        {
            DueDatePicker.ApplyStates();
            ShowDueStoryboard.Pause();
            HideDueStoryboard.Begin();
        }

        private void DueDateCloseButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IsSetDueDate = false;
        }
        #endregion

        #region Datum
        private void DueDatePicker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (IsSetDueDate)
            {
                Navigate(typeof(DatePickerPage), DueDate, "DatePicker");
            }
            else
            {
                IsSetDueDate = true;
            }
        }
        #endregion

        #region Čas
        private void DueTimePicker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Navigate(typeof(TimePickerPage), DueDate, "TimePicker");
        }
        #endregion

        #region Připomenutí
        private void ShowReminder()
        {
            ReminderPicker.ApplyStates();
            HideReminderStoryboard.Pause();
            ShowReminderStoryboard.Begin();
        }

        private void HideReminder()
        {
            ReminderPicker.ApplyStates();
            ShowReminderStoryboard.Pause();
            HideReminderStoryboard.Begin();
        }

        private void ReminderPicker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (IsSetReminder)
            {
                Navigate(typeof(ReminderPickerPage), Reminder, "ReminderPicker");
            }
            else
            {
                IsSetReminder = true;
            }
        }

        private void ReminderCloseButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IsSetReminder = false;
        }
        #endregion

        #region Opakování
        private void ShowRepeats()
        {
            RepeatsPicker.ApplyStates();
            HideRepeatsStoryboard.Pause();
            ShowRepeatsStoryboard.Begin();
        }

        private void HideRepeats()
        {
            RepeatsPicker.ApplyStates();
            ShowRepeatsStoryboard.Pause();
            HideRepeatsStoryboard.Begin();
        }

        private void RepeatsPicker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Navigate(typeof(RepeatsPickerPage), Repeats, "RepeatsPicker");
            //if (IsSetRepeats)
            //{
            //    Navigate(typeof(RepeatsPickerPage), Repeats, "RepeatsPicker");
            //}
            //else
            //{
            //    IsSetRepeats = true;
            //}
        }

        private void RepeatsCloseButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IsSetRepeats = false;
        }
        #endregion // end of Opakování

        #region Podúkoly
        private void EditSubtasks_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Navigate(typeof(SubtasksPage), Task);
        }

        private double _subtasksAngle = 0;
        public double SubtasksAngle
        {
            get { return _subtasksAngle; }
            set { SetProperty(ref _subtasksAngle, value); }
        }

        private void UpdateSubtasksAngle()
        {
            if (Task != null && Task.Subtasks != null && Task.Subtasks.Count > 0)
            {
                double ratio = (double)Task.Subtasks.Count(s => s.IsCompleted) / (double)Task.Subtasks.Count;
                SubtasksAngle = 359.99 * ratio; // není 360 kvůli chování kontrolu RingSlice.
                return;
            }
            SubtasksAngle = 0;
        }
        #endregion
    }
}