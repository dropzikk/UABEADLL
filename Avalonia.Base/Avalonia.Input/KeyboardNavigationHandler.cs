using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Input.Navigation;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace Avalonia.Input;

[Unstable]
public sealed class KeyboardNavigationHandler : IKeyboardNavigationHandler
{
	private IInputRoot? _owner;

	[PrivateApi]
	public void SetOwner(IInputRoot owner)
	{
		if (_owner != null)
		{
			throw new InvalidOperationException("AccessKeyHandler owner has already been set.");
		}
		_owner = owner ?? throw new ArgumentNullException("owner");
		_owner.AddHandler(InputElement.KeyDownEvent, new Action<object, KeyEventArgs>(OnKeyDown));
	}

	public static IInputElement? GetNext(IInputElement element, NavigationDirection direction)
	{
		element = element ?? throw new ArgumentNullException("element");
		ICustomKeyboardNavigation customKeyboardNavigation = (element as Visual)?.FindAncestorOfType<ICustomKeyboardNavigation>(includeSelf: true);
		if (customKeyboardNavigation != null && HandlePreCustomNavigation(customKeyboardNavigation, element, direction, out IInputElement result))
		{
			return result;
		}
		IInputElement inputElement = direction switch
		{
			NavigationDirection.Next => TabNavigation.GetNextTab(element, goDownOnly: false), 
			NavigationDirection.Previous => TabNavigation.GetPrevTab(element, null, goDownOnly: false), 
			_ => throw new NotSupportedException(), 
		};
		if (customKeyboardNavigation == null && HandlePostCustomNavigation(element, inputElement, direction, out result))
		{
			return result;
		}
		return inputElement;
	}

	public void Move(IInputElement? element, NavigationDirection direction, KeyModifiers keyModifiers = KeyModifiers.None)
	{
		if (element != null || _owner != null)
		{
			IInputElement next = GetNext(element ?? _owner, direction);
			if (next != null)
			{
				NavigationMethod method = ((direction == NavigationDirection.Next || direction == NavigationDirection.Previous) ? NavigationMethod.Tab : NavigationMethod.Directional);
				next.Focus(method, keyModifiers);
			}
		}
	}

	private void OnKeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Tab)
		{
			IInputElement element = FocusManager.GetFocusManager(e.Source as IInputElement)?.GetFocusedElement();
			NavigationDirection direction = (((e.KeyModifiers & KeyModifiers.Shift) != 0) ? NavigationDirection.Previous : NavigationDirection.Next);
			Move(element, direction, e.KeyModifiers);
			e.Handled = true;
		}
	}

	private static bool HandlePreCustomNavigation(ICustomKeyboardNavigation customHandler, IInputElement element, NavigationDirection direction, [NotNullWhen(true)] out IInputElement? result)
	{
		var (flag, inputElement) = customHandler.GetNext(element, direction);
		if (flag)
		{
			if (inputElement != null)
			{
				result = inputElement;
				return true;
			}
			IInputElement inputElement2 = direction switch
			{
				NavigationDirection.Next => TabNavigation.GetNextTabOutside(customHandler), 
				NavigationDirection.Previous => TabNavigation.GetPrevTabOutside(customHandler), 
				_ => null, 
			};
			if (inputElement2 != null)
			{
				result = inputElement2;
				return true;
			}
		}
		result = null;
		return false;
	}

	private static bool HandlePostCustomNavigation(IInputElement element, IInputElement? newElement, NavigationDirection direction, [NotNullWhen(true)] out IInputElement? result)
	{
		if (newElement is Visual visual)
		{
			ICustomKeyboardNavigation customKeyboardNavigation = visual.FindAncestorOfType<ICustomKeyboardNavigation>(includeSelf: true);
			if (customKeyboardNavigation != null)
			{
				var (flag, inputElement) = customKeyboardNavigation.GetNext(element, direction);
				if (flag && inputElement != null)
				{
					result = inputElement;
					return true;
				}
			}
		}
		result = null;
		return false;
	}
}
