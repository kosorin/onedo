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
        private static Settings _current = null;
        public static Settings Current
        {
            get { return _current ?? (_current = new Settings()); }
            set { _current = value; }
        }

        public Settings()
        {
        }

        public static Settings LoadFromFile(string fileName)
        {
            Settings settings = FileHelper.ReadFromJson<Settings>(fileName);
            Debug.WriteLine(settings);
            return settings;
        }

        public static void SaveToFile(string fileName, Settings settings)
        {
            FileHelper.WriteToJson(fileName, settings);
        }

        public override string ToString()
        {
#if DEBUG
            return string.Format("> Nastavení: \n{0}", JsonConvert.SerializeObject(this, Formatting.Indented));
#else
            return "";
#endif
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
        private DateTime _DefaultTime = new DateTime(1, 1, 1, 15, 0, 0);
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

        #region CompleteSubtasks
        private bool _completeSubtasks = true;
        [DataMember(Name = "CompleteSubtasks")]
        public bool CompleteSubtasks
        {
            get { return _completeSubtasks; }
            set { SetProperty(ref _completeSubtasks, value); }
        }
        #endregion

        #region SwipeActions
        public List<GestureAction> GestureActionPickerList
        {
            get
            {
                return new List<GestureAction>() 
                    { 
                        GestureAction.None,
                        GestureAction.Complete,
                        GestureAction.Delete,
                        GestureAction.DueToday,
                        GestureAction.DueTomorrow,
                        GestureAction.PostponeDay,
                        GestureAction.PostponeWeek,
                    };
            }
        }
        #endregion // end of SwipeActions

        #region SwipeLeftAction
        private GestureAction _swipeLeftAction = GestureAction.Complete;
        [DataMember(Name = "SwipeLeftAction")]
        public GestureAction SwipeLeftAction
        {
            get { return _swipeLeftAction; }
            set { SetProperty(ref _swipeLeftAction, value); }
        }
        #endregion // end of SwipeLeftAction

        #region SwipeRightAction
        private GestureAction _swipeRightAction = GestureAction.None;
        [DataMember(Name = "SwipeRightAction")]
        public GestureAction SwipeRightAction
        {
            get { return _swipeRightAction; }
            set { SetProperty(ref _swipeRightAction, value); }
        }
        #endregion // end of SwipeRightAction

        #endregion

        #region Tiles

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

        #region ShowTaskCount
        private bool _showTaskCount = true;
        [DataMember(Name = "ShowTaskCount")]
        public bool ShowTaskCount
        {
            get { return _showTaskCount; }
            set { SetProperty(ref _showTaskCount, value); }
        }
        #endregion

        #region SwapDateAndTitleOnWide
        private bool _swapDateAndTitleOnWide = false;
        [DataMember(Name = "SwapDateAndTitleOnWide")]
        public bool SwapDateAndTitleOnWide
        {
            get { return _swapDateAndTitleOnWide; }
            set { SetProperty(ref _swapDateAndTitleOnWide, value); }
        }
        #endregion

        #region DefaultTaskTileSettings
        private TaskTileSettings _defaultTaskTileSettings = new TaskTileSettings();
        [DataMember(Name = "DefaultTaskTileSettings")]
        public TaskTileSettings DefaultTaskTileSettings
        {
            get { return _defaultTaskTileSettings; }
            set { SetProperty(ref _defaultTaskTileSettings, value); }
        }
        #endregion

        #endregion

        #region General

        #region Vibrate
        private bool _vibrate = true;
        [DataMember(Name = "Vibrate")]
        public bool Vibrate
        {
            get { return _vibrate; }
            set { SetProperty(ref _vibrate, value); }
        }
        #endregion

        #region HideTaskCheckBox
        private bool _hideTaskCheckBox = false;
        [DataMember(Name = "HideTaskCheckBox")]
        public bool HideTaskCheckBox
        {
            get { return _hideTaskCheckBox; }
            set { SetProperty(ref _hideTaskCheckBox, value); }
        }
        #endregion

        #region Feedback
        private bool _feedback = true;
        [DataMember(Name = "Feedback")]
        public bool Feedback
        {
            get { return _feedback; }
            set { SetProperty(ref _feedback, value); }
        }
        #endregion

        #endregion
    }
}
