using System;
using System.Runtime.InteropServices;

namespace Avalonia.X11;

internal class X11Info
{
	public IntPtr Display { get; }

	public IntPtr DeferredDisplay { get; }

	public int DefaultScreen { get; }

	public IntPtr BlackPixel { get; }

	public IntPtr RootWindow { get; }

	public IntPtr DefaultRootWindow { get; }

	public IntPtr DefaultCursor { get; }

	public X11Atoms Atoms { get; }

	public IntPtr Xim { get; }

	public int RandrEventBase { get; }

	public int RandrErrorBase { get; }

	public Version RandrVersion { get; }

	public int XInputOpcode { get; }

	public int XInputEventBase { get; }

	public int XInputErrorBase { get; }

	public Version XInputVersion { get; }

	public IntPtr LastActivityTimestamp { get; set; }

	public XVisualInfo? TransparentVisualInfo { get; set; }

	public bool HasXim { get; set; }

	public bool HasXSync { get; set; }

	public IntPtr DefaultFontSet { get; set; }

	[DllImport("libc")]
	private static extern void setlocale(int type, string s);

	public X11Info(IntPtr display, IntPtr deferredDisplay, bool useXim)
	{
		Display = display;
		DeferredDisplay = deferredDisplay;
		DefaultScreen = XLib.XDefaultScreen(display);
		BlackPixel = XLib.XBlackPixel(display, DefaultScreen);
		RootWindow = XLib.XRootWindow(display, DefaultScreen);
		DefaultCursor = XLib.XCreateFontCursor(display, CursorFontShape.XC_left_ptr);
		DefaultRootWindow = XLib.XDefaultRootWindow(display);
		Atoms = new X11Atoms(display);
		DefaultFontSet = XLib.XCreateFontSet(Display, "-*-*-*-*-*-*-*-*-*-*-*-*-*-*", out var _, out var count, IntPtr.Zero);
		setlocale(0, "");
		if (useXim)
		{
			XLib.XSetLocaleModifiers("");
			Xim = XLib.XOpenIM(display, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (Xim != IntPtr.Zero)
			{
				HasXim = true;
			}
		}
		if (Xim == IntPtr.Zero)
		{
			if (XLib.XSetLocaleModifiers("@im=none") == IntPtr.Zero)
			{
				setlocale(0, "en_US.UTF-8");
				if (XLib.XSetLocaleModifiers("@im=none") == IntPtr.Zero)
				{
					setlocale(0, "C.UTF-8");
					XLib.XSetLocaleModifiers("@im=none");
				}
			}
			Xim = XLib.XOpenIM(display, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}
		XLib.XMatchVisualInfo(Display, DefaultScreen, 32, 4, out var info);
		if (info.depth == 32)
		{
			TransparentVisualInfo = info;
		}
		try
		{
			if (XLib.XRRQueryExtension(display, out var event_base_return, out var error_base_return) != 0)
			{
				RandrEventBase = event_base_return;
				RandrErrorBase = error_base_return;
				if (XLib.XRRQueryVersion(display, out var major_version_return, out var minor_version_return) != 0)
				{
					RandrVersion = new Version(major_version_return, minor_version_return);
				}
			}
		}
		catch
		{
		}
		try
		{
			if (XLib.XQueryExtension(display, "XInputExtension", out var majorOpcode, out var firstEvent, out var firstError))
			{
				int major = 2;
				int minor = 2;
				if (XLib.XIQueryVersion(display, ref major, ref minor) == Status.Success)
				{
					XInputVersion = new Version(major, minor);
					XInputOpcode = majorOpcode;
					XInputEventBase = firstEvent;
					XInputErrorBase = firstError;
				}
			}
		}
		catch
		{
		}
		try
		{
			HasXSync = XLib.XSyncInitialize(display, out count, out var _) != Status.Success;
		}
		catch
		{
		}
	}
}
