using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace Avalonia.Input;

[PrivateApi]
public class FocusManager : IFocusManager
{
	private readonly ConditionalWeakTable<IFocusScope, IInputElement?> _focusScopes = new ConditionalWeakTable<IFocusScope, IInputElement>();

	private IInputElement? Current => KeyboardDevice.Instance?.FocusedElement;

	public IFocusScope? Scope { get; private set; }

	static FocusManager()
	{
		InputElement.PointerPressedEvent.AddClassHandler(typeof(IInputElement), OnPreviewPointerPressed, RoutingStrategies.Tunnel);
	}

	public IInputElement? GetFocusedElement()
	{
		return Current;
	}

	public bool Focus(IInputElement? control, NavigationMethod method = NavigationMethod.Unspecified, KeyModifiers keyModifiers = KeyModifiers.None)
	{
		if (control != null)
		{
			IFocusScope focusScope = GetFocusScopeAncestors(control).FirstOrDefault();
			if (focusScope != null)
			{
				Scope = focusScope;
				return SetFocusedElement(focusScope, control, method, keyModifiers);
			}
		}
		else if (Current != null)
		{
			IFocusScope[] array = GetFocusScopeAncestors(Current).Reverse().ToArray();
			foreach (IFocusScope focusScope2 in array)
			{
				if (focusScope2 != Scope && _focusScopes.TryGetValue(focusScope2, out IInputElement value) && value != null)
				{
					return Focus(value, method);
				}
			}
			if (Scope != null)
			{
				return SetFocusedElement(Scope, null);
			}
		}
		return false;
	}

	public void ClearFocus()
	{
		Focus(null);
	}

	public IInputElement? GetFocusedElement(IFocusScope scope)
	{
		_focusScopes.TryGetValue(scope, out IInputElement value);
		return value;
	}

	public bool SetFocusedElement(IFocusScope scope, IInputElement? element, NavigationMethod method = NavigationMethod.Unspecified, KeyModifiers keyModifiers = KeyModifiers.None)
	{
		scope = scope ?? throw new ArgumentNullException("scope");
		if (element != null && !CanFocus(element))
		{
			return false;
		}
		if (_focusScopes.TryGetValue(scope, out IInputElement value))
		{
			if (element != value)
			{
				_focusScopes.Remove(scope);
				_focusScopes.Add(scope, element);
			}
		}
		else
		{
			_focusScopes.Add(scope, element);
		}
		if (Scope == scope)
		{
			KeyboardDevice.Instance?.SetFocusedElement(element, method, keyModifiers);
		}
		return true;
	}

	public void SetFocusScope(IFocusScope scope)
	{
		scope = scope ?? throw new ArgumentNullException("scope");
		if (!_focusScopes.TryGetValue(scope, out IInputElement value))
		{
			value = scope as IInputElement;
			_focusScopes.Add(scope, value);
		}
		Scope = scope;
		Focus(value);
	}

	public void RemoveFocusScope(IFocusScope scope)
	{
		scope = scope ?? throw new ArgumentNullException("scope");
		if (_focusScopes.TryGetValue(scope, out IInputElement _))
		{
			SetFocusedElement(scope, null);
			_focusScopes.Remove(scope);
		}
		if (Scope == scope)
		{
			Scope = null;
		}
	}

	public static bool GetIsFocusScope(IInputElement e)
	{
		return e is IFocusScope;
	}

	internal static FocusManager? GetFocusManager(IInputElement? element)
	{
		return ((FocusManager)(((element as Visual)?.VisualRoot as IInputRoot)?.FocusManager)) ?? ((FocusManager)AvaloniaLocator.Current.GetService<IFocusManager>());
	}

	private static bool CanFocus(IInputElement e)
	{
		if (e.Focusable && e.IsEffectivelyEnabled)
		{
			return IsVisible(e);
		}
		return false;
	}

	private static IEnumerable<IFocusScope> GetFocusScopeAncestors(IInputElement control)
	{
		for (IInputElement c = control; c != null; c = (c as Visual)?.GetVisualParent<IInputElement>() ?? ((c as IHostedVisualTreeRoot)?.Host as IInputElement))
		{
			if (c is IFocusScope focusScope && c is Visual { VisualRoot: Visual { IsVisible: not false } })
			{
				yield return focusScope;
			}
		}
	}

	private static void OnPreviewPointerPressed(object? sender, RoutedEventArgs e)
	{
		if (sender == null)
		{
			return;
		}
		PointerPressedEventArgs pointerPressedEventArgs = (PointerPressedEventArgs)e;
		Visual relativeTo = (Visual)sender;
		if (sender != e.Source || !pointerPressedEventArgs.GetCurrentPoint(relativeTo).Properties.IsLeftButtonPressed)
		{
			return;
		}
		for (Visual visual = (pointerPressedEventArgs.Pointer?.Captured as Visual) ?? (e.Source as Visual); visual != null; visual = visual.VisualParent)
		{
			if (visual is IInputElement inputElement && CanFocus(inputElement))
			{
				inputElement.Focus(NavigationMethod.Pointer, pointerPressedEventArgs.KeyModifiers);
				break;
			}
		}
	}

	private static bool IsVisible(IInputElement e)
	{
		return (e as Visual)?.IsEffectivelyVisible ?? true;
	}
}
