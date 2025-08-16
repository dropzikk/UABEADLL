using System.Collections.Generic;

namespace Avalonia.Input;

public class DataObject : IDataObject
{
	private readonly Dictionary<string, object> _items = new Dictionary<string, object>();

	public bool Contains(string dataFormat)
	{
		return _items.ContainsKey(dataFormat);
	}

	public object? Get(string dataFormat)
	{
		if (!_items.TryGetValue(dataFormat, out object value))
		{
			return null;
		}
		return value;
	}

	public IEnumerable<string> GetDataFormats()
	{
		return _items.Keys;
	}

	public void Set(string dataFormat, object value)
	{
		_items[dataFormat] = value;
	}
}
