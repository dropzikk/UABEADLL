using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Avalonia.X11;

internal class X11Globals
{
	public interface IGlobalsSubscriber
	{
		void WmChanged(string wmName);

		void CompositionChanged(bool compositing);
	}

	private readonly AvaloniaX11Platform _plat;

	private readonly int _screenNumber;

	private readonly X11Info _x11;

	private readonly IntPtr _rootWindow;

	private readonly IntPtr _compositingAtom;

	private readonly List<IGlobalsSubscriber> _subscribers = new List<IGlobalsSubscriber>();

	private string _wmName;

	private IntPtr _compositionAtomOwner;

	private bool _isCompositionEnabled;

	public string WmName
	{
		get
		{
			return _wmName;
		}
		private set
		{
			if (_wmName != value)
			{
				_wmName = value;
				IGlobalsSubscriber[] array = _subscribers.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].WmChanged(value);
				}
			}
		}
	}

	private IntPtr CompositionAtomOwner
	{
		get
		{
			return _compositionAtomOwner;
		}
		set
		{
			if (_compositionAtomOwner != value)
			{
				_compositionAtomOwner = value;
				IsCompositionEnabled = _compositionAtomOwner != IntPtr.Zero;
			}
		}
	}

	public bool IsCompositionEnabled
	{
		get
		{
			return _isCompositionEnabled;
		}
		set
		{
			if (_isCompositionEnabled != value)
			{
				_isCompositionEnabled = value;
				IGlobalsSubscriber[] array = _subscribers.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].CompositionChanged(value);
				}
			}
		}
	}

	public X11Globals(AvaloniaX11Platform plat)
	{
		_plat = plat;
		_x11 = plat.Info;
		_screenNumber = XLib.XDefaultScreen(_x11.Display);
		_rootWindow = XLib.XRootWindow(_x11.Display, _screenNumber);
		plat.Windows[_rootWindow] = OnRootWindowEvent;
		XLib.XSelectInput(_x11.Display, _rootWindow, new IntPtr(4325376));
		_compositingAtom = XLib.XInternAtom(_x11.Display, "_NET_WM_CM_S" + _screenNumber, only_if_exists: false);
		UpdateWmName();
		UpdateCompositingAtomOwner();
	}

	private unsafe IntPtr GetSupportingWmCheck(IntPtr window)
	{
		XLib.XGetWindowProperty(_x11.Display, _rootWindow, _x11.Atoms._NET_SUPPORTING_WM_CHECK, IntPtr.Zero, new IntPtr(IntPtr.Size), delete: false, _x11.Atoms.XA_WINDOW, out var actual_type, out var _, out var nitems, out var _, out var prop);
		if (nitems.ToInt32() != 1)
		{
			return IntPtr.Zero;
		}
		try
		{
			if (actual_type != _x11.Atoms.XA_WINDOW)
			{
				return IntPtr.Zero;
			}
			return *(IntPtr*)prop.ToPointer();
		}
		finally
		{
			XLib.XFree(prop);
		}
	}

	private void UpdateCompositingAtomOwner()
	{
		IntPtr intPtr = XLib.XGetSelectionOwner(_x11.Display, _compositingAtom);
		while (CompositionAtomOwner != intPtr)
		{
			if (CompositionAtomOwner != IntPtr.Zero)
			{
				_plat.Windows.Remove(CompositionAtomOwner);
				XLib.XSelectInput(_x11.Display, CompositionAtomOwner, IntPtr.Zero);
			}
			CompositionAtomOwner = intPtr;
			if (CompositionAtomOwner != IntPtr.Zero)
			{
				_plat.Windows[intPtr] = HandleCompositionAtomOwnerEvents;
				XLib.XSelectInput(_x11.Display, CompositionAtomOwner, new IntPtr(131072));
			}
			intPtr = XLib.XGetSelectionOwner(_x11.Display, _compositingAtom);
		}
	}

	private void HandleCompositionAtomOwnerEvents(ref XEvent ev)
	{
		if (ev.type == XEventName.DestroyNotify)
		{
			UpdateCompositingAtomOwner();
		}
	}

	private void UpdateWmName()
	{
		WmName = GetWmName();
	}

	private string GetWmName()
	{
		IntPtr supportingWmCheck = GetSupportingWmCheck(_rootWindow);
		if (supportingWmCheck == IntPtr.Zero || supportingWmCheck != GetSupportingWmCheck(supportingWmCheck))
		{
			return null;
		}
		XLib.XGetWindowProperty(_x11.Display, supportingWmCheck, _x11.Atoms._NET_WM_NAME, IntPtr.Zero, new IntPtr(int.MaxValue), delete: false, _x11.Atoms.UTF8_STRING, out var _, out var actual_format, out var nitems, out var _, out var prop);
		if (nitems == IntPtr.Zero)
		{
			return null;
		}
		try
		{
			if (actual_format != 8)
			{
				return null;
			}
			return Marshal.PtrToStringAnsi(prop, nitems.ToInt32());
		}
		finally
		{
			XLib.XFree(prop);
		}
	}

	private void OnRootWindowEvent(ref XEvent ev)
	{
		if (ev.type == XEventName.PropertyNotify && ev.PropertyEvent.atom == _x11.Atoms._NET_SUPPORTING_WM_CHECK)
		{
			UpdateWmName();
		}
		if (ev.type == XEventName.ClientMessage && ev.ClientMessageEvent.message_type == _x11.Atoms.MANAGER && ev.ClientMessageEvent.ptr2 == _compositingAtom)
		{
			UpdateCompositingAtomOwner();
		}
	}

	public void AddSubscriber(IGlobalsSubscriber subscriber)
	{
		_subscribers.Add(subscriber);
	}

	public void RemoveSubscriber(IGlobalsSubscriber subscriber)
	{
		_subscribers.Remove(subscriber);
	}
}
