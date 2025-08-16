using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Utilities;

internal struct AvaloniaPropertyDictionary<TValue>
{
	private readonly struct Entry
	{
		public readonly AvaloniaProperty Property;

		public readonly TValue Value;

		public Entry(AvaloniaProperty property, TValue value)
		{
			Property = property;
			Value = value;
		}
	}

	private const int DefaultInitialCapacity = 4;

	private Entry[]? _entries;

	private int _entryCount;

	public int Count => _entryCount;

	public TValue this[AvaloniaProperty property]
	{
		get
		{
			if (!TryGetEntry(property.Id, out var index))
			{
				ThrowNotFound();
			}
			return _entries[index].Value;
		}
		set
		{
			if (TryGetEntry(property.Id, out var index))
			{
				_entries[index] = new Entry(property, value);
			}
			else
			{
				InsertEntry(new Entry(property, value), index);
			}
		}
	}

	public TValue this[int index]
	{
		get
		{
			if (index >= _entryCount)
			{
				ThrowOutOfRange();
			}
			return _entries[index].Value;
		}
	}

	public AvaloniaPropertyDictionary()
	{
		_entries = null;
		_entryCount = 0;
	}

	public AvaloniaPropertyDictionary(int capactity)
	{
		_entries = new Entry[capactity];
		_entryCount = 0;
	}

	public void Add(AvaloniaProperty property, TValue value)
	{
		if (TryGetEntry(property.Id, out var index))
		{
			ThrowDuplicate();
		}
		InsertEntry(new Entry(property, value), index);
	}

	public void Clear()
	{
		if (_entries != null)
		{
			Array.Clear(_entries, 0, _entries.Length);
			_entryCount = 0;
		}
	}

	public bool ContainsKey(AvaloniaProperty property)
	{
		int index;
		return TryGetEntry(property.Id, out index);
	}

	public void GetKeyValue(int index, out AvaloniaProperty key, out TValue value)
	{
		if (index >= _entryCount)
		{
			ThrowOutOfRange();
		}
		ref Entry reference = ref _entries[index];
		key = reference.Property;
		value = reference.Value;
	}

	public bool Remove(AvaloniaProperty property)
	{
		if (TryGetEntry(property.Id, out var index))
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}

	public bool Remove(AvaloniaProperty property, [MaybeNullWhen(false)] out TValue value)
	{
		if (TryGetEntry(property.Id, out var index))
		{
			value = _entries[index].Value;
			RemoveAt(index);
			return true;
		}
		value = default(TValue);
		return false;
	}

	public void RemoveAt(int index)
	{
		if (_entries == null)
		{
			throw new IndexOutOfRangeException();
		}
		Array.Copy(_entries, index + 1, _entries, index, _entryCount - index - 1);
		_entryCount--;
		_entries[_entryCount] = default(Entry);
	}

	public bool TryAdd(AvaloniaProperty property, TValue value)
	{
		if (TryGetEntry(property.Id, out var index))
		{
			return false;
		}
		InsertEntry(new Entry(property, value), index);
		return true;
	}

	public bool TryGetValue(AvaloniaProperty property, [MaybeNullWhen(false)] out TValue value)
	{
		if (TryGetEntry(property.Id, out var index))
		{
			value = _entries[index].Value;
			return true;
		}
		value = default(TValue);
		return false;
	}

	[MemberNotNullWhen(true, "_entries")]
	private bool TryGetEntry(int propertyId, out int index)
	{
		int num = 0;
		int num2 = _entryCount;
		if (num2 <= 0)
		{
			index = 0;
			return false;
		}
		while (num2 - num > 3)
		{
			int num3 = (num2 + num) / 2;
			int id = _entries[num3].Property.Id;
			if (propertyId == id)
			{
				index = num3;
				return true;
			}
			if (propertyId <= id)
			{
				num2 = num3;
			}
			else
			{
				num = num3 + 1;
			}
		}
		do
		{
			int id = _entries[num].Property.Id;
			if (id == propertyId)
			{
				index = num;
				return true;
			}
			if (id > propertyId)
			{
				break;
			}
			num++;
		}
		while (num < num2);
		index = num;
		return false;
	}

	[MemberNotNull("_entries")]
	private void InsertEntry(Entry entry, int entryIndex)
	{
		if (_entryCount > 0)
		{
			if (_entryCount == _entries.Length)
			{
				Entry[] array = new Entry[(_entryCount == 4) ? 8 : ((int)((double)_entryCount * 1.5))];
				Array.Copy(_entries, 0, array, 0, entryIndex);
				array[entryIndex] = entry;
				Array.Copy(_entries, entryIndex, array, entryIndex + 1, _entryCount - entryIndex);
				_entries = array;
			}
			else
			{
				Array.Copy(_entries, entryIndex, _entries, entryIndex + 1, _entryCount - entryIndex);
				_entries[entryIndex] = entry;
			}
		}
		else
		{
			if (_entries == null)
			{
				_entries = new Entry[4];
			}
			_entries[0] = entry;
		}
		_entryCount++;
	}

	[DoesNotReturn]
	private static void ThrowOutOfRange()
	{
		throw new IndexOutOfRangeException();
	}

	[DoesNotReturn]
	private static void ThrowDuplicate()
	{
		throw new ArgumentException("An item with the same key has already been added.");
	}

	[DoesNotReturn]
	private static void ThrowNotFound()
	{
		throw new KeyNotFoundException();
	}
}
