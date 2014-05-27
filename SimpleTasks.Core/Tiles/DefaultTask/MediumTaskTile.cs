using SimpleTasks.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SimpleTasks.Core.Tiles.DefaultTask
{
    public class MediumTaskTile : SmallTaskTile
    {
        public MediumTaskTile()
            : base()
        {
            Width = MediumSize;
            Height = MediumSize;
        }
    }
}
