using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Controls;

internal class IndexToValueTable<T> : IEnumerable<Range<T>>, IEnumerable
{
	private List<Range<T>> _list;

	public int IndexCount
	{
		get
		{
			int num = 0;
			foreach (Range<T> item in _list)
			{
				num += item.Count;
			}
			return num;
		}
	}

	public bool IsEmpty => _list.Count == 0;

	public int RangeCount => _list.Count;

	public IndexToValueTable()
	{
		_list = new List<Range<T>>();
	}

	public void AddValue(int index, T value)
	{
		AddValues(index, 1, value);
	}

	public void AddValues(int startIndex, int count, T value)
	{
		AddValuesPrivate(startIndex, count, value, null);
	}

	public void Clear()
	{
		_list.Clear();
	}

	public bool Contains(int index)
	{
		return IsCorrectRangeIndex(FindRangeIndex(index), index);
	}

	public bool ContainsAll(int startIndex, int endIndex)
	{
		int num = -1;
		int num2 = -1;
		foreach (Range<T> item in _list)
		{
			if (num == -1 && item.UpperBound >= startIndex)
			{
				if (startIndex < item.LowerBound)
				{
					return false;
				}
				num = startIndex;
				num2 = item.UpperBound;
				if (num2 >= endIndex)
				{
					return true;
				}
			}
			else if (num != -1)
			{
				if (item.LowerBound > num2 + 1)
				{
					return false;
				}
				num2 = item.UpperBound;
				if (num2 >= endIndex)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool ContainsIndexAndValue(int index, T value)
	{
		int num = FindRangeIndex(index);
		if (IsCorrectRangeIndex(num, index))
		{
			return _list[num].ContainsValue(value);
		}
		return false;
	}

	public IndexToValueTable<T> Copy()
	{
		IndexToValueTable<T> indexToValueTable = new IndexToValueTable<T>();
		foreach (Range<T> item in _list)
		{
			indexToValueTable._list.Add(item.Copy());
		}
		return indexToValueTable;
	}

	public int GetNextGap(int index)
	{
		int num = index + 1;
		int i = FindRangeIndex(num);
		if (IsCorrectRangeIndex(i, num))
		{
			for (; i < _list.Count - 1 && _list[i].UpperBound == _list[i + 1].LowerBound - 1; i++)
			{
			}
			return _list[i].UpperBound + 1;
		}
		return num;
	}

	public int GetNextIndex(int index)
	{
		int num = index + 1;
		int num2 = FindRangeIndex(num);
		if (IsCorrectRangeIndex(num2, num))
		{
			return num;
		}
		num2++;
		if (num2 >= _list.Count)
		{
			return -1;
		}
		return _list[num2].LowerBound;
	}

	public int GetPreviousGap(int index)
	{
		int num = index - 1;
		int num2 = FindRangeIndex(num);
		if (IsCorrectRangeIndex(num2, num))
		{
			while (num2 > 0 && _list[num2].LowerBound == _list[num2 - 1].UpperBound + 1)
			{
				num2--;
			}
			return _list[num2].LowerBound - 1;
		}
		return num;
	}

	public int GetPreviousIndex(int index)
	{
		int num = index - 1;
		int num2 = FindRangeIndex(num);
		if (IsCorrectRangeIndex(num2, num))
		{
			return num;
		}
		if (num2 < 0 || num2 >= _list.Count)
		{
			return -1;
		}
		return _list[num2].UpperBound;
	}

	public int GetIndexCount(int lowerBound, int upperBound, T value)
	{
		if (_list.Count == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = FindRangeIndex(lowerBound);
		if (IsCorrectRangeIndex(num2, lowerBound) && _list[num2].ContainsValue(value))
		{
			num += _list[num2].UpperBound - lowerBound + 1;
		}
		for (num2++; num2 < _list.Count && _list[num2].UpperBound <= upperBound; num2++)
		{
			if (_list[num2].ContainsValue(value))
			{
				num += _list[num2].Count;
			}
		}
		if (num2 < _list.Count && IsCorrectRangeIndex(num2, upperBound) && _list[num2].ContainsValue(value))
		{
			num += upperBound - _list[num2].LowerBound;
		}
		return num;
	}

	public int GetIndexCount(int lowerBound, int upperBound)
	{
		if (upperBound < lowerBound || _list.Count == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = FindRangeIndex(lowerBound);
		if (IsCorrectRangeIndex(num2, lowerBound))
		{
			num += _list[num2].UpperBound - lowerBound + 1;
		}
		for (num2++; num2 < _list.Count && _list[num2].UpperBound <= upperBound; num2++)
		{
			num += _list[num2].Count;
		}
		if (num2 < _list.Count && IsCorrectRangeIndex(num2, upperBound))
		{
			num += upperBound - _list[num2].LowerBound;
		}
		return num;
	}

	public int GetIndexCountBeforeGap(int startingIndex, int gapSize)
	{
		if (_list.Count == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = startingIndex;
		int num3 = 0;
		int num4 = 0;
		while (num4 <= gapSize && num3 < _list.Count)
		{
			num4 += _list[num3].LowerBound - num2;
			if (num4 <= gapSize)
			{
				num += _list[num3].UpperBound - _list[num3].LowerBound + 1;
				num2 = _list[num3].UpperBound + 1;
				num3++;
			}
		}
		return num;
	}

	public IEnumerable<int> GetIndexes()
	{
		foreach (Range<T> range in _list)
		{
			for (int i = range.LowerBound; i <= range.UpperBound; i++)
			{
				yield return i;
			}
		}
	}

	public IEnumerable<int> GetIndexes(int startIndex)
	{
		int rangeIndex = FindRangeIndex(startIndex);
		if (rangeIndex == -1)
		{
			rangeIndex++;
		}
		for (; rangeIndex < _list.Count; rangeIndex++)
		{
			for (int i = _list[rangeIndex].LowerBound; i <= _list[rangeIndex].UpperBound; i++)
			{
				if (i >= startIndex)
				{
					yield return i;
				}
			}
		}
	}

	public int GetNthIndex(int n)
	{
		int num = 0;
		foreach (Range<T> item in _list)
		{
			if (num + item.Count > n)
			{
				return item.LowerBound + n - num;
			}
			num += item.Count;
		}
		return -1;
	}

	public T GetValueAt(int index)
	{
		bool found;
		return GetValueAt(index, out found);
	}

	public T GetValueAt(int index, out bool found)
	{
		int num = FindRangeIndex(index);
		if (IsCorrectRangeIndex(num, index))
		{
			found = true;
			return _list[num].Value;
		}
		found = false;
		return default(T);
	}

	public int IndexOf(int index)
	{
		int num = 0;
		foreach (Range<T> item in _list)
		{
			if (item.UpperBound >= index)
			{
				num += index - item.LowerBound;
				break;
			}
			num += item.Count;
		}
		return num;
	}

	public void InsertIndex(int index)
	{
		InsertIndexes(index, 1);
	}

	public void InsertIndexAndValue(int index, T value)
	{
		InsertIndexesAndValues(index, 1, value);
	}

	public void InsertIndexes(int startIndex, int count)
	{
		InsertIndexesPrivate(startIndex, count, FindRangeIndex(startIndex));
	}

	public void InsertIndexesAndValues(int startIndex, int count, T value)
	{
		int num = FindRangeIndex(startIndex);
		InsertIndexesPrivate(startIndex, count, num);
		if (num >= 0 && _list[num].LowerBound > startIndex)
		{
			num--;
		}
		AddValuesPrivate(startIndex, count, value, num);
	}

	public void RemoveIndex(int index)
	{
		RemoveIndexes(index, 1);
	}

	public void RemoveIndexAndValue(int index)
	{
		RemoveIndexesAndValues(index, 1);
	}

	public void RemoveIndexes(int startIndex, int count)
	{
		int num = FindRangeIndex(startIndex);
		if (num < 0)
		{
			num = 0;
		}
		for (int i = num; i < _list.Count; i++)
		{
			Range<T> range = _list[i];
			if (range.UpperBound < startIndex)
			{
				continue;
			}
			if (range.LowerBound >= startIndex + count)
			{
				range.LowerBound -= count;
				range.UpperBound -= count;
				continue;
			}
			int rangeIndex = i;
			if (range.LowerBound <= startIndex)
			{
				if (range.UpperBound >= startIndex + count)
				{
					i++;
					_list.Insert(i, new Range<T>(startIndex, range.UpperBound - count, range.Value));
				}
				range.UpperBound = startIndex - 1;
			}
			else
			{
				range.LowerBound = startIndex;
				range.UpperBound -= count;
			}
			if (RemoveRangeIfInvalid(range, rangeIndex))
			{
				i--;
			}
		}
		if (!Merge(num))
		{
			Merge(num + 1);
		}
	}

	public void RemoveIndexesAndValues(int startIndex, int count)
	{
		RemoveValues(startIndex, count);
		RemoveIndexes(startIndex, count);
	}

	public void RemoveValue(int index)
	{
		RemoveValues(index, 1);
	}

	public void RemoveValues(int startIndex, int count)
	{
		int i = FindRangeIndex(startIndex);
		if (i < 0)
		{
			i = 0;
		}
		for (; i < _list.Count && _list[i].UpperBound < startIndex; i++)
		{
		}
		if (i < _list.Count && _list[i].LowerBound <= startIndex + count - 1)
		{
			if (_list[i].LowerBound < startIndex)
			{
				_list.Insert(i, new Range<T>(_list[i].LowerBound, startIndex - 1, _list[i].Value));
				i++;
			}
			_list[i].LowerBound = startIndex + count;
			if (!RemoveRangeIfInvalid(_list[i], i))
			{
				i++;
			}
			while (i < _list.Count && _list[i].UpperBound < startIndex + count)
			{
				_list.RemoveAt(i);
			}
			if (i < _list.Count && _list[i].UpperBound >= startIndex + count && _list[i].LowerBound < startIndex + count)
			{
				_list[i].LowerBound = startIndex + count;
				RemoveRangeIfInvalid(_list[i], i);
			}
		}
	}

	private void AddValuesPrivate(int startIndex, int count, T value, int? startRangeIndex)
	{
		int num = startIndex + count - 1;
		Range<T> item = new Range<T>(startIndex, num, value);
		if (_list.Count == 0)
		{
			_list.Add(item);
			return;
		}
		int num2 = startRangeIndex ?? FindRangeIndex(startIndex);
		Range<T> range = ((num2 < 0) ? null : _list[num2]);
		if (range == null)
		{
			if (num2 < 0)
			{
				num2 = 0;
			}
			_list.Insert(num2, item);
		}
		else if (!range.Value.Equals(value) && range.UpperBound >= startIndex)
		{
			if (range.UpperBound > num)
			{
				_list.Insert(num2 + 1, new Range<T>(num + 1, range.UpperBound, range.Value));
			}
			range.UpperBound = startIndex - 1;
			if (!RemoveRangeIfInvalid(range, num2))
			{
				num2++;
			}
			_list.Insert(num2, item);
		}
		else
		{
			_list.Insert(num2 + 1, item);
			if (!Merge(num2))
			{
				num2++;
			}
		}
		int num3 = num2 + 1;
		while (num3 < _list.Count && _list[num3].UpperBound < num)
		{
			_list.RemoveAt(num3);
		}
		if (num3 < _list.Count)
		{
			Range<T> range2 = _list[num3];
			if (range2.LowerBound <= num)
			{
				range2.LowerBound = num + 1;
				RemoveRangeIfInvalid(range2, num3);
			}
			Merge(num2);
		}
	}

	private int FindRangeIndex(int index)
	{
		if (_list.Count == 0)
		{
			return -1;
		}
		int num = 0;
		int num2 = _list.Count - 1;
		Range<T> range = null;
		while (num2 > num)
		{
			int num3 = (num + num2) / 2;
			range = _list[num3];
			if (range.UpperBound < index)
			{
				num = num3 + 1;
				continue;
			}
			if (range.LowerBound > index)
			{
				num2 = num3 - 1;
				continue;
			}
			return num3;
		}
		if (num == num2)
		{
			range = _list[num];
			if (range.ContainsIndex(index) || range.UpperBound < index)
			{
				return num;
			}
			return num - 1;
		}
		return num2;
	}

	private bool Merge(int lowerRangeIndex)
	{
		int num = lowerRangeIndex + 1;
		if (lowerRangeIndex >= 0 && num < _list.Count)
		{
			Range<T> range = _list[lowerRangeIndex];
			Range<T> range2 = _list[num];
			if (range.UpperBound + 1 >= range2.LowerBound && range.Value.Equals(range2.Value))
			{
				range.UpperBound = Math.Max(range.UpperBound, range2.UpperBound);
				_list.RemoveAt(num);
				return true;
			}
		}
		return false;
	}

	private void InsertIndexesPrivate(int startIndex, int count, int lowerRangeIndex)
	{
		for (int i = ((lowerRangeIndex >= 0) ? lowerRangeIndex : 0); i < _list.Count; i++)
		{
			Range<T> range = _list[i];
			if (range.LowerBound >= startIndex)
			{
				range.LowerBound += count;
			}
			else if (range.UpperBound >= startIndex)
			{
				i++;
				_list.Insert(i, new Range<T>(startIndex, range.UpperBound + count, range.Value));
				range.UpperBound = startIndex - 1;
				continue;
			}
			if (range.UpperBound >= startIndex)
			{
				range.UpperBound += count;
			}
		}
	}

	private bool IsCorrectRangeIndex(int rangeIndex, int index)
	{
		if (-1 != rangeIndex)
		{
			return _list[rangeIndex].ContainsIndex(index);
		}
		return false;
	}

	private bool RemoveRangeIfInvalid(Range<T> range, int rangeIndex)
	{
		if (range.UpperBound < range.LowerBound)
		{
			_list.RemoveAt(rangeIndex);
			return true;
		}
		return false;
	}

	public IEnumerator<Range<T>> GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _list.GetEnumerator();
	}
}
