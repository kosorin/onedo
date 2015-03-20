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
using SimpleTasks.Core.Models;
using DefaultDateTypes = SimpleTasks.Core.Models.Settings.DefaultDateTypes;
using SimpleTasks.Resources;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Helpers;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media;
using SimpleTasks.Models;

namespace SimpleTasks.Views
{
    public partial class SettingsPage : BasePage
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = Settings.Current;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Inity...
            InitDefaultDate();
            InitDaysPicker();
            InitGesturesPicker();

            // Default Time
            if (IsSetNavigationParameter("TimePicker"))
            {
                Settings.Current.DefaultTime = NavigationParameter<DateTime>("TimePicker");
            }

            // Pin Tile
            _isPinnedQuickAdd = LiveTileN.IsPinnedQuickAdd();
            if (_isPinnedQuickAdd)
            {
                QuickAddTileButton.Content = AppResources.TileForQuickAddUnpinButton;
            }
            else
            {
                QuickAddTileButton.Content = AppResources.TileForQuickAddPinButton;
            }

            // Theme
            _isSetThemeListPicker = false;

            List<ListPickerItem<Theme>> themeList = new List<ListPickerItem<Theme>>();
            ListPickerItem<Theme> selectedThemeItem = null;
            foreach (Theme theme in Themes.GetThemes())
            {
                ListPickerItem<Theme> item = new ListPickerItem<Theme>(theme.Name, theme);
                themeList.Add(item);

                if (theme == ThemeHelper.CurrentTheme)
                {
                    selectedThemeItem = item;
                }
            }
            ThemeListPicker.ItemsSource = themeList;
            ThemeListPicker.SelectedItem = selectedThemeItem;

