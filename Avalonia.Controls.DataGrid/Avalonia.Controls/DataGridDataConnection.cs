using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Avalonia.Collections;
using Avalonia.Controls.Utils;

namespace Avalonia.Controls;

internal class DataGridDataConnection
{
	private int _backupSlotForCurrentChanged;

	private int _columnForCurrentChanged;

	private PropertyInfo[] _dataProperties;

	private IEnumerable _dataSource;

	private Type _dataType;

	private bool _expectingCurrentChanged;

	private object _itemToSelectOnCurrentChanged;

	private DataGrid _owner;

	private bool _scrollForCurrentChanged;

	private DataGridSelectionAction _selectionActionForCurrentChanged;

	public bool AllowEdit
	{
		get
		{
			if (List == null)
			{
				return true;
			}
			return !List.IsReadOnly;
		}
	}

	public bool AllowSort
	{
		get
		{
			if (CollectionView == null || (EditableCollectionView != null && (EditableCollectionView.IsAddingNew || EditableCollectionView.IsEditingItem)))
			{
				return false;
			}
			return CollectionView.CanSort;
		}
	}

	public bool CommittingEdit { get; private set; }

	public int Count
	{
		get
		{
			if (!TryGetCount(allowSlow: true, getAny: false, out var count))
			{
				return 0;
			}
			return count;
		}
	}

	public bool DataIsPrimitive => DataTypeIsPrimitive(DataType);

	public PropertyInfo[] DataProperties
	{
		get
		{
			if (_dataProperties == null)
			{
				UpdateDataProperties();
			}
			return _dataProperties;
		}
	}

	public IEnumerable DataSource
	{
		get
		{
			return _dataSource;
		}
		set
		{
			_dataSource = value;
			_dataType = null;
			UpdateDataProperties();
		}
	}

	public Type DataType
	{
		get
		{
			if (_dataType == null && _owner.ItemsSource != null)
			{
				_dataType = _owner.ItemsSource.GetItemType();
			}
			return _dataType;
		}
	}

	public bool EventsWired { get; private set; }

	private bool IsGrouping
	{
		get
		{
			if (CollectionView != null && CollectionView.CanGroup && CollectionView.IsGrouping)
			{
				return CollectionView.GroupingDepth > 0;
			}
			return false;
		}
	}

	public IList List => DataSource as IList;

	public bool ShouldAutoGenerateColumns => false;

	public IDataGridCollectionView CollectionView => DataSource as IDataGridCollectionView;

	public IDataGridEditableCollectionView EditableCollectionView => DataSource as IDataGridEditableCollectionView;

	public DataGridSortDescriptionCollection SortDescriptions
	{
		get
		{
			if (CollectionView != null && CollectionView.CanSort)
			{
				return CollectionView.SortDescriptions;
			}
			return null;
		}
	}

	public DataGridDataConnection(DataGrid owner)
	{
		_owner = owner;
	}

	internal bool TryGetCount(bool allowSlow, bool getAny, out int count)
	{
		IEnumerable dataSource = DataSource;
		(bool, int) tuple;
		if (!(dataSource is ICollection collection))
		{
			if (!(dataSource is DataGridCollectionView dataGridCollectionView))
			{
				if (dataSource == null)
				{
					goto IL_007b;
				}
				if (allowSlow && !getAny)
				{
					tuple = (true, dataSource.Cast<object>().Count());
				}
				else
				{
					IEnumerable source = dataSource;
					if (!getAny)
					{
						goto IL_007b;
					}
					tuple = (true, source.Cast<object>().Any() ? 1 : 0);
				}
			}
			else
			{
				tuple = (true, dataGridCollectionView.Count);
			}
		}
		else
		{
			tuple = (true, collection.Count);
		}
		goto IL_0084;
		IL_007b:
		tuple = (false, 0);
		goto IL_0084;
		IL_0084:
		bool result;
		(result, count) = tuple;
		return result;
	}

	internal bool Any()
	{
		if (TryGetCount(allowSlow: false, getAny: true, out var count))
		{
			return count > 0;
		}
		return false;
	}

	public bool BeginEdit(object dataItem)
	{
		if (dataItem == null)
		{
			return false;
		}
		IDataGridEditableCollectionView editableCollectionView = EditableCollectionView;
		if (editableCollectionView != null)
		{
			if (editableCollectionView.IsEditingItem && dataItem == editableCollectionView.CurrentEditItem)
			{
				return true;
			}
			editableCollectionView.EditItem(dataItem);
			if (!editableCollectionView.IsEditingItem)
			{
				return editableCollectionView.IsAddingNew;
			}
			return true;
		}
		if (dataItem is IEditableObject editableObject)
		{
			editableObject.BeginEdit();
			return true;
		}
		return true;
	}

	public bool CancelEdit(object dataItem)
	{
		IDataGridEditableCollectionView editableCollectionView = EditableCollectionView;
		if (editableCollectionView != null)
		{
			if (editableCollectionView.CanCancelEdit)
			{
				editableCollectionView.CancelEdit();
				return true;
			}
			return false;
		}
		if (dataItem is IEditableObject editableObject)
		{
			editableObject.CancelEdit();
			return true;
		}
		return true;
	}

