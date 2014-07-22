using Newtonsoft.Json;
using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    [DataContract(Name = "Settings", Namespace = "")]
    public class Settings : BindableBase
    {
        #region Settings
        public Settings()
        {
            Tasks = new TasksSettings(this);
            Tiles = new TilesSettings(this);
        }

        [DataMember(Name = "Tasks")]
        public TasksSettings Tasks { get; set; }

        [DataMember(Name = "Tiles")]
        public TilesSettings Tiles { get; set; }

        public static Settings LoadFromFile(string fileName)
        {
            Debug.WriteLine(string.Format("> Nahrávám nastavení ze souboru {0}...", fileName));

            Settings settings = new Settings();
            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isf.FileExists(fileName))
                    {
                        using (Stream stream = isf.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                        {
                            StreamReader sr = new StreamReader(stream);
                            string s = sr.ReadToEnd();
                            Debug.WriteLine("> Settings: \n{0}", s);
                            settings = JsonConvert.DeserializeObject<Settings>(s);
                            sr.Close();
                            Debug.WriteLine(": Nahrávání nastavení dokončeno.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při nahrávání nastavení: {0}", e.Message);
            }

            return settings;
        }

        public static void SaveToFile(string fileName, Settings settings)
        {
            Debug.WriteLine(string.Format("> Ukládám nastavení do souboru {0}...", fileName));

            string data = JsonConvert.SerializeObject(settings, Formatting.Indented);
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (Stream stream = isf.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream);
                    sw.Write(data);
                    sw.Flush();
                    sw.Close();
                    Debug.WriteLine(": Ukládání nastavení dokončeno.");
                }
            }
        }
        #endregion

        #region Version
        private string _Version = null;
        [DataMember(Name = "Version")]
        public string Version
        {
            get { return _Version; }
            set { SetProperty(ref _Version, value); }
        }
        #endregion

        #region Tasks
        [DataContract(Name = "TasksSettings", Namespace = "")]
        public class TasksSettings : SettingsCategory
        {
            public TasksSettings(Settings settings) : base(settings) { }

            #region DefaultDate
            public enum DefaultDateTypes
            {
                NoDueDate,
                Today,
                Tomorrow,
                ThisWeek,
                NextWeek
            }

            private DefaultDateTypes _DefaultDateType = DefaultDateTypes.Today;
            [DataMember(Name = "DefaultDateType")]
            public DefaultDateTypes DefaultDateType
            {
                get { return _DefaultDateType; }
                set { SetProperty(ref _DefaultDateType, value); }
            }

            public DateTime? DefaultDate
            {
                get
                {
                    switch (DefaultDateType)
                    {
                    case DefaultDateTypes.Today: return DateTimeExtensions.Today;
                    case DefaultDateTypes.Tomorrow: return DateTimeExtensions.Tomorrow;
                    case DefaultDateTypes.ThisWeek: return DateTimeExtensions.LastDayOfActualWeek;
                    case DefaultDateTypes.NextWeek: return DateTimeExtensions.LastDayOfNextWeek;

                    case DefaultDateTypes.NoDueDate:
                    default: return null;
                    }
                }
            }

            public List<DefaultDateTypes> DefaultDatePickerList
            {
                get
                {
                    return new List<DefaultDateTypes>() 
                    { 
                        DefaultDateTypes.NoDueDate,
                        DefaultDateTypes.Today,    
                        DefaultDateTypes.Tomorrow, 
                        DefaultDateTypes.ThisWeek, 
                        DefaultDateTypes.NextWeek
                    };
                }
            }
            #endregion

            #region DefaultTime
            private DateTime _DefaultTime = new DateTime(1, 1, 1, 9, 0, 0);
            [DataMember(Name = "DefaultTime")]
            public DateTime DefaultTime
            {
                get { return _DefaultTime; }
                set { SetProperty(ref _DefaultTime, value); }
            }
            #endregion

            #region DeleteCompleted
            private int _DeleteCompleted = 3;
            [DataMember(Name = "DeleteCompleted")]
            public int DeleteCompleted
            {
                get { return _DeleteCompleted; }
                set { SetProperty(ref _DeleteCompleted, value); }
            }

            public DateTime DeleteCompletedBefore
            {
                get
                {
                    if (DeleteCompleted < 0)
                    {
                        // nikdy nic nemazat
                        return DateTime.MinValue; 
                    }
                    else if (DeleteCompleted == 0)
                    {
                        // smazat všechno
                        return DateTime.MaxValue; 
                    }
                    else
                    {
                        return DateTime.Now.AddDays(-DeleteCompleted);
                    }
                }
            }

            public List<int> DeleteCompletedPickerList
            {
                get
                {
                    return new List<int>() 
                    { 
                        -1,
                        0, 
                        1, 
                        2, 
                        3, 
                        7, 
                        14
                    };
                }
            }
            #endregion
        }
        #endregion

        #region Tiles
        [DataContract(Name = "TilesSettings", Namespace = "")]
        public class TilesSettings : SettingsCategory
        {
            public TilesSettings(Settings settings) : base(settings) { }

            #region Enable
            private bool _enable = true;
            [DataMember(Name = "Enable")]
            public bool Enable
            {
                get { return _enable; }
                set
                {
                    if (SetProperty(ref _enable, value))
                    {
                        //LiveTile.UpdateOrReset(value, App.Tasks.Tasks, true);
                    }
                }
            }
            #endregion

            #region UnpinCompleted
            private bool _unpinCompleted = true;
            [DataMember(Name = "UnpinCompleted")]
            public bool UnpinCompleted
            {
                get { return _unpinCompleted; }
                set { SetProperty(ref _unpinCompleted, value); }
            }
            #endregion
        }
        #endregion
    }
}
