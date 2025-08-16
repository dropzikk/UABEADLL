using System;
using System.Collections;
using System.ComponentModel;

namespace Avalonia.Collections;

internal class DataGridCollectionViewGroupInternal : DataGridCollectionViewGroup
{
	private class LeafEnumerator : IEnumerator
	{
		private object _current;

		private DataGridCollectionViewGroupInternal _group;

		private int _index;

		private IEnumerator _subEnum;

		private int _version;

		object IEnumerator.Current
		{
			get
			{
				if (_index < 0 || _index >= _group.Items.Count)
				{
					throw new InvalidOperationException();
				}
				return _current;
			}
		}

		public LeafEnumerator(DataGridCollectionViewGroupInternal group)
		{
			_group = group;
			DoReset();
		}

		private void DoReset()
		{
			_version = _group._version;
			_index = -1;
			_subEnum = null;
		}

		void IEnumerator.Reset()
		{
			DoReset();
		}

		bool IEnumerator.MoveNext()
		{
			if (_group._version != _version)
			{
				throw new InvalidOperationException();
			}
			while (_subEnum == null || !_subEnum.MoveNext())
			{
				_index++;
				if (_index >= _group.Items.Count)
				{
					return false;
				}
				if (!(_group.Items[_index] is DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal))
				{
					_current = _group.Items[_index];
					_subEnum = null;
					return true;
				}
				_subEnum = dataGridCollectionViewGroupInternal.GetLeafEnumerator();
			}
			_current = _subEnum.Current;
			return true;
		}
	}

	internal class ListComparer : IComparer
	{
		private int _index;

		private IList _list;

		internal ListComparer(IList list)
		{
			ResetList(list);
		}

		internal void Reset()
		{
			_index = 0;
		}

		internal void ResetList(IList list)
		{
			_list = list;
			_index = 0;
		}

		public int Compare(object x, object y)
		{
			if (object.Equals(x, y))
			{
				return 0;
			}
			int num = ((_list != null) ? _list.Count : 0);
			while (_index < num)
			{
				object objB = _list[_index];
				if (object.Equals(x, objB))
				{
					return -1;
				}
				if (object.Equals(y, objB))
				{
					return 1;
				}
				_index++;
			}
			return 1;
		}
	}

	internal class CollectionViewGroupComparer : IComparer
	{
		private int _index;

		private CollectionViewGroupRoot _group;

		internal CollectionViewGroupComparer(CollectionViewGroupRoot group)
		{
			ResetGroup(group);
		}

		internal void Reset()
		{
			_index = 0;
		}

		internal void ResetGroup(CollectionViewGroupRoot group)
		{
			_group = group;
			_index = 0;
		}

		public int Compare(object x, object y)
		{
			if (object.Equals(x, y))
			{
				return 0;
			}
			int num = ((_group != null) ? _group.ItemCount : 0);
			while (_index < num)
			{
				object objB = _group.LeafAt(_index);
				if (object.Equals(x, objB))
				{
					return -1;
				}
				if (object.Equals(y, objB))
				{
					return 1;
				}
				_index++;
			}
			return 1;
		}
	}

	private DataGridGroupDescription _groupBy;

	private readonly DataGridCollectionViewGroupInternal _parentGroup;

	private int _version;

	public override bool IsBottomLevel => _groupBy == null;

	internal int FullCount { get; set; }

	internal DataGridGroupDescription GroupBy
	{
		get
		{
			return _groupBy;
		}
		set
		{
			bool isBottomLevel = IsBottomLevel;
			if (_groupBy != null)
			{
				((INotifyPropertyChanged)_groupBy).PropertyChanged -= OnGroupByChanged;
			}
			_groupBy = value;
			if (_groupBy != null)
			{
				((INotifyPropertyChanged)_groupBy).PropertyChanged += OnGroupByChanged;
			}
			if (isBottomLevel != IsBottomLevel)
			{
				OnPropertyChanged(new PropertyChangedEventArgs("IsBottomLevel"));
			}
		}
	}

	internal int LastIndex { get; set; }

	internal object SeedItem
	{
		get
		{
			if (base.ItemCount > 0 && (GroupBy == null || GroupBy.GroupKeys.Count == 0))
			{
				int i = 0;
				for (int count = base.Items.Count; i < count; i++)
				{
					if (!(base.Items[i] is DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal))
					{
						return base.Items[i];
					}
					if (dataGridCollectionViewGroupInternal.ItemCount > 0)
					{
						return dataGridCollectionViewGroupInternal.SeedItem;
					}
				}
				return AvaloniaProperty.UnsetValue;
			}
			return AvaloniaProperty.UnsetValue;
		}
	}

	private DataGridCollectionViewGroupInternal Parent => _parentGroup;

	public DataGridCollectionViewGroupInternal(object key, DataGridCollectionViewGroupInternal parent)
		: base(key)
	{
		_parentGroup = parent;
	}

	private void OnGroupByChanged(object sender, PropertyChangedEventArgs e)
	{
		OnGroupByChanged();
	}

