using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace Avalonia.Controls;

public abstract class MenuBase : SelectingItemsControl, IFocusScope, IMenu, IMenuElement, IInputElement, ILogical
{
	public static readonly DirectProperty<MenuBase, bool> IsOpenProperty;

	public static readonly RoutedEvent<RoutedEventArgs> OpenedEvent;

	public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent;

	private bool _isOpen;

	public bool IsOpen
	{
		get
		{
			return _isOpen;
		}
		protected set
		{
			SetAndRaise(IsOpenProperty, ref _isOpen, value);
		}
	}

	IMenuInteractionHandler IMenu.InteractionHandler => InteractionHandler;

	IRenderRoot? IMenu.VisualRoot => base.VisualRoot;

	IMenuItem? IMenuElement.SelectedItem
	{
		get
		{
			int selectedIndex = base.SelectedIndex;
			if (selectedIndex == -1)
			{
				return null;
			}
			return (IMenuItem)ContainerFromIndex(selectedIndex);
		}
		set
		{
			base.SelectedIndex = ((value is Control container) ? IndexFromContainer(container) : (-1));
		}
	}

	IEnumerable<IMenuItem> IMenuElement.SubItems => GetRealizedContainers().OfType<IMenuItem>();

	protected IMenuInteractionHandler InteractionHandler { get; }

	public event EventHandler<RoutedEventArgs>? Opened
	{
		add
		{
			AddHandler(OpenedEvent, value);
		}
		remove
		{
			RemoveHandler(OpenedEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? Closed
	{
		add
		{
			AddHandler(ClosedEvent, value);
		}
		remove
		{
			RemoveHandler(ClosedEvent, value);
		}
	}

	protected MenuBase()
	{
		InteractionHandler = new DefaultMenuInteractionHandler(isContextMenu: false);
	}

	protected MenuBase(IMenuInteractionHandler interactionHandler)
	{
		InteractionHandler = interactionHandler ?? throw new ArgumentNullException("interactionHandler");
	}

	static MenuBase()
	{
		IsOpenProperty = AvaloniaProperty.RegisterDirect("IsOpen", (MenuBase o) => o.IsOpen, null, unsetValue: false);
		OpenedEvent = RoutedEvent.Register<MenuBase, RoutedEventArgs>("Opened", RoutingStrategies.Bubble);
		ClosedEvent = RoutedEvent.Register<MenuBase, RoutedEventArgs>("Closed", RoutingStrategies.Bubble);
		MenuItem.SubmenuOpenedEvent.AddClassHandler(delegate(MenuBase x, RoutedEventArgs e)
		{
			x.OnSubmenuOpened(e);
		});
	}

	public abstract void Close();

	public abstract void Open();

	bool IMenuElement.MoveSelection(NavigationDirection direction, bool wrap)
	{
		return MoveSelection(direction, wrap);
	}

	protected internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return new MenuItem();
	}

	protected internal override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		if (item is MenuItem || item is Separator)
		{
			recycleKey = null;
			return false;
		}
		recycleKey = ItemsControl.DefaultRecycleKey;
		return true;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		InteractionHandler.Attach(this);
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		InteractionHandler.Detach(this);
	}

	protected virtual void OnSubmenuOpened(RoutedEventArgs e)
	{
		if (e.Source is MenuItem menuItem && menuItem.Parent == this)
		{
			foreach (MenuItem item in this.GetLogicalChildren().OfType<MenuItem>())
			{
				if (item != menuItem && item.IsSubMenuOpen)
				{
					item.IsSubMenuOpen = false;
				}
			}
		}
		IsOpen = true;
	}
}
