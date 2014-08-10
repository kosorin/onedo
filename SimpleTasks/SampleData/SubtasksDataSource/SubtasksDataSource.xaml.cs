﻿//      *********    DO NOT MODIFY THIS FILE     *********
//      This file is regenerated by a design tool. Making
//      changes to this file can cause errors.
namespace Expression.Blend.SampleData.SubtasksDataSource
{
	using System; 
	using System.ComponentModel;

// To significantly reduce the sample data footprint in your production application, you can set
// the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#if DISABLE_SAMPLE_DATA
	internal class SubtasksDataSource { }
#else

	public class SubtasksDataSource : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public SubtasksDataSource()
		{
			try
			{
				Uri resourceUri = new Uri("/SimpleTasks;component/SampleData/SubtasksDataSource/SubtasksDataSource.xaml", UriKind.RelativeOrAbsolute);
				System.Windows.Application.LoadComponent(this, resourceUri);
			}
			catch
			{
			}
		}

		private Subtasks _Subtasks = new Subtasks();

		public Subtasks Subtasks
		{
			get
			{
				return this._Subtasks;
			}
		}
	}

	public class SubtasksItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private bool _IsCompleted = false;

		public bool IsCompleted
		{
			get
			{
				return this._IsCompleted;
			}

			set
			{
				if (this._IsCompleted != value)
				{
					this._IsCompleted = value;
					this.OnPropertyChanged("IsCompleted");
				}
			}
		}

		private string _Text = string.Empty;

		public string Text
		{
			get
			{
				return this._Text;
			}

			set
			{
				if (this._Text != value)
				{
					this._Text = value;
					this.OnPropertyChanged("Text");
				}
			}
		}
	}

	public class Subtasks : System.Collections.ObjectModel.ObservableCollection<SubtasksItem>
	{ 
	}
#endif
}
