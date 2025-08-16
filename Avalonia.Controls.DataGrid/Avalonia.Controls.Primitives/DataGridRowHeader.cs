using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Avalonia.Controls.Primitives;

[TemplatePart("PART_Root", typeof(Control))]
[PseudoClasses(new string[] { ":invalid", ":selected", ":editing", ":current" })]
public class DataGridRowHeader : ContentControl
{
	private const string DATAGRIDROWHEADER_elementRootName = "PART_Root";

	private Control _rootElement;

	public static readonly StyledProperty<IBrush> SeparatorBrushProperty = AvaloniaProperty.Register<DataGridRowHeader, IBrush>("SeparatorBrush");

	public static readonly StyledProperty<bool> AreSeparatorsVisibleProperty = AvaloniaProperty.Register<DataGridRowHeader, bool>("AreSeparatorsVisible", defaultValue: false);

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

	internal Control Owner { get; set; }

	private DataGridRow OwningRow => Owner as DataGridRow;

	private DataGridRowGroupHeader OwningRowGroupHeader => Owner as DataGridRowGroupHeader;

	private DataGrid OwningGrid
	{
		get
		{
			if (OwningRow != null)
			{
				return OwningRow.OwningGrid;
			}
			if (OwningRowGroupHeader != null)
			{
				return OwningRowGroupHeader.OwningGrid;
			}
			return null;
		}
	}

	private int Slot
	{
		get
		{
			if (OwningRow != null)
			{
				return OwningRow.Slot;
			}
			if (OwningRowGroupHeader != null)
			{
				return OwningRowGroupHeader.RowGroupInfo.Slot;
			}
			return -1;
		}
	}

	public DataGridRowHeader()
	{
		AddHandler(InputElement.PointerPressedEvent, DataGridRowHeader_PointerPressed, RoutingStrategies.Direct | RoutingStrategies.Bubble, handledEventsToo: true);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_rootElement = e.NameScope.Find<Control>("PART_Root");
		if (_rootElement != null)
		{
			UpdatePseudoClasses();
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (OwningRow == null || OwningGrid == null)
		{
			return base.MeasureOverride(availableSize);
		}
		double height = (double.IsNaN(OwningGrid.RowHeight) ? availableSize.Height : OwningGrid.RowHeight);
		double width = (double.IsNaN(OwningGrid.RowHeaderWidth) ? availableSize.Width : OwningGrid.RowHeaderWidth);
		Size result = base.MeasureOverride(new Size(width, height));
		if (!double.IsNaN(OwningGrid.RowHeaderWidth) || result.Width < OwningGrid.ActualRowHeaderWidth)
		{
			return new Size(OwningGrid.ActualRowHeaderWidth, result.Height);
		}
		return result;
	}

	internal void UpdatePseudoClasses()
	{
		if (_rootElement == null || Owner == null || !Owner.IsVisible)
		{
			return;
		}
		if (OwningRow != null)
		{
			base.PseudoClasses.Set(":invalid", !OwningRow.IsValid);
			base.PseudoClasses.Set(":selected", OwningRow.IsSelected);
			base.PseudoClasses.Set(":editing", OwningRow.IsEditing);
			if (OwningGrid != null)
			{
				base.PseudoClasses.Set(":current", OwningRow.Slot == OwningGrid.CurrentSlot);
			}
		}
		else if (OwningRowGroupHeader != null && OwningGrid != null)
		{
			base.PseudoClasses.Set(":current", OwningRowGroupHeader.RowGroupInfo.Slot == OwningGrid.CurrentSlot);
		}
	}

	protected override void OnPointerEntered(PointerEventArgs e)
	{
		if (OwningRow != null)
		{
			OwningRow.IsMouseOver = true;
		}
		base.OnPointerEntered(e);
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		if (OwningRow != null)
		{
			OwningRow.IsMouseOver = false;
		}
		base.OnPointerExited(e);
	}

	private void DataGridRowHeader_PointerPressed(object sender, PointerPressedEventArgs e)
	{
		if (OwningGrid == null)
		{
			return;
		}
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			if (!e.Handled)
			{
				OwningGrid.Focus();
			}
			if (OwningRow != null)
			{
				e.Handled = OwningGrid.UpdateStateOnMouseLeftButtonDown(e, -1, Slot, allowEdit: false);
				OwningGrid.UpdatedStateOnMouseLeftButtonDown = true;
			}
		}
		else if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
		{
			if (!e.Handled)
			{
				OwningGrid.Focus();
			}
			if (OwningRow != null)
			{
				e.Handled = OwningGrid.UpdateStateOnMouseRightButtonDown(e, -1, Slot, allowEdit: false);
			}
		}
	}
}