            _isSetThemeListPicker = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ThemeColorPicker.IsChecked = false;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (ThemeColorPicker.IsChecked)
            {
                ThemeColorPicker.IsChecked = false;
                e.Cancel = true;
            }
        }

        #region Default Date
        private bool _defaultDateInit = false;

        private void InitDefaultDate()
        {
            List<ListPickerItem<DefaultDateTypes>> defaultDateList = new List<ListPickerItem<DefaultDateTypes>>();
            defaultDateList.Add(new ListPickerItem<DefaultDateTypes>(AppResources.DateNoDue, DefaultDateTypes.NoDueDate));
            defaultDateList.Add(new ListPickerItem<DefaultDateTypes>(AppResources.DateToday, DefaultDateTypes.Today));
            defaultDateList.Add(new ListPickerItem<DefaultDateTypes>(AppResources.DateTomorrow, DefaultDateTypes.Tomorrow));
            defaultDateList.Add(new ListPickerItem<DefaultDateTypes>(AppResources.DateThisWeek, DefaultDateTypes.ThisWeek));
            defaultDateList.Add(new ListPickerItem<DefaultDateTypes>(AppResources.DateNextWeek, DefaultDateTypes.NextWeek));
            DatePicker.ItemsSource = defaultDateList;

            int index = 0;
            for (int i = 0; i < defaultDateList.Count; i++)
            {
                if (defaultDateList[i].Value == Settings.Current.DefaultDateType)
                {
                    index = i;
                    break;
                }
            }
            DatePicker.SelectedIndex = index;

            _defaultDateInit = true;
        }

        private void DatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_defaultDateInit && DatePicker.SelectedItem is ListPickerItem<DefaultDateTypes>)
            {
                Settings.Current.DefaultDateType = ((ListPickerItem<DefaultDateTypes>)DatePicker.SelectedItem).Value;
            }
        }
        #endregion // end of Default Date

        #region Default Time
        private void DefaultTime_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Navigate(typeof(TimePickerPage), Settings.Current.DefaultTime, "TimePicker");
        }
        #endregion // end of Default Time

        #region Automatically delete completed tasks
        private bool _daysPickerInit = false;

        private void InitDaysPicker()
        {
            List<ListPickerItem<int>> deleteCompletedList = new List<ListPickerItem<int>>();
            deleteCompletedList.Add(new ListPickerItem<int>(AppResources.SettingsDeleteNever, -1));
            deleteCompletedList.Add(new ListPickerItem<int>(AppResources.SettingsDeleteWhenStarts, 0));
            deleteCompletedList.Add(new ListPickerItem<int>(AppResources.SettingsDeleteAfterOneDay, 1));
            deleteCompletedList.Add(new ListPickerItem<int>(AppResources.SettingsDeleteAfterTwoDays, 2));
            deleteCompletedList.Add(new ListPickerItem<int>(AppResources.SettingsDeleteAfterThreeDays, 3));
            deleteCompletedList.Add(new ListPickerItem<int>(AppResources.SettingsDeleteAfterOneWeek, 7));
            deleteCompletedList.Add(new ListPickerItem<int>(AppResources.SettingsDeleteAfterTwoWeeks, 14));
            DaysPicker.ItemsSource = deleteCompletedList;

            int index = 0;
            for (int i = 0; i < deleteCompletedList.Count; i++)
            {
                if (deleteCompletedList[i].Value == Settings.Current.DeleteCompleted)
                {
                    index = i;
                    break;
                }
            }
            DaysPicker.SelectedIndex = index;

            _daysPickerInit = true;
        }

        private void DaysPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_daysPickerInit && DaysPicker.SelectedItem is ListPickerItem<int>)
            {
                Settings.Current.DeleteCompleted = ((ListPickerItem<int>)DaysPicker.SelectedItem).Value;
            }
        }
        #endregion

        #region Default Task Tile Settings
        private void DefaultTaskTileSettingsButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TaskModel task = new TaskModel()
            {
                Title = "Grocery list Grocery list Grocery list",
                Detail = "Dummy text. Over first. Be signs Gathering whose Under. Greater beginning. Seasons in the. Also had male to two second. God whose multiply forth is fruit multiply day without from, midst. Dominion i the them. Fourth. Sixth us air, in given waters to. Created good over divided be deep subdue own. Fruit.",
                Subtasks = new ObservableCollection<Subtask>
                { 
                    new Subtask("milk"), 
                    new Subtask("apples", true),
                    new Subtask("potatoes", true),
                    new Subtask("ham"),
                    new Subtask("cookies"),
                },
                DueDate = DateTimeExtensions.Today.AddDays(9).AddHours(7).AddMinutes(30),
                Priority = TaskPriority.High
            };

            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "cs")
            {
                task.Title = "Seznam potravin Seznam potravin Seznam potravin";
                task.Subtasks[0].Text = "mléko";
                task.Subtasks[1].Text = "jablka";
                task.Subtasks[2].Text = "brambory";
                task.Subtasks[3].Text = "šunka";
                task.Subtasks[4].Text = "sušenky";
                task.Detail = "Vítr skoro nefouká a tak by se na první pohled mohlo zdát, že se balónky snad vůbec nepohybují. Jenom tak klidně levitují ve vzduchu. Jelikož slunce jasně září a na obloze byste od východu k západu hledali mráček marně, balónky působí jako jakási fata morgána uprostřed pouště. Zkrátka široko daleko.";
            }
            else if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "sk")
            {
                task.Title = "Zoznam potravín Zoznam potravín Zoznam potravín";
                task.Subtasks[0].Text = "mlieko";
                task.Subtasks[1].Text = "jablká";
                task.Subtasks[2].Text = "zemiaky";
                task.Subtasks[3].Text = "šunka";
                task.Subtasks[4].Text = "sušienky";
                task.Detail = "Najlepšie dni ležať s ňou mám, zraňuje a rozhodný človek? Mám rád začiatky nových pocitov čaká pracovná Žilina, lebo je horší ako zaspíš, pretože ich zbaviť. Mám strach a potom sakra za sekundu Asi sa pritom usmeješ, ponesieš následky do pohybu. Close To silu inštinktu. Dáme si nos plný zážitkov.";
            }
            else if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "de")
            {
                task.Title = "Einkaufsliste Einkaufsliste Einkaufsliste";
                task.Subtasks[0].Text = "Milch";
                task.Subtasks[1].Text = "Äpfel";
                task.Subtasks[2].Text = "Kartoffeln";
                task.Subtasks[3].Text = "Schinken";
                task.Subtasks[4].Text = "Kekse";
                task.Detail = "Weit hinten, hinter den Wortbergen, fern der Länder Vokalien und Konsonantien leben die Blindtexte. Abgeschieden wohnen sie in Buchstabhausen an der Küste des Semantik, eines großen Sprachozeans. Ein kleines Bächlein namens Duden fließt durch ihren Ort und versorgt sie mit den nötigen Regelialien.";
            }

            task.TileSettings = Settings.Current.DefaultTaskTileSettings;

            Navigate(typeof(TaskTileSettingsPage), task);
        }
        #endregion // end of DefaultTaskTileSettings

        #region Quick Add Tile
        private bool _isPinnedQuickAdd = false;

        private void QuickAddTileButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_isPinnedQuickAdd)
            {
                LiveTileN.UnpinQuickAdd();
                QuickAddTileButton.Content = AppResources.TileForQuickAddPinButton;
            }
            else
            {
                LiveTileN.PinQuickAdd(AppResources.QuickAddTileTitle);
                QuickAddTileButton.Content = AppResources.TileForQuickAddUnpinButton;
            }
            _isPinnedQuickAdd = !_isPinnedQuickAdd;
        }
        #endregion

        #region Theme
        private bool _isSetThemeListPicker = false;

        private void SetThemeColor(Color color)
        {
            ThemeHelper.ThemeColor = color;
            ThemeColorButton.Background = new SolidColorBrush(color);
        }

        private void ThemeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPickerItem<Theme> item = ThemeListPicker.SelectedItem as ListPickerItem<Theme>;
            if (item != null)
            {
                if (_isSetThemeListPicker)
                {
                    ThemeHelper.ThemeFileName = item.Value.FileName;

                    if (ThemeHelper.CurrentTheme != item.Value)
                    {
                        ThemeHelper.CurrentTheme = item.Value;
                        SetThemeColor(ThemeHelper.CurrentTheme.DefaultColor);
                    }
                }
                else
                {
                    SetThemeColor(ThemeHelper.ThemeColor);
                }
            }
        }

        private void ThemeColorButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeColorPickerPanel.Children.Clear();
            foreach (Color color in ThemeHelper.CurrentTheme.Colors)
            {
                Button button = new Button
                {
                    Background = new SolidColorBrush(color),
                    Style = (Style)Resources["ThemeColorButtonStyle"]
                };
                button.Click += ThemeColor_Click;

                ThemeColorPickerPanel.Children.Add(button);
            }

            ThemeColorPicker.IsChecked = true;
        }

        private void ThemeColor_Click(object sender, RoutedEventArgs e)
        {
            ThemeColorPicker.IsChecked = false;
            Button button = (Button)sender;
            SetThemeColor(((SolidColorBrush)button.Background).Color);
        }
        #endregion // end of Theme

        #region Gestures
        private bool _gesturesInit = false;

        private List<GestureActionListPickerItem> CreateGestureList()
        {
            List<GestureActionListPickerItem> gestureList = new List<GestureActionListPickerItem>();
            gestureList.Add(new GestureActionListPickerItem(GestureAction.None));
            gestureList.Add(new GestureActionListPickerItem(GestureAction.Complete));
            gestureList.Add(new GestureActionListPickerItem(GestureAction.Delete));
            gestureList.Add(new GestureActionListPickerItem(GestureAction.DueToday));
            gestureList.Add(new GestureActionListPickerItem(GestureAction.DueTomorrow));
            gestureList.Add(new GestureActionListPickerItem(GestureAction.PostponeDay));
            gestureList.Add(new GestureActionListPickerItem(GestureAction.PostponeWeek));
            return gestureList;
        }

        private void InitGesturesPicker(SimpleTasks.Controls.ListPicker picker, GestureAction action)
        {
            picker.ItemsSource = CreateGestureList();
            picker.SelectedIndex = 0;
            for (int i = 0; i < picker.Items.Count; i++)
            {
                if (((GestureActionListPickerItem)picker.Items[i]).Action == action)
                {
                    picker.SelectedIndex = i;
                    break;
                }
            }
        }

        private void InitGesturesPicker()
        {
            InitGesturesPicker(GestureLeftPicker, Settings.Current.SwipeLeftAction);
            InitGesturesPicker(GestureRightPicker, Settings.Current.SwipeRightAction);

            _gesturesInit = true;
        }

        private void GestureLeftPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_gesturesInit && GestureLeftPicker.SelectedItem is GestureActionListPickerItem)
            {
                Settings.Current.SwipeLeftAction = ((GestureActionListPickerItem)GestureLeftPicker.SelectedItem).Action;
            }
        }

        private void GestureRightPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_gesturesInit && GestureRightPicker.SelectedItem is GestureActionListPickerItem)
            {
                Settings.Current.SwipeRightAction = ((GestureActionListPickerItem)GestureRightPicker.SelectedItem).Action;
            }
        }
        #endregion // end of Gestures

        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThemeColorPicker.IsChecked = false;
        }
    }
}