using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia.OpenGL;
using Avalonia.Platform;
using Avalonia.Reactive;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32.OpenGl;

internal class WglContext : IGlContext, IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider
{
	private readonly object _lock = new object();

	private readonly WglContext? _sharedWith;

	private readonly IntPtr _context;

	private readonly IntPtr _hWnd;

	private readonly IntPtr _dc;

	private readonly int _pixelFormat;

	private readonly PixelFormatDescriptor _formatDescriptor;

	public IntPtr Handle => _context;

	public GlVersion Version { get; }

	public GlInterface GlInterface { get; }

	public int SampleCount => 0;

	public int StencilSize { get; }

	private bool IsCurrent
	{
		get
		{
			if (UnmanagedMethods.wglGetCurrentContext() == _context)
			{
				return UnmanagedMethods.wglGetCurrentDC() == _dc;
			}
			return false;
		}
	}

	public bool IsLost { get; private set; }

	public bool CanCreateSharedContext => true;

	public WglContext(WglContext? sharedWith, GlVersion version, IntPtr context, IntPtr hWnd, IntPtr dc, int pixelFormat, PixelFormatDescriptor formatDescriptor)
	{
		Version = version;
		_sharedWith = sharedWith;
		_context = context;
		_hWnd = hWnd;
		_dc = dc;
		_pixelFormat = pixelFormat;
		_formatDescriptor = formatDescriptor;
		StencilSize = formatDescriptor.StencilBits;
		using (MakeCurrent())
		{
			GlInterface = new GlInterface(version, delegate(string proc)
			{
				IntPtr intPtr = UnmanagedMethods.wglGetProcAddress(proc);
				return (intPtr != IntPtr.Zero) ? intPtr : UnmanagedMethods.GetProcAddress(WglDisplay.OpenGl32Handle, proc);
			});
		}
	}

	public void Dispose()
	{
		UnmanagedMethods.wglDeleteContext(_context);
		WglGdiResourceManager.ReleaseDC(_hWnd, _dc);
		WglGdiResourceManager.DestroyWindow(_hWnd);
		IsLost = true;
	}

	public IDisposable MakeCurrent()
	{
		if (IsLost)
		{
			throw new PlatformGraphicsContextLostException();
		}
		if (IsCurrent)
		{
			return Disposable.Empty;
		}
		return new WglRestoreContext(_dc, _context, _lock);
	}

	public IDisposable EnsureCurrent()
	{
		return MakeCurrent();
	}

	internal IDisposable Lock()
	{
		Monitor.Enter(_lock);
		return Disposable.Create(_lock, Monitor.Exit);
	}

	public IntPtr CreateConfiguredDeviceContext(IntPtr hWnd)
	{
		IntPtr dC = WglGdiResourceManager.GetDC(hWnd);
		PixelFormatDescriptor pfd = _formatDescriptor;
		UnmanagedMethods.SetPixelFormat(dC, _pixelFormat, ref pfd);
		return dC;
	}

	public IDisposable MakeCurrent(IntPtr hdc)
	{
		return new WglRestoreContext(hdc, _context, _lock);
	}

	public bool IsSharedWith(IGlContext context)
	{
		WglContext wglContext = (WglContext)context;
		if (wglContext != this && wglContext._sharedWith != this && _sharedWith != context)
		{
			if (_sharedWith != null)
			{
				return _sharedWith == wglContext._sharedWith;
			}
			return false;
		}
		return true;
	}

	public IGlContext? CreateSharedContext(IEnumerable<GlVersion>? preferredVersions = null)
	{
		return WglDisplay.CreateContext(preferredVersions?.Append(Version).ToArray() ?? new GlVersion[1] { Version }, _sharedWith ?? this);
	}

	public object? TryGetFeature(Type featureType)
	{
		return null;
	}
}
