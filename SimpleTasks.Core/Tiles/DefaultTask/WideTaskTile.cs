using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SimpleTasks.Core.Tiles.DefaultTask
{
    class WideTaskTile : SmallTaskTile
    {
        public WideTaskTile()
            : base()
        {
            Width = WideSize;
            Height = MediumSize;
        }
    }
}
