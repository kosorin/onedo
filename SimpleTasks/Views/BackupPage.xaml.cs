using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using SimpleTasks.Controls;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Helpers;
using SimpleTasks.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using RestoreListPickerItem = SimpleTasks.Controls.ListPickerItem<SimpleTasks.Core.Models.OneDriveFileInfo, System.DateTime>;

namespace SimpleTasks.Views
{
    public partial class BackupPage : BasePage
    {
        #region Public Fields
        public const string BackupFolderName = "OneDo Data";

        public const string BackupFileExtension = ".onedo.backup";
        #endregion

        #region Private Fields
        private const int _maxBackupsInList = 5;
        #endregion

        #region Constructor
        public BackupPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New || !e.IsNavigationInitiator)
            {
                IsLoggedIn = false;

                PerformingAction = true;

                ActionTextBlock.Text = AppResources.SigningIn;
                IsLoggedIn = await LiveConnectHelper.SilentLoginAsync();

                ActionTextBlock.Text = AppResources.BackupDownloadingList;
                await RefreshBackupList();

                PerformingAction = false;
            }
        }
        #endregion

        #region Properties
        private bool _isLoggedIn = false;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                SetProperty(ref _isLoggedIn, value);
                SignButton.Content = value ? AppResources.SignOut : AppResources.SignIn;
                OnPropertyChanged("SignedInText");
                OnPropertyChanged("NotLoggedIn");
            }
        }

        public bool NotLoggedIn
        {
            get { return !IsLoggedIn; }
        }

        public string SignedInText
        {
            get { return IsLoggedIn ? string.Format(AppResources.SignedInYes, LiveConnectHelper.CurrentUserName) : AppResources.SignedInNo; }
        }

        private bool _performingAction = false;
        public bool PerformingAction
        {
            get { return _performingAction; }
            set { SetProperty(ref _performingAction, value); }
        }

        private ObservableCollection<RestoreListPickerItem> _backupList = new ObservableCollection<RestoreListPickerItem>();
        public ObservableCollection<RestoreListPickerItem> BackupList
        {
            get { return _backupList; }
            set
            {
                if (value == null)
                {
                    value = new ObservableCollection<RestoreListPickerItem>();
                }
                SetProperty(ref _backupList, value);
                value.CollectionChanged -= BackupList_CollectionChanged;
                value.CollectionChanged += BackupList_CollectionChanged;
                OnPropertyChanged("IsEnabledRestoreButton");
            }
        }

        void BackupList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("IsEnabledRestoreButton");
        }

        public bool IsEnabledRestoreButton
        {
            get { return (UseTasks || UseSettings) && (BackupList.Count > 0 && BackupList[0].Value1 != null); }
        }

        private bool _useTasks = true;
        public bool UseTasks
        {
            get { return _useTasks; }
            set
            {
                SetProperty(ref _useTasks, value);
                OnPropertyChanged("IsEnabledRestoreButton");
            }
        }

        private bool _useSettings = false;
        public bool UseSettings
        {
            get { return _useSettings; }
            set
            {
                SetProperty(ref _useSettings, value);
                OnPropertyChanged("IsEnabledRestoreButton");
            }
        }

        public string BackupAndRestoreHelpText
        {
            get
            {
                return string.Format(AppResources.BackupAndRestoreHelpText, BackupFolderName);
            }
        }
        #endregion

        #region Methods
        private void Restore(BackupData backupData)
        {
            // Zpracování dat
            if (UseSettings && backupData.Settings != null)
            {
                backupData.Settings.Version = App.VersionString;
                Settings.Current = backupData.Settings;
            }
            if (UseTasks && backupData.Tasks != null)
            {
                App.Tasks.Restore(backupData.Tasks);
            }

            MessageBox.Show(AppResources.RestoreOk, AppResources.BackupAndRestoreTitle.FirstUpper(), MessageBoxButton.OK);
        }

        private async Task RefreshBackupList()
        {
            // Získání souborů
            List<OneDriveFileInfo> files = new List<OneDriveFileInfo>();
            if (IsLoggedIn)
            {
                // Získání složky
                string folderId = await LiveConnectHelper.SearchFolderIdAsync(BackupFolderName);

                // Získání seznamu souborů se zálohami
                if (folderId != null)
                {
                    files = await LiveConnectHelper.GetFilesInfoAsync(folderId, BackupFileExtension);
                }
            }

            // Vytvoření seznamu
            List<RestoreListPickerItem> list = new List<RestoreListPickerItem>();
            foreach (OneDriveFileInfo fileInfo in files)
            {
                string fileName = fileInfo.Name.Replace(BackupFileExtension, "");

                try
                {
                    DateTime createdDate = DateTimeExtensions.FromFileName(fileName);
                    list.Add(new RestoreListPickerItem(createdDate.ToString(), fileInfo, createdDate));
                }
                catch (FormatException)
                {
                    // Při parsování datumu, takže
                }
            }

            // Seřazení podle vytvoření
            list.Sort((f1, f2) =>
            {
                return f2.Value2.CompareTo(f1.Value2);
            });

            if (list.Count > _maxBackupsInList)
            {
                list.RemoveRange(_maxBackupsInList, list.Count - _maxBackupsInList);
            }

            // Naplnění pickeru
            if (list.Count > 0)
            {
                list[0].Label += string.Format(" ({0})", AppResources.BackupLatest);
                BackupList = new ObservableCollection<RestoreListPickerItem>(list);
            }
            else
            {
                BackupList.Clear();
                BackupList.Add(new RestoreListPickerItem(AppResources.NoBackups, null, DateTime.MinValue));
            }
        }

        public static async Task<bool> Backup()
        {
            // Získání id složky
            string folderId = await LiveConnectHelper.SearchFolderIdAsync(BackupFolderName);
            if (folderId == null)
            {
                folderId = await LiveConnectHelper.CreateFolderAsync(LiveConnectHelper.Root, BackupFolderName);
            }

            // Nahrajeme soubor a získáme jeho id
            string fileId = null;
            if (folderId != null)
            {
                // Vytvoření dat
                BackupData backupData = new BackupData();
                backupData.Version = App.VersionString;
                backupData.Created = DateTime.UtcNow;
                backupData.Settings = Settings.Current;
                backupData.Tasks = App.Tasks.Tasks;
#if DEBUG
                Formatting formatting = Formatting.Indented;
#else
                Formatting formatting = Formatting.None;
#endif
                string json = JsonConvert.SerializeObject(backupData, formatting);

                // Nahrání
                string fileName = string.Format("{0}{1}", DateTime.Now.ToFileName(), BackupFileExtension);
                fileId = await LiveConnectHelper.UploadAsync(folderId, fileName, json);
            }

            return fileId != null;
        }

        private async Task<BackupData> GetBackupData()
        {
            BackupData backupData = null;

            RestoreListPickerItem selectedItem = RestorePicker.SelectedItem as RestoreListPickerItem;
            if (selectedItem != null && selectedItem.Value1 != null)
            {
                // Stáhnutí souboru
                string data = await LiveConnectHelper.DownloadAsync(selectedItem.Value1.Id);
                if (data != null)
                {
                    try
                    {
                        backupData = JsonConvert.DeserializeObject<BackupData>(data, new JsonSerializerSettings());
                    }
                    catch
                    {
                        backupData = null;
                    }
                }
            }

            return backupData;
        }
        #endregion

        #region Event Handlers
        private async void SignButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (IsLoggedIn)
            {
                IsLoggedIn = false;
                LiveConnectHelper.Logout();
            }
            else
            {
                PerformingAction = true;

                ActionTextBlock.Text = AppResources.SigningIn;
                IsLoggedIn = await LiveConnectHelper.LoginAsync();

                if (IsLoggedIn)
                {
                    ActionTextBlock.Text = AppResources.BackupDownloadingList;
                    await RefreshBackupList();
                }
                else
                {
                    MessageBox.Show(string.Format(AppResources.UnknownError), AppResources.SigningIn, MessageBoxButton.OK);
                }

                PerformingAction = false;
            }
        }

        private async void BackupButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PerformingAction = true;

            ActionTextBlock.Text = AppResources.BackingUp;
            if (await Backup())
            {
                await RefreshBackupList();
                MessageBox.Show(string.Format(AppResources.BackupOk), AppResources.BackupAndRestoreTitle.FirstUpper(), MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(string.Format(AppResources.UnknownError), AppResources.BackupAndRestoreTitle.FirstUpper(), MessageBoxButton.OK);
            }

            PerformingAction = false;
        }

        private async void RestoreButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PerformingAction = true;

            ActionTextBlock.Text = AppResources.Restoring;
            BackupData backupData = await GetBackupData();
            if (backupData != null)
            {
                if (backupData.Version != App.VersionString)
                {
                    CustomMessageBox messageBox = new CustomMessageBox()
                    {
                        Caption = AppResources.BackupVersionMismatchCaption,
                        Message = AppResources.BackupVersionMismatchContent,
                        LeftButtonContent = AppResources.BackupVersionMismatchYes,
                        RightButtonContent = AppResources.BackupVersionMismatchNo
                    };

                    messageBox.Dismissed += (s1, e1) =>
                    {
                        switch (e1.Result)
                        {
                        case CustomMessageBoxResult.LeftButton:
                            Restore(backupData);
                            break;
                        case CustomMessageBoxResult.RightButton:
                        case CustomMessageBoxResult.None:
                        default:
                            break;
                        }
                    };

                    messageBox.Show();
                }
                else
                {
                    Restore(backupData);
                }
            }
            else
            {
                MessageBox.Show(AppResources.UnknownError, AppResources.BackupAndRestoreTitle.FirstUpper(), MessageBoxButton.OK);
            }

            PerformingAction = false;
        }

        private async void RefreshListButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PerformingAction = true;

            ActionTextBlock.Text = AppResources.BackupDownloadingList;
            await RefreshBackupList();

            PerformingAction = false;
        }

        private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (BackupPanel.Children[0] == LoginPanel)
            {
                BackupPanel.Children.RemoveAt(0);
            }
            if (RestorePanel.Children[0] == LoginPanel)
            {
                RestorePanel.Children.RemoveAt(0);
            }

            if (Pivot.SelectedIndex == 0)
            {
                BackupPanel.Children.Insert(0, LoginPanel);
            }
            else if (Pivot.SelectedIndex == 1)
            {
                RestorePanel.Children.Insert(0, LoginPanel);
            }
        }
        #endregion
    }
}