	public static bool CanEdit(Type type)
	{
		type = type.GetNonNullableType();
		if (!type.IsEnum && !(type == typeof(string)) && !(type == typeof(char)) && !(type == typeof(DateTime)) && !(type == typeof(bool)) && !(type == typeof(byte)) && !(type == typeof(sbyte)) && !(type == typeof(float)) && !(type == typeof(double)) && !(type == typeof(decimal)) && !(type == typeof(short)) && !(type == typeof(int)) && !(type == typeof(long)) && !(type == typeof(ushort)) && !(type == typeof(uint)))
		{
			return type == typeof(ulong);
		}
		return true;
	}

	public bool EndEdit(object dataItem)
	{
		IDataGridEditableCollectionView editableCollectionView = EditableCollectionView;
		if (editableCollectionView != null)
		{
			_owner.NoCurrentCellChangeCount++;
			CommittingEdit = true;
			try
			{
				if (editableCollectionView.IsAddingNew)
				{
					editableCollectionView.CommitNew();
				}
				else
				{
					editableCollectionView.CommitEdit();
				}
			}
			finally
			{
				_owner.NoCurrentCellChangeCount--;
				CommittingEdit = false;
			}
			return true;
		}
		if (dataItem is IEditableObject editableObject)
		{
			editableObject.EndEdit();
		}
		return true;
	}

	public object GetDataItem(int index)
	{
		IList list = List;
		if (list != null)
		{
			if (index >= list.Count)
			{
				return null;
			}
			return list[index];
		}
		if (DataSource is DataGridCollectionView dataGridCollectionView)
		{
			if (index >= dataGridCollectionView.Count)
			{
				return null;
			}
			return dataGridCollectionView.GetItemAt(index);
		}
		IEnumerable dataSource = DataSource;
		if (dataSource != null)
		{
			IEnumerator enumerator = dataSource.GetEnumerator();
			int num = -1;
			while (enumerator.MoveNext() && num < index)
			{
				num++;
				if (num == index)
				{
					return enumerator.Current;
				}
			}
		}
		return null;
	}

