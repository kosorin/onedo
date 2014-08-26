using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Controls;
using Microsoft.Live;
using System.Diagnostics;
using SimpleTasks.Helpers;
using System.Globalization;
using SimpleTasks.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleTasks.Core.Models;
using System.Collections.ObjectModel;
using SimpleTasks.Models;

namespace SimpleTasks.Views
{
    public partial class BackupPage : BasePage
    {
        #region Public Fields
        public const string BackupFolderName = "OneDo Backup";

        public const string BackupFileExtension = ".onedo";
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

                NotPerformingAction = false;
                IsSigning = true;
                IsLoggedIn = await OneDriveHelper.SilentLoginAsync();
                IsSigning = false;

                BackupRestoreProgressRing.Content = AppResources.BackupDownloadingList;
                await RefreshBackupList();
                NotPerformingAction = true;
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
            }
        }

        private bool _isSigning = false;
        public bool IsSigning
        {
            get { return _isSigning; }
            set { SetProperty(ref _isSigning, value); }
        }

        private bool _notPerformingAction = true;
        public bool NotPerformingAction
        {
            get { return _notPerformingAction; }
            set
            {
                SetProperty(ref _notPerformingAction, value);
                OnPropertyChanged("IsEnabledRestoreButton");
            }
        }

        private ObservableCollection<ValueToString<OneDriveFileInfo>> _backupList = new ObservableCollection<ValueToString<OneDriveFileInfo>>();
        public ObservableCollection<ValueToString<OneDriveFileInfo>> BackupList
        {
            get { return _backupList; }
            set
            {
                if (value == null)
                {
                    value = new ObservableCollection<ValueToString<OneDriveFileInfo>>();
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
            get { return NotPerformingAction && (UseTasks || UseSettings) && BackupList.Count > 0; }
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
        #endregion

        #region Methods
        private async Task RefreshBackupList()
        {
            // Získání souborů
            List<OneDriveFileInfo> files = new List<OneDriveFileInfo>();
            if (IsLoggedIn)
            {
                // Získání složky
                string folderId = await OneDriveHelper.SearchFolderIdAsync(BackupFolderName);

                // Získání seznamu souborů se zálohami
                if (folderId != null)
                {
                    files = await OneDriveHelper.GetFilesInfoAsync(folderId, BackupFileExtension);
                }
            }

            // Seřazení podle vytvoření
            files.Sort((f1, f2) =>
            {
                return f2.Created.CompareTo(f1.Created);
            });

            // Naplnění pickeru
            BackupList.Clear();
            bool first = true;
            foreach (OneDriveFileInfo file in files)
            {
                string label = file.Name.Replace(BackupFileExtension, "");
                if (label.StartsWith("__"))
                {
                    label = file.Created.ToString();
                }

                if (first)
                {
                    label += string.Format(" ({0})", AppResources.BackupLatest);
                    first = false;
                }
                BackupList.Add(new ValueToString<OneDriveFileInfo>(file, label));

                if (BackupList.Count >= 8)
                {
                    break;
                }
            }

            if (BackupList.Count == 0)
            {
                BackupList.Add(new ValueToString<OneDriveFileInfo>(null, AppResources.NoBackups));
            }
        }

        private async Task<bool> Backup()
        {
            // Získání id složky
            string folderId = await OneDriveHelper.SearchFolderIdAsync(BackupFolderName);
            if (folderId == null)
            {
                folderId = await OneDriveHelper.CreateFolderAsync(OneDriveHelper.Root, BackupFolderName);
            }

            // Získání id nahraného souboru (pokud máme složku)
            string fileId = null;
            if (folderId != null)
            {
                string fileName = string.Format("__{0}{1}", Guid.NewGuid(), BackupFileExtension);

                BackupData backupData = new BackupData();
                backupData.Info.Version = App.VersionString;
                backupData.Info.UtcDateTime = DateTime.UtcNow;
                backupData.Settings = Settings.Current;
                backupData.Tasks = App.Tasks.Tasks;

#if DEBUG
                Formatting formatting = Formatting.Indented;
#else
                Formatting formatting = Formatting.None;
#endif
                string json = JsonConvert.SerializeObject(backupData, formatting);
                fileId = await OneDriveHelper.UploadAsync(folderId, fileName, json);
            }

            return fileId != null;
        }

        private async Task<bool> Restore()
        {
            // Získání souboru
            string folderId = await OneDriveHelper.SearchFolderIdAsync(BackupFolderName);

            // Získání seznamu souborů se zálohami
            BackupData backupData = null;
            if (folderId != null)
            {
                ValueToString<OneDriveFileInfo> selectedItem = RestorePicker.SelectedItem as ValueToString<OneDriveFileInfo>;
                if (selectedItem != null && selectedItem.Value !=null)
                {
                    OneDriveFileInfo fileInfo = selectedItem.Value;
                    string fileDate = await OneDriveHelper.DownloadAsync(fileInfo.Id);
                    backupData = JsonConvert.DeserializeObject<BackupData>(fileDate, new JsonSerializerSettings());

                    if (backupData.Info.Version != App.VersionString)
                    {

                    }
                }
            }

            return backupData != null;
        }
        #endregion

        #region Event Handlers
        private async void SignButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (IsLoggedIn)
            {
                IsLoggedIn = false;
                OneDriveHelper.Logout();
            }
            else
            {
                NotPerformingAction = false;

                IsSigning = true;
                IsLoggedIn = await OneDriveHelper.LoginAsync();
                IsSigning = false;

                if (IsLoggedIn)
                {
                    BackupRestoreProgressRing.Content = AppResources.BackupDownloadingList;
                    await RefreshBackupList();
                }
                else
                {
                    MessageBox.Show(string.Format(AppResources.UnknownError), AppResources.SignIn, MessageBoxButton.OK);
                }

                NotPerformingAction = true;
            }
        }

        private async void BackupButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BackupRestoreProgressRing.Content = AppResources.BackingUp;
            NotPerformingAction = false;

            if (await Backup())
            {
                await RefreshBackupList();
            }
            else
            {
                MessageBox.Show(string.Format(AppResources.UnknownError), AppResources.BackupText, MessageBoxButton.OK);
            }

            NotPerformingAction = true;
        }

        private async void RestoreButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BackupRestoreProgressRing.Content = AppResources.Restoring;
            NotPerformingAction = false;

            if (await Restore())
            {
                // OK
            }
            else
            {
                MessageBox.Show(string.Format(AppResources.UnknownError), AppResources.RestoreText, MessageBoxButton.OK);
            }

            NotPerformingAction = true;
        }

        private async void RefreshListButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BackupRestoreProgressRing.Content = AppResources.BackupDownloadingList;
            NotPerformingAction = false;

            await RefreshBackupList();

            NotPerformingAction = true;
        }
        #endregion
    }
}