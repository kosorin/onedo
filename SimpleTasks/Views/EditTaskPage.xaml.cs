using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Controls;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SimpleTasks.Views
{
    public partial class EditTaskPage : BasePage
    {
        #region Original
        private TaskModel _original = null;
        public TaskModel Original
        {
            get { return _original; }
            set { SetProperty(ref _original, value); }
        }
        #endregion

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
                Tile.Refresh(task, 48, App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush);
                Load(task);

                CreateAppBarItems();

                FirstTimeLoaded();
            }
            else
            {
                // Příchod ze stránky výběru datumu.
                if (BasePickerPage.CanRetrieve("DueDate"))
                {
                    DueDate = BasePickerPage.Retrieve<DateTime>("DueDate");
                }

                // Příchod ze stránky výběru času.
                if (BasePickerPage.CanRetrieve("DueTime"))
                {
                    DueDate = BasePickerPage.Retrieve<DateTime>("DueTime");
                }

                // Příchod ze stránky výběru připomenutí.
                if (BasePickerPage.CanRetrieve("Reminder"))
                {
                    Reminder = BasePickerPage.Retrieve<TimeSpan>("Reminder");
                    IsSetReminder = true;
                }
            }

            BuildAppBar();
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

        private ObservableCollection<Subtask> _subtasks = new ObservableCollection<Subtask>();
        public ObservableCollection<Subtask> Subtasks
        {
            get { return _subtasks; }
            set { SetProperty(ref _subtasks, value); }
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

        private bool _isCompleted;
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set { SetProperty(ref _isCompleted, value); }
        }
        #endregion

        #region Task methods
        private void Load(TaskModel task)
        {
            Original = task;
            IsNew = (Original == null);

            DateTime? defaultDate = App.Settings.Tasks.DefaultDate;
            DateTime defaultTime = App.Settings.Tasks.DefaultTime;
            if (IsNew)
            {
                Original = new TaskModel();
                IsCompleted = false;

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
                IsCompleted = task.IsCompleted;

                // text
                Title = task.Title;
                Detail = task.Detail;

                // podúkoly
                foreach (Subtask subtask in task.Subtasks)
                {
                    Subtasks.Add(new Subtask { Text = subtask.Text, IsCompleted = subtask.IsCompleted });
                }

                // priorita
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

            //Subtasks.Add(new Subtask("mléko"));
            //Subtasks.Add(new Subtask("rohlíky rohlíky rohlíky rohlíky rohlíky rohlíky"));
            //Subtasks.Add(new Subtask("máslo"));
            //Subtasks.Add(new Subtask("test"));
            //Subtasks.Add(new Subtask("čokoláda"));
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

        public void Save()
        {
            // Title
            Original.Title = Title;

            // Detail
            Original.Detail = Detail;

            // Priority
            Original.Priority = Priority;

            // Subtasks
            Original.Subtasks = Subtasks;

            // Due Date
            if (IsSetDueDate)
                Original.DueDate = DueDate;
            else
                Original.DueDate = null;

            // Reminder Date
            if (IsSetDueDate && IsSetReminder)
                Original.Reminder = Reminder;
            else
                Original.Reminder = null;

            // Completed Date
            if (IsCompleted)
                Original.Completed = DateTime.Now;
            else
                Original.Completed = null;

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
                IsCompleted = false;
            }
        }

        public void Complete()
        {
            if (!IsNew)
            {
                IsCompleted = true;
                Save();

                if (App.Settings.Tasks.CompleteSubtasks)
                {
                    foreach (Subtask subtask in Original.Subtasks)
                    {
                        subtask.IsCompleted = true;
                    }
                }
                if (App.Settings.Tiles.UnpinCompleted)
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
                ApplicationBar.Buttons.Add(appBarSaveButton);
                ApplicationBar.Buttons.Add(appBarPinButton);
            }
            else
            {
                if (IsCompleted)
                {
                    ApplicationBar.Buttons.Add(appBarActivateButton);
                }
                else
                {
                    ApplicationBar.Buttons.Add(appBarSaveButton);
                    if (IsPinned())
                    {
                        ApplicationBar.Buttons.Add(appBarUnpinButton);
                    }
                    else
                    {
                        ApplicationBar.Buttons.Add(appBarPinButton);
                    }
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

            ApplicationBar.Buttons.RemoveAt(1);
            ApplicationBar.Buttons.Insert(1, appBarPinButton);
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
                BasePickerPage.Navigate("DueDate", DueDate);
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
            {
                PhoneApplicationService.Current.State["DueTime"] = DueDate;
                phoneApplicationFrame.Navigate(new Uri("/Views/DueTimePickerPage.xaml", UriKind.Relative));
            }
            BasePickerPage.Navigate("DueTime", DueDate);
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
            BasePickerPage.Navigate("Reminder", Reminder);
        }

        private void ReminderCloseButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IsSetReminder = false;
        }
        #endregion

        #region Podúkoly
        private void EditSubtasks_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BasePickerPage.Navigate("Subtasks", Subtasks);
        }
        #endregion
    }
}