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
        private static string SmallTileFileName { get { return "SmallTile.jpg"; } }

        private static string MediumTileFileName { get { return "MediumTile.jpg"; } }

        private static string WideTileFileName { get { return "WideTile.jpg"; } }

        private static string TileImageDirectory { get { return "/Shared/ShellContent/"; } }

        public static void UpdateOrReset(bool update, TaskCollection tasksSource = null, bool updateUI = false)
        {
            if (update)
            {
                if (updateUI)
                {
                    UpdateUI(tasksSource);
                }
                else
                {
                    Update(tasksSource);
                }
            }
            else
            {
                Reset();
            }
        }

        public static void Reset()
        {
            FlipTileData flipTileData = new FlipTileData
            {
                SmallBackgroundImage = new Uri("/Assets/Tiles/FlipTileSmall.png", UriKind.Relative),
                BackgroundImage = new Uri("/Assets/Tiles/FlipTileMedium.png", UriKind.Relative),
                WideBackgroundImage = new Uri("/Assets/Tiles/FlipTileWide.png", UriKind.Relative),
                Title = "Simple Tasks",
                Count = 0,
            };

            try
            {
                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
                if (tile != null)
                {
                    tile.Update(flipTileData);
                    Debug.WriteLine("> Reset primární dlaždice dokončena.");
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Chyba při resetu primární dlaždice.");
            }
        }

        public static void Update(TaskCollection tasksSource)
        {
            Debug.WriteLine("> Aktualizuji živé dlaždice...");
            foreach (TaskModel task in tasksSource)
            {
                Debug.WriteLine(": '" + task.Title + "'");
            }

            List<TaskModel> tasks = tasksSource.SortedActiveTasks;

            // Počet dnešních úkolů (včetně zmeškaných)
            int todayTaskCount = Math.Min(tasks.Count((t) => { return t.DueDate <= DateTimeExtensions.Today; }), 99);

            // Vytvoření obrázků dlaždic
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(TileImageDirectory + SmallTileFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new SimpleListTile(4, 159, 159);
                WriteableBitmap wb = tile.Render(tasks);
                wb.WritePNG(stream);
                //wb.SaveJpeg(stream, tile.Width, tile.Height, 0, 100);
            }
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(TileImageDirectory + MediumTileFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new NormalListTile(7, 336, 336);
                WriteableBitmap wb = tile.Render(tasks);
                wb.WritePNG(stream);
                //wb.SaveJpeg(stream, tile.Width, tile.Height, 0, 100);
            }
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(TileImageDirectory + WideTileFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new WideListTile(7, 691, 336);
                WriteableBitmap wb = tile.Render(tasks);
                wb.WritePNG(stream);
                //wb.SaveJpeg(stream, tile.Width, tile.Height, 0, 100);
            }

            FlipTileData flipTileData = new FlipTileData
            {
                SmallBackgroundImage = new Uri("isostore:" + TileImageDirectory + SmallTileFileName, UriKind.Absolute),
                BackgroundImage = new Uri("isostore:" + TileImageDirectory + MediumTileFileName, UriKind.Absolute),
                WideBackgroundImage = new Uri("isostore:" + TileImageDirectory + WideTileFileName, UriKind.Absolute),
                Title = "",
                Count = 0,
            };

            try
            {
                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
                if (tile != null)
                {
                    tile.Update(flipTileData);
                    Debug.WriteLine("> Aktualizace živé dlaždice dokončena.");
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Chyba při aktualizaci živé dlaždice.");
            }
        }

        public static void UpdateUI(TaskCollection tasksSource)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Update(tasksSource);
            });
        }
    }
}
