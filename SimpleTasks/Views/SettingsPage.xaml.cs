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

namespace SimpleTasks.Views
{
    public partial class SettingsPage : BasePivotPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = Settings.Current;
            PinTileTextBox.Text = PinTileHelpText;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Default Time
            if (IsSetNavigationParameter("TimePicker"))
            {
                Settings.Current.DefaultTime = NavigationParameter<DateTime>("TimePicker");
            }

            // Pin Tile
            _isPinnedQuickAdd = LiveTile.IsPinnedQuickAdd();
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
            string systemTheme = ThemeHelper.SystemTheme == Theme.Dark ? AppResources.SettingsThemeDark : AppResources.SettingsThemeLight;

            List<ListPickerItem<Theme>> themeList = new List<ListPickerItem<Theme>>();
            themeList.Add(new ListPickerItem<Theme>(string.Format(AppResources.SettingsThemeSystem, systemTheme), Theme.System));
            themeList.Add(new ListPickerItem<Theme>(AppResources.SettingsThemeLight, Theme.Light));
            themeList.Add(new ListPickerItem<Theme>(AppResources.SettingsThemeDark, Theme.Dark));
            themeList.Add(new ListPickerItem<Theme>("Solarized Light", Theme.SolarizedLight));
            themeList.Add(new ListPickerItem<Theme>("Solarized Dark", Theme.SolarizedDark));
            themeList.Add(new ListPickerItem<Theme>("Ocean", Theme.Ocean));
            ThemeListPicker.ItemsSource = themeList;
            ThemeListPicker.SelectedIndex = (int)ThemeHelper.Theme;

            List<ColorListPickerItem> themeColorList = new List<ColorListPickerItem>();
            themeColorList.Add(new ColorListPickerItem("Yellow", Color.FromArgb(255, 181, 137, 0)));
            themeColorList.Add(new ColorListPickerItem("Orange", Color.FromArgb(255, 203, 75, 22)));
            themeColorList.Add(new ColorListPickerItem("Red", Color.FromArgb(255, 220, 50, 47)));
            themeColorList.Add(new ColorListPickerItem("Magenta", Color.FromArgb(255, 211, 54, 130)));
            themeColorList.Add(new ColorListPickerItem("Violet", Color.FromArgb(255, 108, 113, 196)));
            themeColorList.Add(new ColorListPickerItem("Blue", Color.FromArgb(255, 38, 139, 210)));
            themeColorList.Add(new ColorListPickerItem("Cyan", Color.FromArgb(255, 42, 161, 152)));
            themeColorList.Add(new ColorListPickerItem("Green", Color.FromArgb(255, 133, 153, 0)));
            ThemeColorListPicker.ItemsSource = themeColorList;
            int themeColorIndex = themeColorList.FindIndex(item => item.Color == ThemeHelper.ThemeColor);
            ThemeColorListPicker.SelectedIndex = (themeColorIndex == -1 ? 1 : themeColorIndex);

            _isSetThemeListPicker = true;
        }

        #region Pin Tile
        public string PinTileHelpText { get { return string.Format(AppResources.SettingsPinTile, AppInfo.Name); } }
        #endregion // end of Pin Tile

        #region Default Time
        private void DefaultTime_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Navigate(typeof(TimePickerPage), Settings.Current.DefaultTime, "TimePicker");
        }
        #endregion // end of Default Time

        #region Feedback
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Current.Feedback = true;
            GoogleAnalyticsHelper.SetDimension(CustomDimension.Feedback, "True");
            GoogleAnalyticsHelper.SendEvent("Settings", "Edit", "set feedback");
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Settings.Current.Feedback)
            {
                // Pokud už byl feedback nastavený na false a check box se změní na unchecked,
                // tak se znovu neposílá info o změně.
                GoogleAnalyticsHelper.SetDimension(CustomDimension.Feedback, "False");
                GoogleAnalyticsHelper.SendEvent("Settings", "Edit", "set feedback");
                Settings.Current.Feedback = false;
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            App.FeedbackEmail();
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

            Navigate(typeof(EditTaskTilePage), task);
        }
        #endregion // end of DefaultTaskTileSettings

        #region Quick Add Tile
        private bool _isPinnedQuickAdd = false;

        private void QuickAddTileButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_isPinnedQuickAdd)
            {
                LiveTile.UnpinQuickAdd();
                QuickAddTileButton.Content = AppResources.TileForQuickAddPinButton;
            }
            else
            {
                LiveTile.PinQuickAdd(AppResources.QuickAddTileTitle);
                QuickAddTileButton.Content = AppResources.TileForQuickAddUnpinButton;
            }
            _isPinnedQuickAdd = !_isPinnedQuickAdd;
        }
        #endregion

        #region Theme
        private bool _isSetThemeListPicker = false;

        private void ThemeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPickerItem<Theme> item = ThemeListPicker.SelectedItem as ListPickerItem<Theme>;
            if (item != null)
            {
                ThemeColorPanel.Visibility = (item.Value.IsSolarized() ? Visibility.Visible : Visibility.Collapsed);
                if (_isSetThemeListPicker)
                {
                    ThemeHelper.Theme = item.Value;
                    if (MainPivot.SelectedItem == ThemePivotItem)
                    {
                        Toast.Show(AppResources.SettingsThemeNote);
                    }
                }
            }
            else
            {
                ThemeColorPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ThemeColorListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSetThemeListPicker)
            {
                ColorListPickerItem item = ThemeColorListPicker.SelectedItem as ColorListPickerItem;
                if (item != null)
                {
                    ThemeHelper.ThemeColor = item.Color;
                    if (MainPivot.SelectedItem == ThemePivotItem)
                    {
                        Toast.Show(AppResources.SettingsThemeNote);
                    }
                }
            }
        }
        #endregion // end of Theme
    }
}