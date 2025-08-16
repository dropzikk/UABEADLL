using System.Collections;
using System.Linq;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Avalonia.Controls;

[TemplatePart("PART_ScrollViewer", typeof(IScrollable))]
public class ListBox : SelectingItemsControl
{
	private static readonly FuncTemplate<Panel?> DefaultPanel;

	public static readonly DirectProperty<ListBox, IScrollable?> ScrollProperty;

	public new static readonly DirectProperty<SelectingItemsControl, IList?> SelectedItemsProperty;

	public new static readonly DirectProperty<SelectingItemsControl, ISelectionModel> SelectionProperty;

	public new static readonly StyledProperty<SelectionMode> SelectionModeProperty;

	private IScrollable? _scroll;

	public IScrollable? Scroll
	{
		get
		{
			return _scroll;
		}
		private set
		{
			SetAndRaise(ScrollProperty, ref _scroll, value);
		}
	}

	public new IList? SelectedItems
	{
		get
		{
			return base.SelectedItems;
		}
		set
		{
			base.SelectedItems = value;
		}
	}

	public new ISelectionModel Selection
	{
		get
		{
			return base.Selection;
		}
		set
		{
			base.Selection = value;
		}
	}

	public new SelectionMode SelectionMode
	{
		get
		{
			return base.SelectionMode;
		}
		set
		{
			base.SelectionMode = value;
		}
	}

	static ListBox()
	{
		DefaultPanel = new FuncTemplate<Panel>(() => new VirtualizingStackPanel());
		ScrollProperty = AvaloniaProperty.RegisterDirect("Scroll", (ListBox o) => o.Scroll);
		SelectedItemsProperty = SelectingItemsControl.SelectedItemsProperty;
		SelectionProperty = SelectingItemsControl.SelectionProperty;
		SelectionModeProperty = SelectingItemsControl.SelectionModeProperty;
		ItemsControl.ItemsPanelProperty.OverrideDefaultValue<ListBox>(DefaultPanel);
		KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(typeof(ListBox), KeyboardNavigationMode.Once);
	}

	public void SelectAll()
	{
		Selection.SelectAll();
	}

	public void UnselectAll()
	{
		Selection.Clear();
	}

	protected internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return new ListBoxItem();
	}

	protected internal override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		return NeedsContainer<ListBoxItem>(item, out recycleKey);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		PlatformHotkeyConfiguration platformHotkeyConfiguration = Application.Current.PlatformSettings?.HotkeyConfiguration;
		bool flag = platformHotkeyConfiguration != null && e.KeyModifiers.HasAllFlags(platformHotkeyConfiguration.CommandModifiers);
		if (!flag)
		{
			NavigationDirection? navigationDirection = e.Key.ToNavigationDirection();
			if (navigationDirection.HasValue)
			{
				NavigationDirection valueOrDefault = navigationDirection.GetValueOrDefault();
				if (valueOrDefault.IsDirectional())
				{
					e.Handled |= MoveSelection(valueOrDefault, base.WrapSelection, e.KeyModifiers.HasAllFlags(KeyModifiers.Shift));
					goto IL_0136;
				}
			}
		}
		if (SelectionMode.HasAllFlags(SelectionMode.Multiple) && platformHotkeyConfiguration != null && platformHotkeyConfiguration.SelectAll.Any((KeyGesture x) => x.Matches(e)))
		{
			Selection.SelectAll();
			e.Handled = true;
		}
		else if (e.Key == Key.Space || e.Key == Key.Return)
		{
			UpdateSelectionFromEventSource(e.Source, select: true, e.KeyModifiers.HasFlag(KeyModifiers.Shift), flag);
		}
		goto IL_0136;
		IL_0136:
		base.OnKeyDown(e);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		Scroll = e.NameScope.Find<IScrollable>("PART_ScrollViewer");
	}

	internal bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
	{
		PlatformHotkeyConfiguration platformHotkeyConfiguration = Application.Current.PlatformSettings?.HotkeyConfiguration;
		bool toggleModifier = platformHotkeyConfiguration != null && e.KeyModifiers.HasAllFlags(platformHotkeyConfiguration.CommandModifiers);
		return UpdateSelectionFromEventSource(source, select: true, e.KeyModifiers.HasAllFlags(KeyModifiers.Shift), toggleModifier, e.GetCurrentPoint(source).Properties.IsRightButtonPressed);
	}
}
