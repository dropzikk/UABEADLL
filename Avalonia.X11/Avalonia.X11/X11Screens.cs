using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Platform;

namespace Avalonia.X11;

internal class X11Screens : IScreenImpl
{
	private class Randr15ScreensImpl : IX11Screens
	{
		private readonly X11ScreensUserSettings _settings;

		private X11Screen[] _cache;

		private X11Info _x11;

		private IntPtr _window;

		private const int EDIDStructureLength = 32;

		public unsafe X11Screen[] Screens
		{
			get
			{
				if (_cache != null)
				{
					return _cache;
				}
				int nmonitors;
				XRRMonitorInfo* ptr = XLib.XRRGetMonitors(_x11.Display, _window, get_active: true, out nmonitors);
				X11Screen[] array = new X11Screen[nmonitors];
				for (int i = 0; i < nmonitors; i++)
				{
					XRRMonitorInfo xRRMonitorInfo = ptr[i];
					IntPtr intPtr = XLib.XGetAtomName(_x11.Display, xRRMonitorInfo.Name);
					string text = Marshal.PtrToStringAnsi(intPtr);
					XLib.XFree(intPtr);
					PixelRect pixelRect = new PixelRect(xRRMonitorInfo.X, xRRMonitorInfo.Y, xRRMonitorInfo.Width, xRRMonitorInfo.Height);
					Size? physicalSize = null;
					double value = 0.0;
					Dictionary<string, double> namedScaleFactors = _settings.NamedScaleFactors;
					if (namedScaleFactors == null || !namedScaleFactors.TryGetValue(text, out value))
					{
						for (int j = 0; j < xRRMonitorInfo.NOutput; j++)
						{
							Size? physicalMonitorSizeFromEDID = GetPhysicalMonitorSizeFromEDID(xRRMonitorInfo.Outputs[j]);
							double num = 1.0;
							if (physicalMonitorSizeFromEDID.HasValue)
							{
								num = X11Screen.GuessPixelDensity(pixelRect, physicalMonitorSizeFromEDID.Value);
							}
							if (value == 0.0 || value > num)
							{
								value = num;
								physicalSize = physicalMonitorSizeFromEDID;
							}
						}
					}
					if (value == 0.0)
					{
						value = 1.0;
					}
					value *= _settings.GlobalScaleFactor;
					array[i] = new X11Screen(pixelRect, xRRMonitorInfo.Primary != 0, text, physicalSize, value);
				}
				XLib.XFree(new IntPtr(ptr));
				_cache = UpdateWorkArea(_x11, array);
				return array;
			}
		}

		public Randr15ScreensImpl(AvaloniaX11Platform platform, X11ScreensUserSettings settings)
		{
			_settings = settings;
			_x11 = platform.Info;
			_window = XLib.CreateEventWindow(platform, OnEvent);
			XLib.XRRSelectInput(_x11.Display, _window, RandrEventMask.RRScreenChangeNotify);
		}

		private void OnEvent(ref XEvent ev)
		{
			if (ev.type == (XEventName)_x11.RandrEventBase)
			{
				_cache = null;
			}
		}

