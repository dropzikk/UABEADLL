using System;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Reactive;
using Avalonia.Rendering;
using Avalonia.Threading;

namespace Avalonia.Controls.Platform;

[Unstable]
public class DefaultMenuInteractionHandler : IMenuInteractionHandler
{
	private readonly bool _isContextMenu;

	private IDisposable? _inputManagerSubscription;

	private IRenderRoot? _root;

	protected Action<Action, TimeSpan> DelayRun { get; }

	protected IInputManager? InputManager { get; }

	internal IMenu? Menu { get; private set; }

	protected static TimeSpan MenuShowDelay { get; } = TimeSpan.FromMilliseconds(400.0);

	public DefaultMenuInteractionHandler(bool isContextMenu)
		: this(isContextMenu, Avalonia.Input.InputManager.Instance, DefaultDelayRun)
	{
	}

	public DefaultMenuInteractionHandler(bool isContextMenu, IInputManager? inputManager, Action<Action, TimeSpan> delayRun)
	{
		delayRun = delayRun ?? throw new ArgumentNullException("delayRun");
		_isContextMenu = isContextMenu;
		InputManager = inputManager;
		DelayRun = delayRun;
	}

	public void Attach(MenuBase menu)
	{
		AttachCore(menu);
	}

	public void Detach(MenuBase menu)
	{
		DetachCore(menu);
	}

	protected internal virtual void GotFocus(object? sender, GotFocusEventArgs e)
	{
		IMenuItem menuItemCore = GetMenuItemCore(e.Source as Control);
		if (menuItemCore?.Parent != null)
		{
			menuItemCore.SelectedItem = menuItemCore;
		}
	}

	protected internal virtual void LostFocus(object? sender, RoutedEventArgs e)
	{
		IMenuItem menuItemCore = GetMenuItemCore(e.Source as Control);
		if (menuItemCore != null)
		{
			menuItemCore.SelectedItem = null;
		}
	}

	protected internal virtual void KeyDown(object? sender, KeyEventArgs e)
	{
		KeyDown(GetMenuItemCore(e.Source as Control), e);
	}

	protected internal virtual void AccessKeyPressed(object? sender, RoutedEventArgs e)
	{
		IMenuItem menuItemCore = GetMenuItemCore(e.Source as Control);
		if (menuItemCore != null)
		{
			if (menuItemCore.HasSubMenu && menuItemCore.IsEffectivelyEnabled)
			{
				Open(menuItemCore, selectFirst: true);
			}
			else
			{
				Click(menuItemCore);
			}
			e.Handled = true;
		}
	}

	protected internal virtual void PointerEntered(object? sender, RoutedEventArgs e)
	{
		IMenuItem menuItemCore = GetMenuItemCore(e.Source as Control);
		if (menuItemCore?.Parent == null)
		{
			return;
		}
		if (menuItemCore.IsTopLevel)
		{
			if (menuItemCore != menuItemCore.Parent.SelectedItem)
			{
				IMenuItem? selectedItem = menuItemCore.Parent.SelectedItem;
				if (selectedItem != null && selectedItem.IsSubMenuOpen)
				{
					menuItemCore.Parent.SelectedItem.Close();
					SelectItemAndAncestors(menuItemCore);
					if (menuItemCore.HasSubMenu)
					{
						Open(menuItemCore, selectFirst: false);
					}
					return;
				}
			}
			SelectItemAndAncestors(menuItemCore);
			return;
		}
		SelectItemAndAncestors(menuItemCore);
		if (menuItemCore.HasSubMenu)
		{
			OpenWithDelay(menuItemCore);
		}
		else
		{
			if (menuItemCore.Parent == null)
			{
				return;
			}
			foreach (IMenuItem subItem in menuItemCore.Parent.SubItems)
			{
				if (subItem.IsSubMenuOpen)
				{
					CloseWithDelay(subItem);
				}
			}
		}
	}

