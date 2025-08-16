using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class ScreenImpl : IScreenImpl
{
	private Screen[]? _allScreens;

	public int ScreenCount => UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CMONITORS);

	public IReadOnlyList<Screen> AllScreens
	{
		get
		{
			if (_allScreens == null)
			{
				int index = 0;
				Screen[] screens = new Screen[ScreenCount];
				UnmanagedMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, delegate(IntPtr monitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr data)
				{
					UnmanagedMethods.MONITORINFO lpmi = UnmanagedMethods.MONITORINFO.Create();
					if (UnmanagedMethods.GetMonitorInfo(monitor, ref lpmi))
					{
						double num = 1.0;
						if (UnmanagedMethods.GetProcAddress(UnmanagedMethods.LoadLibrary("shcore.dll"), "GetDpiForMonitor") != IntPtr.Zero)
						{
							UnmanagedMethods.GetDpiForMonitor(monitor, UnmanagedMethods.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var _);
							num = dpiX;
						}
						else
						{
							IntPtr dC = UnmanagedMethods.GetDC(IntPtr.Zero);
							double num2 = UnmanagedMethods.GetDeviceCaps(dC, UnmanagedMethods.DEVICECAP.HORZRES);
							double num3 = UnmanagedMethods.GetDeviceCaps(dC, UnmanagedMethods.DEVICECAP.DESKTOPHORZRES);
							num = 96.0 * num3 / num2;
							UnmanagedMethods.ReleaseDC(IntPtr.Zero, dC);
						}
						UnmanagedMethods.RECT rcMonitor = lpmi.rcMonitor;
						UnmanagedMethods.RECT rcWork = lpmi.rcWork;
						PixelRect bounds = rcMonitor.ToPixelRect();
						PixelRect workingArea = rcWork.ToPixelRect();
						screens[index] = new WinScreen(num / 96.0, bounds, workingArea, lpmi.dwFlags == 1, monitor);
						index++;
					}
					return true;
				}, IntPtr.Zero);
				_allScreens = screens;
			}
			return _allScreens;
		}
	}

	public void InvalidateScreensCache()
	{
		_allScreens = null;
	}

	public Screen? ScreenFromWindow(IWindowBaseImpl window)
	{
		IntPtr handle = UnmanagedMethods.MonitorFromWindow(window.Handle.Handle, UnmanagedMethods.MONITOR.MONITOR_DEFAULTTONULL);
		return FindScreenByHandle(handle);
	}

	public Screen? ScreenFromPoint(PixelPoint point)
	{
		UnmanagedMethods.POINT pt = default(UnmanagedMethods.POINT);
		pt.X = point.X;
		pt.Y = point.Y;
		IntPtr handle = UnmanagedMethods.MonitorFromPoint(pt, UnmanagedMethods.MONITOR.MONITOR_DEFAULTTONULL);
		return FindScreenByHandle(handle);
	}

	public Screen? ScreenFromRect(PixelRect rect)
	{
		UnmanagedMethods.RECT rect2 = default(UnmanagedMethods.RECT);
		rect2.left = rect.TopLeft.X;
		rect2.top = rect.TopLeft.Y;
		rect2.right = rect.TopRight.X;
		rect2.bottom = rect.BottomRight.Y;
		IntPtr handle = UnmanagedMethods.MonitorFromRect(rect2, UnmanagedMethods.MONITOR.MONITOR_DEFAULTTONULL);
		return FindScreenByHandle(handle);
	}

	private Screen? FindScreenByHandle(IntPtr handle)
	{
		return AllScreens.Cast<WinScreen>().FirstOrDefault((WinScreen m) => m.Handle == handle);
	}
}
