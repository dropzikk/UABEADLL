using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Avalonia.Controls;

[PseudoClasses(new string[] { ":pressed", ":selected" })]
public class ListBoxItem : ContentControl, ISelectable
{
	public static readonly StyledProperty<bool> IsSelectedProperty;

	private static readonly Point s_invalidPoint;

	private Point _pointerDownPoint = s_invalidPoint;

	public bool IsSelected
	{
		get
		{
			return GetValue(IsSelectedProperty);
		}
		set
		{
			SetValue(IsSelectedProperty, value);
		}
	}

	static ListBoxItem()
	{
		IsSelectedProperty = SelectingItemsControl.IsSelectedProperty.AddOwner<ListBoxItem>();
		s_invalidPoint = new Point(double.NaN, double.NaN);
		SelectableMixin.Attach<ListBoxItem>(IsSelectedProperty);
		PressedMixin.Attach<ListBoxItem>();
		InputElement.FocusableProperty.OverrideDefaultValue<ListBoxItem>(defaultValue: true);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ListItemAutomationPeer(this);
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		_pointerDownPoint = s_invalidPoint;
		if (e.Handled || e.Handled || !(ItemsControl.ItemsControlFromItemContaner(this) is ListBox listBox))
		{
			return;
		}
		PointerPoint currentPoint = e.GetCurrentPoint(this);
		PointerUpdateKind pointerUpdateKind = currentPoint.Properties.PointerUpdateKind;
		if (pointerUpdateKind == PointerUpdateKind.LeftButtonPressed || pointerUpdateKind == PointerUpdateKind.RightButtonPressed)
		{
			if (currentPoint.Pointer.Type == PointerType.Mouse)
			{
				e.Handled = listBox.UpdateSelectionFromPointerEvent(this, e);
			}
			else
			{
				_pointerDownPoint = currentPoint.Position;
			}
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (!e.Handled && !double.IsNaN(_pointerDownPoint.X))
		{
			MouseButton initialPressMouseButton = e.InitialPressMouseButton;
			if (initialPressMouseButton == MouseButton.Left || initialPressMouseButton == MouseButton.Right)
			{
				PointerPoint currentPoint = e.GetCurrentPoint(this);
				Size size = (TopLevel.GetTopLevel(e.Source as Visual)?.PlatformSettings)?.GetTapSize(currentPoint.Pointer.Type) ?? new Size(4.0, 4.0);
				Rect rect = new Rect(_pointerDownPoint, default(Size)).Inflate(new Thickness(size.Width, size.Height));
				if (new Rect(base.Bounds.Size).ContainsExclusive(currentPoint.Position) && rect.ContainsExclusive(currentPoint.Position) && ItemsControl.ItemsControlFromItemContaner(this) is ListBox listBox && listBox.UpdateSelectionFromPointerEvent(this, e))
				{
					e.Handled = true;
				}
			}
		}
		_pointerDownPoint = s_invalidPoint;
	}
}