	protected internal virtual void PointerMoved(object? sender, PointerEventArgs e)
	{
		if (!(GetMenuItemCore(e.Source as Control) is MenuItem menuItem))
		{
			return;
		}
		Matrix? matrix = menuItem?.CompositionVisual?.TryGetServerGlobalTransform();
		if (matrix.HasValue)
		{
			PointerPoint currentPoint = e.GetCurrentPoint(null);
			Point p = currentPoint.Position.Transform(matrix.Value);
			if (currentPoint.Properties.IsLeftButtonPressed && !new Rect(menuItem.Bounds.Size).Contains(p))
			{
				e.Pointer.Capture(null);
			}
		}
	}

	protected internal virtual void PointerExited(object? sender, RoutedEventArgs e)
	{
		IMenuItem item = GetMenuItemCore(e.Source as Control);
		if (item?.Parent == null || item.Parent.SelectedItem != item)
		{
			return;
		}
		if (item.IsTopLevel)
		{
			if (!((IMenu)item.Parent).IsOpen)
			{
				item.Parent.SelectedItem = null;
			}
		}
		else if (!item.HasSubMenu)
		{
			item.Parent.SelectedItem = null;
		}
		else
		{
			if (item.IsPointerOverSubMenu)
			{
				return;
			}
			DelayRun(delegate
			{
				if (!item.IsPointerOverSubMenu)
				{
					item.IsSubMenuOpen = false;
				}
			}, MenuShowDelay);
		}
	}

	protected internal virtual void PointerPressed(object? sender, PointerPressedEventArgs e)
	{
		IMenuItem menuItemCore = GetMenuItemCore(e.Source as Control);
		if (!(sender is Visual relativeTo) || !e.GetCurrentPoint(relativeTo).Properties.IsLeftButtonPressed || menuItemCore == null || !menuItemCore.HasSubMenu)
		{
			return;
		}
		if (menuItemCore.IsSubMenuOpen)
		{
			Popup popup = (e.Source as ILogical)?.FindLogicalAncestorOfType<Popup>();
			if (menuItemCore.IsTopLevel && popup == null)
			{
				CloseMenu(menuItemCore);
			}
		}
		else
		{
			if (menuItemCore.IsTopLevel && menuItemCore.Parent is IMainMenu mainMenu)
			{
				mainMenu.Open();
			}
			Open(menuItemCore, selectFirst: false);
		}
		e.Handled = true;
	}

