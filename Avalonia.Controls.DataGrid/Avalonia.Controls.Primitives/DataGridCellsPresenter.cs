using System;
using System.Collections.Specialized;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls.Primitives;

public sealed class DataGridCellsPresenter : Panel, IChildIndexProvider
{
	private double _fillerLeftEdge;

	private EventHandler<ChildIndexChangedEventArgs> _childIndexChanged;

	private double DesiredHeight { get; set; }

	private DataGrid OwningGrid => OwningRow?.OwningGrid;

	internal DataGridRow OwningRow { get; set; }

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
		if (!(child is DataGridCell dataGridCell))
		{
			throw new InvalidOperationException("Invalid cell type");
		}
		return OwningGrid.ColumnsInternal.GetColumnDisplayIndex(dataGridCell.ColumnIndex);
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
		double num2 = 0.0 - OwningGrid.HorizontalOffset;
		foreach (DataGridColumn visibleColumn in OwningGrid.ColumnsInternal.GetVisibleColumns())
		{
			DataGridCell dataGridCell = OwningRow.Cells[visibleColumn.Index];
			double x;
			if (visibleColumn.IsFrozen)
			{
				x = num;
				num += visibleColumn.ActualWidth;
			}
			else
			{
				x = num2;
			}
			if (dataGridCell.IsVisible)
			{
				dataGridCell.Arrange(new Rect(x, 0.0, visibleColumn.LayoutRoundedWidth, finalSize.Height));
				EnsureCellClip(dataGridCell, visibleColumn.ActualWidth, finalSize.Height, num, num2);
			}
			num2 += visibleColumn.ActualWidth;
			visibleColumn.IsInitialDesiredWidthDetermined = true;
		}
		_fillerLeftEdge = num2;
		OwningRow.FillerCell.Arrange(new Rect(_fillerLeftEdge, 0.0, OwningGrid.ColumnsInternal.FillerColumn.FillerWidth, finalSize.Height));
		return finalSize;
	}

	private static void EnsureCellClip(DataGridCell cell, double width, double height, double frozenLeftEdge, double cellLeftEdge)
	{
		if (!cell.OwningColumn.IsFrozen && frozenLeftEdge > cellLeftEdge)
		{
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			double num = Math.Round(Math.Min(width, frozenLeftEdge - cellLeftEdge));
			rectangleGeometry.Rect = new Rect(num, 0.0, Math.Max(0.0, width - num), height);
			cell.Clip = rectangleGeometry;
		}
		else
		{
			cell.Clip = null;
		}
	}

	protected override void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		base.ChildrenChanged(sender, e);
		InvalidateChildIndex();
	}

	private static void EnsureCellDisplay(DataGridCell cell, bool displayColumn)
	{
		if (cell.IsCurrent)
		{
			if (displayColumn)
			{
				cell.IsVisible = true;
				cell.Clip = null;
			}
			else
			{
				RectangleGeometry rectangleGeometry = new RectangleGeometry();
				rectangleGeometry.Rect = default(Rect);
				cell.Clip = rectangleGeometry;
			}
		}
		else
		{
			cell.IsVisible = displayColumn;
		}
	}

	internal void EnsureFillerVisibility()
	{
		DataGridFillerColumn fillerColumn = OwningGrid.ColumnsInternal.FillerColumn;
		bool isActive = fillerColumn.IsActive;
		if (OwningRow.FillerCell.IsVisible != isActive)
		{
			OwningRow.FillerCell.IsVisible = isActive;
			if (isActive)
			{
				OwningRow.FillerCell.Arrange(new Rect(_fillerLeftEdge, 0.0, fillerColumn.FillerWidth, base.Bounds.Height));
			}
		}
		DataGridColumn lastVisibleColumn = OwningGrid.ColumnsInternal.LastVisibleColumn;
		if (lastVisibleColumn != null)
		{
			OwningRow.Cells[lastVisibleColumn.Index].EnsureGridLine(lastVisibleColumn);
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (OwningGrid == null)
		{
			return base.MeasureOverride(availableSize);
		}
		bool flag;
		double height;
		if (double.IsNaN(OwningGrid.RowHeight))
		{
			flag = true;
			InvalidateDesiredHeight();
			height = double.PositiveInfinity;
		}
		else
		{
			DesiredHeight = OwningGrid.RowHeight;
			height = DesiredHeight;
			flag = false;
		}
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0 - OwningGrid.HorizontalOffset;
		OwningGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
		DataGridColumn lastVisibleColumn = OwningGrid.ColumnsInternal.LastVisibleColumn;
		foreach (DataGridColumn visibleColumn in OwningGrid.ColumnsInternal.GetVisibleColumns())
		{
			DataGridCell dataGridCell = OwningRow.Cells[visibleColumn.Index];
			bool flag2 = ShouldDisplayCell(visibleColumn, num, num3) || OwningRow.Index == 0;
			EnsureCellDisplay(dataGridCell, flag2);
			if (flag2)
			{
				DataGridLength width = visibleColumn.Width;
				bool num4 = width.IsSizeToCells || width.IsAuto;
				if (visibleColumn != lastVisibleColumn)
				{
					dataGridCell.EnsureGridLine(lastVisibleColumn);
				}
				if (!OwningGrid.UsesStarSizing || (!visibleColumn.ActualCanUserResize && !visibleColumn.Width.IsStar))
				{
					double widthDisplayValue = (visibleColumn.Width.IsStar ? Math.Min(visibleColumn.ActualMaxWidth, 10000.0) : Math.Max(visibleColumn.ActualMinWidth, Math.Min(visibleColumn.ActualMaxWidth, visibleColumn.Width.DesiredValue)));
					visibleColumn.SetWidthDisplayValue(widthDisplayValue);
				}
				if (num4)
				{
					dataGridCell.Measure(new Size(visibleColumn.ActualMaxWidth, height));
					OwningGrid.AutoSizeColumn(visibleColumn, dataGridCell.DesiredSize.Width);
					visibleColumn.ComputeLayoutRoundedWidth(num2);
				}
				else if (!OwningGrid.UsesStarSizing)
				{
					visibleColumn.ComputeLayoutRoundedWidth(num3);
					dataGridCell.Measure(new Size(visibleColumn.LayoutRoundedWidth, height));
				}
				if (flag)
				{
					DesiredHeight = Math.Max(DesiredHeight, dataGridCell.DesiredSize.Height);
				}
			}
			if (visibleColumn.IsFrozen)
			{
				num += visibleColumn.ActualWidth;
			}
			num3 += visibleColumn.ActualWidth;
			num2 += visibleColumn.ActualWidth;
		}
		if (OwningGrid.UsesStarSizing && !OwningGrid.AutoSizingColumns)
		{
			double num5 = OwningGrid.CellsWidth - num2;
			num2 += num5 - OwningGrid.AdjustColumnWidths(0, num5, userInitiated: false);
			double num6 = 0.0;
			if (flag)
			{
				DesiredHeight = 0.0;
			}
			foreach (DataGridColumn visibleColumn2 in OwningGrid.ColumnsInternal.GetVisibleColumns())
			{
				DataGridCell dataGridCell2 = OwningRow.Cells[visibleColumn2.Index];
				visibleColumn2.ComputeLayoutRoundedWidth(num6);
				dataGridCell2.Measure(new Size(visibleColumn2.LayoutRoundedWidth, height));
				if (flag)
				{
					DesiredHeight = Math.Max(DesiredHeight, dataGridCell2.DesiredSize.Height);
				}
				num6 += visibleColumn2.ActualWidth;
			}
		}
		OwningRow.FillerCell.Measure(new Size(double.PositiveInfinity, DesiredHeight));
		OwningGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
		return new Size(OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth, DesiredHeight);
	}

	internal void Recycle()
	{
		DesiredHeight = 0.0;
	}

	internal void InvalidateDesiredHeight()
	{
		DesiredHeight = 0.0;
	}

	internal void InvalidateChildIndex()
	{
		_childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);
	}

	private bool ShouldDisplayCell(DataGridColumn column, double frozenLeftEdge, double scrollingLeftEdge)
	{
		if (!column.IsVisible)
		{
			return false;
		}
		scrollingLeftEdge += OwningGrid.HorizontalAdjustment;
		double num = (column.IsFrozen ? frozenLeftEdge : scrollingLeftEdge);
		double value = num + column.ActualWidth;
		if (MathUtilities.GreaterThan(value, 0.0) && MathUtilities.LessThanOrClose(num, OwningGrid.CellsWidth))
		{
			return MathUtilities.GreaterThan(value, frozenLeftEdge);
		}
		return false;
	}
}
