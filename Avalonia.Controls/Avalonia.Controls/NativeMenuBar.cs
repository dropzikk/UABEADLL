using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class NativeMenuBar : TemplatedControl
{
	public static readonly AttachedProperty<bool> EnableMenuItemClickForwardingProperty;

	static NativeMenuBar()
	{
		EnableMenuItemClickForwardingProperty = AvaloniaProperty.RegisterAttached<NativeMenuBar, MenuItem, bool>("EnableMenuItemClickForwarding", defaultValue: false);
		EnableMenuItemClickForwardingProperty.Changed.Subscribe(delegate(AvaloniaPropertyChangedEventArgs<bool> args)
		{
			MenuItem menuItem = (MenuItem)args.Sender;
			if (args.NewValue.GetValueOrDefault<bool>())
			{
				menuItem.Click += OnMenuItemClick;
			}
			else
			{
				menuItem.Click -= OnMenuItemClick;
			}
		});
	}

	public static void SetEnableMenuItemClickForwarding(MenuItem menuItem, bool enable)
	{
		menuItem.SetValue(EnableMenuItemClickForwardingProperty, enable);
	}

	private static void OnMenuItemClick(object? sender, RoutedEventArgs e)
	{
		(((MenuItem)sender).DataContext as INativeMenuItemExporterEventsImplBridge)?.RaiseClicked();
	}
}
