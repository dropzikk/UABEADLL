using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace Avalonia.Input;

[PrivateApi]
public class KeyboardDevice : IKeyboardDevice, IInputDevice, INotifyPropertyChanged
{
	private IInputElement? _focusedElement;

	private IInputRoot? _focusedRoot;

	private readonly TextInputMethodManager _textInputManager = new TextInputMethodManager();

	internal static KeyboardDevice? Instance => AvaloniaLocator.Current.GetService<IKeyboardDevice>() as KeyboardDevice;

	public IInputManager? InputManager => AvaloniaLocator.Current.GetService<IInputManager>();

	public IFocusManager? FocusManager => AvaloniaLocator.Current.GetService<IFocusManager>();

	public IInputElement? FocusedElement => _focusedElement;

	public event PropertyChangedEventHandler? PropertyChanged;

	private static void ClearFocusWithinAncestors(IInputElement? element)
	{
		for (IInputElement inputElement = element; inputElement != null; inputElement = (IInputElement)((inputElement as Visual)?.VisualParent))
		{
			if (inputElement is InputElement inputElement2)
			{
				inputElement2.IsKeyboardFocusWithin = false;
			}
		}
	}

	private void ClearFocusWithin(IInputElement element, bool clearRoot)
	{
		if (element is Visual visual)
		{
			foreach (Visual visualChild in visual.VisualChildren)
			{
				if (visualChild is IInputElement { IsKeyboardFocusWithin: not false } inputElement)
				{
					ClearFocusWithin(inputElement, clearRoot: true);
					break;
				}
			}
		}
		if (clearRoot && element is InputElement inputElement2)
		{
			inputElement2.IsKeyboardFocusWithin = false;
		}
	}

	private void SetIsFocusWithin(IInputElement? oldElement, IInputElement? newElement)
	{
		if (newElement == null && oldElement != null)
		{
			ClearFocusWithinAncestors(oldElement);
			return;
		}
		IInputElement inputElement = null;
		IInputElement inputElement2;
		for (inputElement2 = newElement; inputElement2 != null; inputElement2 = (inputElement2 as Visual)?.VisualParent as IInputElement)
		{
			if (inputElement2.IsKeyboardFocusWithin)
			{
				inputElement = inputElement2;
				break;
			}
		}
		inputElement2 = oldElement;
		if (inputElement2 != null && inputElement != null)
		{
			ClearFocusWithin(inputElement, clearRoot: false);
		}
		inputElement2 = newElement;
		while (inputElement2 != null && inputElement2 != inputElement)
		{
			if (inputElement2 is InputElement inputElement3)
			{
				inputElement3.IsKeyboardFocusWithin = true;
			}
			inputElement2 = (inputElement2 as Visual)?.VisualParent as IInputElement;
		}
	}

	private void ClearChildrenFocusWithin(IInputElement element, bool clearRoot)
	{
		if (element is Visual visual)
		{
			foreach (Visual visualChild in visual.VisualChildren)
			{
				if (visualChild is IInputElement { IsKeyboardFocusWithin: not false } inputElement)
				{
					ClearChildrenFocusWithin(inputElement, clearRoot: true);
					break;
				}
			}
		}
		if (clearRoot && element is InputElement inputElement2)
		{
			inputElement2.IsKeyboardFocusWithin = false;
		}
	}

	public void SetFocusedElement(IInputElement? element, NavigationMethod method, KeyModifiers keyModifiers)
	{
		if (element != FocusedElement)
		{
			Interactive obj = FocusedElement as Interactive;
			if (FocusedElement != null && (!((Visual)FocusedElement).IsAttachedToVisualTree || _focusedRoot != ((Visual)element)?.VisualRoot as IInputRoot) && _focusedRoot != null)
			{
				ClearChildrenFocusWithin(_focusedRoot, clearRoot: true);
			}
			SetIsFocusWithin(FocusedElement, element);
			_focusedElement = element;
			_focusedRoot = ((Visual)_focusedElement)?.VisualRoot as IInputRoot;
			obj?.RaiseEvent(new RoutedEventArgs
			{
				RoutedEvent = InputElement.LostFocusEvent
			});
			(element as Interactive)?.RaiseEvent(new GotFocusEventArgs
			{
				NavigationMethod = method,
				KeyModifiers = keyModifiers
			});
			_textInputManager.SetFocusedElement(element);
			RaisePropertyChanged("FocusedElement");
		}
	}

	protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public void ProcessRawEvent(RawInputEventArgs e)
	{
		if (e.Handled)
		{
			return;
		}
		IInputElement inputElement = FocusedElement ?? e.Root;
		if (e is RawKeyEventArgs { Type: var type } rawKeyEventArgs && (uint)type <= 1u)
		{
			RoutedEvent<KeyEventArgs> routedEvent = ((rawKeyEventArgs.Type == RawKeyEventType.KeyDown) ? InputElement.KeyDownEvent : InputElement.KeyUpEvent);
			KeyEventArgs keyEventArgs = new KeyEventArgs
			{
				RoutedEvent = routedEvent,
				Key = rawKeyEventArgs.Key,
				KeyModifiers = rawKeyEventArgs.Modifiers.ToKeyModifiers(),
				Source = inputElement
			};
			Visual visual = inputElement as Visual;
			while (visual != null && !keyEventArgs.Handled && rawKeyEventArgs.Type == RawKeyEventType.KeyDown)
			{
				List<KeyBinding> list = (visual as IInputElement)?.KeyBindings;
				if (list != null)
				{
					KeyBinding[] array = null;
					foreach (KeyBinding item in list)
					{
						KeyGesture gesture = item.Gesture;
						if ((object)gesture != null && gesture.Matches(keyEventArgs))
						{
							array = list.ToArray();
							break;
						}
					}
					if (array != null)
					{
						KeyBinding[] array2 = array;
						foreach (KeyBinding keyBinding in array2)
						{
							if (keyEventArgs.Handled)
							{
								break;
							}
							keyBinding.TryHandle(keyEventArgs);
						}
					}
				}
				visual = visual.VisualParent;
			}
			inputElement.RaiseEvent(keyEventArgs);
			e.Handled = keyEventArgs.Handled;
		}
		if (e is RawTextInputEventArgs rawTextInputEventArgs)
		{
			TextInputEventArgs textInputEventArgs = new TextInputEventArgs
			{
				Text = rawTextInputEventArgs.Text,
				Source = inputElement,
				RoutedEvent = InputElement.TextInputEvent
			};
			inputElement.RaiseEvent(textInputEventArgs);
			e.Handled = textInputEventArgs.Handled;
		}
	}
}
