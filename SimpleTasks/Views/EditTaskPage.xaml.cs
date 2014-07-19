using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Controls;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using SimpleTasks.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SimpleTasks.Views
{
    public partial class EditTaskPage : BasePage
    {
        private TaskModel Original { get; set; }

        public bool IsNew { get; private set; }

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
                SetTaskData(task);

                CreateAppBarItems();

                FirstTimeLoaded();
            }
            else
            {
                // Příchod ze stránky výběru datumu.
                if (PhoneApplicationService.Current.State.ContainsKey("DueDate"))
                {
                    DueDate = (DateTime)PhoneApplicationService.Current.State["DueDate"];
                    PhoneApplicationService.Current.State.Remove("DueDate");
                }

                // Příchod ze stránky výběru času.
                if (PhoneApplicationService.Current.State.ContainsKey("DueTime"))
                {
                    DueDate = (DateTime)PhoneApplicationService.Current.State["DueTime"];
                    PhoneApplicationService.Current.State.Remove("DueTime");
                }

                // Příchod ze stránky výběru připomenutí.
                if (PhoneApplicationService.Current.State.ContainsKey("Reminder"))
                {
                    Reminder = (TimeSpan)PhoneApplicationService.Current.State["Reminder"];
                    PhoneApplicationService.Current.State.Remove("Reminder");
                }
            }

            BuildAppBar();
        }

        private void SetTaskData(TaskModel task)
        {
            Original = task;
            IsNew = (Original == null);

            DateTime? defaultDate = App.Settings.DefaultDate;
            DateTime defaultTime = App.Settings.DefaultTimeSetting;
            if (IsNew)
            {
                Original = new TaskModel();
                IsComplete = false;

                // datum & čas
                IsSetDueDate = (defaultDate != null);
                DueDate = defaultDate ?? DateTime.Today;
                DueDate = DueDate.AddHours(defaultTime.Hour).AddMinutes(defaultTime.Minute);

                // připomenutí
                IsSetReminder = false;
                Reminder = TimeSpan.Zero;
            }
            else
            {
                IsComplete = task.IsComplete;

                // text
                Title = task.Title;
                Detail = task.Detail;
                Priority = task.Priority;

                // datum & čas
                IsSetDueDate = (task.DueDate != null);
                DueDate = task.DueDate ?? (defaultDate ?? DateTime.Today);
                if (task.DueDate == null)
                {
                    DueDate = DueDate.AddHours(defaultTime.Hour).AddMinutes(defaultTime.Minute);
                }

                // připomenutí
                IsSetReminder = (task.Reminder != null);
                Reminder = task.Reminder ?? TimeSpan.Zero;
            }
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

        #region Task properties
        private string _title = "";
        public new string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _detail = "";
        public string Detail
        {
            get { return _detail; }
            set { SetProperty(ref _detail, value); }
        }

        private TaskPriority _priority = TaskPriority.Normal;
        public TaskPriority Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }

        private DateTime _dueDate;
        public DateTime DueDate
        {
            get { return _dueDate; }
            set { SetProperty(ref _dueDate, value); }
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

        private TimeSpan? _reminder = null;
        public TimeSpan? Reminder
        {
            get { return _reminder; }
            set { SetProperty(ref _reminder, value); }
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

        private bool _isComplete;
        public bool IsComplete
        {
            get { return _isComplete; }
            set { SetProperty(ref _isComplete, value); }
        }
        #endregion

        #region Task methods
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

        public void Save()
        {
            // Title
            Original.Title = Title;

            // Detail
            Original.Detail = Detail;

            // Priority
            Original.Priority = Priority;

            // Due Date
            if (IsSetDueDate)
                Original.DueDate = DueDate;
            else
                Original.DueDate = null;

            // Reminder Date
            if (IsSetReminder)
                Original.Reminder = Reminder;
            else
                Original.Reminder = null;

            // Completed Date
            if (IsComplete)
                Original.CompletedDate = DateTime.Now;
            else
                Original.CompletedDate = null;

            // ULOŽENÍ
            Original.ModifiedSinceStart = true;
            if (IsNew)
            {
                App.Tasks.Add(Original);
            }
            else
            {
                App.Tasks.Update(Original);
            }

            IsNew = false;
        }

        public void Activate()
        {
            if (!IsNew)
            {
                IsComplete = false;
            }
        }

        public void Complete()
        {
            if (!IsNew)
            {
                IsComplete = true;
                Save();

                if (App.Settings.UnpinCompletedSetting)
                {
                    Unpin();
                }
            }
        }

        public void Delete()
        {
            App.Tasks.Delete(Original);
        }

        public void Pin()
        {
            Save();
            LiveTile.PinEmpty(Original);
        }

        public void Unpin()
        {
            if (!IsNew)
            {
                LiveTile.Unpin(Original);
            }
        }

        public bool IsPinned()
        {
            return LiveTile.IsPinned(Original);
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
            if (IsNew)
            {
                ApplicationBar.Buttons.Add(appBarPinButton);
                ApplicationBar.Buttons.Add(appBarSaveButton);
            }
            else
            {
                if (IsComplete)
                {
                    ApplicationBar.Buttons.Add(appBarActivateButton);
                }
                else
                {
                    if (IsPinned())
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
                Pin();

                ApplicationBar.Buttons.RemoveAt(0);
                ApplicationBar.Buttons.Insert(0, appBarUnpinButton);
            }
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

            BuildAppBar();
        }

        private void CompleteButton(object sender, EventArgs e)
        {
            if (CanSave())
            {
                Complete();
                GoBack();
            }
        }

        private void SaveButton(object sender, EventArgs e)
        {
            if (CanSave())
            {
                Save();
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
                    Delete();
                    GoBack();
                }
            };
            messageBox.Show();
        }
        #endregion

        #region Název
        SupportedPageOrientation orientation;

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
                var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (phoneApplicationFrame != null)
                { // TODO: vymyslet navigaci
                    PhoneApplicationService.Current.State["DueDate"] = DueDate;
                    phoneApplicationFrame.Navigate(new Uri("/Views/DueDatePickerPage.xaml", UriKind.Relative));
                }
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
            var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (phoneApplicationFrame != null)
            { // TODO: vymyslet navigaci
                PhoneApplicationService.Current.State["DueTime"] = DueDate;
                phoneApplicationFrame.Navigate(new Uri("/Views/DueTimePickerPage.xaml", UriKind.Relative));
            }
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
                var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (phoneApplicationFrame != null)
                { // TODO: vymyslet navigaci
                    PhoneApplicationService.Current.State["Reminder"] = Reminder;
                    phoneApplicationFrame.Navigate(new Uri("/Views/ReminderPickerPage.xaml", UriKind.Relative));
                }
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
    }
}