	protected virtual void OnGroupByChanged()
	{
		_parentGroup?.OnGroupByChanged();
	}

	internal void Add(object item)
	{
		ChangeCounts(item, 1);
		base.ProtectedItems.Add(item);
	}

	internal void Clear()
	{
		base.ProtectedItems.Clear();
		FullCount = 1;
		base.ProtectedItemCount = 0;
	}

	protected virtual int FindIndex(object item, object seed, IComparer comparer, int low, int high)
	{
		int i;
		if (comparer != null)
		{
			if (comparer is ListComparer listComparer)
			{
				listComparer.Reset();
			}
			if (comparer is CollectionViewGroupComparer collectionViewGroupComparer)
			{
				collectionViewGroupComparer.Reset();
			}
			for (i = low; i < high; i++)
			{
				object obj = ((base.ProtectedItems[i] is DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal) ? dataGridCollectionViewGroupInternal.SeedItem : base.ProtectedItems[i]);
				if (obj != AvaloniaProperty.UnsetValue && comparer.Compare(seed, obj) < 0)
				{
					break;
				}
			}
		}
		else
		{
			i = high;
		}
		return i;
	}

	internal IEnumerator GetLeafEnumerator()
	{
		return new LeafEnumerator(this);
	}

	internal int Insert(object item, object seed, IComparer comparer)
	{
		int low = ((GroupBy != null) ? GroupBy.GroupKeys.Count : 0);
		int num = FindIndex(item, seed, comparer, low, base.ProtectedItems.Count);
		ChangeCounts(item, 1);
		base.ProtectedItems.Insert(num, item);
		return num;
	}

	internal object LeafAt(int index)
	{
		int i = 0;
		for (int count = base.Items.Count; i < count; i++)
		{
			if (base.Items[i] is DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal)
			{
				if (index < dataGridCollectionViewGroupInternal.ItemCount)
				{
					return dataGridCollectionViewGroupInternal.LeafAt(index);
				}
				index -= dataGridCollectionViewGroupInternal.ItemCount;
			}
			else
			{
				if (index == 0)
				{
					return base.Items[i];
				}
				index--;
			}
		}
		return null;
	}

	internal int LeafIndexFromItem(object item, int index)
	{
		int num = 0;
		DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal = this;
		while (dataGridCollectionViewGroupInternal != null)
		{
			int i = 0;
			for (int count = dataGridCollectionViewGroupInternal.Items.Count; i < count && (index >= 0 || !object.Equals(item, dataGridCollectionViewGroupInternal.Items[i])) && index != i; i++)
			{
				num += (dataGridCollectionViewGroupInternal.Items[i] as DataGridCollectionViewGroupInternal)?.ItemCount ?? 1;
			}
			item = dataGridCollectionViewGroupInternal;
			dataGridCollectionViewGroupInternal = dataGridCollectionViewGroupInternal.Parent;
			index = -1;
		}
		return num;
	}

	internal int LeafIndexOf(object item)
	{
		int num = 0;
		int i = 0;
		for (int count = base.Items.Count; i < count; i++)
		{
			if (base.Items[i] is DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal)
			{
				int num2 = dataGridCollectionViewGroupInternal.LeafIndexOf(item);
				if (num2 >= 0)
				{
					return num + num2;
				}
				num += dataGridCollectionViewGroupInternal.ItemCount;
			}
			else
			{
				if (object.Equals(item, base.Items[i]))
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	internal int Remove(object item, bool returnLeafIndex)
	{
		int result = -1;
		int num = base.ProtectedItems.IndexOf(item);
		if (num >= 0)
		{
			if (returnLeafIndex)
			{
				result = LeafIndexFromItem(null, num);
			}
			ChangeCounts(item, -1);
			base.ProtectedItems.RemoveAt(num);
		}
		return result;
	}

	private static void RemoveEmptyGroup(DataGridCollectionViewGroupInternal group)
	{
		DataGridCollectionViewGroupInternal parent = group.Parent;
		if (parent != null)
		{
			DataGridGroupDescription groupBy = parent.GroupBy;
			if (parent.ProtectedItems.IndexOf(group) >= groupBy.GroupKeys.Count)
			{
				parent.Remove(group, returnLeafIndex: false);
			}
		}
	}

	protected void ChangeCounts(object item, int delta)
	{
		bool flag = !(item is DataGridCollectionViewGroup);
		for (DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal = this; dataGridCollectionViewGroupInternal != null; dataGridCollectionViewGroupInternal = dataGridCollectionViewGroupInternal._parentGroup)
		{
			dataGridCollectionViewGroupInternal.FullCount += delta;
			if (flag)
			{
				dataGridCollectionViewGroupInternal.ProtectedItemCount += delta;
				if (dataGridCollectionViewGroupInternal.ProtectedItemCount == 0)
				{
					RemoveEmptyGroup(dataGridCollectionViewGroupInternal);
				}
			}
		}
		_version++;
	}
}