	protected internal virtual void PointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		IMenuItem menuItemCore = GetMenuItemCore(e.Source as Control);
		if (e.InitialPressMouseButton == MouseButton.Left && menuItemCore != null && !menuItemCore.HasSubMenu)
		{
			Click(menuItemCore);
			e.Handled = true;
		}
	}

	protected internal virtual void MenuOpened(object? sender, RoutedEventArgs e)
	{
		if (e.Source is Menu)
		{
			Menu?.MoveSelection(NavigationDirection.First, wrap: true);
		}
	}

	protected internal virtual void RawInput(RawInputEventArgs e)
	{
		RawPointerEventArgs obj = e as RawPointerEventArgs;
		if (obj != null && obj.Type == RawPointerEventType.NonClientLeftButtonDown)
		{
			Menu?.Close();
		}
	}

	protected internal virtual void RootPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		IMenu? menu = Menu;
		if (menu != null && menu.IsOpen && e.Source is ILogical target && !Menu.IsLogicalAncestorOf(target))
		{
			Menu.Close();
		}
	}

	protected internal virtual void WindowDeactivated(object? sender, EventArgs e)
	{
		Menu?.Close();
	}

	internal static MenuItem? GetMenuItem(StyledElement? item)
	{
		return (MenuItem)GetMenuItemCore(item);
	}

	internal void AttachCore(IMenu menu)
	{
		if (Menu != null)
		{
			throw new NotSupportedException("DefaultMenuInteractionHandler is already attached.");
		}
		Menu = menu;
		Menu.GotFocus += GotFocus;
		Menu.LostFocus += LostFocus;
		Menu.KeyDown += KeyDown;
		Menu.PointerPressed += PointerPressed;
		Menu.PointerReleased += PointerReleased;
		Menu.AddHandler(AccessKeyHandler.AccessKeyPressedEvent, new Action<object, RoutedEventArgs>(AccessKeyPressed));
		Menu.AddHandler(MenuBase.OpenedEvent, new Action<object, RoutedEventArgs>(MenuOpened));
		Menu.AddHandler(MenuItem.PointerEnteredItemEvent, new Action<object, RoutedEventArgs>(PointerEntered));
		Menu.AddHandler(MenuItem.PointerExitedItemEvent, new Action<object, RoutedEventArgs>(PointerExited));
		Menu.AddHandler(InputElement.PointerMovedEvent, new Action<object, PointerEventArgs>(PointerMoved));
		_root = Menu.VisualRoot;
		if (_root is InputElement inputElement)
		{
			inputElement.AddHandler(InputElement.PointerPressedEvent, RootPointerPressed, RoutingStrategies.Tunnel);
		}
		if (_root is WindowBase windowBase)
		{
			windowBase.Deactivated += WindowDeactivated;
		}
		if (_root is TopLevel topLevel)
		{
			ITopLevelImpl platformImpl = topLevel.PlatformImpl;
			if (platformImpl != null)
			{
				platformImpl.LostFocus = (Action)Delegate.Combine(platformImpl.LostFocus, new Action(TopLevelLostPlatformFocus));
			}
		}
		_inputManagerSubscription = InputManager?.Process.Subscribe(RawInput);
	}

	internal void DetachCore(IMenu menu)
	{
		if (Menu != menu)
		{
			throw new NotSupportedException("DefaultMenuInteractionHandler is not attached to the menu.");
		}
		Menu.GotFocus -= GotFocus;
		Menu.LostFocus -= LostFocus;
		Menu.KeyDown -= KeyDown;
		Menu.PointerPressed -= PointerPressed;
		Menu.PointerReleased -= PointerReleased;
		Menu.RemoveHandler(AccessKeyHandler.AccessKeyPressedEvent, new Action<object, RoutedEventArgs>(AccessKeyPressed));
		Menu.RemoveHandler(MenuBase.OpenedEvent, new Action<object, RoutedEventArgs>(MenuOpened));
		Menu.RemoveHandler(MenuItem.PointerEnteredItemEvent, new Action<object, RoutedEventArgs>(PointerEntered));
		Menu.RemoveHandler(MenuItem.PointerExitedItemEvent, new Action<object, RoutedEventArgs>(PointerExited));
		Menu.RemoveHandler(InputElement.PointerMovedEvent, new Action<object, PointerEventArgs>(PointerMoved));
		if (_root is InputElement inputElement)
		{
			inputElement.RemoveHandler(InputElement.PointerPressedEvent, RootPointerPressed);
		}
		if (_root is WindowBase windowBase)
		{
			windowBase.Deactivated -= WindowDeactivated;
		}
		if (_root is TopLevel { PlatformImpl: not null } topLevel)
		{
			ITopLevelImpl? platformImpl = topLevel.PlatformImpl;
			platformImpl.LostFocus = (Action)Delegate.Remove(platformImpl.LostFocus, new Action(TopLevelLostPlatformFocus));
		}
		_inputManagerSubscription?.Dispose();
		Menu = null;
		_root = null;
	}

	internal void Click(IMenuItem item)
	{
		item.RaiseClick();
		if (!item.StaysOpenOnClick)
		{
			CloseMenu(item);
		}
	}

	internal void CloseMenu(IMenuItem item)
	{
		IMenuElement menuElement = item;
		while (menuElement != null && !(menuElement is IMenu))
		{
			menuElement = (menuElement as IMenuItem)?.Parent;
		}
		menuElement?.Close();
	}

	internal void CloseWithDelay(IMenuItem item)
	{
		DelayRun(Execute, MenuShowDelay);
		void Execute()
		{
			if (item.Parent?.SelectedItem != item)
			{
				item.Close();
			}
		}
	}

	internal void KeyDown(IMenuItem? item, KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Up:
		case Key.Down:
			if (item != null && item.IsTopLevel && item.HasSubMenu)
			{
				if (!item.IsSubMenuOpen)
				{
					Open(item, selectFirst: true);
				}
				else
				{
					item.MoveSelection(NavigationDirection.First, wrap: true);
				}
				e.Handled = true;
				break;
			}
			goto default;
		case Key.Left:
			if (item != null && item.IsSubMenuOpen && item.SelectedItem == null)
			{
				item.Close();
				break;
			}
			if (item?.Parent is IMenuItem { IsTopLevel: false, IsSubMenuOpen: not false } menuItem)
			{
				menuItem.Close();
				menuItem.Focus();
				e.Handled = true;
				break;
			}
			goto default;
		case Key.Right:
			if (item != null && !item.IsTopLevel && item.HasSubMenu)
			{
				Open(item, selectFirst: true);
				e.Handled = true;
				break;
			}
			goto default;
		case Key.Return:
			if (item != null)
			{
				if (!item.HasSubMenu)
				{
					Click(item);
				}
				else
				{
					Open(item, selectFirst: true);
				}
				e.Handled = true;
			}
			break;
		case Key.Escape:
		{
			IMenuElement menuElement = item?.Parent;
			if (menuElement != null)
			{
				menuElement.Close();
				menuElement.Focus();
			}
			else
			{
				Menu.Close();
			}
			e.Handled = true;
			break;
		}
		default:
		{
			NavigationDirection? navigationDirection = e.Key.ToNavigationDirection();
			if (!navigationDirection.HasValue || !navigationDirection.GetValueOrDefault().IsDirectional())
			{
				break;
			}
			if (item == null && _isContextMenu)
			{
				if (Menu.MoveSelection(navigationDirection.Value, wrap: true))
				{
					e.Handled = true;
				}
			}
			else if (item != null && item.Parent?.MoveSelection(navigationDirection.Value, wrap: true) == true)
			{
				if (item.IsSubMenuOpen && item.Parent is IMenu && item.Parent.SelectedItem != null && item.Parent.SelectedItem != item)
				{
					item.Close();
					Open(item.Parent.SelectedItem, selectFirst: true);
				}
				e.Handled = true;
			}
			break;
		}
		}
		if (!e.Handled && item?.Parent is IMenuItem item2)
		{
			KeyDown(item2, e);
		}
	}

	internal void Open(IMenuItem item, bool selectFirst)
	{
		item.Open();
		if (selectFirst)
		{
			item.MoveSelection(NavigationDirection.First, wrap: true);
		}
	}

	internal void OpenWithDelay(IMenuItem item)
	{
		DelayRun(Execute, MenuShowDelay);
		void Execute()
		{
			if (item.Parent?.SelectedItem == item)
			{
				Open(item, selectFirst: false);
			}
		}
	}

	internal void SelectItemAndAncestors(IMenuItem item)
	{
		IMenuItem menuItem = item;
		while (menuItem?.Parent != null)
		{
			menuItem.Parent.SelectedItem = menuItem;
			menuItem = menuItem.Parent as IMenuItem;
		}
	}

	internal static IMenuItem? GetMenuItemCore(StyledElement? item)
	{
		IMenuItem menuItem;
		while (true)
		{
			if (item == null)
			{
				return null;
			}
			menuItem = item as IMenuItem;
			if (menuItem != null)
			{
				break;
			}
			item = item.Parent;
		}
		return menuItem;
	}

	private void TopLevelLostPlatformFocus()
	{
		Menu?.Close();
	}

	private static void DefaultDelayRun(Action action, TimeSpan timeSpan)
	{
		DispatcherTimer.RunOnce(action, timeSpan);
	}
}
