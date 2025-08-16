using System.ComponentModel;

namespace Avalonia.Collections;

public abstract class DataGridCollectionViewGroup : INotifyPropertyChanged
{
	private int _itemCount;

	public object Key { get; }

	public int ItemCount => _itemCount;

	public IAvaloniaReadOnlyList<object> Items => ProtectedItems;

	protected AvaloniaList<object> ProtectedItems { get; }

	protected int ProtectedItemCount
	{
		get
		{
			return _itemCount;
		}
		set
		{
			_itemCount = value;
			OnPropertyChanged(new PropertyChangedEventArgs("ItemCount"));
		}
	}

	public abstract bool IsBottomLevel { get; }

	protected virtual event PropertyChangedEventHandler PropertyChanged;

	event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
	{
		add
		{
			PropertyChanged += value;
		}
		remove
		{
			PropertyChanged -= value;
		}
	}

	protected DataGridCollectionViewGroup(object key)
	{
		Key = key;
		ProtectedItems = new AvaloniaList<object>();
	}

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		this.PropertyChanged?.Invoke(this, e);
	}
}
