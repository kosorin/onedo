using Microsoft.Phone.Shell;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Helpers
{
    public class LiveTileExtensions
    {
        public static void AddSecondaryTile(TaskCollection tasks)
        {
            RemoveSecondaryTile();

            try
            {
                ShellTile.Create(LiveTile.TileUri, LiveTile.CreateSecondaryTileData(tasks.SortedActiveTasks), true);
            }
            catch (Exception)
            {
                Debug.WriteLine("Chyba při přidání sekundární dlaždice.");
            }
        }

        public static void RemoveSecondaryTile()
        {
            try
            {
                ShellTile tile = LiveTile.FindSecondaryTile();
                if (tile != null)
                {
                    tile.Delete();
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Chyba při mazání sekundární dlaždice.");
            }
        }
    }
}