		private unsafe Size? GetPhysicalMonitorSizeFromEDID(IntPtr rrOutput)
		{
			if (rrOutput == IntPtr.Zero)
			{
				return null;
			}
			int count;
			IntPtr* ptr = XLib.XRRListOutputProperties(_x11.Display, rrOutput, out count);
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				if (ptr[i] == _x11.Atoms.EDID)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return null;
			}
			XLib.XRRGetOutputProperty(_x11.Display, rrOutput, _x11.Atoms.EDID, 0, 32, _delete: false, pending: false, _x11.Atoms.AnyPropertyType, out var actual_type, out var actual_format, out var nitems, out var _, out var prop);
			if (actual_type != _x11.Atoms.XA_INTEGER)
			{
				return null;
			}
			if (actual_format != 8)
			{
				return null;
			}
			byte[] array = new byte[nitems];
			Marshal.Copy(prop, array, 0, nitems);
			XLib.XFree(prop);
			XLib.XFree(new IntPtr(ptr));
			if (array.Length < 22)
			{
				return null;
			}
			byte b = array[21];
			byte b2 = array[22];
			if (b == 0 && b2 == 0)
			{
				return null;
			}
			return new Size(b * 10, b2 * 10);
		}
	}

	private class FallbackScreensImpl : IX11Screens
	{
		public X11Screen[] Screens { get; }

		public FallbackScreensImpl(X11Info info, X11ScreensUserSettings settings)
		{
			if (XLib.XGetGeometry(info.Display, info.RootWindow, out var geo))
			{
				Screens = UpdateWorkArea(info, new X11Screen[1]
				{
					new X11Screen(new PixelRect(0, 0, geo.width, geo.height), isPrimary: true, "Default", null, settings.GlobalScaleFactor)
				});
			}
			else
			{
				Screens = new X11Screen[1]
				{
					new X11Screen(new PixelRect(0, 0, 1920, 1280), isPrimary: true, "Default", null, settings.GlobalScaleFactor)
				};
			}
		}
	}

	private IX11Screens _impl;

	public int ScreenCount => _impl.Screens.Length;

	public IReadOnlyList<Screen> AllScreens => _impl.Screens.Select((X11Screen s) => new Screen(s.Scaling, s.Bounds, s.WorkingArea, s.IsPrimary)).ToArray();

	public X11Screens(IX11Screens impl)
	{
		_impl = impl;
	}

	private unsafe static X11Screen[] UpdateWorkArea(X11Info info, X11Screen[] screens)
	{
		PixelRect pixelRect = default(PixelRect);
		X11Screen[] array = screens;
		foreach (X11Screen x11Screen in array)
		{
			pixelRect = pixelRect.Union(x11Screen.Bounds);
			x11Screen.WorkingArea = x11Screen.Bounds;
		}
		if (XLib.XGetWindowProperty(info.Display, info.RootWindow, info.Atoms._NET_WORKAREA, IntPtr.Zero, new IntPtr(128), delete: false, info.Atoms.AnyPropertyType, out var actual_type, out var actual_format, out var nitems, out var bytes_after, out var prop) != 0 || actual_type == IntPtr.Zero || actual_format == 0 || bytes_after.ToInt64() != 0L || nitems.ToInt64() % 4 != 0L)
		{
			return screens;
		}
		IntPtr* ptr = (IntPtr*)(void*)prop;
		PixelRect rect = new PixelRect(ptr->ToInt32(), ptr[1].ToInt32(), ptr[2].ToInt32(), ptr[3].ToInt32());
		array = screens;
		foreach (X11Screen x11Screen2 in array)
		{
			x11Screen2.WorkingArea = x11Screen2.Bounds.Intersect(rect);
			if (x11Screen2.WorkingArea.Width <= 0 || x11Screen2.WorkingArea.Height <= 0)
			{
				x11Screen2.WorkingArea = x11Screen2.Bounds;
			}
		}
		XLib.XFree(prop);
		return screens;
	}

	public static IX11Screens Init(AvaloniaX11Platform platform)
	{
		X11Info info = platform.Info;
		X11ScreensUserSettings settings = X11ScreensUserSettings.Detect();
		if (!(info.RandrVersion != null) || !(info.RandrVersion >= new Version(1, 5)))
		{
			return new FallbackScreensImpl(info, settings);
		}
		return new Randr15ScreensImpl(platform, settings);
	}

	public Screen ScreenFromPoint(PixelPoint point)
	{
		return ScreenHelper.ScreenFromPoint(point, AllScreens);
	}

	public Screen ScreenFromRect(PixelRect rect)
	{
		return ScreenHelper.ScreenFromRect(rect, AllScreens);
	}

	public Screen ScreenFromWindow(IWindowBaseImpl window)
	{
		return ScreenHelper.ScreenFromWindow(window, AllScreens);
	}
}
