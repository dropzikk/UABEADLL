using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia.Controls.Utils;

namespace Avalonia.Collections;

public sealed class DataGridCollectionView : IDataGridCollectionView, IEnumerable, INotifyCollectionChanged, IDataGridEditableCollectionView, INotifyPropertyChanged
{
	[Flags]
	private enum CollectionViewFlags
	{
		IsDataInGroupOrder = 1,
		IsDataSorted = 2,
		ShouldProcessCollectionChanged = 4,
		IsCurrentBeforeFirst = 8,
		IsCurrentAfterLast = 0x10,
		NeedsRefresh = 0x20,
		CachedIsEmpty = 0x40,
		IsPageChanging = 0x80,
		IsMoveToPageDeferred = 0x100,
		IsUpdatePageSizeDeferred = 0x200
	}

	private class CultureSensitiveComparer : IComparer<object>
	{
		private CultureInfo _culture;

		public CultureSensitiveComparer(CultureInfo culture)
		{
			_culture = culture ?? CultureInfo.InvariantCulture;
		}

		public int Compare(object x, object y)
		{
			if (x == null)
			{
				if (y != null)
				{
					return -1;
				}
				return 0;
			}
			if (y == null)
			{
				return 1;
			}
			if (x.GetType() == typeof(string) && y.GetType() == typeof(string))
			{
				return _culture.CompareInfo.Compare((string)x, (string)y);
			}
			return Comparer<object>.Default.Compare(x, y);
		}
	}

	private class DeferHelper : IDisposable
	{
		private DataGridCollectionView collectionView;

		public DeferHelper(DataGridCollectionView collectionView)
		{
			this.collectionView = collectionView;
		}

		public void Dispose()
		{
			if (collectionView != null)
			{
				collectionView.EndDefer();
				collectionView = null;
			}
			GC.SuppressFinalize(this);
		}
	}

	private class SimpleMonitor : IDisposable
	{
		private bool entered;

		public bool Busy => entered;

		public bool Enter()
		{
			if (entered)
			{
				return false;
			}
			entered = true;
			return true;
		}

		public void Dispose()
		{
			entered = false;
			GC.SuppressFinalize(this);
		}
	}

	private class NewItemAwareEnumerator : IEnumerator
	{
		private enum Position
		{
			BeforeNewItem,
			OnNewItem,
			AfterNewItem
		}

		private DataGridCollectionView _collectionView;

		private IEnumerator _baseEnumerator;

		private Position _position;

		private object _newItem;

		private int _timestamp;

		public object Current
		{
			get
			{
				if (_position != Position.OnNewItem)
				{
					return _baseEnumerator.Current;
				}
				return _newItem;
			}
		}

		public NewItemAwareEnumerator(DataGridCollectionView collectionView, IEnumerator baseEnumerator, object newItem)
		{
			_collectionView = collectionView;
			_timestamp = collectionView.Timestamp;
			_baseEnumerator = baseEnumerator;
			_newItem = newItem;
		}

		public bool MoveNext()
		{
			if (_timestamp != _collectionView.Timestamp)
			{
				throw new InvalidOperationException("Collection was modified; enumeration operation cannot execute.");
			}
			if (_position == Position.BeforeNewItem)
			{
				if (!_baseEnumerator.MoveNext() || (_newItem != null && _baseEnumerator.Current == _newItem && !_baseEnumerator.MoveNext()))
				{
					if (_newItem == null)
					{
						return false;
					}
					_position = Position.OnNewItem;
				}
				return true;
			}
			_position = Position.AfterNewItem;
			if (_baseEnumerator.MoveNext())
			{
				if (_newItem != null && _baseEnumerator.Current == _newItem)
				{
					return _baseEnumerator.MoveNext();
				}
				return true;
			}
			return false;
		}

		public void Reset()
		{
			_position = Position.BeforeNewItem;
			_baseEnumerator.Reset();
		}
	}

	internal class MergedComparer
	{
		private readonly IComparer<object>[] _comparers;

		public MergedComparer(DataGridSortDescriptionCollection coll)
		{
			_comparers = MakeComparerArray(coll);
		}

		public MergedComparer(DataGridCollectionView collectionView)
			: this(collectionView.SortDescriptions)
		{
		}

		private static IComparer<object>[] MakeComparerArray(DataGridSortDescriptionCollection coll)
		{
			return coll.Select((DataGridSortDescription c) => c.Comparer).ToArray();
		}

		public int Compare(object x, object y)
		{
			int num = 0;
			for (int i = 0; i < _comparers.Length; i++)
			{
				num = _comparers[i].Compare(x, y);
				if (num != 0)
				{
					break;
				}
			}
			return num;
		}

