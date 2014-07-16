using SimpleTasks.Core.Helpers;
using SimpleTasks.Helpers;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;

namespace SimpleTasks.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
#if DEBUG
            Debug.WriteLine("> Nastavení:");
            foreach (object o in settings)
            {
                Debug.WriteLine("  {0}", o.ToString());
            }
#endif
            SetDatePicker();
            SetDaysPicker();
        }

        #region Settings

        private IsolatedStorageSettings settings;

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }

        #endregion Settings

        #region LastVersion
        private const string LastVersionKeyName = "LastVersion";
        private readonly string LastVersionDefault = null;
        public string LastVersionSetting
        {
            get { return GetValueOrDefault<string>(LastVersionKeyName, LastVersionDefault); }
            set
            {
                if (AddOrUpdateValue(LastVersionKeyName, value))
                {
                    Save();
                }
            }
        }

        #endregion

        #region DefaultDate
        private const string DefaultDateKeyName = "DefaultDueDate";
        private const DefaultDateTypes DefaultDateDefault = DefaultDateTypes.NextWeek;
        public DefaultDateTypes DefaultDateSetting
        {
            get
            {
                return GetValueOrDefault<DefaultDateTypes>(DefaultDateKeyName, DefaultDateDefault);
            }
            set
            {
                if (AddOrUpdateValue(DefaultDateKeyName, value))
                {
                    Save();
                }
            }
        }

        public enum DefaultDateTypes
        {
            NoDueDate,
            Today,
            Tomorrow,
            ThisWeek,
            NextWeek
        }

        public DateTime? DefaultDate
        {
            get
            {
                switch (DefaultDateSetting)
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

        public List<KeyValuePair<DefaultDateTypes, string>> DatePickerItems { get; private set; }

        private KeyValuePair<DefaultDateTypes, string> _datePickerSelectedItem;
        public KeyValuePair<DefaultDateTypes, string> DatePickerSelectedItem
        {
            get
            {
                return _datePickerSelectedItem;
            }
            set
            {
                SetProperty(ref _datePickerSelectedItem, value);
                DefaultDateSetting = value.Key;
            }
        }

        private void SetDatePicker()
        {
            DatePickerItems = new List<KeyValuePair<DefaultDateTypes, string>>();
            DatePickerItems.Add(new KeyValuePair<DefaultDateTypes, string>(DefaultDateTypes.NoDueDate, AppResources.DateNoDue));
            DatePickerItems.Add(new KeyValuePair<DefaultDateTypes, string>(DefaultDateTypes.Today, AppResources.DateToday));
            DatePickerItems.Add(new KeyValuePair<DefaultDateTypes, string>(DefaultDateTypes.Tomorrow, AppResources.DateTomorrow));
            DatePickerItems.Add(new KeyValuePair<DefaultDateTypes, string>(DefaultDateTypes.ThisWeek, AppResources.DateThisWeek));
            DatePickerItems.Add(new KeyValuePair<DefaultDateTypes, string>(DefaultDateTypes.NextWeek, AppResources.DateNextWeek));

            var selectedItems = DatePickerItems.Where((i) => { return i.Key == DefaultDateSetting; });
            if (selectedItems.Count() > 0)
                DatePickerSelectedItem = selectedItems.First();
            else
                DatePickerSelectedItem = DatePickerItems.First();
        }
        #endregion DefaultDate

        #region DefaultTime
        private const string DefaultTimeKeyName = "DefaultReminderTime";
        private readonly DateTime DefaultTimeDefault = new DateTime(1, 1, 1, 9, 0, 0);
        public DateTime DefaultTimeSetting
        {
            get
            {
                return GetValueOrDefault<DateTime>(DefaultTimeKeyName, DefaultTimeDefault);
            }
            set
            {
                if (AddOrUpdateValue(DefaultTimeKeyName, value))
                {
                    Save();
                }
            }
        }
        #endregion

        #region EnableLiveTile

        private const string EnableLiveTileKeyName = "EnableLiveTile";
        private readonly bool EnableLiveTileDefault = true;
        public bool EnableLiveTileSetting
        {
            get
            {
                return GetValueOrDefault<bool>(EnableLiveTileKeyName, EnableLiveTileDefault);
            }
            set
            {
                if (AddOrUpdateValue(EnableLiveTileKeyName, value))
                {
                    LiveTile.UpdateOrReset(value, App.Tasks.Tasks, true);
                    Save();
                }
            }
        }

        #endregion

        #region UnpinCompletedTile

        private const string UnpinCompletedKeyName = "UnpinCompleted";
        private readonly bool UnpinCompletedDefault = true;
        public bool UnpinCompletedSetting
        {
            get
            {
                return GetValueOrDefault<bool>(UnpinCompletedKeyName, UnpinCompletedDefault);
            }
            set
            {
                if (AddOrUpdateValue(UnpinCompletedKeyName, value))
                {
                    Save();
                }
            }
        }

        #endregion

        #region DeleteCompleted

        private const string DeleteCompletedTasksDaysKeyName = "DeleteCompletedTasksDays";
        private const int DeleteCompletedTasksDaysDefault = 3;
        public int DeleteCompletedTasksDaysSetting
        {
            get
            {
                return GetValueOrDefault<int>(DeleteCompletedTasksDaysKeyName, DeleteCompletedTasksDaysDefault);
            }
            set
            {
                if (AddOrUpdateValue(DeleteCompletedTasksDaysKeyName, value))
                {
                    Save();
                }
            }
        }

        private KeyValuePair<int, string> _daysPickerSelectedItem;
        public KeyValuePair<int, string> DaysPickerSelectedItem
        {
            get
            {
                return _daysPickerSelectedItem;
            }
            set
            {
                SetProperty(ref _daysPickerSelectedItem, value);
                DeleteCompletedTasksDaysSetting = value.Key;
            }
        }

        public List<KeyValuePair<int, string>> DaysPickerItems { get; private set; }

        private void SetDaysPicker()
        {
            DaysPickerItems = new List<KeyValuePair<int, string>>();
            DaysPickerItems.Add(new KeyValuePair<int, string>(-1, AppResources.SettingsDeleteNever));
            DaysPickerItems.Add(new KeyValuePair<int, string>(0, AppResources.SettingsDeleteWhenStarts));
            DaysPickerItems.Add(new KeyValuePair<int, string>(1, AppResources.SettingsDeleteAfterOneDay));
            DaysPickerItems.Add(new KeyValuePair<int, string>(2, AppResources.SettingsDeleteAfterTwoDays));
            DaysPickerItems.Add(new KeyValuePair<int, string>(3, AppResources.SettingsDeleteAfterThreeDays));
            DaysPickerItems.Add(new KeyValuePair<int, string>(7, AppResources.SettingsDeleteAfterOneWeek));
            DaysPickerItems.Add(new KeyValuePair<int, string>(14, AppResources.SettingsDeleteAfterTwoWeeks));

            var selectedItems = DaysPickerItems.Where((i) => { return i.Key == DeleteCompletedTasksDaysSetting; });
            if (selectedItems.Count() > 0)
                DaysPickerSelectedItem = selectedItems.First();
            else
                DaysPickerSelectedItem = DaysPickerItems.First();
        }

        #endregion DeleteCompleted
    }
}
