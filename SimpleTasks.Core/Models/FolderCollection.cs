using Newtonsoft.Json;
using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    [CollectionDataContract(Name = "Folders", Namespace = "")]
    public class FolderCollection : ObservableCollection<Folder>
    {
        public FolderCollection() : base() { }

        public FolderCollection(IEnumerable<Folder> folders) : base(folders) { }

        public static FolderCollection LoadFromFile(string fileName)
        {
            return FileHelper.ReadFromJson<FolderCollection>(fileName);
        }

        public static void SaveToFile(string fileName, FolderCollection folders)
        {
            FileHelper.WriteToJson(fileName, folders);
        }
    }
}
