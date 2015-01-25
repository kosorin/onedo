using Microsoft.Phone.Shell;
using SimpleTasks.Core.Helpers;
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

namespace SimpleTasks.Helpers
{
    public static class LiveTileN
    {
        #region Dlaždice pro úkol
        public static void PinEmpty(TaskModel task)
        {
            Pin(task, new FlipTileData());
        }

        public static void Pin(TaskModel task)
        {
            Pin(task, SimpleTasks.Core.Helpers.LiveTile.CreateTile(task));
        }

        public static void Pin(TaskModel task, StandardTileData tileData)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    ShellTile.Create(new Uri(string.Format("/Views/EditTaskPage.xaml?Task={0}", task.Uid), UriKind.Relative), tileData, true);
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při vytváření živé dlaždice: {0}", e.Message);
            }
        }
        #endregion

        #region Dlaždice pro rychlé přidání
        public static void PinQuickAdd(string tileTitle)
        {
            FlipTileData flipTileData = new FlipTileData
            {
                SmallBackgroundImage = new Uri("/Assets/Tiles/NewTaskSmallTile.png", UriKind.Relative),
                BackgroundImage = new Uri("/Assets/Tiles/NewTaskMediumTile.png", UriKind.Relative),
                Title = tileTitle,
                Count = 0,
            };

            try
            {
                ShellTile.Create(new Uri("/Views/EditTaskPage.xaml", UriKind.Relative), flipTileData, false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při vytváření živé dlaždice pro rychlé přidání: {0}", e.Message);
            }
        }

        public static ShellTile PinnedQuickAddTile()
        {
            return ShellTile.ActiveTiles.FirstOrDefault((t) =>
            {
                return t.NavigationUri.OriginalString == "/Views/EditTaskPage.xaml";
            });
        }

        public static bool IsPinnedQuickAdd()
        {
            return PinnedQuickAddTile() != null;
        }

        public static void UnpinQuickAdd()
        {
            try
            {
                ShellTile tile = PinnedQuickAddTile();
                if (tile != null)
                {
                    tile.Delete();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při mazání živé dlaždice pro rychlé přidání: {0}", e.Message);
            }
        }
        #endregion
    }
}
