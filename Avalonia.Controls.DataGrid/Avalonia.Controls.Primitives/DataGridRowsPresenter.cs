using System;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace Avalonia.Controls.Primitives;

public sealed class DataGridRowsPresenter : Panel, IChildIndexProvider
{
	private EventHandler<ChildIndexChangedEventArgs> _childIndexChanged;

	private double _measureHeightOffset;

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

	public DataGridRowsPresenter()
	{
		AddHandler(Gestures.ScrollGestureEvent, OnScrollGesture);
	}

	private double CalculateEstimatedAvailableHeight(Size availableSize)
	{
		if (!double.IsPositiveInfinity(availableSize.Height))
		{
			return availableSize.Height + _measureHeightOffset;
		}
		return availableSize.Height;
	}

	int IChildIndexProvider.GetChildIndex(ILogical child)
	{
		if (!(child is DataGridRow dataGridRow))
		{
			throw new InvalidOperationException("Invalid DataGrid child");
		}
		return dataGridRow.Index;
	}

	bool IChildIndexProvider.TryGetTotalCount(out int count)
	{
		return OwningGrid.DataConnection.TryGetCount(allowSlow: false, getAny: true, out count);
	}

	internal void InvalidateChildIndex(DataGridRow row)
	{
		_childIndexChanged?.Invoke(this, new ChildIndexChangedEventArgs(row, row.Index));
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (finalSize.Height == 0.0 || OwningGrid == null)
		{
			return base.ArrangeOverride(finalSize);
		}
		if (OwningGrid.RowsPresenterAvailableSize.HasValue)
		{
			double height = OwningGrid.RowsPresenterAvailableSize.Value.Height;
			if (!double.IsPositiveInfinity(height))
			{
				_measureHeightOffset = finalSize.Height - height;
				OwningGrid.RowsPresenterEstimatedAvailableHeight = finalSize.Height;
			}
		}
		OwningGrid.OnFillerColumnWidthNeeded(finalSize.Width);
		double num = OwningGrid.RowHeadersDesiredWidth + OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth + OwningGrid.ColumnsInternal.FillerColumn.FillerWidth;
		double num2 = 0.0 - OwningGrid.NegVerticalOffset;
		foreach (Control scrollingElement in OwningGrid.DisplayData.GetScrollingElements())
		{
			if (scrollingElement is DataGridRow dataGridRow)
			{
				dataGridRow.EnsureFillerVisibility();
				dataGridRow.Arrange(new Rect(0.0 - OwningGrid.HorizontalOffset, num2, num, scrollingElement.DesiredSize.Height));
			}
			else if (scrollingElement is DataGridRowGroupHeader dataGridRowGroupHeader)
			{
				double num3 = (OwningGrid.AreRowGroupHeadersFrozen ? 0.0 : (0.0 - OwningGrid.HorizontalOffset));
				dataGridRowGroupHeader.Arrange(new Rect(num3, num2, num - num3, scrollingElement.DesiredSize.Height));
			}
			num2 += scrollingElement.DesiredSize.Height;
		}
		double height2 = Math.Max(num2 + OwningGrid.NegVerticalOffset, finalSize.Height);
		RectangleGeometry clip = new RectangleGeometry
		{
			Rect = new Rect(0.0, 0.0, finalSize.Width, height2)
		};
		base.Clip = clip;
		return new Size(finalSize.Width, height2);
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (double.IsInfinity(availableSize.Height) && base.VisualRoot is TopLevel topLevel)
		{
			double height = (topLevel.IsArrangeValid ? topLevel.Bounds.Height : LayoutHelper.ApplyLayoutConstraints(topLevel, availableSize).Height);
			availableSize = availableSize.WithHeight(height);
		}
		if (availableSize.Height == 0.0 || OwningGrid == null)
		{
			return base.MeasureOverride(availableSize);
		}
		bool flag = (!OwningGrid.RowsPresenterAvailableSize.HasValue || availableSize.Width != OwningGrid.RowsPresenterAvailableSize.Value.Width) && !double.IsInfinity(availableSize.Width);
		OwningGrid.RowsPresenterAvailableSize = availableSize;
		OwningGrid.RowsPresenterEstimatedAvailableHeight = CalculateEstimatedAvailableHeight(availableSize);
		OwningGrid.OnRowsMeasure();
		double num = 0.0 - OwningGrid.NegVerticalOffset;
		double visibleEdgedColumnsWidth = OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth;
		double num2 = 0.0;
		foreach (Control scrollingElement in OwningGrid.DisplayData.GetScrollingElements())
		{
			DataGridRow dataGridRow = scrollingElement as DataGridRow;
			if (dataGridRow != null && flag)
			{
				dataGridRow.InvalidateMeasure();
			}
			scrollingElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			if (dataGridRow != null && dataGridRow.HeaderCell != null)
			{
				num2 = Math.Max(num2, dataGridRow.HeaderCell.DesiredSize.Width);
			}
			else if (scrollingElement is DataGridRowGroupHeader { HeaderCell: not null } dataGridRowGroupHeader)
			{
				num2 = Math.Max(num2, dataGridRowGroupHeader.HeaderCell.DesiredSize.Width);
			}
			num += scrollingElement.DesiredSize.Height;
		}
		OwningGrid.RowHeadersDesiredWidth = num2;
		OwningGrid.AvailableSlotElementRoom = availableSize.Height - num;
		num = Math.Max(0.0, num);
		return new Size(visibleEdgedColumnsWidth + num2, num);
	}

	private void OnScrollGesture(object sender, ScrollGestureEventArgs e)
	{
		e.Handled = e.Handled || OwningGrid.UpdateScroll(-e.Delta);
	}
}
