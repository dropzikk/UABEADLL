using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace Avalonia.Collections;

internal class CollectionViewGroupRoot : DataGridCollectionViewGroupInternal, INotifyCollectionChanged
{
	private class TopLevelGroupDescription : DataGridGroupDescription
	{
		public override object GroupKeyFromItem(object item, int level, CultureInfo culture)
		{
			return null;
		}
	}

	private const string RootName = "Root";

	private static readonly object UseAsItemDirectly = new object();

	private static DataGridGroupDescription topLevelGroupDescription;

	private readonly AvaloniaList<DataGridGroupDescription> _groupBy = new AvaloniaList<DataGridGroupDescription>();

	private bool _isDataInGroupOrder;

	private readonly IDataGridCollectionView _view;

	public virtual AvaloniaList<DataGridGroupDescription> GroupDescriptions => _groupBy;

	internal IComparer ActiveComparer { get; set; }

	internal CultureInfo Culture => _view.Culture;

	internal bool IsDataInGroupOrder
	{
		get
		{
			return _isDataInGroupOrder;
		}
		set
		{
			_isDataInGroupOrder = value;
		}
	}

	public virtual Func<DataGridCollectionViewGroup, int, DataGridGroupDescription> GroupBySelector { get; set; }

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	internal event EventHandler GroupDescriptionChanged;

	internal CollectionViewGroupRoot(IDataGridCollectionView view, bool isDataInGroupOrder)
		: base("Root", null)
	{
		_view = view;
		_isDataInGroupOrder = isDataInGroupOrder;
	}

	protected override int FindIndex(object item, object seed, IComparer comparer, int low, int high)
	{
		if (_view is IDataGridEditableCollectionView { IsAddingNew: not false })
		{
			high--;
		}
		return base.FindIndex(item, seed, comparer, low, high);
	}

	internal void Initialize()
	{
		if (topLevelGroupDescription == null)
		{
			topLevelGroupDescription = new TopLevelGroupDescription();
		}
		InitializeGroup(this, 0, null);
	}

