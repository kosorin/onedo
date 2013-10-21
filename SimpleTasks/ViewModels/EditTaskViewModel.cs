using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.ViewModels
{
    public class EditTaskViewModel : BindableBase
    {
        private TaskModel _oldTask = null;
        public TaskModel OldTask
        {
            get
            {
                return _oldTask;
            }
            set
            {
                SetProperty(ref _oldTask, value);
            }
        }

        private TaskModel _currentTask;
        public TaskModel CurrentTask
        {
            get
            {
                return _currentTask;
            }
            set
            {
                SetProperty(ref _currentTask, value);
            }
        }

        private const string IsOverduePropertyString = "IsOverdue";
        public bool IsOverdue
        {
            get
            {
                if (CurrentDueDate.Date == null)
                    return false;
                else
                    return (CurrentDueDate.Date < DateTimeExtensions.Today);
            }
        }

        public List<DueDateModel> DueDateList { get; private set; }

        private DueDateModel _currentDueDate;
        public DueDateModel CurrentDueDate
        {
            get
            {
                return _currentDueDate;
            }
            set
            {
                SetProperty(ref _currentDueDate, value);
                OnPropertyChanged(IsOverduePropertyString);
            }
        }

        private DueDateModel _customDueDate;
        public DueDateModel CustomDueDate
        {
            get
            {
                return _customDueDate;
            }
            set
            {
                SetProperty(ref _customDueDate, value);
                CustomDueDate.PropertyChanged += (s, e) => { OnPropertyChanged(IsOverduePropertyString); };
            }
        }

        public EditTaskViewModel()
            : this(null)
        {
        }

        public EditTaskViewModel(TaskModel oldTask)
        {
            OldTask = oldTask;
            CurrentTask = new TaskModel();

            if (OldTask != null)
            {
                CurrentTask.Title = OldTask.Title;
                CurrentTask.Date = OldTask.Date;
                CurrentTask.IsImportant = OldTask.IsImportant;
                CurrentTask.IsComplete = OldTask.IsComplete;
            }

            DueDateList = BuildDueDateList();
        }

        private List<DueDateModel> BuildDueDateList()
        {
            List<DueDateModel> dueDateList = new List<DueDateModel>();

            // Vlastní datum
            dueDateList.Add(CustomDueDate = new DueDateModel()
            {
                Type = DueDateModel.DueDatePickerType.CustomDate,
                Title = AppResources.DateCustomDueText,
                Date = DateTimeExtensions.Today
            });

            // Bez data
            DueDateModel noDueDate;
            dueDateList.Add(noDueDate = new DueDateModel()
            {
                Type = DueDateModel.DueDatePickerType.NoDueDate,
                Title = AppResources.DateNoDueText,
                Date = null
            });

            // Dnes
            DueDateModel todayDueDate;
            dueDateList.Add(todayDueDate = new DueDateModel()
            {
                Type = DueDateModel.DueDatePickerType.Date,
                Title = AppResources.DateTodayText,
                Date = DateTimeExtensions.Today
            });

            // Zítra
            DueDateModel tomorrowDueDate;
            dueDateList.Add(tomorrowDueDate = new DueDateModel()
            {
                Type = DueDateModel.DueDatePickerType.Date,
                Title = AppResources.DateTomorrowText,
                Date = DateTimeExtensions.Tomorrow
            });

            // Pár dní po zítřku
            int daysAfterTomorrow = 4;
            for (int i = 1; i <= daysAfterTomorrow; i++)
            {
                DateTime date = DateTimeExtensions.Tomorrow.AddDays(i);
                dueDateList.Add(new DueDateModel()
                {
                    Type = DueDateModel.DueDatePickerType.Date,
                    Title = date.ToString("dddd", CultureInfo.CurrentCulture).ToLower(),
                    Date = date
                });
            }

            // Tento a přístí týden
            // Tento týden se zobrazí pouze v případě, že poslední den v týdnu není dnešek nebo zítřek.
            DueDateModel thisWeekDueDate;
            DueDateModel nextWeekDueDate;
            dueDateList.Add(thisWeekDueDate = new DueDateModel()
            {
                Type = DueDateModel.DueDatePickerType.Date,
                Title = AppResources.DateThisWeekText,
                Date = DateTimeExtensions.LastDayOfWeek
            });
            dueDateList.Add(nextWeekDueDate = new DueDateModel()
            {
                Type = DueDateModel.DueDatePickerType.Date,
                Title = AppResources.DateNextWeekText,
                Date = DateTimeExtensions.LastDayOfNextWeek
            });

            // Tento a přístí měsíc
            // Tento měsíc se zobrazí pouze v případě, že poslední den v měsíci není dnešek nebo zítřek.
            dueDateList.Add(new DueDateModel()
            {
                Type = DueDateModel.DueDatePickerType.Date,
                Title = AppResources.DateThisMonthText,
                Date = DateTimeExtensions.LastDayOfMonth
            });
            dueDateList.Add(new DueDateModel()
            {
                Type = DueDateModel.DueDatePickerType.Date,
                Title = AppResources.DateNextMonthText,
                Date = DateTimeExtensions.LastDayOfNextMonth
            });

            // Nastaví aktuální termín
            if (OldTask != null)
            {
                if (CurrentTask.Date == null)
                {
                    CurrentDueDate = noDueDate;
                }
                else if (CurrentTask.Date == DateTimeExtensions.Today)
                {
                    CurrentDueDate = todayDueDate;
                }
                else if (CurrentTask.Date == DateTimeExtensions.Tomorrow)
                {
                    CurrentDueDate = tomorrowDueDate;
                }
                else
                {
                    CustomDueDate.Date = CurrentTask.Date;
                    CurrentDueDate = CustomDueDate;
                }
            }
            else
            {
                switch (App.Settings.DefaultDueDateSetting)
                {
                case SettingsViewModel.DefaultDueDate.Today:
                    CurrentDueDate = todayDueDate;
                    break;
                case SettingsViewModel.DefaultDueDate.Tomorrow:
                    CurrentDueDate = tomorrowDueDate;
                    break;
                case SettingsViewModel.DefaultDueDate.ThisWeek:
                    CurrentDueDate = thisWeekDueDate;
                    break;
                case SettingsViewModel.DefaultDueDate.NextWeek:
                    CurrentDueDate = nextWeekDueDate;
                    break;

                case SettingsViewModel.DefaultDueDate.NoDueDate:
                default:
                    CurrentDueDate = noDueDate;
                    break;
                }
            }

            return dueDateList;
        }

        public void ActivateTask()
        {
            CurrentTask.IsComplete = false;
            CurrentTask.CompletedDate = null;
            SaveTask();
        }

        public void CompleteTask()
        {
            CurrentTask.IsComplete = true;
            CurrentTask.CompletedDate = DateTime.Now;
            SaveTask();
        }

        public void SaveTask()
        {
            CurrentTask.Date = CurrentDueDate.Date;

            App.ViewModel.Tasks.Remove(OldTask);
            App.ViewModel.Tasks.Add(CurrentTask);
        }

        public void DeleteTask()
        {
            App.ViewModel.Tasks.Remove(OldTask);
        }
    }
}
