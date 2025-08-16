using System;
using System.Collections.Specialized;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace Avalonia.Controls.Primitives;

public sealed class DataGridColumnHeadersPresenter : Panel, IChildIndexProvider
{
	private Control _dragIndicator;

	private Control _dropLocationIndicator;

	private EventHandler<ChildIndexChangedEventArgs> _childIndexChanged;

	internal DataGridColumn DragColumn { get; set; }

	internal Control DragIndicator
	{
		get
		{
			return _dragIndicator;
		}
		set
		{
			if (value != _dragIndicator)
			{
				if (base.Children.Contains(_dragIndicator))
				{
					base.Children.Remove(_dragIndicator);
				}
				_dragIndicator = value;
				if (_dragIndicator != null)
				{
					base.Children.Add(_dragIndicator);
				}
			}
		}
	}

	internal double DragIndicatorOffset { get; set; }

	internal Control DropLocationIndicator
	{
		get
		{
			return _dropLocationIndicator;
		}
		set
		{
			if (value != _dropLocationIndicator)
			{
				if (base.Children.Contains(_dropLocationIndicator))
				{
					base.Children.Remove(_dropLocationIndicator);
				}
				_dropLocationIndicator = value;
				if (_dropLocationIndicator != null)
				{
					base.Children.Add(_dropLocationIndicator);
				}
			}
		}
	}

	internal double DropLocationIndicatorOffset { get; set; }

	internal DataGrid OwningGrid { get; set; }

	event EventHandler<ChildIndexChangedEventArgs> IChildIndexProvider.ChildIndexChanged
	{
		add
		{
			_childIndexChanged = (EventHandler<ChildIndexChangedEventArgs>)Delegate.Combine(_childIndexChanged, value);
		}
		remove
		{
			_childIndexChanged = (EventHandler<ChildIndexChangedEventArgs>)Delegate.Remove(_childIndexChanged, value);
		}
	}

	int IChildIndexProvider.GetChildIndex(ILogical child)
	{
		if (!(child is DataGridColumnHeader dataGridColumnHeader))
		{
			throw new InvalidOperationException("Invalid cell type");
		}
		return OwningGrid.ColumnsInternal.GetColumnDisplayIndex(dataGridColumnHeader.ColumnIndex);
	}

	bool IChildIndexProvider.TryGetTotalCount(out int count)
	{
		count = OwningGrid.ColumnsInternal.VisibleColumnCount;
		return true;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (OwningGrid == null)
		{
			return base.ArrangeOverride(finalSize);
		}
		if (OwningGrid.AutoSizingColumns)
		{
			OwningGrid.AutoSizingColumns = false;
			return base.ArrangeOverride(finalSize);
		}
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0 - OwningGrid.HorizontalOffset;
		foreach (DataGridColumn visibleColumn in OwningGrid.ColumnsInternal.GetVisibleColumns())
		{
			DataGridColumnHeader headerCell = visibleColumn.HeaderCell;
			if (visibleColumn.IsFrozen)
			{
				headerCell.Arrange(new Rect(num2, 0.0, visibleColumn.LayoutRoundedWidth, finalSize.Height));
				headerCell.Clip = null;
				if (DragColumn == visibleColumn && DragIndicator != null)
				{
					num = num2 + DragIndicatorOffset;
				}
				num2 += visibleColumn.ActualWidth;
			}
			else
			{
				headerCell.Arrange(new Rect(num3, 0.0, visibleColumn.LayoutRoundedWidth, finalSize.Height));
				EnsureColumnHeaderClip(headerCell, visibleColumn.ActualWidth, finalSize.Height, num2, num3);
				if (DragColumn == visibleColumn && DragIndicator != null)
				{
					num = num3 + DragIndicatorOffset;
				}
			}
			num3 += visibleColumn.ActualWidth;
		}
		if (DragColumn != null)
		{
			if (DragIndicator != null)
			{
				EnsureColumnReorderingClip(DragIndicator, finalSize.Height, num2, num);
				double height = DragIndicator.Bounds.Height;
				if (height <= 0.0)
				{
					height = DragIndicator.DesiredSize.Height;
				}
				DragIndicator.Arrange(new Rect(num, 0.0, DragIndicator.Bounds.Width, height));
			}
			if (DropLocationIndicator != null)
			{
				Control dropLocationIndicator = DropLocationIndicator;
				if (dropLocationIndicator != null)
				{
					EnsureColumnReorderingClip(dropLocationIndicator, finalSize.Height, num2, DropLocationIndicatorOffset);
				}
				DropLocationIndicator.Arrange(new Rect(DropLocationIndicatorOffset, 0.0, DropLocationIndicator.Bounds.Width, DropLocationIndicator.Bounds.Height));
			}
		}
		OwningGrid.OnFillerColumnWidthNeeded(finalSize.Width);
		DataGridFillerColumn fillerColumn = OwningGrid.ColumnsInternal.FillerColumn;
		if (fillerColumn.FillerWidth > 0.0)
		{
			fillerColumn.HeaderCell.IsVisible = true;
			fillerColumn.HeaderCell.Arrange(new Rect(num3, 0.0, fillerColumn.FillerWidth, finalSize.Height));
		}
		else
		{
			fillerColumn.HeaderCell.IsVisible = false;
		}
		DataGridColumn lastVisibleColumn = OwningGrid.ColumnsInternal.LastVisibleColumn;
		lastVisibleColumn?.HeaderCell.UpdateSeparatorVisibility(lastVisibleColumn);
		return finalSize;
	}

