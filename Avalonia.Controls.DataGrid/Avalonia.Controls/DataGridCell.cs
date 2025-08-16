using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

[TemplatePart("PART_RightGridLine", typeof(Rectangle))]
[PseudoClasses(new string[] { ":selected", ":current", ":edited", ":invalid", ":focus" })]
public class DataGridCell : ContentControl
{
	private const string DATAGRIDCELL_elementRightGridLine = "PART_RightGridLine";

	private Rectangle _rightGridLine;

	private DataGridColumn _owningColumn;

	private bool _isValid = true;

	public static readonly DirectProperty<DataGridCell, bool> IsValidProperty;

	public bool IsValid
	{
		get
		{
			return _isValid;
		}
		internal set
		{
			SetAndRaise(IsValidProperty, ref _isValid, value);
		}
	}

	internal DataGridColumn OwningColumn
	{
		get
		{
			return _owningColumn;
		}
		set
		{
			if (_owningColumn != value)
			{
				_owningColumn = value;
				OnOwningColumnSet(value);
			}
		}
	}

	internal DataGridRow OwningRow { get; set; }

	internal DataGrid OwningGrid
	{
		get
		{
			object obj = OwningRow?.OwningGrid;
			if (obj == null)
			{
				DataGridColumn owningColumn = OwningColumn;
				if (owningColumn == null)
				{
					return null;
				}
				obj = owningColumn.OwningGrid;
			}
			return (DataGrid)obj;
		}
	}

	internal double ActualRightGridLineWidth => _rightGridLine?.Bounds.Width ?? 0.0;

	internal int ColumnIndex => OwningColumn?.Index ?? (-1);

	internal int RowIndex => OwningRow?.Index ?? (-1);

	internal bool IsCurrent
	{
		get
		{
			if (OwningGrid.CurrentColumnIndex == OwningColumn.Index)
			{
				return OwningGrid.CurrentSlot == OwningRow.Slot;
			}
			return false;
		}
	}

	private bool IsEdited
	{
		get
		{
			if (OwningGrid.EditingRow == OwningRow)
			{
				return OwningGrid.EditingColumnIndex == ColumnIndex;
			}
			return false;
		}
	}

	private bool IsMouseOver
	{
		get
		{
			if (OwningRow != null)
			{
				return OwningRow.MouseOverColumnIndex == ColumnIndex;
			}
			return false;
		}
		set
		{
			if (value != IsMouseOver)
			{
				if (value)
				{
					OwningRow.MouseOverColumnIndex = ColumnIndex;
				}
				else
				{
					OwningRow.MouseOverColumnIndex = null;
				}
			}
		}
	}

	static DataGridCell()
	{
		IsValidProperty = AvaloniaProperty.RegisterDirect("IsValid", (DataGridCell o) => o.IsValid, null, unsetValue: false);
		InputElement.PointerPressedEvent.AddClassHandler(delegate(DataGridCell x, PointerPressedEventArgs e)
		{
			x.DataGridCell_PointerPressed(e);
		}, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true);
		InputElement.FocusableProperty.OverrideDefaultValue<DataGridCell>(defaultValue: true);
		InputElement.IsTabStopProperty.OverrideDefaultValue<DataGridCell>(defaultValue: false);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		UpdatePseudoClasses();
		_rightGridLine = e.NameScope.Find<Rectangle>("PART_RightGridLine");
		if (_rightGridLine != null && OwningColumn == null)
		{
			_rightGridLine.IsVisible = false;
		}
		else
		{
			EnsureGridLine(null);
		}
	}

	protected override void OnPointerEntered(PointerEventArgs e)
	{
		base.OnPointerEntered(e);
		if (OwningRow != null)
		{
			IsMouseOver = true;
		}
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		base.OnPointerExited(e);
		if (OwningRow != null)
		{
			IsMouseOver = false;
		}
	}

	private void DataGridCell_PointerPressed(PointerPressedEventArgs e)
	{
		if (OwningGrid == null)
		{
			return;
		}
		OwningGrid.OnCellPointerPressed(new DataGridCellPointerPressedEventArgs(this, OwningRow, OwningColumn, e));
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			if (!e.Handled && OwningGrid.IsTabStop)
			{
				OwningGrid.Focus();
			}
			if (OwningRow != null)
			{
				bool handled = OwningGrid.UpdateStateOnMouseLeftButtonDown(e, ColumnIndex, OwningRow.Slot, !e.Handled);
				if (e.Pointer.Type != PointerType.Touch && e.Pointer.Type != PointerType.Pen)
				{
					e.Handled = handled;
				}
				OwningGrid.UpdatedStateOnMouseLeftButtonDown = true;
			}
		}
		else if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
		{
			if (!e.Handled && OwningGrid.IsTabStop)
			{
				OwningGrid.Focus();
			}
			if (OwningRow != null)
			{
				e.Handled = OwningGrid.UpdateStateOnMouseRightButtonDown(e, ColumnIndex, OwningRow.Slot, !e.Handled);
			}
		}
	}

	internal void UpdatePseudoClasses()
	{
		if (OwningGrid != null && OwningColumn != null && OwningRow != null && OwningRow.IsVisible && OwningRow.Slot != -1)
		{
			base.PseudoClasses.Set(":selected", OwningRow.IsSelected);
			base.PseudoClasses.Set(":current", IsCurrent);
			base.PseudoClasses.Set(":edited", IsEdited);
			base.PseudoClasses.Set(":invalid", !IsValid);
			base.PseudoClasses.Set(":focus", OwningGrid.IsFocused && IsCurrent);
		}
	}

	internal void EnsureGridLine(DataGridColumn lastVisibleColumn)
	{
		if (OwningGrid != null && _rightGridLine != null)
		{
			if (OwningGrid.VerticalGridLinesBrush != null && OwningGrid.VerticalGridLinesBrush != _rightGridLine.Fill)
			{
				_rightGridLine.Fill = OwningGrid.VerticalGridLinesBrush;
			}
			bool flag = (OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.Vertical || OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.All) && (OwningGrid.ColumnsInternal.FillerColumn.IsActive || OwningColumn != lastVisibleColumn);
			if (flag != _rightGridLine.IsVisible)
			{
				_rightGridLine.IsVisible = flag;
			}
		}
	}

	private void OnOwningColumnSet(DataGridColumn column)
	{
		if (column == null)
		{
			base.Classes.Clear();
			ClearValue(StyledElement.ThemeProperty);
			return;
		}
		if (base.Theme != column.CellTheme)
		{
			base.Theme = column.CellTheme;
		}
		base.Classes.Replace(column.CellStyleClasses);
	}
}
