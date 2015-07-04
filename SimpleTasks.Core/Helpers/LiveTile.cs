using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Phone.Shell;
using SimpleTasks.Core.Models;
using SimpleTasks.Core.Tiles;

namespace SimpleTasks.Core.Helpers
{
    public class LiveTile
    {
        private static string SmallTileFileName { get { return "SmallTile.png"; } }

        private static string MediumTileFileName { get { return "MediumTile.png"; } }

        private static string WideTileFileName { get { return "WideTile.png"; } }

        private static string TileImageDirectory { get { return "/Shared/ShellContent/"; } }

        #region Dlaždice pro úkol

        public static ShellTile PinnedTile(TaskModel task)
        {
            if (task == null)
                return null;
            return ShellTile.ActiveTiles.FirstOrDefault((t) =>
            {
                return t.NavigationUri.OriginalString.Contains("Task=" + task.Uid);
            });
        }

        public static bool IsPinned(TaskModel task)
        {
            return PinnedTile(task) != null;
        }

        public static FlipTileData CreateTile(TaskModel task)
        {
            string smallFileName = string.Format("{0}{1}_{2}", TileImageDirectory, task.Uid, SmallTileFileName);
            string mediumFileName = string.Format("{0}{1}_{2}", TileImageDirectory, task.Uid, MediumTileFileName);
            string wideFileName = string.Format("{0}{1}_{2}", TileImageDirectory, task.Uid, WideTileFileName);

            // Vytvoření obrázků dlaždic
            var sw = Stopwatch.StartNew();
            new SmallTaskTile(task).SaveToPng(smallFileName);
            new MediumTaskTile(task).SaveToPng(mediumFileName);
            new WideTaskTile(task).SaveToPng(wideFileName);
            sw.Stop();
            Debug.WriteLine("> CREATED TILE FOR '{0}' (in {1:0.000} seconds)", task.Title, sw.ElapsedMilliseconds / 1000.0);

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

        public static void Update(TaskModel task)
        {
            try
            {
                ShellTile tile = PinnedTile(task);
                if (tile != null)
                {
                    //Deployment.Current.Dispatcher.BeginInvoke(delegate
                    //{
                    tile.Update(CreateTile(task));
                    //});
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
                        try
                        {
                            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                            storage.DeleteFile(smallFileName);
                            storage.DeleteFile(mediumFileName);
                            storage.DeleteFile(wideFileName);
                        }
                        catch (IsolatedStorageException)
                        {
                        }
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
        public static void UpdateOrReset(bool update, TaskCollection tasksSource = null)
        {
            if (update)
            {
                Update(tasksSource);
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
                SmallBackgroundImage = new Uri("/Assets/Tiles/SmallTile.png", UriKind.Relative),
                BackgroundImage = new Uri("/Assets/Tiles/MediumTile.png", UriKind.Relative),
                WideBackgroundImage = new Uri("/Assets/Tiles/WideTile.png", UriKind.Relative),
                Title = AppInfo.Name,
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
            catch
            {
                Debug.WriteLine("Chyba při resetu primární dlaždice.");
            }
        }

        public static void Update(TaskCollection tasksSource)
        {
#if DEBUG
            Debug.WriteLine("> Aktualizuji živé dlaždice...");
            foreach (TaskModel task in tasksSource)
            {
                Debug.WriteLine(": '" + task.Title + "'");
            }
#endif

            // Vybere aktivní (nedokončené) úkoly a úkoly s termínem dokončení.
            // Uspořádá je podle termínu. Důležité úkoly ve stejném dnu mají přednost.
            List<TaskModel> tasks = tasksSource
                .Where((t) => { return t.IsActive && t.ActualDueDate != null; })
                .OrderBy(t => t.ActualDueDate.Value)
                .ThenByDescending(t => t.Priority)
                .ToList();

            // Přidá úkoly bez termínu na konec seznamu (opět uspořádané podle důležitosti).
            tasks.AddRange(tasksSource
                .Where((t) => { return t.IsActive && t.ActualDueDate == null; })
                .OrderByDescending(t => t.Priority));

            // Vytvoření obrázků dlaždic
            var sw = Stopwatch.StartNew();
            new SmallListTile(tasks).SaveToPng(TileImageDirectory + SmallTileFileName);
            new MediumListTile(tasks).SaveToPng(TileImageDirectory + MediumTileFileName);
            new WideListTile(tasks).SaveToPng(TileImageDirectory + WideTileFileName);
            sw.Stop();
            Debug.WriteLine("> CREATED MAIN TILE (in {0:0.000} seconds)", sw.ElapsedMilliseconds / 1000.0);

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
        #endregion
    }
}