	private static void EnsureColumnHeaderClip(DataGridColumnHeader columnHeader, double width, double height, double frozenLeftEdge, double columnHeaderLeftEdge)
	{
		if (frozenLeftEdge > columnHeaderLeftEdge)
		{
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			double num = Math.Min(width, frozenLeftEdge - columnHeaderLeftEdge);
			rectangleGeometry.Rect = new Rect(num, 0.0, width - num, height);
			columnHeader.Clip = rectangleGeometry;
		}
		else
		{
			columnHeader.Clip = null;
		}
	}

	private void EnsureColumnReorderingClip(Control control, double height, double frozenColumnsWidth, double controlLeftEdge)
	{
		double num = 0.0;
		double num2 = OwningGrid.CellsWidth;
		double width = control.Bounds.Width;
		if (DragColumn.IsFrozen)
		{
			if (control == DragIndicator)
			{
				num2 = Math.Min(num2, frozenColumnsWidth);
			}
		}
		else if (OwningGrid.FrozenColumnCount > 0)
		{
			num = frozenColumnsWidth;
		}
		RectangleGeometry rectangleGeometry = null;
		if (num > controlLeftEdge)
		{
			rectangleGeometry = new RectangleGeometry();
			double num3 = Math.Min(width, num - controlLeftEdge);
			rectangleGeometry.Rect = new Rect(num3, 0.0, width - num3, height);
		}
		if (controlLeftEdge + width >= num2)
		{
			if (rectangleGeometry == null)
			{
				rectangleGeometry = new RectangleGeometry();
			}
			rectangleGeometry.Rect = new Rect(rectangleGeometry.Rect.X, rectangleGeometry.Rect.Y, Math.Max(0.0, num2 - controlLeftEdge - rectangleGeometry.Rect.X), height);
		}
		control.Clip = rectangleGeometry;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (OwningGrid == null)
		{
			return base.MeasureOverride(availableSize);
		}
		if (!OwningGrid.AreColumnHeadersVisible)
		{
			return default(Size);
		}
		double num = OwningGrid.ColumnHeaderHeight;
		bool flag;
		if (double.IsNaN(num))
		{
			num = 0.0;
			flag = true;
		}
		else
		{
			flag = false;
		}
		double num2 = 0.0;
		OwningGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
		DataGridColumn lastVisibleColumn = OwningGrid.ColumnsInternal.LastVisibleColumn;
		foreach (DataGridColumn visibleColumn in OwningGrid.ColumnsInternal.GetVisibleColumns())
		{
			bool num3 = visibleColumn.Width.IsAuto || visibleColumn.Width.IsSizeToHeader;
			DataGridColumnHeader headerCell = visibleColumn.HeaderCell;
			if (visibleColumn != lastVisibleColumn)
			{
				headerCell.UpdateSeparatorVisibility(lastVisibleColumn);
			}
			if (!OwningGrid.UsesStarSizing || (!visibleColumn.ActualCanUserResize && !visibleColumn.Width.IsStar))
			{
				double widthDisplayValue = (visibleColumn.Width.IsStar ? Math.Min(visibleColumn.ActualMaxWidth, 10000.0) : Math.Max(visibleColumn.ActualMinWidth, Math.Min(visibleColumn.ActualMaxWidth, visibleColumn.Width.DesiredValue)));
				visibleColumn.SetWidthDisplayValue(widthDisplayValue);
			}
			if (num3)
			{
				headerCell.Measure(new Size(visibleColumn.ActualMaxWidth, double.PositiveInfinity));
				OwningGrid.AutoSizeColumn(visibleColumn, headerCell.DesiredSize.Width);
				visibleColumn.ComputeLayoutRoundedWidth(num2);
			}
			else if (!OwningGrid.UsesStarSizing)
			{
				visibleColumn.ComputeLayoutRoundedWidth(num2);
				headerCell.Measure(new Size(visibleColumn.LayoutRoundedWidth, double.PositiveInfinity));
			}
			if (flag)
			{
				num = Math.Max(num, headerCell.DesiredSize.Height);
			}
			num2 += visibleColumn.ActualWidth;
		}
		if (OwningGrid.UsesStarSizing && !OwningGrid.AutoSizingColumns)
		{
			double num4 = (double.IsPositiveInfinity(availableSize.Width) ? OwningGrid.CellsWidth : (availableSize.Width - num2));
			num2 += num4 - OwningGrid.AdjustColumnWidths(0, num4, userInitiated: false);
			double num5 = 0.0;
			foreach (DataGridColumn visibleColumn2 in OwningGrid.ColumnsInternal.GetVisibleColumns())
			{
				visibleColumn2.ComputeLayoutRoundedWidth(num5);
				visibleColumn2.HeaderCell.Measure(new Size(visibleColumn2.LayoutRoundedWidth, double.PositiveInfinity));
				if (flag)
				{
					num = Math.Max(num, visibleColumn2.HeaderCell.DesiredSize.Height);
				}
				num5 += visibleColumn2.ActualWidth;
			}
		}
		DataGridFillerColumn fillerColumn = OwningGrid.ColumnsInternal.FillerColumn;
		if (!fillerColumn.IsRepresented)
		{
			fillerColumn.HeaderCell.AreSeparatorsVisible = false;
			base.Children.Insert(OwningGrid.ColumnsInternal.Count, fillerColumn.HeaderCell);
			fillerColumn.IsRepresented = true;
			fillerColumn.HeaderCell.IsVisible = false;
		}
		fillerColumn.HeaderCell.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		if (DragIndicator != null)
		{
			DragIndicator.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		}
		if (DropLocationIndicator != null)
		{
			DropLocationIndicator.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		}
		OwningGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
		return new Size(OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth, num);
	}

	protected override void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		base.ChildrenChanged(sender, e);
		InvalidateChildIndex();
	}

	internal void InvalidateChildIndex()
	{
		_childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);
	}
}
