using Microsoft.Phone.Shell;
using SimpleTasks.Core.Models;
using SimpleTasks.Core.Tiles;
using SimpleTasks.Core.Tiles.DefaultList;
using SimpleTasks.Core.Tiles.DefaultTask;
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

        #region Dlaždice pro úkol

        public static ShellTile PinnedTile(TaskModel task)
        {
            return ShellTile.ActiveTiles.FirstOrDefault((t) =>
            {
                return t.NavigationUri.OriginalString.Contains("Task=" + task.Uid);
            });
        }

        public static bool IsPinned(TaskModel task)
        {
            return PinnedTile(task) != null;
        }

        private static FlipTileData CreateTile(TaskModel task)
        {
            string smallFileName = string.Format("{0}{1}_{2}", TileImageDirectory, task.Uid, SmallTileFileName);
            string mediumFileName = string.Format("{0}{1}_{2}", TileImageDirectory, task.Uid, MediumTileFileName);
            string wideFileName = string.Format("{0}{1}_{2}", TileImageDirectory, task.Uid, WideTileFileName);

            // Vytvoření obrázků dlaždic
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(smallFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new MediumTaskTile();
                WriteableBitmap wb = tile.Render(task);
                wb.WritePNG(stream);
            }
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(mediumFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new MediumTaskTile();
                WriteableBitmap wb = tile.Render(task);
                wb.WritePNG(stream);
            }
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(wideFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new WideTaskTile();
                WriteableBitmap wb = tile.Render(task);
                wb.WritePNG(stream);
            }

            FlipTileData flipTileData = new FlipTileData
            {
                SmallBackgroundImage = new Uri("isostore:" + smallFileName, UriKind.Absolute),
                BackgroundImage = new Uri("isostore:" + mediumFileName, UriKind.Absolute),
                WideBackgroundImage = new Uri("isostore:" + wideFileName, UriKind.Absolute),
                Title = "",
                Count = 0,
            };
            return flipTileData;
        }

        public static void Pin(TaskModel task)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    ShellTile.Create(new Uri(string.Format("/Views/EditTaskPage.xaml?Task={0}", task.Uid), UriKind.Relative), CreateTile(task), true);
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při vytváření živé dlaždice: {0}", e.Message);
            }
        }

        public static void Update(TaskModel task)
        {
            try
            {
                ShellTile tile = PinnedTile(task);
                if (tile != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(delegate
                    {
                        tile.Update(CreateTile(task));
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při aktualizaci živé dlaždice: {0}", e.Message);
            }
        }

        public static void Unpin(TaskModel task)
        {
            try
            {
                ShellTile tile = PinnedTile(task);
                if (tile != null)
                {
                    try
                    {
                        string smallFileName = string.Format("{0}{1}_{2}", TileImageDirectory, task.Uid, SmallTileFileName);
                        string mediumFileName = string.Format("{0}{1}_{2}", TileImageDirectory, task.Uid, MediumTileFileName);
                        string wideFileName = string.Format("{0}{1}_{2}", TileImageDirectory, task.Uid, WideTileFileName);
                        IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                        storage.DeleteFile(smallFileName);
                        storage.DeleteFile(mediumFileName);
                        storage.DeleteFile(wideFileName);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(": Chyba při mazání obrázků pro živé dlaždice: {0}", e.Message);
                    }

                    tile.Delete();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při mazání živé dlaždice: {0}", e.Message);
            }
        }
        
        #endregion

        #region Hlavní dlaždice

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

            // Vybere aktivní (nedokončené) úkoly a úkoly s termínem dokončení.
            // Uspořádá je podle termínu. Důležité úkoly ve stejném dnu mají přednost.
            List<TaskModel> tasks = tasksSource
                .Where((t) => { return t.IsActive && t.DueDate != null; })
                .OrderBy(t => t.DueDate.Value)
                .ThenByDescending(t => t.Priority)
                .ToList();

            // Přidá úkoly bez termínu na konec seznamu (opět uspořádané podle důležitosti).
            tasks.AddRange(tasksSource
                .Where((t) => { return t.IsActive && t.DueDate == null; })
                .OrderByDescending(t => t.Priority));

            // Počet dnešních úkolů (včetně zmeškaných)
            int todayTaskCount = Math.Min(tasks.Count((t) => { return t.DueDate <= DateTimeExtensions.Today; }), 99);

            // Vytvoření obrázků dlaždic
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(TileImageDirectory + SmallTileFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new SmallListTile();
                WriteableBitmap wb = tile.Render(tasks);
                wb.WritePNG(stream);
            }
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(TileImageDirectory + MediumTileFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new MediumListTile();
                WriteableBitmap wb = tile.Render(tasks);
                wb.WritePNG(stream);
            }
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(TileImageDirectory + WideTileFileName, System.IO.FileMode.Create))
            {
                TileTemplate tile = new WideListTile();
                WriteableBitmap wb = tile.Render(tasks);
                wb.WritePNG(stream);
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
                    Debug.WriteLine(": Aktualizace živé dlaždice dokončena.");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při aktualizaci živé dlaždice: {0}", e.Message);
            }
        }

        public static void UpdateUI(TaskCollection tasksSource)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Update(tasksSource);
            });
        }

        #endregion
    }
}
