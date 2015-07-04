using System.Windows.Media;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Core.Tiles
{
    public partial class SmallTaskTile : TaskTileControl
    {
        public SmallTaskTile()
        {
            InitializeComponent();
        }

        public SmallTaskTile(TaskModel task)
            : base(task)
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            TaskModel task = Task;
            if (task == null)
                return;

            DataContext = task;
            TaskTileSettings settings = task.TileSettings ?? Settings.Current.DefaultTaskTileSettings;

            // Název
            Title.FontSize = settings.LineHeight * 0.72;
            Title.Text = task.Title;

            // Pozadí
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            //LayoutRoot.Background = new SolidColorBrush(task.Color) { Opacity = settings.BackgroundOpacity };
        }
    }
}
