using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Avalonia.Input;

internal class AccessKeyHandler : IAccessKeyHandler
{
	public static readonly RoutedEvent<RoutedEventArgs> AccessKeyPressedEvent = RoutedEvent.Register<RoutedEventArgs>("AccessKeyPressed", RoutingStrategies.Bubble, typeof(AccessKeyHandler));

	private readonly List<(string AccessKey, IInputElement Element)> _registered = new List<(string, IInputElement)>();

	private IInputRoot? _owner;

	private bool _showingAccessKeys;

	private bool _ignoreAltUp;

	private bool _altIsDown;

	private IInputElement? _restoreFocusElement;

	private IMainMenu? _mainMenu;

	public IMainMenu? MainMenu
	{
		get
		{
			return _mainMenu;
		}
		set
		{
			if (_mainMenu != null)
			{
				_mainMenu.Closed -= MainMenuClosed;
			}
			_mainMenu = value;
			if (_mainMenu != null)
			{
				_mainMenu.Closed += MainMenuClosed;
			}
		}
	}

	public void SetOwner(IInputRoot owner)
	{
		if (_owner != null)
		{
			throw new InvalidOperationException("AccessKeyHandler owner has already been set.");
		}
		_owner = owner ?? throw new ArgumentNullException("owner");
		_owner.AddHandler(InputElement.KeyDownEvent, new Action<object, KeyEventArgs>(OnPreviewKeyDown), RoutingStrategies.Tunnel);
		_owner.AddHandler(InputElement.KeyDownEvent, new Action<object, KeyEventArgs>(OnKeyDown), RoutingStrategies.Bubble);
		_owner.AddHandler(InputElement.KeyUpEvent, new Action<object, KeyEventArgs>(OnPreviewKeyUp), RoutingStrategies.Tunnel);
		_owner.AddHandler(InputElement.PointerPressedEvent, new Action<object, PointerEventArgs>(OnPreviewPointerPressed), RoutingStrategies.Tunnel);
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

	protected virtual void OnPreviewKeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
		{
			_altIsDown = true;
			if (MainMenu == null || !MainMenu.IsOpen)
			{
				_restoreFocusElement = FocusManager.GetFocusManager(e.Source as IInputElement)?.GetFocusedElement();
				_owner.ShowAccessKeys = (_showingAccessKeys = true);
				return;
			}
			CloseMenu();
			_ignoreAltUp = true;
			_restoreFocusElement?.Focus();
			_restoreFocusElement = null;
		}
		else if (_altIsDown)
		{
			_ignoreAltUp = true;
		}
	}

	protected virtual void OnKeyDown(object? sender, KeyEventArgs e)
	{
		bool flag = MainMenu?.IsOpen ?? false;
		if (!((e.KeyModifiers.HasAllFlags(KeyModifiers.Alt) && !e.KeyModifiers.HasAllFlags(KeyModifiers.Control)) || flag))
		{
			return;
		}
		string text = e.Key.ToString();
		IEnumerable<IInputElement> source = from x in _registered
			where string.Equals(x.AccessKey, text, StringComparison.OrdinalIgnoreCase) && x.Element.IsEffectivelyVisible
			select x.Element;
		if (flag)
		{
			source = source.Where((IInputElement x) => x != null && ((Visual)MainMenu).IsLogicalAncestorOf((Visual)x));
		}
		source.FirstOrDefault()?.RaiseEvent(new RoutedEventArgs(AccessKeyPressedEvent));
	}

	protected virtual void OnPreviewKeyUp(object? sender, KeyEventArgs e)
	{
		Key key = e.Key;
		if ((uint)(key - 120) <= 1u)
		{
			_altIsDown = false;
			if (_ignoreAltUp)
			{
				_ignoreAltUp = false;
			}
			else if (_showingAccessKeys && MainMenu != null)
			{
				MainMenu.Open();
			}
		}
	}

	protected virtual void OnPreviewPointerPressed(object? sender, PointerEventArgs e)
	{
		if (_showingAccessKeys)
		{
			_owner.ShowAccessKeys = false;
		}
	}

	private void CloseMenu()
	{
		MainMenu.Close();
		_owner.ShowAccessKeys = (_showingAccessKeys = false);
	}

	private void MainMenuClosed(object? sender, EventArgs e)
	{
		_owner.ShowAccessKeys = false;
	}
}