	public bool GetPropertyIsReadOnly(string propertyName)
	{
		if (DataType != null)
		{
			if (!string.IsNullOrEmpty(propertyName))
			{
				Type type = DataType;
				PropertyInfo propertyInfo = null;
				List<string> list = TypeHelper.SplitPropertyPath(propertyName);
				for (int i = 0; i < list.Count; i++)
				{
					propertyInfo = type.GetPropertyOrIndexer(list[i], out var _);
					if (propertyInfo == null || type.GetIsReadOnly() || propertyInfo.GetIsReadOnly())
					{
						return true;
					}
					object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(EditableAttribute), inherit: true);
					if (customAttributes != null && customAttributes.Length != 0 && !((EditableAttribute)customAttributes[0]).AllowEdit)
					{
						return true;
					}
					type = propertyInfo.PropertyType.GetNonNullableType();
				}
				if (!(propertyInfo == null) && propertyInfo.CanWrite && AllowEdit)
				{
					return !CanEdit(type);
				}
				return true;
			}
			if (DataType.GetIsReadOnly())
			{
				return true;
			}
		}
		return !AllowEdit;
	}

	public int IndexOf(object dataItem)
	{
		IList list = List;
		if (list != null)
		{
			return list.IndexOf(dataItem);
		}
		if (DataSource is DataGridCollectionView dataGridCollectionView)
		{
			return dataGridCollectionView.IndexOf(dataItem);
		}
		IEnumerable dataSource = DataSource;
		if (dataSource != null && dataItem != null)
		{
			int num = 0;
			foreach (object item in dataSource)
			{
				if ((dataItem == null && item == null) || dataItem.Equals(item))
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	internal void ClearDataProperties()
	{
		_dataProperties = null;
	}

	internal static IDataGridCollectionView CreateView(IEnumerable source)
	{
		IDataGridCollectionView dataGridCollectionView = null;
		if (source is IDataGridCollectionViewFactory dataGridCollectionViewFactory)
		{
			dataGridCollectionView = dataGridCollectionViewFactory.CreateView();
		}
		if (dataGridCollectionView == null)
		{
			dataGridCollectionView = new DataGridCollectionView(source);
		}
		return dataGridCollectionView;
	}

	internal static bool DataTypeIsPrimitive(Type dataType)
	{
		if (dataType != null)
		{
			Type nonNullableType = dataType.GetNonNullableType();
			if (!nonNullableType.IsPrimitive && !(nonNullableType == typeof(string)) && !(nonNullableType == typeof(DateTime)))
			{
				return nonNullableType == typeof(decimal);
			}
			return true;
		}
		return false;
	}

	internal void MoveCurrentTo(object item, int backupSlot, int columnIndex, DataGridSelectionAction action, bool scrollIntoView)
	{
		if (CollectionView != null)
		{
			_expectingCurrentChanged = true;
			_columnForCurrentChanged = columnIndex;
			_itemToSelectOnCurrentChanged = item;
			_selectionActionForCurrentChanged = action;
			_scrollForCurrentChanged = scrollIntoView;
			_backupSlotForCurrentChanged = backupSlot;
			CollectionView.MoveCurrentTo((item is DataGridCollectionViewGroup) ? null : item);
			_expectingCurrentChanged = false;
		}
	}

	internal void UnWireEvents(IEnumerable value)
	{
		if (value is INotifyCollectionChanged notifyCollectionChanged)
		{
			notifyCollectionChanged.CollectionChanged -= NotifyingDataSource_CollectionChanged;
		}
		if (SortDescriptions != null)
		{
			SortDescriptions.CollectionChanged -= CollectionView_SortDescriptions_CollectionChanged;
		}
		if (CollectionView != null)
		{
			CollectionView.CurrentChanged -= CollectionView_CurrentChanged;
			CollectionView.CurrentChanging -= CollectionView_CurrentChanging;
		}
		EventsWired = false;
	}

	internal void WireEvents(IEnumerable value)
	{
		if (value is INotifyCollectionChanged notifyCollectionChanged)
		{
			notifyCollectionChanged.CollectionChanged += NotifyingDataSource_CollectionChanged;
		}
		if (SortDescriptions != null)
		{
			SortDescriptions.CollectionChanged += CollectionView_SortDescriptions_CollectionChanged;
		}
		if (CollectionView != null)
		{
			CollectionView.CurrentChanged += CollectionView_CurrentChanged;
			CollectionView.CurrentChanging += CollectionView_CurrentChanging;
		}
		EventsWired = true;
	}

	private void CollectionView_CurrentChanged(object sender, EventArgs e)
	{
		if (_expectingCurrentChanged)
		{
			if (_itemToSelectOnCurrentChanged is DataGridCollectionViewGroup collectionViewGroup && _owner.RowGroupInfoFromCollectionViewGroup(collectionViewGroup) == null)
			{
				if (!_owner.IsSlotVisible(_backupSlotForCurrentChanged))
				{
					_backupSlotForCurrentChanged = _owner.GetNextVisibleSlot(_backupSlotForCurrentChanged);
				}
				if (_backupSlotForCurrentChanged >= _owner.SlotCount)
				{
					_backupSlotForCurrentChanged = _owner.GetPreviousVisibleSlot(_owner.SlotCount);
				}
				int rowIndex = -1;
				_itemToSelectOnCurrentChanged = _owner.ItemFromSlot(_backupSlotForCurrentChanged, ref rowIndex);
			}
			_owner.ProcessSelectionAndCurrency(_columnForCurrentChanged, _itemToSelectOnCurrentChanged, _backupSlotForCurrentChanged, _selectionActionForCurrentChanged, _scrollForCurrentChanged);
		}
		else if (CollectionView != null)
		{
			_owner.UpdateStateOnCurrentChanged(CollectionView.CurrentItem, CollectionView.CurrentPosition);
		}
	}

	private void CollectionView_CurrentChanging(object sender, DataGridCurrentChangingEventArgs e)
	{
		if (_owner.NoCurrentCellChangeCount == 0 && !_expectingCurrentChanged && !CommittingEdit && !_owner.CommitEdit())
		{
			if (e.IsCancelable)
			{
				e.Cancel = true;
			}
			else
			{
				_owner.CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
			}
		}
	}

	private void CollectionView_SortDescriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (_owner.ColumnsItemsInternal.Count == 0)
		{
			return;
		}
		foreach (DataGridColumn item in _owner.ColumnsItemsInternal)
		{
			item.HeaderCell.UpdatePseudoClasses();
		}
	}

	private void NotifyingDataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (_owner.LoadingOrUnloadingRow)
		{
			throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
		}
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
			if (ShouldAutoGenerateColumns)
			{
				_owner.InitializeElements(recycleRows: false);
			}
			else if (!IsGrouping)
			{
				_owner.InsertRowAt(e.NewStartingIndex);
			}
			break;
		case NotifyCollectionChangedAction.Remove:
			if (e.OldItems == null || e.OldStartingIndex < 0)
			{
				return;
			}
			if (IsGrouping)
			{
				break;
			}
			foreach (object oldItem in e.OldItems)
			{
				_owner.RemoveRowAt(e.OldStartingIndex, oldItem);
			}
			break;
		case NotifyCollectionChangedAction.Replace:
			throw new NotSupportedException();
		case NotifyCollectionChangedAction.Reset:
		{
			Type dataType = _dataType;
			_dataType = null;
			if (dataType != DataType)
			{
				ClearDataProperties();
				_owner.InitializeElements(recycleRows: false);
			}
			else
			{
				_owner.InitializeElements(!ShouldAutoGenerateColumns);
			}
			break;
		}
		}
		_owner.UpdatePseudoClasses();
	}

	private void UpdateDataProperties()
	{
		Type dataType = DataType;
		if (DataSource != null && dataType != null && !DataTypeIsPrimitive(dataType))
		{
			_dataProperties = dataType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
		}
		else
		{
			_dataProperties = null;
		}
	}
}
