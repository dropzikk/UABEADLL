using System;
using System.ComponentModel;
using Avalonia.Collections;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Utils;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Controls;

[PseudoClasses(new string[] { ":dragIndicator", ":pressed", ":sortascending", ":sortdescending" })]
public class DataGridColumnHeader : ContentControl
{
	private enum DragMode
	{
		None,
		MouseDown,
		Drag,
		Resize,
		Reorder
	}

	private const int DATAGRIDCOLUMNHEADER_resizeRegionWidth = 5;

	private const int DATAGRIDCOLUMNHEADER_columnsDragTreshold = 5;

	private bool _areHandlersSuspended;

	private static DragMode _dragMode;

	private static Point? _lastMousePositionHeaders;

	private static Cursor _originalCursor;

	private static double _originalHorizontalOffset;

	private static double _originalWidth;

	private bool _desiredSeparatorVisibility = true;

	private static Point? _dragStart;

	private static DataGridColumn _dragColumn;

	private static double _frozenColumnsWidth;

	private static Lazy<Cursor> _resizeCursor;

	public static readonly StyledProperty<IBrush> SeparatorBrushProperty;

	public static readonly StyledProperty<bool> AreSeparatorsVisibleProperty;

	public IBrush SeparatorBrush
	{
		get
		{
			return GetValue(SeparatorBrushProperty);
		}
		set
		{
			SetValue(SeparatorBrushProperty, value);
		}
	}

	public bool AreSeparatorsVisible
	{
		get
		{
			return GetValue(AreSeparatorsVisibleProperty);
		}
		set
		{
			SetValue(AreSeparatorsVisibleProperty, value);
		}
	}

	internal DataGridColumn OwningColumn { get; set; }

	internal DataGrid OwningGrid => OwningColumn?.OwningGrid;

	internal int ColumnIndex
	{
		get
		{
			if (OwningColumn == null)
			{
				return -1;
			}
			return OwningColumn.Index;
		}
	}

	internal ListSortDirection? CurrentSortingState { get; private set; }

	private bool IsMouseOver { get; set; }

	private bool IsPressed { get; set; }

	static DataGridColumnHeader()
	{
		_resizeCursor = new Lazy<Cursor>(() => new Cursor(StandardCursorType.SizeWestEast));
		SeparatorBrushProperty = AvaloniaProperty.Register<DataGridColumnHeader, IBrush>("SeparatorBrush");
		AreSeparatorsVisibleProperty = AvaloniaProperty.Register<DataGridColumnHeader, bool>("AreSeparatorsVisible", defaultValue: true);
		AreSeparatorsVisibleProperty.Changed.AddClassHandler(delegate(DataGridColumnHeader x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnAreSeparatorsVisibleChanged(e);
		});
		PressedMixin.Attach<DataGridColumnHeader>();
		InputElement.IsTabStopProperty.OverrideDefaultValue<DataGridColumnHeader>(defaultValue: false);
	}

	public DataGridColumnHeader()
	{
		base.PointerPressed += DataGridColumnHeader_PointerPressed;
		base.PointerReleased += DataGridColumnHeader_PointerReleased;
		base.PointerMoved += DataGridColumnHeader_PointerMoved;
		base.PointerEntered += DataGridColumnHeader_PointerEntered;
		base.PointerExited += DataGridColumnHeader_PointerExited;
	}

	private void OnAreSeparatorsVisibleChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!_areHandlersSuspended)
		{
			_desiredSeparatorVisibility = (bool)e.NewValue;
			if (OwningGrid != null)
			{
				UpdateSeparatorVisibility(OwningGrid.ColumnsInternal.LastVisibleColumn);
			}
			else
			{
				UpdateSeparatorVisibility(null);
			}
		}
	}

	private void SetValueNoCallback<T>(AvaloniaProperty<T> property, T value, BindingPriority priority = BindingPriority.LocalValue)
	{
		_areHandlersSuspended = true;
		try
		{
			SetValue(property, value, priority);
		}
		finally
		{
			_areHandlersSuspended = false;
		}
	}

	internal void UpdatePseudoClasses()
	{
		CurrentSortingState = null;
		if (OwningGrid != null && OwningGrid.DataConnection != null && OwningGrid.DataConnection.AllowSort)
		{
			DataGridSortDescription sortDescription = OwningColumn.GetSortDescription();
			if (sortDescription != null)
			{
				CurrentSortingState = sortDescription.Direction;
			}
		}
		base.PseudoClasses.Set(":sortascending", CurrentSortingState == ListSortDirection.Ascending);
		base.PseudoClasses.Set(":sortdescending", CurrentSortingState == ListSortDirection.Descending);
	}

	internal void UpdateSeparatorVisibility(DataGridColumn lastVisibleColumn)
	{
		bool flag = _desiredSeparatorVisibility;
		if (OwningColumn != null && OwningGrid != null && _desiredSeparatorVisibility && OwningColumn == lastVisibleColumn && !OwningGrid.ColumnsInternal.FillerColumn.IsActive)
		{
			flag = false;
		}
		if (AreSeparatorsVisible != flag)
		{
			SetValueNoCallback(AreSeparatorsVisibleProperty, flag);
		}
	}

	internal void OnMouseLeftButtonUp_Click(KeyModifiers keyModifiers, ref bool handled)
	{
		InvokeProcessSort(keyModifiers, null);
		handled = true;
	}

	internal void InvokeProcessSort(KeyModifiers keyModifiers, ListSortDirection? forcedDirection = null)
	{
		if (!OwningGrid.WaitForLostFocus(delegate
		{
			InvokeProcessSort(keyModifiers, forcedDirection);
		}) && OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
		{
			Dispatcher.UIThread.Post(delegate
			{
				ProcessSort(keyModifiers, forcedDirection);
			});
		}
	}

	internal void ProcessSort(KeyModifiers keyModifiers, ListSortDirection? forcedDirection = null)
	{
		if (OwningColumn == null || OwningGrid == null || OwningGrid.EditingRow != null || OwningColumn == OwningGrid.ColumnsInternal.FillerColumn || !OwningGrid.CanUserSortColumns || !OwningColumn.CanUserSort)
		{
			return;
		}
		DataGridColumnEventArgs dataGridColumnEventArgs = new DataGridColumnEventArgs(OwningColumn);
		OwningGrid.OnColumnSorting(dataGridColumnEventArgs);
		if (dataGridColumnEventArgs.Handled || !OwningGrid.DataConnection.AllowSort || OwningGrid.DataConnection.SortDescriptions == null)
		{
			return;
		}
		DataGrid owningGrid = OwningGrid;
		KeyboardHelper.GetMetaKeyState(this, keyModifiers, out var ctrlOrCmd, out var shift);
		DataGridSortDescription sortDescription = OwningColumn.GetSortDescription();
		IDataGridCollectionView collectionView = owningGrid.DataConnection.CollectionView;
		using (collectionView.DeferRefresh())
		{
			if (!shift || owningGrid.DataConnection.SortDescriptions.Count == 0)
			{
				owningGrid.DataConnection.SortDescriptions.Clear();
			}
			if (ctrlOrCmd)
			{
				return;
			}
			if (sortDescription != null)
			{
				DataGridSortDescription item = ((forcedDirection.HasValue && sortDescription.Direction == forcedDirection) ? sortDescription : sortDescription.SwitchSortDirection());
				int num = owningGrid.DataConnection.SortDescriptions.IndexOf(sortDescription);
				if (num >= 0)
				{
					owningGrid.DataConnection.SortDescriptions.Remove(sortDescription);
					owningGrid.DataConnection.SortDescriptions.Insert(num, item);
				}
				else
				{
					owningGrid.DataConnection.SortDescriptions.Add(item);
				}
				return;
			}
			if (OwningColumn.CustomSortComparer != null)
			{
				DataGridSortDescription item = (forcedDirection.HasValue ? DataGridSortDescription.FromComparer(OwningColumn.CustomSortComparer, forcedDirection.Value) : DataGridSortDescription.FromComparer(OwningColumn.CustomSortComparer));
				owningGrid.DataConnection.SortDescriptions.Add(item);
				return;
			}
			string sortPropertyName = OwningColumn.GetSortPropertyName();
			if (!string.IsNullOrEmpty(sortPropertyName))
			{
				DataGridSortDescription item = DataGridSortDescription.FromPath(sortPropertyName, ListSortDirection.Ascending, collectionView.Culture);
				if (forcedDirection.HasValue && item.Direction != forcedDirection)
				{
					item = item.SwitchSortDirection();
				}
				owningGrid.DataConnection.SortDescriptions.Add(item);
			}
		}
	}

	private bool CanReorderColumn(DataGridColumn column)
	{
		if (OwningGrid.CanUserReorderColumns && !(column is DataGridFillerColumn))
		{
			if (!column.CanUserReorderInternal.HasValue || !column.CanUserReorderInternal.Value)
			{
				return !column.CanUserReorderInternal.HasValue;
			}
			return true;
		}
		return false;
	}

	private static bool CanResizeColumn(DataGridColumn column)
	{
		if (column.OwningGrid != null && column.OwningGrid.ColumnsInternal != null && column.OwningGrid.UsesStarSizing && (column.OwningGrid.ColumnsInternal.LastVisibleColumn == column || !MathUtilities.AreClose(column.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth, column.OwningGrid.CellsWidth)))
		{
			return false;
		}
		return column.ActualCanUserResize;
	}

	private static bool TrySetResizeColumn(DataGridColumn column)
	{
		if (CanResizeColumn(column))
		{
			_dragColumn = column;
			_dragMode = DragMode.Resize;
			return true;
		}
		return false;
	}

	internal void OnMouseLeftButtonDown(ref bool handled, PointerEventArgs args, Point mousePosition)
	{
		IsPressed = true;
		if (OwningGrid != null && OwningGrid.ColumnHeaders != null)
		{
			_dragMode = DragMode.MouseDown;
			_frozenColumnsWidth = OwningGrid.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();
			_lastMousePositionHeaders = this.Translate(OwningGrid.ColumnHeaders, mousePosition);
			double x = mousePosition.X;
			double num = base.Bounds.Width - x;
			DataGridColumn owningColumn = OwningColumn;
			DataGridColumn dataGridColumn = null;
			if (!(OwningColumn is DataGridFillerColumn))
			{
				dataGridColumn = OwningGrid.ColumnsInternal.GetPreviousVisibleNonFillerColumn(owningColumn);
			}
			if (_dragMode == DragMode.MouseDown && _dragColumn == null && num <= 5.0)
			{
				handled = TrySetResizeColumn(owningColumn);
			}
			else if (_dragMode == DragMode.MouseDown && _dragColumn == null && x <= 5.0 && dataGridColumn != null)
			{
				handled = TrySetResizeColumn(dataGridColumn);
			}
			if (_dragMode == DragMode.Resize && _dragColumn != null)
			{
				_dragStart = _lastMousePositionHeaders;
				_originalWidth = _dragColumn.ActualWidth;
				_originalHorizontalOffset = OwningGrid.HorizontalOffset;
				handled = true;
			}
		}
	}

	internal void OnMouseLeftButtonUp(ref bool handled, PointerEventArgs args, Point mousePosition, Point mousePositionHeaders)
	{
		IsPressed = false;
		if (OwningGrid == null || OwningGrid.ColumnHeaders == null)
		{
			return;
		}
		if (_dragMode == DragMode.MouseDown)
		{
			OnMouseLeftButtonUp_Click(args.KeyModifiers, ref handled);
		}
		else if (_dragMode == DragMode.Reorder)
		{
			int reorderingTargetDisplayIndex = GetReorderingTargetDisplayIndex(mousePositionHeaders);
			if ((!OwningColumn.IsFrozen && reorderingTargetDisplayIndex >= OwningGrid.FrozenColumnCount) || (OwningColumn.IsFrozen && reorderingTargetDisplayIndex < OwningGrid.FrozenColumnCount))
			{
				OwningColumn.DisplayIndex = reorderingTargetDisplayIndex;
				DataGridColumnEventArgs e = new DataGridColumnEventArgs(OwningColumn);
				OwningGrid.OnColumnReordered(e);
			}
		}
		SetDragCursor(mousePosition);
		args.Pointer.Capture(null);
		OnLostMouseCapture();
		_dragMode = DragMode.None;
		handled = true;
	}

	internal void OnMouseMove(PointerEventArgs args, Point mousePosition, Point mousePositionHeaders)
	{
		bool handled = args.Handled;
		if (!handled && OwningGrid != null && OwningGrid.ColumnHeaders != null)
		{
			OnMouseMove_Resize(ref handled, mousePositionHeaders);
			OnMouseMove_Reorder(ref handled, mousePosition, mousePositionHeaders);
			SetDragCursor(mousePosition);
		}
	}

	private void DataGridColumnHeader_PointerEntered(object sender, PointerEventArgs e)
	{
		if (base.IsEnabled)
		{
			Point position = e.GetPosition(this);
			OnMouseEnter(position);
			UpdatePseudoClasses();
		}
	}

	private void DataGridColumnHeader_PointerExited(object sender, PointerEventArgs e)
	{
		if (base.IsEnabled)
		{
			OnMouseLeave();
			UpdatePseudoClasses();
		}
	}

	private void DataGridColumnHeader_PointerPressed(object sender, PointerPressedEventArgs e)
	{
		if (OwningColumn != null && !e.Handled && base.IsEnabled && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			Point position = e.GetPosition(this);
			bool handled = e.Handled;
			OnMouseLeftButtonDown(ref handled, e, position);
			e.Handled = handled;
			UpdatePseudoClasses();
		}
	}

	private void DataGridColumnHeader_PointerReleased(object sender, PointerReleasedEventArgs e)
	{
		if (OwningColumn != null && !e.Handled && base.IsEnabled && e.InitialPressMouseButton == MouseButton.Left)
		{
			Point position = e.GetPosition(this);
			Point position2 = e.GetPosition(OwningGrid.ColumnHeaders);
			bool handled = e.Handled;
			OnMouseLeftButtonUp(ref handled, e, position, position2);
			e.Handled = handled;
			UpdatePseudoClasses();
		}
	}

	private void DataGridColumnHeader_PointerMoved(object sender, PointerEventArgs e)
	{
		if (OwningGrid != null && base.IsEnabled)
		{
			Point position = e.GetPosition(this);
			Point position2 = e.GetPosition(OwningGrid.ColumnHeaders);
			OnMouseMove(e, position, position2);
		}
	}

	private DataGridColumn GetReorderingTargetColumn(Point mousePositionHeaders, bool scroll, out double scrollAmount)
	{
		scrollAmount = 0.0;
		double num = (OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented ? OwningGrid.ColumnsInternal.RowGroupSpacerColumn.ActualWidth : 0.0);
		double num2 = OwningGrid.CellsWidth;
		if (OwningColumn.IsFrozen)
		{
			num2 = Math.Min(num2, _frozenColumnsWidth);
		}
		else if (OwningGrid.FrozenColumnCount > 0)
		{
			num = _frozenColumnsWidth;
		}
		if (mousePositionHeaders.X < num)
		{
			if (scroll && OwningGrid.HorizontalScrollBar != null && OwningGrid.HorizontalScrollBar.IsVisible && OwningGrid.HorizontalScrollBar.Value > 0.0)
			{
				double val = mousePositionHeaders.X - num;
				scrollAmount = Math.Min(val, OwningGrid.HorizontalScrollBar.Value);
				OwningGrid.UpdateHorizontalOffset(scrollAmount + OwningGrid.HorizontalScrollBar.Value);
			}
			mousePositionHeaders = mousePositionHeaders.WithX(num);
		}
		else if (mousePositionHeaders.X >= num2)
		{
			if (scroll && OwningGrid.HorizontalScrollBar != null && OwningGrid.HorizontalScrollBar.IsVisible && OwningGrid.HorizontalScrollBar.Value < OwningGrid.HorizontalScrollBar.Maximum)
			{
				double val2 = mousePositionHeaders.X - num2;
				scrollAmount = Math.Min(val2, OwningGrid.HorizontalScrollBar.Maximum - OwningGrid.HorizontalScrollBar.Value);
				OwningGrid.UpdateHorizontalOffset(scrollAmount + OwningGrid.HorizontalScrollBar.Value);
			}
			mousePositionHeaders = mousePositionHeaders.WithX(num2 - 1.0);
		}
		foreach (DataGridColumn displayedColumn in OwningGrid.ColumnsInternal.GetDisplayedColumns())
		{
			Point point = OwningGrid.ColumnHeaders.Translate(displayedColumn.HeaderCell, mousePositionHeaders);
			double num3 = displayedColumn.HeaderCell.Bounds.Width / 2.0;
			if (point.X >= 0.0 && point.X <= num3)
			{
				return displayedColumn;
			}
			if (point.X > num3 && point.X < displayedColumn.HeaderCell.Bounds.Width)
			{
				return OwningGrid.ColumnsInternal.GetNextVisibleColumn(displayedColumn);
			}
		}
		return null;
	}

	private int GetReorderingTargetDisplayIndex(Point mousePositionHeaders)
	{
		double scrollAmount;
		DataGridColumn reorderingTargetColumn = GetReorderingTargetColumn(mousePositionHeaders, scroll: false, out scrollAmount);
		if (reorderingTargetColumn != null)
		{
			if (reorderingTargetColumn.DisplayIndex <= OwningColumn.DisplayIndex)
			{
				return reorderingTargetColumn.DisplayIndex;
			}
			return reorderingTargetColumn.DisplayIndex - 1;
		}
		return OwningGrid.Columns.Count - 1;
	}

	private bool IsReorderTargeted(Point mousePosition, Control element, bool ignoreVertical)
	{
		Point point = this.Translate(element, mousePosition);
		if (point.X < 0.0 || (point.X >= 0.0 && point.X <= element.Bounds.Width / 2.0))
		{
			if (!ignoreVertical)
			{
				if (point.Y >= 0.0)
				{
					return point.Y <= element.Bounds.Height;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	private void OnLostMouseCapture()
	{
		if (_dragColumn != null && _dragColumn.HeaderCell != null)
		{
			_dragColumn.HeaderCell.Cursor = _originalCursor;
		}
		_dragMode = DragMode.None;
		_dragColumn = null;
		_dragStart = null;
		_lastMousePositionHeaders = null;
		if (OwningGrid != null && OwningGrid.ColumnHeaders != null)
		{
			OwningGrid.ColumnHeaders.DragColumn = null;
			OwningGrid.ColumnHeaders.DragIndicator = null;
			OwningGrid.ColumnHeaders.DropLocationIndicator = null;
		}
	}

	private void OnMouseEnter(Point mousePosition)
	{
		IsMouseOver = true;
		SetDragCursor(mousePosition);
	}

	private void OnMouseLeave()
	{
		IsMouseOver = false;
	}

	private void OnMouseMove_BeginReorder(Point mousePosition)
	{
		DataGridColumnHeader dataGridColumnHeader = new DataGridColumnHeader
		{
			OwningColumn = OwningColumn,
			IsEnabled = false,
			Content = base.Content,
			ContentTemplate = base.ContentTemplate
		};
		ControlTheme columnHeaderTheme = OwningGrid.ColumnHeaderTheme;
		if (columnHeaderTheme != null)
		{
			dataGridColumnHeader.SetValue(StyledElement.ThemeProperty, columnHeaderTheme, BindingPriority.Template);
		}
		dataGridColumnHeader.PseudoClasses.Add(":dragIndicator");
		Control control = OwningGrid.DropLocationIndicatorTemplate?.Build();
		if (control != null && double.IsNaN(control.Height) && control != null)
		{
			Control control2 = control;
			control2.Height = base.Bounds.Height;
		}
		DataGridColumnReorderingEventArgs dataGridColumnReorderingEventArgs = new DataGridColumnReorderingEventArgs(OwningColumn)
		{
			DropLocationIndicator = control,
			DragIndicator = dataGridColumnHeader
		};
		OwningGrid.OnColumnReordering(dataGridColumnReorderingEventArgs);
		if (!dataGridColumnReorderingEventArgs.Cancel)
		{
			_dragColumn = OwningColumn;
			_dragMode = DragMode.Reorder;
			_dragStart = mousePosition;
			OwningGrid.ColumnHeaders.DragColumn = OwningColumn;
			OwningGrid.ColumnHeaders.DragIndicator = dataGridColumnReorderingEventArgs.DragIndicator;
			OwningGrid.ColumnHeaders.DropLocationIndicator = dataGridColumnReorderingEventArgs.DropLocationIndicator;
			if (double.IsNaN(dataGridColumnHeader.Width))
			{
				dataGridColumnHeader.Width = base.Bounds.Width;
			}
		}
	}

	private void OnMouseMove_Reorder(ref bool handled, Point mousePosition, Point mousePositionHeaders)
	{
		if (handled)
		{
			return;
		}
		if (_dragMode == DragMode.MouseDown && _dragColumn == null && _lastMousePositionHeaders.HasValue)
		{
			Point value = mousePositionHeaders;
			Point? lastMousePositionHeaders = _lastMousePositionHeaders;
			if (((Vector)(value - lastMousePositionHeaders).Value).Length > 5.0)
			{
				handled = CanReorderColumn(OwningColumn);
				if (handled)
				{
					OnMouseMove_BeginReorder(mousePosition);
				}
			}
		}
		if (_dragMode != DragMode.Reorder || OwningGrid.ColumnHeaders.DragIndicator == null)
		{
			return;
		}
		DataGridColumn reorderingTargetColumn = GetReorderingTargetColumn(mousePositionHeaders, !OwningColumn.IsFrozen, out var scrollAmount);
		OwningGrid.ColumnHeaders.DragIndicatorOffset = mousePosition.X - _dragStart.Value.X + scrollAmount;
		OwningGrid.ColumnHeaders.InvalidateArrange();
		if (OwningGrid.ColumnHeaders.DropLocationIndicator != null)
		{
			Point fromPoint = new Point(0.0, 0.0);
			if (reorderingTargetColumn == null || reorderingTargetColumn == OwningGrid.ColumnsInternal.FillerColumn || reorderingTargetColumn.IsFrozen != OwningColumn.IsFrozen)
			{
				reorderingTargetColumn = OwningGrid.ColumnsInternal.GetLastColumn(true, OwningColumn.IsFrozen, null);
				fromPoint = reorderingTargetColumn.HeaderCell.Translate(OwningGrid.ColumnHeaders, fromPoint);
				fromPoint = fromPoint.WithX(fromPoint.X + reorderingTargetColumn.ActualWidth);
			}
			else
			{
				fromPoint = reorderingTargetColumn.HeaderCell.Translate(OwningGrid.ColumnHeaders, fromPoint);
			}
			OwningGrid.ColumnHeaders.DropLocationIndicatorOffset = fromPoint.X - scrollAmount;
		}
		handled = true;
	}

	private void OnMouseMove_Resize(ref bool handled, Point mousePositionHeaders)
	{
		if (!handled && _dragMode == DragMode.Resize && _dragColumn != null && _dragStart.HasValue)
		{
			double num = mousePositionHeaders.X - _dragStart.Value.X;
			double val = _originalWidth + num;
			val = Math.Max(_dragColumn.ActualMinWidth, Math.Min(_dragColumn.ActualMaxWidth, val));
			_dragColumn.Resize(_dragColumn.Width, new DataGridLength(_dragColumn.Width.Value, _dragColumn.Width.UnitType, _dragColumn.Width.DesiredValue, val), userInitiated: true);
			OwningGrid.UpdateHorizontalOffset(_originalHorizontalOffset);
			handled = true;
		}
	}

	private void SetDragCursor(Point mousePosition)
	{
		if (_dragMode != 0 || OwningGrid == null || OwningColumn == null)
		{
			return;
		}
		double x = mousePosition.X;
		double num = base.Bounds.Width - x;
		DataGridColumn owningColumn = OwningColumn;
		DataGridColumn dataGridColumn = null;
		if (!(OwningColumn is DataGridFillerColumn))
		{
			dataGridColumn = OwningGrid.ColumnsInternal.GetPreviousVisibleNonFillerColumn(owningColumn);
		}
		if ((num <= 5.0 && owningColumn != null && CanResizeColumn(owningColumn)) || (x <= 5.0 && dataGridColumn != null && CanResizeColumn(dataGridColumn)))
		{
			Cursor value = _resizeCursor.Value;
			if (base.Cursor != value)
			{
				_originalCursor = base.Cursor;
				base.Cursor = value;
			}
		}
		else
		{
			base.Cursor = _originalCursor;
		}
	}
}
