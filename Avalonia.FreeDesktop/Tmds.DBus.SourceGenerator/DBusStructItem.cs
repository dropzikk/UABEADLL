using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tmds.DBus.SourceGenerator;

internal class DBusStructItem : DBusItem, IList<DBusItem>, ICollection<DBusItem>, IEnumerable<DBusItem>, IEnumerable
{
	private readonly IList<DBusItem> _value;

	public int Count => _value.Count;

	public bool IsReadOnly => _value.IsReadOnly;

	public DBusItem this[int index]
	{
		get
		{
			return _value[index];
		}
		set
		{
			_value[index] = value;
		}
	}

	public DBusStructItem(IEnumerable<DBusItem> value)
	{
		_value = value.ToList();
	}

	public IEnumerator<DBusItem> GetEnumerator()
	{
		return _value.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_value).GetEnumerator();
	}

	public void Add(DBusItem item)
	{
		_value.Add(item);
	}

	public void Clear()
	{
		_value.Clear();
	}

	public bool Contains(DBusItem item)
	{
		return _value.Contains(item);
	}

	public void CopyTo(DBusItem[] array, int arrayIndex)
	{
		_value.CopyTo(array, arrayIndex);
	}

	public bool Remove(DBusItem item)
	{
		return _value.Remove(item);
	}

	public int IndexOf(DBusItem item)
	{
		return _value.IndexOf(item);
	}

	public void Insert(int index, DBusItem item)
	{
		_value.Insert(index, item);
	}

	public void RemoveAt(int index)
	{
		_value.RemoveAt(index);
	}
}
