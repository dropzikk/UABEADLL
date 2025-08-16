using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;

namespace Avalonia.Controls.Primitives;

public class TabStrip : SelectingItemsControl
{
	private static readonly FuncTemplate<Panel?> DefaultPanel;

	static TabStrip()
	{
		DefaultPanel = new FuncTemplate<Panel>(() => new WrapPanel
		{
			Orientation = Orientation.Horizontal
		});
		SelectingItemsControl.SelectionModeProperty.OverrideDefaultValue<TabStrip>(SelectionMode.AlwaysSelected);
		InputElement.FocusableProperty.OverrideDefaultValue(typeof(TabStrip), defaultValue: false);
		ItemsControl.ItemsPanelProperty.OverrideDefaultValue<TabStrip>(DefaultPanel);
	}

	protected internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return new TabStripItem();
	}

	protected internal override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		return NeedsContainer<TabStripItem>(item, out recycleKey);
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		if (e.NavigationMethod == NavigationMethod.Directional)
		{
			e.Handled = UpdateSelectionFromEventSource(e.Source);
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.Source is Visual relativeTo && e.GetCurrentPoint(relativeTo).Properties.IsLeftButtonPressed)
		{
			e.Handled = UpdateSelectionFromEventSource(e.Source);
		}
	}
}
