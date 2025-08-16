using System;
using System.Collections.Generic;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace Avalonia.Input;

[NotClientImplementable]
public interface IInputElement
{
	bool Focusable { get; }

	bool IsEnabled { get; }

	Cursor? Cursor { get; }

	bool IsEffectivelyEnabled { get; }

	bool IsEffectivelyVisible { get; }

	bool IsKeyboardFocusWithin { get; }

	bool IsFocused { get; }

	bool IsHitTestVisible { get; }

	bool IsPointerOver { get; }

	List<KeyBinding> KeyBindings { get; }

	event EventHandler<GotFocusEventArgs>? GotFocus;

	event EventHandler<RoutedEventArgs>? LostFocus;

	event EventHandler<KeyEventArgs>? KeyDown;

	event EventHandler<KeyEventArgs>? KeyUp;

	event EventHandler<TextInputEventArgs>? TextInput;

	event EventHandler<PointerEventArgs>? PointerEntered;

	event EventHandler<PointerEventArgs>? PointerExited;

	event EventHandler<PointerPressedEventArgs>? PointerPressed;

	event EventHandler<PointerEventArgs>? PointerMoved;

	event EventHandler<PointerReleasedEventArgs>? PointerReleased;

	event EventHandler<PointerWheelEventArgs>? PointerWheelChanged;

	bool Focus(NavigationMethod method = NavigationMethod.Unspecified, KeyModifiers keyModifiers = KeyModifiers.None);

	void AddHandler(RoutedEvent routedEvent, Delegate handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false);

	void RemoveHandler(RoutedEvent routedEvent, Delegate handler);

	void RaiseEvent(RoutedEventArgs e);
}
