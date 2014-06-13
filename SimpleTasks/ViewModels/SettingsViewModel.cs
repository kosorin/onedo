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
            foreach (object o in IsolatedStorageSettings.ApplicationSettings)
            {
                Debug.WriteLine("  {0}", o.ToString());
            }
#endif
            SetDueDatePicker();
            SetFirstDayOfWeekPicker();
            ApplyFirstDayOfWeekSetting();
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
            get
            {
                return GetValueOrDefault<string>(LastVersionKeyName, LastVersionDefault);
            }
            set
            {
                if (AddOrUpdateValue(LastVersionKeyName, value))
                {
                    Save();
                }
            }
        }

        #endregion

        #region DefaultDueDate

        private const string DefaultDueDateKeyName = "DefaultDueDate";
        private const DefaultDueDate DefaultDueDateDefault = DefaultDueDate.NoDueDate;
        public DefaultDueDate DefaultDueDateSetting
        {
            get
            {
                return GetValueOrDefault<DefaultDueDate>(DefaultDueDateKeyName, DefaultDueDateDefault);
            }
            set
            {
                if (AddOrUpdateValue(DefaultDueDateKeyName, value))
                {
                    Save();
                }
            }
        }

        public enum DefaultDueDate
        {
            NoDueDate,
            Today,
            Tomorrow,
            ThisWeek,
            NextWeek
        }

        public DateTime? DefaultDueDateSettingToDateTime
        {
            get
            {
                switch (DefaultDueDateSetting)
                {
                case DefaultDueDate.Today: return DateTimeExtensions.Today;
                case DefaultDueDate.Tomorrow: return DateTimeExtensions.Tomorrow;
                case DefaultDueDate.ThisWeek: return DateTimeExtensions.LastDayOfWeek;
                case DefaultDueDate.NextWeek: return DateTimeExtensions.LastDayOfNextWeek;

                case DefaultDueDate.NoDueDate:
                default: return null;
                }
            }
        }

        public List<KeyValuePair<DefaultDueDate, string>> DueDatePickerItems { get; private set; }

        private KeyValuePair<DefaultDueDate, string> _dueDatePickerSelectedItem;
        public KeyValuePair<DefaultDueDate, string> DueDatePickerSelectedItem
        {
            get
            {
                return _dueDatePickerSelectedItem;
            }
            set
            {
                SetProperty(ref _dueDatePickerSelectedItem, value);
                DefaultDueDateSetting = value.Key;
            }
        }

        private void SetDueDatePicker()
        {
            DueDatePickerItems = new List<KeyValuePair<DefaultDueDate, string>>();
            DueDatePickerItems.Add(new KeyValuePair<DefaultDueDate, string>(DefaultDueDate.NoDueDate, AppResources.DateNoDue));
            DueDatePickerItems.Add(new KeyValuePair<DefaultDueDate, string>(DefaultDueDate.Today, AppResources.DateToday));
            DueDatePickerItems.Add(new KeyValuePair<DefaultDueDate, string>(DefaultDueDate.Tomorrow, AppResources.DateTomorrow));
            DueDatePickerItems.Add(new KeyValuePair<DefaultDueDate, string>(DefaultDueDate.ThisWeek, AppResources.DateThisWeek));
            DueDatePickerItems.Add(new KeyValuePair<DefaultDueDate, string>(DefaultDueDate.NextWeek, AppResources.DateNextWeek));

            var selectedItems = DueDatePickerItems.Where((i) => { return i.Key == DefaultDueDateSetting; });
            if (selectedItems.Count() > 0)
                DueDatePickerSelectedItem = selectedItems.First();
            else
                DueDatePickerSelectedItem = DueDatePickerItems.First();
        }

        #endregion DefaultDueDate

        #region DefaultReminderTime

        private const string DefaultReminderTimeKeyName = "DefaultReminderTime";
        private readonly DateTime DefaultReminderTimeDefault = new DateTime(1, 1, 1, 8, 0, 0);
        public DateTime DefaultReminderTimeSetting
        {
            get
            {
                return GetValueOrDefault<DateTime>(DefaultReminderTimeKeyName, DefaultReminderTimeDefault);
            }
            set
            {
                if (AddOrUpdateValue(DefaultReminderTimeKeyName, value))
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

        #region DEPRECATED FirstDayOfWeek

        private const string FirstDayOfWeekKeyName = "FirstDayOfWeek";
        private readonly DayOfWeek FirstDayOfWeekDefault = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        public DayOfWeek FirstDayOfWeekSetting
        {
            get
            {
                return GetValueOrDefault<DayOfWeek>(FirstDayOfWeekKeyName, FirstDayOfWeekDefault);
            }
            set
            {
                if (AddOrUpdateValue(FirstDayOfWeekKeyName, value))
                {
                    Save();
                }
                ApplyFirstDayOfWeekSetting();
            }
        }

        public List<KeyValuePair<DayOfWeek, string>> FirstDayOfWeekPickerItems { get; private set; }

        private KeyValuePair<DayOfWeek, string> _firstDayOfWeekPickerSelectedItem;
        public KeyValuePair<DayOfWeek, string> FirstDayOfWeekPickerSelectedItem
        {
            get
            {
                return _firstDayOfWeekPickerSelectedItem;
            }
            set
            {
                SetProperty(ref _firstDayOfWeekPickerSelectedItem, value);
                FirstDayOfWeekSetting = value.Key;
            }
        }

        private void ApplyFirstDayOfWeekSetting()
        {
            DateTimeExtensions.FirstDayOfWeek = FirstDayOfWeekSetting;
        }

        private void SetFirstDayOfWeekPicker()
        {
            FirstDayOfWeekPickerItems = new List<KeyValuePair<DayOfWeek, string>>();
            for (int i = 0; i < Enum.GetNames(typeof(DayOfWeek)).Length; i++)
                FirstDayOfWeekPickerItems.Add(new KeyValuePair<DayOfWeek, string>((DayOfWeek)i, CultureInfo.CurrentCulture.DateTimeFormat.DayNames[i].ToString()));

            var selectedItems = FirstDayOfWeekPickerItems.Where((i) => { return i.Key == FirstDayOfWeekSetting; });
            if (selectedItems.Count() > 0)
                FirstDayOfWeekPickerSelectedItem = selectedItems.First();
            else
                FirstDayOfWeekPickerSelectedItem = FirstDayOfWeekPickerItems.First();
        }

        #endregion FirstDayOfWeek

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
