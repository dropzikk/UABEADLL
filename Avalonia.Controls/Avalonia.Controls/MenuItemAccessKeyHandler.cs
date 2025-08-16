using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

internal class MenuItemAccessKeyHandler : IAccessKeyHandler
{
	private readonly List<(string AccessKey, IInputElement Element)> _registered = new List<(string, IInputElement)>();

	private IInputRoot? _owner;

	public IMainMenu? MainMenu { get; set; }

	public void SetOwner(IInputRoot owner)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (_owner != null)
		{
			throw new InvalidOperationException("AccessKeyHandler owner has already been set.");
		}
		_owner = owner;
		_owner.AddHandler(InputElement.TextInputEvent, new Action<object, TextInputEventArgs>(OnTextInput));
	}

	public void Register(char accessKey, IInputElement element)
	{
		(string, IInputElement) tuple = _registered.FirstOrDefault<(string, IInputElement)>(((string AccessKey, IInputElement Element) x) => x.Element == element);
		var (text, inputElement) = tuple;
		if (text != null || inputElement != null)
		{
			_registered.Remove(tuple);
		}
		_registered.Add((accessKey.ToString().ToUpperInvariant(), element));
	}

	public void Unregister(IInputElement element)
	{
		foreach (var item in _registered.Where<(string, IInputElement)>(((string AccessKey, IInputElement Element) x) => x.Element == element).ToList())
		{
			_registered.Remove(item);
		}
	}

	protected virtual void OnTextInput(object? sender, TextInputEventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(e.Text))
		{
			string text = e.Text;
			_registered.FirstOrDefault<(string, IInputElement)>(((string AccessKey, IInputElement Element) x) => string.Equals(x.AccessKey, text, StringComparison.OrdinalIgnoreCase) && x.Element.IsEffectivelyVisible).Item2?.RaiseEvent(new RoutedEventArgs(AccessKeyHandler.AccessKeyPressedEvent));
			e.Handled = true;
		}
	}
}