	internal void InsertSpecialItem(int index, object item, bool loading)
	{
		ChangeCounts(item, 1);
		base.ProtectedItems.Insert(index, item);
		if (!loading)
		{
			int index2 = LeafIndexFromItem(item, index);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index2));
		}
	}

	public void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
	{
		this.CollectionChanged?.Invoke(this, args);
	}

	protected override void OnGroupByChanged()
	{
		this.GroupDescriptionChanged?.Invoke(this, EventArgs.Empty);
	}

	internal bool RemoveFromSubgroups(object item)
	{
		return RemoveFromSubgroups(item, this, 0);
	}

	internal void RemoveItemFromSubgroupsByExhaustiveSearch(object item)
	{
		RemoveItemFromSubgroupsByExhaustiveSearch(this, item);
	}

	internal void RemoveSpecialItem(int index, object item, bool loading)
	{
		int index2 = -1;
		if (!loading)
		{
			index2 = LeafIndexFromItem(item, index);
		}
		ChangeCounts(item, -1);
		base.ProtectedItems.RemoveAt(index);
		if (!loading)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index2));
		}
	}

	internal void AddToSubgroups(object item, bool loading)
	{
		AddToSubgroups(item, this, 0, loading);
	}

	private void AddToSubgroup(object item, DataGridCollectionViewGroupInternal group, int level, object key, bool loading)
	{
		int i = (_isDataInGroupOrder ? group.LastIndex : 0);
		for (int count = group.Items.Count; i < count; i++)
		{
			if (group.Items[i] is DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal && group.GroupBy.KeysMatch(dataGridCollectionViewGroupInternal.Key, key))
			{
				group.LastIndex = i;
				AddToSubgroups(item, dataGridCollectionViewGroupInternal, level + 1, loading);
				return;
			}
		}
		DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal2 = new DataGridCollectionViewGroupInternal(key, group);
		InitializeGroup(dataGridCollectionViewGroupInternal2, level + 1, item);
		if (loading)
		{
			group.Add(dataGridCollectionViewGroupInternal2);
			group.LastIndex = i;
		}
		else
		{
			group.Insert(dataGridCollectionViewGroupInternal2, item, ActiveComparer);
		}
		AddToSubgroups(item, dataGridCollectionViewGroupInternal2, level + 1, loading);
	}

	private void AddToSubgroups(object item, DataGridCollectionViewGroupInternal group, int level, bool loading)
	{
		object groupKey = GetGroupKey(item, group.GroupBy, level);
		if (groupKey == UseAsItemDirectly)
		{
			if (loading)
			{
				group.Add(item);
				return;
			}
			int index = group.Insert(item, item, ActiveComparer);
			int index2 = group.LeafIndexFromItem(item, index);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index2));
			return;
		}
		if (groupKey is ICollection collection)
		{
			{
				foreach (object item2 in collection)
				{
					AddToSubgroup(item, group, level, item2, loading);
				}
				return;
			}
		}
		AddToSubgroup(item, group, level, groupKey, loading);
	}

	private DataGridGroupDescription GetGroupDescription(DataGridCollectionViewGroup group, int level)
	{
		DataGridGroupDescription dataGridGroupDescription = null;
		if (group == this)
		{
			group = null;
		}
		if (dataGridGroupDescription == null && GroupBySelector != null)
		{
			dataGridGroupDescription = GroupBySelector?.Invoke(group, level);
		}
		if (dataGridGroupDescription == null && level < GroupDescriptions.Count)
		{
			dataGridGroupDescription = GroupDescriptions[level];
		}
		return dataGridGroupDescription;
	}

	private object GetGroupKey(object item, DataGridGroupDescription groupDescription, int level)
	{
		if (groupDescription != null)
		{
			return groupDescription.GroupKeyFromItem(item, level, Culture);
		}
		return UseAsItemDirectly;
	}

	private void InitializeGroup(DataGridCollectionViewGroupInternal group, int level, object seedItem)
	{
		AvaloniaList<object> avaloniaList = (group.GroupBy = GetGroupDescription(group, level))?.GroupKeys;
		if (avaloniaList != null)
		{
			int i = 0;
			for (int count = avaloniaList.Count; i < count; i++)
			{
				DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal = new DataGridCollectionViewGroupInternal(avaloniaList[i], group);
				InitializeGroup(dataGridCollectionViewGroupInternal, level + 1, seedItem);
				group.Add(dataGridCollectionViewGroupInternal);
			}
		}
		group.LastIndex = 0;
	}

	private bool RemoveFromGroupDirectly(DataGridCollectionViewGroupInternal group, object item)
	{
		int num = group.Remove(item, returnLeafIndex: true);
		if (num >= 0)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, num));
			return false;
		}
		return true;
	}

	private bool RemoveFromSubgroup(object item, DataGridCollectionViewGroupInternal group, int level, object key)
	{
		bool result = false;
		int i = 0;
		for (int count = group.Items.Count; i < count; i++)
		{
			if (group.Items[i] is DataGridCollectionViewGroupInternal dataGridCollectionViewGroupInternal && group.GroupBy.KeysMatch(dataGridCollectionViewGroupInternal.Key, key))
			{
				if (RemoveFromSubgroups(item, dataGridCollectionViewGroupInternal, level + 1))
				{
					result = true;
				}
				return result;
			}
		}
		return true;
	}

	private bool RemoveFromSubgroups(object item, DataGridCollectionViewGroupInternal group, int level)
	{
		bool result = false;
		object groupKey = GetGroupKey(item, group.GroupBy, level);
		if (groupKey == UseAsItemDirectly)
		{
			result = RemoveFromGroupDirectly(group, item);
		}
		else if (groupKey is ICollection collection)
		{
			foreach (object item2 in collection)
			{
				if (RemoveFromSubgroup(item, group, level, item2))
				{
					result = true;
				}
			}
		}
		else if (RemoveFromSubgroup(item, group, level, groupKey))
		{
			result = true;
		}
		return result;
	}

	private void RemoveItemFromSubgroupsByExhaustiveSearch(DataGridCollectionViewGroupInternal group, object item)
	{
		if (!RemoveFromGroupDirectly(group, item))
		{
			return;
		}
		for (int num = group.Items.Count - 1; num >= 0; num--)
		{
			if (group.Items[num] is DataGridCollectionViewGroupInternal group2)
			{
				RemoveItemFromSubgroupsByExhaustiveSearch(group2, item);
			}
		}
	}
}
