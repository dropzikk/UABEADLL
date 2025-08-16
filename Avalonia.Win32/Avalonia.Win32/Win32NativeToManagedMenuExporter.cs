using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Reactive;

namespace Avalonia.Win32;

internal class Win32NativeToManagedMenuExporter : INativeMenuExporter
{
	private NativeMenu? _nativeMenu;

	public void SetNativeMenu(NativeMenu? nativeMenu)
	{
		_nativeMenu = nativeMenu;
	}

	private static AvaloniaList<MenuItem> Populate(NativeMenu nativeMenu)
	{
		AvaloniaList<MenuItem> avaloniaList = new AvaloniaList<MenuItem>();
		foreach (NativeMenuItemBase item in nativeMenu.Items)
		{
			if (item is NativeMenuItemSeparator)
			{
				avaloniaList.Add(new MenuItem
				{
					Header = "-"
				});
			}
			else
			{
				if (!(item is NativeMenuItem nativeMenuItem))
				{
					continue;
				}
				MenuItem menuItem = new MenuItem
				{
					[!HeaderedSelectingItemsControl.HeaderProperty] = nativeMenuItem.GetObservable(NativeMenuItem.HeaderProperty).ToBinding(),
					[!MenuItem.IconProperty] = (from i in nativeMenuItem.GetObservable(NativeMenuItem.IconProperty)
						select (i == null) ? null : new Image
						{
							Source = i
						}).ToBinding(),
					[!InputElement.IsEnabledProperty] = nativeMenuItem.GetObservable(NativeMenuItem.IsEnabledProperty).ToBinding(),
					[!MenuItem.CommandProperty] = nativeMenuItem.GetObservable(NativeMenuItem.CommandProperty).ToBinding(),
					[!MenuItem.CommandParameterProperty] = nativeMenuItem.GetObservable(NativeMenuItem.CommandParameterProperty).ToBinding(),
					[!MenuItem.InputGestureProperty] = nativeMenuItem.GetObservable(NativeMenuItem.GestureProperty).ToBinding()
				};
				if (nativeMenuItem.Menu != null)
				{
					menuItem.ItemsSource = Populate(nativeMenuItem.Menu);
				}
				else if (nativeMenuItem.HasClickHandlers)
				{
					INativeMenuItemExporterEventsImplBridge bridge = nativeMenuItem;
					if (bridge != null)
					{
						menuItem.Click += delegate
						{
							bridge.RaiseClicked();
						};
					}
				}
				avaloniaList.Add(menuItem);
			}
		}
		return avaloniaList;
	}

	public AvaloniaList<MenuItem>? GetMenu()
	{
		if (_nativeMenu != null)
		{
			return Populate(_nativeMenu);
		}
		return null;
	}
}
