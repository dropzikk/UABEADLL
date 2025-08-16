using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Avalonia.Controls;

public class Menu : MenuBase, IMainMenu
{
	private static readonly FuncTemplate<Panel?> DefaultPanel;

	public Menu()
	{
	}

	public Menu(IMenuInteractionHandler interactionHandler)
		: base(interactionHandler)
	{
	}

	static Menu()
	{
		DefaultPanel = new FuncTemplate<Panel>(() => new StackPanel
		{
			Orientation = Orientation.Horizontal
		});
		ItemsControl.ItemsPanelProperty.OverrideDefaultValue(typeof(Menu), DefaultPanel);
		KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(typeof(Menu), KeyboardNavigationMode.Once);
		AutomationProperties.AccessibilityViewProperty.OverrideDefaultValue<Menu>(AccessibilityView.Control);
		AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<Menu>(AutomationControlType.Menu);
	}

	public override void Close()
	{
		if (!base.IsOpen)
		{
			return;
		}
		foreach (IMenuItem subItem in ((IMenuElement)this).SubItems)
		{
			subItem.Close();
		}
		base.IsOpen = false;
		base.SelectedIndex = -1;
		RaiseEvent(new RoutedEventArgs
		{
			RoutedEvent = MenuBase.ClosedEvent,
			Source = this
		});
	}

	public override void Open()
	{
		if (!base.IsOpen)
		{
			base.IsOpen = true;
			RaiseEvent(new RoutedEventArgs
			{
				RoutedEvent = MenuBase.OpenedEvent,
				Source = this
			});
		}
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		TopLevel topLevel = e.Root as TopLevel;
		if (topLevel?.AccessKeyHandler != null)
		{
			topLevel.AccessKeyHandler.MainMenu = this;
		}
	}

	protected internal override void PrepareContainerForItemOverride(Control element, object? item, int index)
	{
		base.PrepareContainerForItemOverride(element, item, index);
		if ((element as MenuItem)?.ItemContainerTheme == base.ItemContainerTheme)
		{
			element.ClearValue(ItemsControl.ItemContainerThemeProperty);
		}
	}
}
