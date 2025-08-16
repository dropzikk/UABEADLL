using System;
using Avalonia.Layout;

namespace Avalonia.Controls.Primitives;

public class UniformGrid : Panel
{
	public static readonly StyledProperty<int> RowsProperty;

	public static readonly StyledProperty<int> ColumnsProperty;

	public static readonly StyledProperty<int> FirstColumnProperty;

	private int _rows;

	private int _columns;

	public int Rows
	{
		get
		{
			return GetValue(RowsProperty);
		}
		set
		{
			SetValue(RowsProperty, value);
		}
	}

	public int Columns
	{
		get
		{
			return GetValue(ColumnsProperty);
		}
		set
		{
			SetValue(ColumnsProperty, value);
		}
	}

	public int FirstColumn
	{
		get
		{
			return GetValue(FirstColumnProperty);
		}
		set
		{
			SetValue(FirstColumnProperty, value);
		}
	}

	static UniformGrid()
	{
		RowsProperty = AvaloniaProperty.Register<UniformGrid, int>("Rows", 0);
		ColumnsProperty = AvaloniaProperty.Register<UniformGrid, int>("Columns", 0);
		FirstColumnProperty = AvaloniaProperty.Register<UniformGrid, int>("FirstColumn", 0);
		Layoutable.AffectsMeasure<UniformGrid>(new AvaloniaProperty[3] { RowsProperty, ColumnsProperty, FirstColumnProperty });
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		UpdateRowsAndColumns();
		double num = 0.0;
		double num2 = 0.0;
		Size availableSize2 = new Size(availableSize.Width / (double)_columns, availableSize.Height / (double)_rows);
		foreach (Control child in base.Children)
		{
			child.Measure(availableSize2);
			if (child.DesiredSize.Width > num)
			{
				num = child.DesiredSize.Width;
			}
			if (child.DesiredSize.Height > num2)
			{
				num2 = child.DesiredSize.Height;
			}
		}
		return new Size(num * (double)_columns, num2 * (double)_rows);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		int num = FirstColumn;
		int num2 = 0;
		double num3 = finalSize.Width / (double)_columns;
		double num4 = finalSize.Height / (double)_rows;
		foreach (Control child in base.Children)
		{
			if (child.IsVisible)
			{
				child.Arrange(new Rect((double)num * num3, (double)num2 * num4, num3, num4));
				num++;
				if (num >= _columns)
				{
					num = 0;
					num2++;
				}
			}
		}
		return finalSize;
	}

	private void UpdateRowsAndColumns()
	{
		_rows = Rows;
		_columns = Columns;
		if (FirstColumn >= Columns)
		{
			SetCurrentValue(FirstColumnProperty, 0);
		}
		int num = FirstColumn;
		foreach (Control child in base.Children)
		{
			if (child.IsVisible)
			{
				num++;
			}
		}
		if (_rows == 0)
		{
			if (_columns == 0)
			{
				_rows = (_columns = (int)Math.Ceiling(Math.Sqrt(num)));
				return;
			}
			_rows = Math.DivRem(num, _columns, out var result);
			if (result != 0)
			{
				_rows++;
			}
		}
		else if (_columns == 0)
		{
			_columns = Math.DivRem(num, _rows, out var result2);
			if (result2 != 0)
			{
				_columns++;
			}
		}
	}
}
