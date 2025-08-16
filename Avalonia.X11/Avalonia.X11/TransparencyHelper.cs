using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace Avalonia.X11;

internal class TransparencyHelper : IDisposable, X11Globals.IGlobalsSubscriber
{
	private readonly X11Info _x11;

	private readonly IntPtr _window;

	private readonly X11Globals _globals;

	private WindowTransparencyLevel _currentLevel;

	private IReadOnlyList<WindowTransparencyLevel>? _requestedLevels;

	private bool _blurAtomsAreSet;

	public Action<WindowTransparencyLevel>? TransparencyLevelChanged { get; set; }

	public WindowTransparencyLevel CurrentLevel
	{
		get
		{
			return _currentLevel;
		}
		set
		{
			if (_currentLevel != value)
			{
				_currentLevel = value;
				TransparencyLevelChanged?.Invoke(value);
			}
		}
	}

	public TransparencyHelper(X11Info x11, IntPtr window, X11Globals globals)
	{
		_x11 = x11;
		_window = window;
		_globals = globals;
		_globals.AddSubscriber(this);
	}

	public void SetTransparencyRequest(IReadOnlyList<WindowTransparencyLevel> levels)
	{
		_requestedLevels = levels;
		foreach (WindowTransparencyLevel level in levels)
		{
			if (IsSupported(level))
			{
				SetBlur(level == WindowTransparencyLevel.Blur);
				CurrentLevel = level;
				return;
			}
		}
		SetBlur(blur: false);
		CurrentLevel = (_globals.IsCompositionEnabled ? WindowTransparencyLevel.Transparent : WindowTransparencyLevel.None);
	}

	private bool IsSupported(WindowTransparencyLevel level)
	{
		if (level == WindowTransparencyLevel.None)
		{
			return !_globals.IsCompositionEnabled;
		}
		if (level == WindowTransparencyLevel.Transparent)
		{
			return _globals.IsCompositionEnabled;
		}
		if (level == WindowTransparencyLevel.Blur)
		{
			if (_globals.IsCompositionEnabled)
			{
				return _globals.WmName == "KWin";
			}
			return false;
		}
		return false;
	}

	private void UpdateTransparency()
	{
		SetTransparencyRequest(_requestedLevels ?? Array.Empty<WindowTransparencyLevel>());
	}

	private void SetBlur(bool blur)
	{
		if (blur)
		{
			if (!_blurAtomsAreSet)
			{
				IntPtr value = IntPtr.Zero;
				XLib.XChangeProperty(_x11.Display, _window, _x11.Atoms._KDE_NET_WM_BLUR_BEHIND_REGION, _x11.Atoms.XA_CARDINAL, 32, PropertyMode.Replace, ref value, 1);
				_blurAtomsAreSet = true;
			}
		}
		else if (_blurAtomsAreSet)
		{
			XLib.XDeleteProperty(_x11.Display, _window, _x11.Atoms._KDE_NET_WM_BLUR_BEHIND_REGION);
			_blurAtomsAreSet = false;
		}
	}

	public void Dispose()
	{
		_globals.RemoveSubscriber(this);
	}

	void X11Globals.IGlobalsSubscriber.WmChanged(string wmName)
	{
		UpdateTransparency();
	}

	void X11Globals.IGlobalsSubscriber.CompositionChanged(bool compositing)
	{
		UpdateTransparency();
	}
}
