using System;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Platform;

namespace Avalonia.X11;

internal class X11FramebufferSurface : IFramebufferPlatformSurface
{
	private readonly IntPtr _display;

	private readonly IntPtr _xid;

	private readonly int _depth;

	private readonly Func<double> _scaling;

	public X11FramebufferSurface(IntPtr display, IntPtr xid, int depth, Func<double> scaling)
	{
		_display = display;
		_xid = xid;
		_depth = depth;
		_scaling = scaling;
	}

	public ILockedFramebuffer Lock()
	{
		XLib.XLockDisplay(_display);
		XLib.XGetGeometry(_display, _xid, out var _, out var _, out var _, out var width, out var height, out var _, out var _);
		XLib.XUnlockDisplay(_display);
		return new X11Framebuffer(_display, _xid, _depth, width, height, _scaling());
	}

	public IFramebufferRenderTarget CreateFramebufferRenderTarget()
	{
		return new FuncFramebufferRenderTarget(Lock);
	}
}
