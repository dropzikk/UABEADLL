using System;
using Avalonia.Layout;
using Avalonia.Media;

namespace Avalonia.Controls.Primitives;

public sealed class DataGridDetailsPresenter : Panel
{
	public static readonly StyledProperty<double> ContentHeightProperty = AvaloniaProperty.Register<DataGridDetailsPresenter, double>("ContentHeight", 0.0);

	public double ContentHeight
	{
		get
		{
			return GetValue(ContentHeightProperty);
		}
		set
		{
			SetValue(ContentHeightProperty, value);
		}
	}

	internal DataGridRow OwningRow { get; set; }

	private DataGrid OwningGrid => OwningRow?.OwningGrid;

	public DataGridDetailsPresenter()
	{
		Layoutable.AffectsMeasure<DataGridDetailsPresenter>(new AvaloniaProperty[1] { ContentHeightProperty });
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (OwningGrid == null)
		{
			return base.ArrangeOverride(finalSize);
		}
		double value = OwningGrid.ColumnsInternal.RowGroupSpacerColumn.Width.Value;
		double num = value;
		double num2 = (OwningGrid.AreRowGroupHeadersFrozen ? value : 0.0);
		double num3;
		if (OwningGrid.AreRowDetailsFrozen)
		{
			num += OwningGrid.HorizontalOffset;
			num3 = OwningGrid.CellsWidth;
		}
		else
		{
			num2 += OwningGrid.HorizontalOffset;
			num3 = Math.Max(OwningGrid.CellsWidth, OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth);
		}
		num3 -= value;
		double height = Math.Max(0.0, double.IsNaN(ContentHeight) ? 0.0 : ContentHeight);
		foreach (Control child in base.Children)
		{
			child.Arrange(new Rect(num, 0.0, num3, height));
		}
		if (OwningGrid.AreRowDetailsFrozen)
		{
			base.Clip = null;
		}
		else
		{
			base.Clip = new RectangleGeometry
			{
				Rect = new Rect(num2, 0.0, Math.Max(0.0, num3 - num2 + value), height)
			};
		}
		return finalSize;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (OwningGrid == null || base.Children.Count == 0)
		{
			return default(Size);
		}
		double num = (OwningGrid.AreRowDetailsFrozen ? OwningGrid.CellsWidth : Math.Max(OwningGrid.CellsWidth, OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth));
		num -= OwningGrid.ColumnsInternal.RowGroupSpacerColumn.Width.Value;
		foreach (Control child in base.Children)
		{
			child.Measure(new Size(num, double.PositiveInfinity));
		}
		double height = Math.Max(0.0, double.IsNaN(ContentHeight) ? 0.0 : ContentHeight);
		return new Size(num, height);
	}
}
