using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls.Mixins;

public static class PressedMixin
{
	public static void Attach<TControl>() where TControl : Control
	{
		InputElement.PointerPressedEvent.AddClassHandler(delegate(TControl x, PointerPressedEventArgs e)
		{
			HandlePointerPressed(x, e);
		}, RoutingStrategies.Tunnel);
		InputElement.PointerReleasedEvent.AddClassHandler(delegate(TControl x, PointerReleasedEventArgs e)
		{
			HandlePointerReleased(x);
		}, RoutingStrategies.Tunnel);
		InputElement.PointerCaptureLostEvent.AddClassHandler(delegate(TControl x, PointerCaptureLostEventArgs e)
		{
			HandlePointerReleased(x);
		}, RoutingStrategies.Tunnel);
	}

	private static void HandlePointerPressed<TControl>(TControl sender, PointerPressedEventArgs e) where TControl : Control
	{
		if (e.GetCurrentPoint(sender).Properties.IsLeftButtonPressed)
		{
			PseudolassesExtensions.Set(sender.Classes, ":pressed", value: true);
		}
	}

	private static void HandlePointerReleased<TControl>(TControl sender) where TControl : Control
	{
		PseudolassesExtensions.Set(sender.Classes, ":pressed", value: false);
	}
}
