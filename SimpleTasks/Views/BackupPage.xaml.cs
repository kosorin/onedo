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
        private string _userName = "";
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

                BackupRestoreProgressRing.Text = AppResources.BackupDownloadingList;
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
                OnPropertyChanged("SignedInText");
            }
        }

        private bool _isSigning = false;
        public bool IsSigning
        {
            get { return _isSigning; }
            set { SetProperty(ref _isSigning, value); }
        }

        public string SignedInText
        {
            get { return IsLoggedIn ? string.Format(AppResources.SignedInYes, OneDriveHelper.CurrentUserName) : AppResources.SignedInNo; }
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
            get { return NotPerformingAction && (UseTasks || UseSettings) && (BackupList.Count > 0 && BackupList[0].Value1 != null); }
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

                if (list.Count >= 8)
                {
                    break;
                }
            }

            // Seřazení podle vytvoření
            list.Sort((f1, f2) =>
            {
                return f2.Value2.CompareTo(f1.Value2);
            });

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

        private async Task<bool> Backup()
        {
            // Získání id složky
            string folderId = await OneDriveHelper.SearchFolderIdAsync(BackupFolderName);
            if (folderId == null)
            {
                folderId = await OneDriveHelper.CreateFolderAsync(OneDriveHelper.Root, BackupFolderName);
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
                fileId = await OneDriveHelper.UploadAsync(folderId, fileName, json);
            }

            return fileId != null;
        }

        private async Task<bool> Restore()
        {
            BackupData backupData = null;

            RestoreListPickerItem selectedItem = RestorePicker.SelectedItem as RestoreListPickerItem;
            if (selectedItem != null && selectedItem.Value1 != null)
            {
                // Stáhnutí souboru
                string data = await OneDriveHelper.DownloadAsync(selectedItem.Value1.Id);
                if (data != null)
                {
                    backupData = JsonConvert.DeserializeObject<BackupData>(data, new JsonSerializerSettings());

                    // Zpracování dat
                    if (backupData.Version != App.VersionString)
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
                    BackupRestoreProgressRing.Text = AppResources.BackupDownloadingList;
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
            BackupRestoreProgressRing.Text = AppResources.BackingUp;
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
            BackupRestoreProgressRing.Text = AppResources.Restoring;
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
            BackupRestoreProgressRing.Text = AppResources.BackupDownloadingList;
            NotPerformingAction = false;

            await RefreshBackupList();

            NotPerformingAction = true;
        }
        #endregion
    }
}