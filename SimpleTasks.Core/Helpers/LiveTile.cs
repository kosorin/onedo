using Microsoft.Phone.Shell;
using SimpleTasks.Core.Models;
using SimpleTasks.Core.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SimpleTasks.Core.Helpers
{
    public class LiveTile
    {
        static public void UpdateTiles(TaskModelCollection tasksSource)
        {
            Debug.WriteLine("> Aktualizuji živé dlaždice...");

            List<TaskModel> tasks = tasksSource.SortedActiveTasks;

            UpdateApplicationTile(tasks);
            UpdateSecondaryTile(tasks);
        }

        static public void UpdateTilesUI(TaskModelCollection tasksSource)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                UpdateTiles(tasksSource);
            });
        }

        static private void UpdateApplicationTile(List<TaskModel> sortedTasks)
        {
            try
            {
                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
                if (tile != null)
                {
                    IconicTileData iconicTileData = new IconicTileData
                    {
                        Count = Math.Min(sortedTasks.Count((t) => { return t.Date <= DateTimeExtensions.Today; }), 99),

                        WideContent1 = sortedTasks.Count > 0 ? sortedTasks[0].Title : "",
                        WideContent2 = sortedTasks.Count > 1 ? sortedTasks[1].Title : "",
                        WideContent3 = sortedTasks.Count > 2 ? sortedTasks[2].Title : "",
                    };

                    tile.Update(iconicTileData);
                    Debug.WriteLine("> Aktualizace primární dlaždice dokončena.");
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Chyba při aktualizaci primární dlaždice.");
            }
        }

        static public bool HasSecondaryTile
        {
            get
            {
                return ShellTile.ActiveTiles.Count() > 1;
            }
        }

        private const string SmallTileFileName = "SmallTile.jpg";
        private const string MediumTileFileName = "MediumTile.jpg";
        private const string WideTileFileName = "WideTile.jpg";
        private const string TileImageDirectory = "/Shared/ShellContent/";

        private static string TileUriPartString = "?LiveTileId=1";
        private static string TileUriString = "/Views/MainPage.xaml" + TileUriPartString;
        public static Uri TileUri = new Uri(TileUriString, UriKind.Relative);

        static private void UpdateSecondaryTile(List<TaskModel> sortedTasks)
        {
            // Pokud neexistuje sekundární dlaždice, tak neděláme nic
            if (!HasSecondaryTile)
                return;

            try
            {
                ShellTile tile = FindSecondaryTile();
                if (tile != null)
                {
                    tile.Update(CreateSecondaryTileData(sortedTasks));
                    Debug.WriteLine("> Aktualizace sekundární dlaždice dokončena.");
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Chyba při aktualizaci sekundární dlaždice.");
            }
        }

        static public ShellTileData CreateSecondaryTileData(List<TaskModel> sortedTasks)
        {
            // Počet dnešních úkolů (včetně zmeškaných)
            int todayTaskCount = Math.Min(sortedTasks.Count((t) => { return t.Date <= DateTimeExtensions.Today; }), 99);

            // Vytvoření obrázků dlaždic
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(TileImageDirectory + SmallTileFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new SimpleListTile(4, 159,159);
                WriteableBitmap wb = tile.Render(sortedTasks);
                wb.SaveJpeg(stream, tile.Width, tile.Height, 0, 100);
            }
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(TileImageDirectory + MediumTileFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new NormalListTile(7, 336, 336);
                WriteableBitmap wb = tile.Render(sortedTasks);
                wb.SaveJpeg(stream, tile.Width, tile.Height, 0, 100);
            }
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(TileImageDirectory + WideTileFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new WideListTile(7, 691, 336);
                WriteableBitmap wb = tile.Render(sortedTasks);
                wb.SaveJpeg(stream, tile.Width, tile.Height, 0, 100);
            }

            FlipTileData flipTileData = new FlipTileData
            {
                SmallBackgroundImage = new Uri("isostore:" + TileImageDirectory + SmallTileFileName, UriKind.Absolute),
                BackgroundImage = new Uri("isostore:" + TileImageDirectory + MediumTileFileName, UriKind.Absolute),
                WideBackgroundImage = new Uri("isostore:" + TileImageDirectory + WideTileFileName, UriKind.Absolute),
            };

            return flipTileData;
        }

        static public ShellTile FindSecondaryTile()
        {
            return ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri.ToString().Contains(TileUriPartString));
        }
    }
}