		public int FindInsertIndex(object x, IList list)
		{
			int num = 0;
			int num2 = list.Count - 1;
			while (num <= num2)
			{
				int num3 = (num + num2) / 2;
				int num4 = Compare(x, list[num3]);
				if (num4 == 0)
				{
					return num3;
				}
				if (num4 > 0)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3 - 1;
				}
			}
			return num;
		}
	}

	private static readonly DataGridCurrentChangingEventArgs uncancelableCurrentChangingEventArgs = new DataGridCurrentChangingEventArgs(isCancelable: false);

	private int _cachedPageIndex = -1;

	private int _cachedPageSize;

	private CultureInfo _culture;

	private SimpleMonitor _currentChangedMonitor = new SimpleMonitor();

	private object _currentItem;

	private int _currentPosition;

	private int _deferLevel;

	private object _editItem;

	private Func<object, bool> _filter;

	private CollectionViewFlags _flags = CollectionViewFlags.ShouldProcessCollectionChanged;

	private CollectionViewGroupRoot _group;

	private IList _internalList;

	private bool _isGrouping;

	private bool _isUsingTemporaryGroup;

	private ConstructorInfo _itemConstructor;

	private bool _itemConstructorIsValid;

	private object _newItem;

	private int _pageIndex = -1;

	private int _pageSize;

	private bool _pollForChanges;

	private DataGridSortDescriptionCollection _sortDescriptions;

	private IEnumerable _sourceCollection;

	private CollectionViewGroupRoot _temporaryGroup;

	private int _timestamp;

	private IEnumerator _trackingEnumerator;

	private Type _itemType;

	private Type ItemType
	{
		get
		{
			if (_itemType == null)
			{
				_itemType = GetItemType(useRepresentativeItem: true);
			}
			return _itemType;
		}
	}

	public bool CanAddNew
	{
		get
		{
			if (!IsEditingItem)
			{
				if (SourceList != null && !SourceList.IsFixedSize)
				{
					return CanConstructItem;
				}
				return false;
			}
			return false;
		}
	}

	public bool CanCancelEdit => _editItem is IEditableObject;

	public bool CanChangePage => true;

	public bool CanFilter => true;

	public bool CanGroup => true;

	public bool CanRemove
	{
		get
		{
			if (!IsEditingItem && !IsAddingNew)
			{
				if (SourceList != null)
				{
					return !SourceList.IsFixedSize;
				}
				return false;
			}
			return false;
		}
	}

	public bool CanSort => true;

	public int Count
	{
		get
		{
			EnsureCollectionInSync();
			VerifyRefreshNotDeferred();
			if (PageSize > 0 && PageIndex > -1)
			{
				if (IsGrouping && !_isUsingTemporaryGroup)
				{
					return _group.ItemCount;
				}
				return Math.Max(0, Math.Min(PageSize, InternalCount - _pageSize * PageIndex));
			}
			if (IsGrouping)
			{
				if (_isUsingTemporaryGroup)
				{
					return _temporaryGroup.ItemCount;
				}
				return _group.ItemCount;
			}
			return InternalCount;
		}
	}

	public CultureInfo Culture
	{
		get
		{
			return _culture;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (_culture != value)
			{
				_culture = value;
				OnPropertyChanged("Culture");
			}
		}
	}

	public object CurrentAddItem
	{
		get
		{
			return _newItem;
		}
		private set
		{
			if (_newItem != value)
			{
				_newItem = value;
				OnPropertyChanged("IsAddingNew");
				OnPropertyChanged("CurrentAddItem");
			}
		}
	}

	public object CurrentEditItem
	{
		get
		{
			return _editItem;
		}
		private set
		{
			if (_editItem != value)
			{
				bool canCancelEdit = CanCancelEdit;
				_editItem = value;
				OnPropertyChanged("IsEditingItem");
				OnPropertyChanged("CurrentEditItem");
				if (canCancelEdit != CanCancelEdit)
				{
					OnPropertyChanged("CanCancelEdit");
				}
			}
		}
	}

	public object CurrentItem
	{
		get
		{
			VerifyRefreshNotDeferred();
			return _currentItem;
		}
	}

	public int CurrentPosition
	{
		get
		{
			VerifyRefreshNotDeferred();
			return _currentPosition;
		}
	}

	public Func<object, bool> Filter
	{
		get
		{
			return _filter;
		}
		set
		{
			if (IsAddingNew || IsEditingItem)
			{
				throw new InvalidOperationException(GetOperationNotAllowedDuringAddOrEditText("Filter"));
			}
			if (!CanFilter)
			{
				throw new NotSupportedException("The Filter property cannot be set when the CanFilter property returns false.");
			}
			if (_filter != value)
			{
				_filter = value;
				RefreshOrDefer();
				OnPropertyChanged("Filter");
			}
		}
	}

	public AvaloniaList<DataGridGroupDescription> GroupDescriptions => _group?.GroupDescriptions;

	int IDataGridCollectionView.GroupingDepth => GroupDescriptions?.Count ?? 0;

	public IAvaloniaReadOnlyList<object> Groups
	{
		get
		{
			if (!IsGrouping)
			{
				return null;
			}
			return RootGroup?.Items;
		}
	}

	public bool IsAddingNew => _newItem != null;

	public bool IsCurrentAfterLast
	{
		get
		{
			VerifyRefreshNotDeferred();
			return CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
		}
	}

	public bool IsCurrentBeforeFirst
	{
		get
		{
			VerifyRefreshNotDeferred();
			return CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
		}
	}

	public bool IsEditingItem => _editItem != null;

	public bool IsEmpty
	{
		get
		{
			EnsureCollectionInSync();
			return InternalCount == 0;
		}
	}

	public bool IsPageChanging
	{
		get
		{
			return CheckFlag(CollectionViewFlags.IsPageChanging);
		}
		private set
		{
			if (CheckFlag(CollectionViewFlags.IsPageChanging) != value)
			{
				SetFlag(CollectionViewFlags.IsPageChanging, value);
				OnPropertyChanged("IsPageChanging");
			}
		}
	}

	public int ItemCount => InternalList.Count;

	public bool NeedsRefresh => CheckFlag(CollectionViewFlags.NeedsRefresh);

	public int PageIndex => _pageIndex;

	public int PageSize
	{
		get
		{
			return _pageSize;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value", "PageSize cannot have a negative value.");
			}
			if (IsRefreshDeferred)
			{
				_cachedPageSize = value;
				SetFlag(CollectionViewFlags.IsUpdatePageSizeDeferred, value: true);
				return;
			}
			int count = Count;
			if (_pageSize == value)
			{
				return;
			}
			object currentItem = CurrentItem;
			int currentPosition = CurrentPosition;
			bool isCurrentAfterLast = IsCurrentAfterLast;
			bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
			if (CurrentAddItem != null || CurrentEditItem != null)
			{
				if (!OkToChangeCurrent())
				{
					throw new InvalidOperationException("Changing the PageSize is not allowed during an AddNew or EditItem transaction.");
				}
				SetCurrentToPosition(-1);
				RaiseCurrencyChanges(fireChangedEvent: true, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
				if (CurrentAddItem != null || CurrentEditItem != null)
				{
					throw new InvalidOperationException("Changing the PageSize is not allowed during an AddNew or EditItem transaction.");
				}
			}
			_pageSize = value;
			OnPropertyChanged("PageSize");
			if (_pageSize == 0)
			{
				PrepareGroups();
				MoveToPage(-1);
			}
			else if (_pageIndex != 0)
			{
				if (!CheckFlag(CollectionViewFlags.IsMoveToPageDeferred))
				{
					if (IsGrouping && _temporaryGroup.ItemCount != InternalList.Count)
					{
						PrepareTemporaryGroups();
					}
					MoveToFirstPage();
				}
			}
			else if (IsGrouping)
			{
				if (_temporaryGroup.ItemCount != InternalList.Count)
				{
					PrepareTemporaryGroups();
				}
				PrepareGroupsForCurrentPage();
			}
			if (Count != count)
			{
				OnPropertyChanged("Count");
			}
			ResetCurrencyValues(currentItem, isCurrentBeforeFirst, isCurrentAfterLast);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			RaiseCurrencyChanges(fireChangedEvent: false, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
		}
	}

	public DataGridSortDescriptionCollection SortDescriptions
	{
		get
		{
			if (_sortDescriptions == null)
			{
				SetSortDescriptions(new DataGridSortDescriptionCollection());
			}
			return _sortDescriptions;
		}
	}

	public IEnumerable SourceCollection => _sourceCollection;

	public int TotalItemCount => InternalList.Count;

	private bool CanConstructItem
	{
		get
		{
			if (!_itemConstructorIsValid)
			{
				EnsureItemConstructor();
			}
			return _itemConstructor != null;
		}
	}

	private int InternalCount => InternalList.Count;

	private IList InternalList => _internalList;

	private bool IsCurrentInSync
	{
		get
		{
			if (IsCurrentInView)
			{
				return GetItemAt(CurrentPosition).Equals(CurrentItem);
			}
			return CurrentItem == null;
		}
	}

	private bool IsCurrentInView
	{
		get
		{
			VerifyRefreshNotDeferred();
			return IndexOf(CurrentItem) >= 0;
		}
	}

	private bool IsGrouping => _isGrouping;

	bool IDataGridCollectionView.IsGrouping => IsGrouping;

	private bool IsRefreshDeferred => _deferLevel > 0;

	private bool NeedToMoveToPreviousPage
	{
		get
		{
			if (PageSize > 0 && Count == 0 && PageIndex != 0)
			{
				return PageCount == PageIndex;
			}
			return false;
		}
	}

	private bool OnLastLocalPage
	{
		get
		{
			if (PageSize == 0)
			{
				return false;
			}
			if (PageCount == 1)
			{
				return true;
			}
			return PageIndex == PageCount - 1;
		}
	}

	private int PageCount
	{
		get
		{
			if (_pageSize <= 0)
			{
				return 0;
			}
			return Math.Max(1, (int)Math.Ceiling((double)ItemCount / (double)_pageSize));
		}
	}

	private CollectionViewGroupRoot RootGroup
	{
		get
		{
			if (!_isUsingTemporaryGroup)
			{
				return _group;
			}
			return _temporaryGroup;
		}
	}

	private IList SourceList => SourceCollection as IList;

	private int Timestamp => _timestamp;

	private bool UsesLocalArray
	{
		get
		{
			if (SortDescriptions.Count <= 0 && Filter == null && _pageSize <= 0)
			{
				return GroupDescriptions.Count > 0;
			}
			return true;
		}
	}

	public object this[int index] => GetItemAt(index);

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
	{
		add
		{
			CollectionChanged += value;
		}
		remove
		{
			CollectionChanged -= value;
		}
	}

	public event EventHandler CurrentChanged;

	public event EventHandler<DataGridCurrentChangingEventArgs> CurrentChanging;

	public event EventHandler<EventArgs> PageChanged;

	public event EventHandler<PageChangingEventArgs> PageChanging;

	public event PropertyChangedEventHandler PropertyChanged;

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

	public DataGridCollectionView(IEnumerable source)
		: this(source, isDataSorted: false, isDataInGroupOrder: false)
	{
	}

	public DataGridCollectionView(IEnumerable source, bool isDataSorted, bool isDataInGroupOrder)
	{
		_sourceCollection = source ?? throw new ArgumentNullException("source");
		SetFlag(CollectionViewFlags.IsDataSorted, isDataSorted);
		SetFlag(CollectionViewFlags.IsDataInGroupOrder, isDataInGroupOrder);
		_temporaryGroup = new CollectionViewGroupRoot(this, isDataInGroupOrder);
		_group = new CollectionViewGroupRoot(this, isDataInGroupOrder: false);
		_group.GroupDescriptionChanged += OnGroupDescriptionChanged;
		_group.GroupDescriptions.CollectionChanged += OnGroupByChanged;
		CopySourceToInternalList();
		_trackingEnumerator = source.GetEnumerator();
		if (_internalList.Count > 0)
		{
			SetCurrent(_internalList[0], 0, 1);
		}
		else
		{
			SetCurrent(null, -1, 0);
		}
		SetFlag(CollectionViewFlags.CachedIsEmpty, Count == 0);
		if (source is INotifyCollectionChanged notifyCollectionChanged)
		{
			notifyCollectionChanged.CollectionChanged += delegate(object? _, NotifyCollectionChangedEventArgs args)
			{
				ProcessCollectionChanged(args);
			};
		}
		else
		{
			_pollForChanges = true;
		}
	}

	private string GetOperationNotAllowedDuringAddOrEditText(string action)
	{
		return "'" + action + "' is not allowed during an AddNew or EditItem transaction.";
	}

	private string GetOperationNotAllowedText(string action, string transaction = null)
	{
		if (string.IsNullOrWhiteSpace(transaction))
		{
			return "'" + action + "' is not allowed for this view.";
		}
		return $"'{action}' is not allowed during a transaction started by '{transaction}'.";
	}

	string IDataGridCollectionView.GetGroupingPropertyNameAtDepth(int level)
	{
		AvaloniaList<DataGridGroupDescription> groupDescriptions = GroupDescriptions;
		if (groupDescriptions != null && level >= 0 && level < groupDescriptions.Count)
		{
			return groupDescriptions[level].PropertyName;
		}
		return string.Empty;
	}

	public object AddNew()
	{
		EnsureCollectionInSync();
		VerifyRefreshNotDeferred();
		if (IsEditingItem)
		{
			CommitEdit();
		}
		CommitNew();
		if (!CanAddNew)
		{
			throw new InvalidOperationException(GetOperationNotAllowedText("AddNew"));
		}
		object obj = null;
		if (_itemConstructor != null)
		{
			obj = _itemConstructor.Invoke(null);
		}
		try
		{
			SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, value: false);
			if (SourceList != null)
			{
				SourceList.Add(obj);
			}
		}
		finally
		{
			SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, value: true);
		}
		_trackingEnumerator = _sourceCollection.GetEnumerator();
		int num = -1;
		int num2;
		if (PageSize > 0)
		{
			num2 = Count - ((Count == PageSize) ? 1 : 0);
			num = ((Count == PageSize) ? num2 : (-1));
		}
		else
		{
			num2 = Count;
		}
		if (num > -1)
		{
			object itemAt = GetItemAt(num);
			if (IsGrouping)
			{
				_group.RemoveFromSubgroups(itemAt);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemAt, num));
		}
		_internalList.Insert(ConvertToInternalIndex(num2), obj);
		OnPropertyChanged("ItemCount");
		object currentItem = CurrentItem;
		int currentPosition = CurrentPosition;
		bool isCurrentAfterLast = IsCurrentAfterLast;
		bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
		AdjustCurrencyForAdd(null, num2);
		if (IsGrouping)
		{
			_group.InsertSpecialItem(_group.Items.Count, obj, loading: false);
			if (PageSize > 0)
			{
				_temporaryGroup.InsertSpecialItem(_temporaryGroup.Items.Count, obj, loading: false);
			}
		}
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, num2));
		RaiseCurrencyChanges(fireChangedEvent: false, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
		CurrentAddItem = obj;
		MoveCurrentTo(obj);
		if (obj is IEditableObject editableObject)
		{
			editableObject.BeginEdit();
		}
		return obj;
	}

	public void CancelEdit()
	{
		if (IsAddingNew)
		{
			throw new InvalidOperationException(GetOperationNotAllowedText("CancelEdit", "AddNew"));
		}
		if (!CanCancelEdit)
		{
			throw new InvalidOperationException("CancelEdit is not supported for the current edit item.");
		}
		VerifyRefreshNotDeferred();
		if (CurrentEditItem != null)
		{
			object currentEditItem = CurrentEditItem;
			CurrentEditItem = null;
			if (!(currentEditItem is IEditableObject editableObject))
			{
				throw new InvalidOperationException("CancelEdit is not supported for the current edit item.");
			}
			editableObject.CancelEdit();
		}
	}

	public void CancelNew()
	{
		if (IsEditingItem)
		{
			throw new InvalidOperationException(GetOperationNotAllowedText("CancelNew", "EditItem"));
		}
		VerifyRefreshNotDeferred();
		if (CurrentAddItem == null)
		{
			return;
		}
		int index = IndexOf(CurrentAddItem);
		try
		{
			SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, value: false);
			if (SourceList != null)
			{
				SourceList.Remove(CurrentAddItem);
			}
		}
		finally
		{
			SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, value: true);
		}
		_trackingEnumerator = _sourceCollection.GetEnumerator();
		if (CurrentAddItem == null)
		{
			return;
		}
		object obj = EndAddNew(cancel: true);
		int num = -1;
		if (PageSize > 0 && !OnLastLocalPage)
		{
			num = Count - 1;
		}
		InternalList.Remove(obj);
		if (IsGrouping)
		{
			_group.RemoveSpecialItem(_group.Items.Count - 1, obj, loading: false);
			if (PageSize > 0)
			{
				_temporaryGroup.RemoveSpecialItem(_temporaryGroup.Items.Count - 1, obj, loading: false);
			}
		}
		OnPropertyChanged("ItemCount");
		object currentItem = CurrentItem;
		int currentPosition = CurrentPosition;
		bool isCurrentAfterLast = IsCurrentAfterLast;
		bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
		AdjustCurrencyForRemove(index);
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, index));
		RaiseCurrencyChanges(fireChangedEvent: false, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
		if (num > -1)
		{
			int index2 = ConvertToInternalIndex(num);
			object obj2 = null;
			if (IsGrouping)
			{
				obj2 = _temporaryGroup.LeafAt(index2);
				_group.AddToSubgroups(obj2, loading: false);
			}
			else
			{
				obj2 = InternalItemAt(index2);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj2, IndexOf(obj2)));
		}
	}

	public void CommitEdit()
	{
		if (IsAddingNew)
		{
			throw new InvalidOperationException(GetOperationNotAllowedText("CommitEdit", "AddNew"));
		}
		VerifyRefreshNotDeferred();
		if (CurrentEditItem == null)
		{
			return;
		}
		object currentEditItem = CurrentEditItem;
		CurrentEditItem = null;
		if (currentEditItem is IEditableObject editableObject)
		{
			editableObject.EndEdit();
		}
		int num;
		object newCurrentItem;
		object currentItem;
		int currentPosition;
		bool isCurrentAfterLast;
		bool isCurrentBeforeFirst;
		int num6;
		int num7;
		int num5;
		if (UsesLocalArray)
		{
			num = IndexOf(currentEditItem);
			int index = InternalIndexOf(currentEditItem);
			_internalList.Remove(currentEditItem);
			newCurrentItem = ((currentEditItem == CurrentItem) ? currentEditItem : null);
			if (num >= 0 && IsGrouping)
			{
				_group.RemoveItemFromSubgroupsByExhaustiveSearch(currentEditItem);
				if (PageSize > 0)
				{
					_temporaryGroup.RemoveItemFromSubgroupsByExhaustiveSearch(currentEditItem);
				}
			}
			currentItem = CurrentItem;
			currentPosition = CurrentPosition;
			isCurrentAfterLast = IsCurrentAfterLast;
			isCurrentBeforeFirst = IsCurrentBeforeFirst;
			if (num >= 0)
			{
				AdjustCurrencyForRemove(num);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, currentEditItem, num));
			}
			bool flag = PassesFilter(currentEditItem);
			if (NeedToMoveToPreviousPage && !flag)
			{
				MoveToPreviousPage();
				return;
			}
			ProcessInsertToCollection(currentEditItem, index);
			int num2 = PageIndex * PageSize;
			int num3 = num2 + PageSize;
			if (IsGrouping)
			{
				int num4 = -1;
				if (flag && PageSize > 0)
				{
					_temporaryGroup.AddToSubgroups(currentEditItem, loading: false);
					num4 = _temporaryGroup.LeafIndexOf(currentEditItem);
				}
				if (flag && (PageSize == 0 || (num2 <= num4 && num3 > num4)))
				{
					_group.AddToSubgroups(currentEditItem, loading: false);
					int index2 = IndexOf(currentEditItem);
					AdjustCurrencyForEdit(newCurrentItem, index2);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, currentEditItem, index2));
				}
				else if (PageSize > 0)
				{
					int index3 = -1;
					if (flag && num4 < num2)
					{
						index3 = num2;
					}
					else if (!OnLastLocalPage && num >= 0)
					{
						index3 = num3 - 1;
					}
					object obj = _temporaryGroup.LeafAt(index3);
					if (obj != null)
					{
						_group.AddToSubgroups(obj, loading: false);
						index3 = IndexOf(obj);
						AdjustCurrencyForEdit(newCurrentItem, index3);
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, index3));
					}
				}
			}
			else
			{
				num5 = IndexOf(currentEditItem);
				if (num5 >= 0)
				{
					AdjustCurrencyForEdit(newCurrentItem, num5);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, currentEditItem, num5));
				}
				else if (PageSize > 0)
				{
					if (PassesFilter(currentEditItem))
					{
						num6 = ((InternalIndexOf(currentEditItem) < ConvertToInternalIndex(0)) ? 1 : 0);
						if (num6 != 0)
						{
							num7 = 0;
							goto IL_0282;
						}
					}
					else
					{
						num6 = 0;
					}
					num7 = Count - 1;
					goto IL_0282;
				}
			}
			goto IL_02b2;
		}
		if (!Contains(currentEditItem))
		{
			InternalList.Add(currentEditItem);
		}
		return;
		IL_0282:
		num5 = num7;
		if (num6 != 0 || (!OnLastLocalPage && num >= 0))
		{
			AdjustCurrencyForEdit(newCurrentItem, num5);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetItemAt(num5), num5));
		}
		goto IL_02b2;
		IL_02b2:
		RaiseCurrencyChanges(fireChangedEvent: true, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
	}

	public void CommitNew()
	{
		if (IsEditingItem)
		{
			throw new InvalidOperationException(GetOperationNotAllowedText("CommitNew", "EditItem"));
		}
		VerifyRefreshNotDeferred();
		if (CurrentAddItem == null)
		{
			return;
		}
		object obj = EndAddNew(cancel: false);
		object currentItem = CurrentItem;
		_trackingEnumerator = _sourceCollection.GetEnumerator();
		if (!UsesLocalArray)
		{
			return;
		}
		int index = Count - 1;
		int index2 = _internalList.IndexOf(obj);
		_internalList.Remove(obj);
		if (IsGrouping)
		{
			_group.RemoveSpecialItem(_group.Items.Count - 1, obj, loading: false);
			if (PageSize > 0)
			{
				_temporaryGroup.RemoveSpecialItem(_temporaryGroup.Items.Count - 1, obj, loading: false);
			}
		}
		object currentItem2 = CurrentItem;
		int currentPosition = CurrentPosition;
		bool isCurrentAfterLast = IsCurrentAfterLast;
		bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
		AdjustCurrencyForRemove(index);
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, index));
		bool flag = PassesFilter(obj);
		ProcessInsertToCollection(obj, index2);
		int num = PageIndex * PageSize;
		int num2 = num + PageSize;
		if (IsGrouping)
		{
			int num3 = -1;
			if (flag && PageSize > 0)
			{
				_temporaryGroup.AddToSubgroups(obj, loading: false);
				num3 = _temporaryGroup.LeafIndexOf(obj);
			}
			if (flag && (PageSize == 0 || (num <= num3 && num2 > num3)))
			{
				_group.AddToSubgroups(obj, loading: false);
				int index3 = IndexOf(obj);
				if (currentItem != null)
				{
					if (Contains(currentItem))
					{
						AdjustCurrencyForAdd(currentItem, index3);
					}
					else
					{
						AdjustCurrencyForAdd(GetItemAt(Count - 1), index3);
					}
				}
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, index3));
			}
			else if (!flag && (PageSize == 0 || OnLastLocalPage))
			{
				AdjustCurrencyForRemove(index);
			}
			else if (PageSize > 0)
			{
				int index4 = -1;
				if (flag && num3 < num)
				{
					index4 = num;
				}
				else if (!OnLastLocalPage)
				{
					index4 = num2 - 1;
				}
				object obj2 = _temporaryGroup.LeafAt(index4);
				if (obj2 != null)
				{
					_group.AddToSubgroups(obj2, loading: false);
					index4 = IndexOf(obj2);
					if (currentItem != null)
					{
						if (Contains(currentItem))
						{
							AdjustCurrencyForAdd(currentItem, index4);
						}
						else
						{
							AdjustCurrencyForAdd(GetItemAt(Count - 1), index4);
						}
					}
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj2, index4));
				}
			}
		}
		else
		{
			int num4 = IndexOf(obj);
			if (num4 >= 0)
			{
				AdjustCurrencyForAdd(obj, num4);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, num4));
			}
			else if (!flag && (PageSize == 0 || OnLastLocalPage))
			{
				AdjustCurrencyForRemove(index);
			}
			else if (PageSize > 0)
			{
				bool num5 = InternalIndexOf(obj) < ConvertToInternalIndex(0);
				num4 = ((!num5) ? (Count - 1) : 0);
				if (num5 || !OnLastLocalPage)
				{
					AdjustCurrencyForAdd(null, num4);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetItemAt(num4), num4));
				}
			}
		}
		RaiseCurrencyChanges(fireChangedEvent: true, currentItem2, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
	}

	public bool Contains(object item)
	{
		EnsureCollectionInSync();
		VerifyRefreshNotDeferred();
		return IndexOf(item) >= 0;
	}

	public IDisposable DeferRefresh()
	{
		if (IsAddingNew || IsEditingItem)
		{
			throw new InvalidOperationException(GetOperationNotAllowedDuringAddOrEditText("DeferRefresh"));
		}
		_deferLevel++;
		return new DeferHelper(this);
	}

	public void EditItem(object item)
	{
		VerifyRefreshNotDeferred();
		if (IsAddingNew)
		{
			if (object.Equals(item, CurrentAddItem))
			{
				return;
			}
			CommitNew();
		}
		CommitEdit();
		CurrentEditItem = item;
		if (item is IEditableObject editableObject)
		{
			editableObject.BeginEdit();
		}
	}

	public IEnumerator GetEnumerator()
	{
		EnsureCollectionInSync();
		VerifyRefreshNotDeferred();
		if (IsGrouping)
		{
			return RootGroup?.GetLeafEnumerator();
		}
		if (PageSize > 0)
		{
			List<object> list = new List<object>();
			if (PageIndex < 0)
			{
				return list.GetEnumerator();
			}
			for (int i = _pageSize * PageIndex; i < Math.Min(_pageSize * (PageIndex + 1), InternalList.Count); i++)
			{
				list.Add(InternalList[i]);
			}
			return new NewItemAwareEnumerator(this, list.GetEnumerator(), CurrentAddItem);
		}
		return new NewItemAwareEnumerator(this, InternalList.GetEnumerator(), CurrentAddItem);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public object GetItemAt(int index)
	{
		EnsureCollectionInSync();
		VerifyRefreshNotDeferred();
		if (index >= Count || index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (IsGrouping)
		{
			return RootGroup?.LeafAt(_isUsingTemporaryGroup ? ConvertToInternalIndex(index) : index);
		}
		if (IsAddingNew && UsesLocalArray && index == Count - 1)
		{
			return CurrentAddItem;
		}
		return InternalItemAt(ConvertToInternalIndex(index));
	}

	public int IndexOf(object item)
	{
		EnsureCollectionInSync();
		VerifyRefreshNotDeferred();
		if (IsGrouping)
		{
			return RootGroup?.LeafIndexOf(item) ?? (-1);
		}
		if (IsAddingNew && object.Equals(item, CurrentAddItem) && UsesLocalArray)
		{
			return Count - 1;
		}
		int num = InternalIndexOf(item);
		if (PageSize > 0 && num != -1)
		{
			if (num >= PageIndex * _pageSize && num < (PageIndex + 1) * _pageSize)
			{
				return num - PageIndex * _pageSize;
			}
			return -1;
		}
		return num;
	}

	public bool MoveCurrentTo(object item)
	{
		VerifyRefreshNotDeferred();
		if (object.Equals(CurrentItem, item) && (item != null || IsCurrentInView))
		{
			return IsCurrentInView;
		}
		return MoveCurrentToPosition(IndexOf(item));
	}

	public bool MoveCurrentToFirst()
	{
		VerifyRefreshNotDeferred();
		return MoveCurrentToPosition(0);
	}

	public bool MoveCurrentToLast()
	{
		VerifyRefreshNotDeferred();
		int position = Count - 1;
		return MoveCurrentToPosition(position);
	}

	public bool MoveCurrentToNext()
	{
		VerifyRefreshNotDeferred();
		int num = CurrentPosition + 1;
		if (num <= Count)
		{
			return MoveCurrentToPosition(num);
		}
		return false;
	}

	public bool MoveCurrentToPosition(int position)
	{
		VerifyRefreshNotDeferred();
		if (position < -1 || position > Count)
		{
			throw new ArgumentOutOfRangeException("position");
		}
		if ((position != CurrentPosition || !IsCurrentInSync) && OkToChangeCurrent())
		{
			bool isCurrentAfterLast = IsCurrentAfterLast;
			bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
			SetCurrentToPosition(position);
			OnCurrentChanged();
			if (IsCurrentAfterLast != isCurrentAfterLast)
			{
				OnPropertyChanged("IsCurrentAfterLast");
			}
			if (IsCurrentBeforeFirst != isCurrentBeforeFirst)
			{
				OnPropertyChanged("IsCurrentBeforeFirst");
			}
			OnPropertyChanged("CurrentPosition");
			OnPropertyChanged("CurrentItem");
		}
		return IsCurrentInView;
	}

	public bool MoveCurrentToPrevious()
	{
		VerifyRefreshNotDeferred();
		int num = CurrentPosition - 1;
		if (num >= -1)
		{
			return MoveCurrentToPosition(num);
		}
		return false;
	}

	public bool MoveToFirstPage()
	{
		return MoveToPage(0);
	}

	public bool MoveToLastPage()
	{
		if (TotalItemCount != -1 && PageSize > 0)
		{
			return MoveToPage(PageCount - 1);
		}
		return false;
	}

	public bool MoveToNextPage()
	{
		return MoveToPage(_pageIndex + 1);
	}

	public bool MoveToPage(int pageIndex)
	{
		if (pageIndex < -1)
		{
			return false;
		}
		if (IsRefreshDeferred)
		{
			_cachedPageIndex = pageIndex;
			SetFlag(CollectionViewFlags.IsMoveToPageDeferred, value: true);
			return false;
		}
		if (pageIndex == -1 && PageSize > 0)
		{
			return false;
		}
		if (pageIndex >= PageCount || _pageIndex == pageIndex)
		{
			return false;
		}
		if (!OkToChangeCurrent())
		{
			return false;
		}
		if (RaisePageChanging(pageIndex) && pageIndex != -1)
		{
			return false;
		}
		if (CurrentAddItem != null || CurrentEditItem != null)
		{
			object currentItem = CurrentItem;
			int currentPosition = CurrentPosition;
			bool isCurrentAfterLast = IsCurrentAfterLast;
			bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
			SetCurrentToPosition(-1);
			RaiseCurrencyChanges(fireChangedEvent: true, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
			if (CurrentAddItem != null || CurrentEditItem != null)
			{
				RaisePageChanged();
				SetCurrentToPosition(currentPosition);
				RaiseCurrencyChanges(fireChangedEvent: false, null, -1, oldIsCurrentBeforeFirst: true, oldIsCurrentAfterLast: false);
				return false;
			}
			OnCurrentChanging();
		}
		IsPageChanging = true;
		CompletePageMove(pageIndex);
		return true;
	}

	public bool MoveToPreviousPage()
	{
		return MoveToPage(_pageIndex - 1);
	}

	public bool PassesFilter(object item)
	{
		if (Filter != null)
		{
			return Filter(item);
		}
		return true;
	}

	public void Refresh()
	{
		if (this != null && (((IDataGridEditableCollectionView)this).IsAddingNew || ((IDataGridEditableCollectionView)this).IsEditingItem))
		{
			throw new InvalidOperationException(GetOperationNotAllowedDuringAddOrEditText("Refresh"));
		}
		RefreshInternal();
	}

	public void Remove(object item)
	{
		int num = IndexOf(item);
		if (num >= 0)
		{
			RemoveAt(num);
		}
	}

	public void RemoveAt(int index)
	{
		if (index < 0 || index >= Count)
		{
			throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
		}
		if (IsEditingItem || IsAddingNew)
		{
			throw new InvalidOperationException(GetOperationNotAllowedDuringAddOrEditText("RemoveAt"));
		}
		if (!CanRemove)
		{
			throw new InvalidOperationException("Remove/RemoveAt is not supported.");
		}
		VerifyRefreshNotDeferred();
		object itemAt = GetItemAt(index);
		bool flag = PageSize > 0 && !OnLastLocalPage;
		try
		{
			SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, value: false);
			if (SourceList != null)
			{
				SourceList.Remove(itemAt);
			}
		}
		finally
		{
			SetFlag(CollectionViewFlags.ShouldProcessCollectionChanged, value: true);
		}
		_trackingEnumerator = _sourceCollection.GetEnumerator();
		_internalList.Remove(itemAt);
		if (IsGrouping)
		{
			if (PageSize > 0)
			{
				_temporaryGroup.RemoveFromSubgroups(itemAt);
			}
			_group.RemoveFromSubgroups(itemAt);
		}
		object currentItem = CurrentItem;
		int currentPosition = CurrentPosition;
		bool isCurrentAfterLast = IsCurrentAfterLast;
		bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
		AdjustCurrencyForRemove(index);
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemAt, index));
		RaiseCurrencyChanges(fireChangedEvent: false, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
		if (NeedToMoveToPreviousPage)
		{
			MoveToPreviousPage();
		}
		else
		{
			if (!flag)
			{
				return;
			}
			if (IsGrouping)
			{
				object obj = _temporaryGroup.LeafAt(PageSize * (PageIndex + 1) - 1);
				if (obj != null)
				{
					_group.AddToSubgroups(obj, loading: false);
				}
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetItemAt(PageSize - 1), PageSize - 1));
		}
	}

	private static object InvokePath(object item, string propertyPath, Type propertyType)
	{
		Exception exception;
		object nestedPropertyValue = TypeHelper.GetNestedPropertyValue(item, propertyPath, propertyType, out exception);
		if (exception != null)
		{
			throw exception;
		}
		return nestedPropertyValue;
	}

	private void AdjustCurrencyForAdd(object newCurrentItem, int index)
	{
		if (newCurrentItem != null)
		{
			int num = IndexOf(newCurrentItem);
			if (num >= 0 && (num != CurrentPosition || !IsCurrentInSync))
			{
				OnCurrentChanging();
				SetCurrent(newCurrentItem, num);
			}
		}
		else if (Count == 1)
		{
			if (CurrentItem != null || CurrentPosition != -1)
			{
				OnCurrentChanging();
			}
			SetCurrent(null, -1);
		}
		else if (index <= CurrentPosition)
		{
			OnCurrentChanging();
			int num2 = CurrentPosition + 1;
			if (num2 >= Count)
			{
				num2 = Count - 1;
			}
			SetCurrent(GetItemAt(num2), num2);
		}
	}

	private void AdjustCurrencyForEdit(object newCurrentItem, int index)
	{
		if (newCurrentItem != null && IndexOf(newCurrentItem) >= 0)
		{
			OnCurrentChanging();
			SetCurrent(newCurrentItem, IndexOf(newCurrentItem));
		}
		else if (index <= CurrentPosition)
		{
			OnCurrentChanging();
			int num = CurrentPosition + 1;
			if (num < Count)
			{
				SetCurrent(GetItemAt(num), num);
			}
			else
			{
				SetCurrent(null, Count);
			}
		}
	}

	private void AdjustCurrencyForRemove(int index)
	{
		if (index < CurrentPosition)
		{
			OnCurrentChanging();
			SetCurrent(CurrentItem, CurrentPosition - 1);
		}
		if (CurrentPosition >= Count)
		{
			OnCurrentChanging();
			SetCurrentToPosition(Count - 1);
		}
		if (!IsCurrentInSync)
		{
			OnCurrentChanging();
			SetCurrentToPosition(CurrentPosition);
		}
	}

	private bool CheckFlag(CollectionViewFlags flags)
	{
		return _flags.HasAllFlags(flags);
	}

	private void CompletePageMove(int pageIndex)
	{
		int count = Count;
		object currentItem = CurrentItem;
		int currentPosition = CurrentPosition;
		bool isCurrentAfterLast = IsCurrentAfterLast;
		bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
		_pageIndex = pageIndex;
		if (IsGrouping && PageSize > 0)
		{
			PrepareGroupsForCurrentPage();
		}
		if (Count >= 1)
		{
			SetCurrent(GetItemAt(0), 0);
		}
		else
		{
			SetCurrent(null, -1);
		}
		IsPageChanging = false;
		OnPropertyChanged("PageIndex");
		RaisePageChanged();
		if (Count != count)
		{
			OnPropertyChanged("Count");
		}
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		RaiseCurrencyChanges(fireChangedEvent: true, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
	}

	private int ConvertToInternalIndex(int index)
	{
		if (PageSize > 0)
		{
			return _pageSize * PageIndex + index;
		}
		return index;
	}

	private void CopySourceToInternalList()
	{
		_internalList = new List<object>();
		IEnumerator enumerator = SourceCollection.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_internalList.Add(enumerator.Current);
		}
	}

	private object EndAddNew(bool cancel)
	{
		object currentAddItem = CurrentAddItem;
		CurrentAddItem = null;
		if (currentAddItem is IEditableObject editableObject)
		{
			if (cancel)
			{
				editableObject.CancelEdit();
				return currentAddItem;
			}
			editableObject.EndEdit();
		}
		return currentAddItem;
	}

	private void EndDefer()
	{
		_deferLevel--;
		if (_deferLevel == 0)
		{
			if (CheckFlag(CollectionViewFlags.IsUpdatePageSizeDeferred))
			{
				SetFlag(CollectionViewFlags.IsUpdatePageSizeDeferred, value: false);
				PageSize = _cachedPageSize;
			}
			if (CheckFlag(CollectionViewFlags.IsMoveToPageDeferred))
			{
				SetFlag(CollectionViewFlags.IsMoveToPageDeferred, value: false);
				MoveToPage(_cachedPageIndex);
				_cachedPageIndex = -1;
			}
			if (CheckFlag(CollectionViewFlags.NeedsRefresh))
			{
				Refresh();
			}
		}
	}

	private void EnsureItemConstructor()
	{
		if (!_itemConstructorIsValid)
		{
			Type itemType = ItemType;
			if (itemType != null)
			{
				_itemConstructor = itemType.GetConstructor(Type.EmptyTypes);
				_itemConstructorIsValid = true;
			}
		}
	}

	private void EnsureCollectionInSync()
	{
		if (_pollForChanges)
		{
			try
			{
				_trackingEnumerator.MoveNext();
			}
			catch (InvalidOperationException)
			{
				_trackingEnumerator = SourceCollection.GetEnumerator();
				RefreshOrDefer();
			}
		}
	}

	private Type GetItemType(bool useRepresentativeItem)
	{
		Type[] interfaces = SourceCollection.GetType().GetInterfaces();
		foreach (Type type in interfaces)
		{
			if (type.Name == typeof(IEnumerable<>).Name)
			{
				Type[] genericArguments = type.GetGenericArguments();
				if (genericArguments.Length == 1)
				{
					return genericArguments[0];
				}
			}
		}
		if (useRepresentativeItem)
		{
			object representativeItem = GetRepresentativeItem();
			if (representativeItem != null)
			{
				return representativeItem.GetType();
			}
		}
		return null;
	}

	private object GetRepresentativeItem()
	{
		if (IsEmpty)
		{
			return null;
		}
		IEnumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			object current = enumerator.Current;
			if (current != null)
			{
				return current;
			}
		}
		return null;
	}

	private int InternalIndexOf(object item)
	{
		return InternalList.IndexOf(item);
	}

	private object InternalItemAt(int index)
	{
		if (index >= 0 && index < InternalList.Count)
		{
			return InternalList[index];
		}
		return null;
	}

	private bool OkToChangeCurrent()
	{
		DataGridCurrentChangingEventArgs dataGridCurrentChangingEventArgs = new DataGridCurrentChangingEventArgs();
		OnCurrentChanging(dataGridCurrentChangingEventArgs);
		return !dataGridCurrentChangingEventArgs.Cancel;
	}

	private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
	{
		if (args == null)
		{
			throw new ArgumentNullException("args");
		}
		_timestamp++;
		if (this.CollectionChanged != null && (args.Action != 0 || PageSize == 0 || args.NewStartingIndex < Count))
		{
			this.CollectionChanged(this, args);
		}
		if (args.Action != NotifyCollectionChangedAction.Replace)
		{
			OnPropertyChanged("Count");
		}
		bool isEmpty = IsEmpty;
		if (isEmpty != CheckFlag(CollectionViewFlags.CachedIsEmpty))
		{
			SetFlag(CollectionViewFlags.CachedIsEmpty, isEmpty);
			OnPropertyChanged("IsEmpty");
		}
	}

	private void OnCurrentChanged()
	{
		if (this.CurrentChanged != null && _currentChangedMonitor.Enter())
		{
			using (_currentChangedMonitor)
			{
				this.CurrentChanged(this, EventArgs.Empty);
			}
		}
	}

	private void OnCurrentChanging()
	{
		OnCurrentChanging(uncancelableCurrentChangingEventArgs);
	}

	private void OnCurrentChanging(DataGridCurrentChangingEventArgs args)
	{
		if (args == null)
		{
			throw new ArgumentNullException("args");
		}
		if (_currentChangedMonitor.Busy)
		{
			if (args.IsCancelable)
			{
				args.Cancel = true;
			}
		}
		else
		{
			this.CurrentChanging?.Invoke(this, args);
		}
	}

	private void OnGroupByChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (IsAddingNew || IsEditingItem)
		{
			throw new InvalidOperationException(GetOperationNotAllowedDuringAddOrEditText("Grouping"));
		}
		RefreshOrDefer();
	}

	private void OnGroupDescriptionChanged(object sender, EventArgs e)
	{
		if (IsAddingNew || IsEditingItem)
		{
			throw new InvalidOperationException(GetOperationNotAllowedDuringAddOrEditText("Grouping"));
		}
		RefreshOrDefer();
		if (PageSize > 0)
		{
			if (IsRefreshDeferred)
			{
				_cachedPageIndex = 0;
				SetFlag(CollectionViewFlags.IsMoveToPageDeferred, value: true);
			}
			else
			{
				MoveToFirstPage();
			}
		}
	}

	private void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		this.PropertyChanged?.Invoke(this, e);
	}

	private void OnPropertyChanged(string propertyName)
	{
		OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
	}

	private void PrepareGroupingComparer(CollectionViewGroupRoot groupRoot)
	{
		if (groupRoot == _temporaryGroup || PageSize == 0)
		{
			if (groupRoot.ActiveComparer is DataGridCollectionViewGroupInternal.ListComparer listComparer)
			{
				listComparer.ResetList(InternalList);
			}
			else
			{
				groupRoot.ActiveComparer = new DataGridCollectionViewGroupInternal.ListComparer(InternalList);
			}
		}
		else if (groupRoot == _group)
		{
			groupRoot.ActiveComparer = new DataGridCollectionViewGroupInternal.CollectionViewGroupComparer(_temporaryGroup);
		}
	}

	private void PrepareGroups()
	{
		_group.Clear();
		_group.Initialize();
		_group.IsDataInGroupOrder = CheckFlag(CollectionViewFlags.IsDataInGroupOrder);
		_isGrouping = false;
		if (_group.GroupDescriptions.Count > 0)
		{
			int i = 0;
			for (int count = _internalList.Count; i < count; i++)
			{
				object obj = _internalList[i];
				if (obj != null && (!IsAddingNew || !object.Equals(CurrentAddItem, obj)))
				{
					_group.AddToSubgroups(obj, loading: true);
				}
			}
			if (IsAddingNew)
			{
				_group.InsertSpecialItem(_group.Items.Count, CurrentAddItem, loading: true);
			}
		}
		_isGrouping = _group.GroupBy != null;
		_group.IsDataInGroupOrder = false;
		PrepareGroupingComparer(_group);
	}

	private void PrepareTemporaryGroups()
	{
		_temporaryGroup = new CollectionViewGroupRoot(this, CheckFlag(CollectionViewFlags.IsDataInGroupOrder));
		foreach (DataGridGroupDescription groupDescription in _group.GroupDescriptions)
		{
			_temporaryGroup.GroupDescriptions.Add(groupDescription);
		}
		_temporaryGroup.Initialize();
		_isGrouping = false;
		if (_temporaryGroup.GroupDescriptions.Count > 0)
		{
			int i = 0;
			for (int count = _internalList.Count; i < count; i++)
			{
				object obj = _internalList[i];
				if (obj != null && (!IsAddingNew || !object.Equals(CurrentAddItem, obj)))
				{
					_temporaryGroup.AddToSubgroups(obj, loading: true);
				}
			}
			if (IsAddingNew)
			{
				_temporaryGroup.InsertSpecialItem(_temporaryGroup.Items.Count, CurrentAddItem, loading: true);
			}
		}
		_isGrouping = _temporaryGroup.GroupBy != null;
		PrepareGroupingComparer(_temporaryGroup);
	}

	private void PrepareGroupsForCurrentPage()
	{
		_group.Clear();
		_group.Initialize();
		_isUsingTemporaryGroup = true;
		_group.IsDataInGroupOrder = true;
		_group.ActiveComparer = null;
		if (GroupDescriptions.Count > 0)
		{
			int i = 0;
			for (int count = Count; i < count; i++)
			{
				object itemAt = GetItemAt(i);
				if (itemAt != null && (!IsAddingNew || !object.Equals(CurrentAddItem, itemAt)))
				{
					_group.AddToSubgroups(itemAt, loading: true);
				}
			}
			if (IsAddingNew)
			{
				_group.InsertSpecialItem(_group.Items.Count, CurrentAddItem, loading: true);
			}
		}
		_isUsingTemporaryGroup = false;
		_group.IsDataInGroupOrder = false;
		PrepareGroupingComparer(_group);
		_isGrouping = _group.GroupBy != null;
	}

	private IList PrepareLocalArray(IEnumerable enumerable)
	{
		List<object> list = new List<object>();
		foreach (object item in enumerable)
		{
			if (Filter == null || PassesFilter(item))
			{
				list.Add(item);
			}
		}
		if (!CheckFlag(CollectionViewFlags.IsDataSorted) && SortDescriptions.Count > 0)
		{
			list = SortList(list);
		}
		return list;
	}

	private void ProcessAddEvent(object addedItem, int addIndex)
	{
		object obj = null;
		if (PageSize > 0 && !IsGrouping)
		{
			obj = ((Count == PageSize) ? GetItemAt(PageSize - 1) : null);
		}
		ProcessInsertToCollection(addedItem, addIndex);
		bool flag = false;
		if (Count == 1 && GroupDescriptions.Count > 0)
		{
			if (PageSize > 0)
			{
				PrepareGroupingComparer(_temporaryGroup);
			}
			PrepareGroupingComparer(_group);
		}
		if (IsGrouping)
		{
			int num = -1;
			if (PageSize > 0)
			{
				_temporaryGroup.AddToSubgroups(addedItem, loading: false);
				num = _temporaryGroup.LeafIndexOf(addedItem);
			}
			if (PageSize == 0 || (PageIndex + 1) * PageSize > num)
			{
				flag = true;
				int num2 = PageIndex * PageSize;
				if (num2 > num && PageSize > 0)
				{
					addedItem = _temporaryGroup.LeafAt(num2);
				}
				if (PageSize > 0 && _group.ItemCount == PageSize)
				{
					obj = _group.LeafAt(PageSize - 1);
					_group.RemoveFromSubgroups(obj);
				}
			}
		}
		if (PageSize > 0 && !OnLastLocalPage && ((IsGrouping && obj != null) || (!IsGrouping && (PageIndex + 1) * PageSize > InternalIndexOf(addedItem))) && obj != null && obj != addedItem)
		{
			AdjustCurrencyForRemove(PageSize - 1);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, PageSize - 1));
		}
		if (flag)
		{
			_group.AddToSubgroups(addedItem, loading: false);
		}
		int num3 = IndexOf(addedItem);
		if (num3 >= 0)
		{
			object currentItem = CurrentItem;
			int currentPosition = CurrentPosition;
			bool isCurrentAfterLast = IsCurrentAfterLast;
			bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
			AdjustCurrencyForAdd(null, num3);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItem, num3));
			RaiseCurrencyChanges(fireChangedEvent: false, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
		}
		else if (PageSize > 0 && InternalIndexOf(addedItem) < ConvertToInternalIndex(0))
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetItemAt(0), 0));
		}
	}

	private void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
	{
		if (!CheckFlag(CollectionViewFlags.ShouldProcessCollectionChanged))
		{
			return;
		}
		if (args.Action == NotifyCollectionChangedAction.Reset)
		{
			if (!SourceCollection.GetEnumerator().MoveNext())
			{
				_internalList.Clear();
			}
			RefreshOrDefer();
			return;
		}
		if (args.OldItems != null && (args.Action == NotifyCollectionChangedAction.Remove || args.Action == NotifyCollectionChangedAction.Replace))
		{
			foreach (object oldItem in args.OldItems)
			{
				ProcessRemoveEvent(oldItem, args.Action == NotifyCollectionChangedAction.Replace);
			}
		}
		if (args.NewItems != null && (args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Replace))
		{
			for (int i = 0; i < args.NewItems.Count; i++)
			{
				if (Filter == null || PassesFilter(args.NewItems[i]))
				{
					ProcessAddEvent(args.NewItems[i], args.NewStartingIndex + i);
				}
			}
		}
		if (args.Action != NotifyCollectionChangedAction.Replace)
		{
			OnPropertyChanged("ItemCount");
		}
	}

	private void ProcessRemoveEvent(object removedItem, bool isReplace)
	{
		int num = -1;
		num = ((!IsGrouping) ? InternalIndexOf(removedItem) : ((PageSize > 0) ? _temporaryGroup.LeafIndexOf(removedItem) : _group.LeafIndexOf(removedItem)));
		int num2 = IndexOf(removedItem);
		_internalList.Remove(removedItem);
		bool flag = (PageSize == 0 && num2 >= 0) || num < (PageIndex + 1) * PageSize;
		if (IsGrouping)
		{
			if (PageSize > 0)
			{
				_temporaryGroup.RemoveFromSubgroups(removedItem);
			}
			if (flag)
			{
				_group.RemoveFromSubgroups((num2 >= 0) ? removedItem : _group.LeafAt(0));
			}
		}
		if (!flag)
		{
			return;
		}
		object currentItem = CurrentItem;
		int currentPosition = CurrentPosition;
		bool isCurrentAfterLast = IsCurrentAfterLast;
		bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
		AdjustCurrencyForRemove(num2);
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, Math.Max(0, num2)));
		RaiseCurrencyChanges(fireChangedEvent: false, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
		if (NeedToMoveToPreviousPage && !isReplace)
		{
			MoveToPreviousPage();
		}
		else
		{
			if (PageSize <= 0 || Count != PageSize)
			{
				return;
			}
			if (IsGrouping)
			{
				object obj = _temporaryGroup.LeafAt(PageSize * (PageIndex + 1) - 1);
				if (obj != null)
				{
					_group.AddToSubgroups(obj, loading: false);
				}
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetItemAt(PageSize - 1), PageSize - 1));
		}
	}

	private void ProcessInsertToCollection(object item, int index)
	{
		if (Filter != null && !PassesFilter(item))
		{
			return;
		}
		if (SortDescriptions.Count > 0)
		{
			Type itemType = ItemType;
			foreach (DataGridSortDescription sortDescription in SortDescriptions)
			{
				sortDescription.Initialize(itemType);
			}
			MergedComparer mergedComparer = new MergedComparer(this);
			if (index < 0 || (index > 0 && mergedComparer.Compare(item, InternalItemAt(index - 1)) < 0) || (index < InternalList.Count - 1 && mergedComparer.Compare(item, InternalItemAt(index)) > 0))
			{
				index = mergedComparer.FindInsertIndex(item, _internalList);
			}
		}
		if (index < 0 || index > _internalList.Count)
		{
			index = _internalList.Count;
		}
		_internalList.Insert(index, item);
	}

	private void RaiseCurrencyChanges(bool fireChangedEvent, object oldCurrentItem, int oldCurrentPosition, bool oldIsCurrentBeforeFirst, bool oldIsCurrentAfterLast)
	{
		if (fireChangedEvent || CurrentItem != oldCurrentItem || CurrentPosition != oldCurrentPosition)
		{
			OnCurrentChanged();
		}
		if (CurrentItem != oldCurrentItem)
		{
			OnPropertyChanged("CurrentItem");
		}
		if (CurrentPosition != oldCurrentPosition)
		{
			OnPropertyChanged("CurrentPosition");
		}
		if (IsCurrentAfterLast != oldIsCurrentAfterLast)
		{
			OnPropertyChanged("IsCurrentAfterLast");
		}
		if (IsCurrentBeforeFirst != oldIsCurrentBeforeFirst)
		{
			OnPropertyChanged("IsCurrentBeforeFirst");
		}
	}

	private void RaisePageChanged()
	{
		this.PageChanged?.Invoke(this, EventArgs.Empty);
	}

	private bool RaisePageChanging(int newPageIndex)
	{
		EventHandler<PageChangingEventArgs> eventHandler = this.PageChanging;
		if (eventHandler != null)
		{
			PageChangingEventArgs pageChangingEventArgs = new PageChangingEventArgs(newPageIndex);
			eventHandler(this, pageChangingEventArgs);
			return pageChangingEventArgs.Cancel;
		}
		return false;
	}

	private void RefreshInternal()
	{
		RefreshOverride();
		SetFlag(CollectionViewFlags.NeedsRefresh, value: false);
	}

	private void RefreshOrDefer()
	{
		if (IsRefreshDeferred)
		{
			SetFlag(CollectionViewFlags.NeedsRefresh, value: true);
		}
		else
		{
			RefreshInternal();
		}
	}

	private void RefreshOverride()
	{
		object currentItem = CurrentItem;
		int currentPosition = CurrentPosition;
		bool isCurrentAfterLast = IsCurrentAfterLast;
		bool isCurrentBeforeFirst = IsCurrentBeforeFirst;
		_isGrouping = false;
		OnCurrentChanging();
		if (UsesLocalArray)
		{
			try
			{
				_internalList = PrepareLocalArray(_sourceCollection);
				if (PageSize == 0)
				{
					PrepareGroups();
				}
				else
				{
					PrepareTemporaryGroups();
					PrepareGroupsForCurrentPage();
				}
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw;
			}
		}
		else
		{
			CopySourceToInternalList();
		}
		if (PageSize > 0 && PageIndex > 0 && PageIndex >= PageCount)
		{
			MoveToPage(PageCount - 1);
		}
		ResetCurrencyValues(currentItem, isCurrentBeforeFirst, isCurrentAfterLast);
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		RaiseCurrencyChanges(fireChangedEvent: false, currentItem, currentPosition, isCurrentBeforeFirst, isCurrentAfterLast);
	}

	private void ResetCurrencyValues(object oldCurrentItem, bool oldIsCurrentBeforeFirst, bool oldIsCurrentAfterLast)
	{
		if (oldIsCurrentBeforeFirst || IsEmpty)
		{
			SetCurrent(null, -1);
			return;
		}
		if (oldIsCurrentAfterLast)
		{
			SetCurrent(null, Count);
			return;
		}
		int num = IndexOf(oldCurrentItem);
		if (num < 0)
		{
			num = 0;
			if (num < Count)
			{
				SetCurrent(GetItemAt(num), num);
			}
			else if (!IsEmpty)
			{
				SetCurrent(GetItemAt(0), 0);
			}
			else
			{
				SetCurrent(null, -1);
			}
		}
		else
		{
			SetCurrent(oldCurrentItem, num);
		}
	}

	private void SetCurrent(object newItem, int newPosition)
	{
		int count = ((newItem == null) ? ((!IsEmpty) ? Count : 0) : 0);
		SetCurrent(newItem, newPosition, count);
	}

	private void SetCurrent(object newItem, int newPosition, int count)
	{
		if (newItem != null)
		{
			SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, value: false);
			SetFlag(CollectionViewFlags.IsCurrentAfterLast, value: false);
		}
		else if (count == 0)
		{
			SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, value: true);
			SetFlag(CollectionViewFlags.IsCurrentAfterLast, value: true);
			newPosition = -1;
		}
		else
		{
			SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, newPosition < 0);
			SetFlag(CollectionViewFlags.IsCurrentAfterLast, newPosition >= count);
		}
		_currentItem = newItem;
		_currentPosition = newPosition;
	}

	private void SetCurrentToPosition(int position)
	{
		if (position < 0)
		{
			SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, value: true);
			SetCurrent(null, -1);
		}
		else if (position >= Count)
		{
			SetFlag(CollectionViewFlags.IsCurrentAfterLast, value: true);
			SetCurrent(null, Count);
		}
		else
		{
			SetFlag(CollectionViewFlags.IsCurrentBeforeFirst | CollectionViewFlags.IsCurrentAfterLast, value: false);
			SetCurrent(GetItemAt(position), position);
		}
	}

	private void SetFlag(CollectionViewFlags flags, bool value)
	{
		if (value)
		{
			_flags |= flags;
		}
		else
		{
			_flags &= ~flags;
		}
	}

	private void SetSortDescriptions(DataGridSortDescriptionCollection descriptions)
	{
		if (_sortDescriptions != null)
		{
			_sortDescriptions.CollectionChanged -= SortDescriptionsChanged;
		}
		_sortDescriptions = descriptions;
		if (_sortDescriptions != null)
		{
			_sortDescriptions.CollectionChanged += SortDescriptionsChanged;
		}
	}

	private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (IsAddingNew || IsEditingItem)
		{
			throw new InvalidOperationException(GetOperationNotAllowedDuringAddOrEditText("Sorting"));
		}
		RefreshOrDefer();
		if (PageSize > 0)
		{
			if (IsRefreshDeferred)
			{
				_cachedPageIndex = 0;
				SetFlag(CollectionViewFlags.IsMoveToPageDeferred, value: true);
			}
			else
			{
				MoveToFirstPage();
			}
		}
		OnPropertyChanged("SortDescriptions");
	}

	private List<object> SortList(List<object> list)
	{
		IEnumerable<object> enumerable = list;
		new CultureSensitiveComparer(Culture);
		Type itemType = ItemType;
		foreach (DataGridSortDescription sortDescription in SortDescriptions)
		{
			sortDescription.Initialize(itemType);
			enumerable = ((!(enumerable is IOrderedEnumerable<object> seq)) ? sortDescription.OrderBy(enumerable) : sortDescription.ThenBy(seq));
		}
		return enumerable.ToList();
	}

	private void VerifyRefreshNotDeferred()
	{
		if (IsRefreshDeferred)
		{
			throw new InvalidOperationException("Cannot change or check the contents or current position of the CollectionView while Refresh is being deferred.");
		}
	}
}
