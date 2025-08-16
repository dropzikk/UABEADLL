using System.ComponentModel;
using System.Globalization;

namespace Avalonia.Collections;

public abstract class DataGridGroupDescription : INotifyPropertyChanged
{
	public AvaloniaList<object> GroupKeys { get; }

	public virtual string PropertyName => string.Empty;

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

	public DataGridGroupDescription()
	{
		GroupKeys = new AvaloniaList<object>();
		GroupKeys.CollectionChanged += delegate
		{
			OnPropertyChanged(new PropertyChangedEventArgs("GroupKeys"));
		};
	}

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		this.PropertyChanged?.Invoke(this, e);
	}

	public abstract object GroupKeyFromItem(object item, int level, CultureInfo culture);

	public virtual bool KeysMatch(object groupKey, object itemKey)
	{
		return object.Equals(groupKey, itemKey);
	}
}
