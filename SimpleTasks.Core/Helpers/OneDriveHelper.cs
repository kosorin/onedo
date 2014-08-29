using Microsoft.Live;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Helpers
{
    public static class OneDriveHelper
    {
        #region Public fields
        public const string ClientId = "0000000048122FFF";

        public const string Root = "me/skydrive";

        public static readonly List<string> Scopes = new List<string> 
        { 
            "wl.signin", 
            "wl.skydrive_update", 
            "wl.offline_access",
            "wl.basic"
        };
        #endregion

        #region Private fields
        private static LiveConnectClient _client = null;

        private static LiveAuthClient _auth = null;
        #endregion

        #region Login/Logout, User
        public async static Task<bool> LoginAsync()
        {
            try
            {
                _auth = new LiveAuthClient(ClientId);
                LiveLoginResult result = await _auth.InitializeAsync(Scopes);
                if (result.Status != LiveConnectSessionStatus.Connected)
                {
                    result = await _auth.LoginAsync(Scopes);
                }
                if (result.Status == LiveConnectSessionStatus.Connected)
                {
                    _client = new LiveConnectClient(result.Session);
                    return true;
                }
            }
            catch (Exception) { }
            _client = null;
            _auth = null;
            return false;
        }

        public async static Task<bool> SilentLoginAsync()
        {
            try
            {
                _auth = new LiveAuthClient(ClientId);
                LiveLoginResult result = await _auth.InitializeAsync(Scopes);
                if (result.Status == LiveConnectSessionStatus.Connected)
                {
                    _client = new LiveConnectClient(result.Session);
                    return true;
                }
            }
            catch (Exception) { }
            _client = null;
            _auth = null;
            return false;
        }

        public static void Logout()
        {
            if (_auth != null)
            {
                try { _auth.Logout(); }
                catch (Exception) { }
            }
            _client = null;
            _auth = null;
        }

        public async static Task<string> GetUserNameAsync()
        {
            string userName = null;
            try
            {
                LiveOperationResult operationResult = await _client.GetAsync("me");
                dynamic result = operationResult.Result;
                userName = result.name;
            }
            catch (Exception) { }
            return userName;
        }
        #endregion

        #region Folders
        public async static Task<string> SearchFolderIdAsync(string folderName)
        {
            string folderId = null;
            try
            {
                LiveOperationResult operationResult = await _client.GetAsync(string.Format("{0}/search?q={1}", Root, folderName));
                dynamic result = operationResult.Result;
                foreach (dynamic folder in result.data)
                {
                    if (folder.type == "folder" && folder.name.ToLowerInvariant() == folderName.ToLowerInvariant())
                    {
                        folderId = folder.id;
                        break;
                    }
                }
            }
            catch (Exception) { }
            return folderId;
        }

        public async static Task<string> GetFolderIdAsync(string parentFolderId, string folderName)
        {
            string folderId = null;
            try
            {
                LiveOperationResult operationResult = await _client.GetAsync(parentFolderId + "/files?filter=folders");
                dynamic result = operationResult.Result;
                foreach (dynamic folder in result.data)
                {
                    if (folder.name.ToLowerInvariant() == folderName.ToLowerInvariant())
                    {
                        folderId = folder.id;
                        break;
                    }
                }
            }
            catch (Exception) { }
            return folderId;
        }

        public async static Task<string> CreateFolderAsync(string parentFolderId, string folderName)
        {
            try
            {
                Dictionary<string, object> folderData = new Dictionary<string, object>();
                folderData["name"] = folderName;

                LiveOperationResult operationResult = await _client.PostAsync(parentFolderId, folderData);
                dynamic result = operationResult.Result;

                return result.id;
            }
            catch (Exception) { }
            return null;
        }
        #endregion

        #region Files
        public async static Task<List<OneDriveFileInfo>> GetFilesInfoAsync(string folderId, string fileExtension)
        {
            List<OneDriveFileInfo> files = new List<OneDriveFileInfo>();
            try
            {
                LiveOperationResult operationResult = await _client.GetAsync(folderId + "/files");
                dynamic result = operationResult.Result;
                foreach (dynamic file in result.data)
                {
                    if (file.type == "file" && ((string)file.name).EndsWith(fileExtension))
                    {
                        DateTime created = DateTime.MinValue;
                        DateTime.TryParse(file.created_time, out created);

                        OneDriveFileInfo fileInfo = new OneDriveFileInfo
                        {
                            Id = file.id,
                            Name = file.name,
                            FolderId = file.parent_id,
                            Created = created
                        };
                        files.Add(fileInfo);
                    }
                }
            }
            catch (Exception) { }
            return files;
        }

        public async static Task<string> GetFileIdAsync(string folderId, string fileName)
        {
            string fileId = null;
            try
            {
                LiveOperationResult operationResult = await _client.GetAsync(folderId + "/files");
                dynamic result = operationResult.Result;
                foreach (dynamic file in result.data)
                {
                    if (file.type == "file" && file.name.ToLowerInvariant() == fileName.ToLowerInvariant())
                    {
                        fileId = file.id;
                        break;
                    }
                }
            }
            catch (Exception) { }
            return fileId;
        }
        #endregion

        #region Upload/Download
        public async static Task<string> UploadAsync(string folderId, string fileName, Stream data)
        {
            string fileId = null;
            try
            {
                LiveOperationResult operationResult = await _client.UploadAsync(folderId, fileName, data, OverwriteOption.Overwrite);
                foreach (var item in operationResult.Result)
                {
                    if (item.Key == "id")
                    {
                        fileId = item.Value as string;
                        break;
                    }
                }
            }
            catch (Exception) { }
            return fileId;
        }

        public async static Task<string> UploadAsync(string folderId, string fileName, string data)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter sw = new StreamWriter(stream);
            sw.Write(data);
            sw.Flush();
            stream.Position = 0;

            return await UploadAsync(folderId, fileName, stream);
        }

        public async static Task<string> DownloadAsync(string folderId, string fileName)
        {
            try
            {
                string fileId = await GetFileIdAsync(folderId, fileName);
                if (fileId != null)
                {
                    LiveDownloadOperationResult operationResult = await _client.DownloadAsync(fileId + "/content");
                    if (operationResult.Stream != null)
                    {
                        operationResult.Stream.Position = 0;
                        StreamReader sr = new StreamReader(operationResult.Stream);
                        return await sr.ReadToEndAsync();
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        public async static Task<string> DownloadAsync(string fileId)
        {
            try
            {
                if (fileId != null)
                {
                    LiveDownloadOperationResult operationResult = await _client.DownloadAsync(fileId + "/content");
                    if (operationResult.Stream != null)
                    {
                        operationResult.Stream.Position = 0;
                        StreamReader sr = new StreamReader(operationResult.Stream);
                        return await sr.ReadToEndAsync();
                    }
                }
            }
            catch (Exception) { }
            return null;
        }
        #endregion
    }
}
