using System;
using System.Threading;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Platform;
using Avalonia.Platform.Internal;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class FramebufferManager : IFramebufferPlatformSurface, IDisposable
{
	private readonly struct FramebufferData
	{
		public UnmanagedBlob Data { get; }

		public PixelSize Size { get; }

		public int RowBytes => Size.Width * 4;

		public UnmanagedMethods.BITMAPINFOHEADER Header { get; }

		public FramebufferData(UnmanagedBlob data, int width, int height)
		{
			Data = data;
			Size = new PixelSize(width, height);
			UnmanagedMethods.BITMAPINFOHEADER bITMAPINFOHEADER = default(UnmanagedMethods.BITMAPINFOHEADER);
			bITMAPINFOHEADER.Init();
			bITMAPINFOHEADER.biPlanes = 1;
			bITMAPINFOHEADER.biBitCount = 32;
			bITMAPINFOHEADER.Init();
			bITMAPINFOHEADER.biWidth = width;
			bITMAPINFOHEADER.biHeight = -height;
			Header = bITMAPINFOHEADER;
		}

		public void Dispose()
		{
			Data.Dispose();
		}
	}

	private const int _bytesPerPixel = 4;

	private static readonly PixelFormat s_format = PixelFormat.Bgra8888;

	private readonly IntPtr _hwnd;

	private readonly object _lock;

	private readonly Action _onDisposeAction;

	private FramebufferData? _framebufferData;

	public FramebufferManager(IntPtr hwnd)
	{
		_hwnd = hwnd;
		_lock = new object();
		_onDisposeAction = DrawAndUnlock;
	}

	public ILockedFramebuffer Lock()
	{
		Monitor.Enter(_lock);
		LockedFramebuffer lockedFramebuffer = null;
		try
		{
			UnmanagedMethods.GetClientRect(_hwnd, out var lpRect);
			int num = Math.Max(1, lpRect.right - lpRect.left);
			int num2 = Math.Max(1, lpRect.bottom - lpRect.top);
			FramebufferData? framebufferData = _framebufferData;
			if (framebufferData.HasValue)
			{
				ref FramebufferData? framebufferData2 = ref _framebufferData;
				if (framebufferData2.HasValue && framebufferData2.GetValueOrDefault().Size.Width == num)
				{
					ref FramebufferData? framebufferData3 = ref _framebufferData;
					if (framebufferData3.HasValue && framebufferData3.GetValueOrDefault().Size.Height == num2)
					{
						goto IL_00e7;
					}
				}
			}
			_framebufferData?.Dispose();
			_framebufferData = AllocateFramebufferData(num, num2);
			goto IL_00e7;
			IL_00e7:
			FramebufferData value = _framebufferData.Value;
			return lockedFramebuffer = new LockedFramebuffer(value.Data.Address, value.Size, value.RowBytes, GetCurrentDpi(), s_format, _onDisposeAction);
		}
		finally
		{
			if (lockedFramebuffer == null)
			{
				Monitor.Exit(_lock);
			}
		}
	}

	public IFramebufferRenderTarget CreateFramebufferRenderTarget()
	{
		return new FuncFramebufferRenderTarget(Lock);
	}

	public void Dispose()
	{
		lock (_lock)
		{
			_framebufferData?.Dispose();
			_framebufferData = null;
		}
	}

	private void DrawAndUnlock()
	{
		try
		{
			if (_framebufferData.HasValue)
			{
				DrawToWindow(_hwnd, _framebufferData.Value);
			}
		}
		finally
		{
			Monitor.Exit(_lock);
		}
	}

	private Vector GetCurrentDpi()
	{
		if (UnmanagedMethods.ShCoreAvailable && Win32Platform.WindowsVersion > PlatformConstants.Windows8 && UnmanagedMethods.GetDpiForMonitor(UnmanagedMethods.MonitorFromWindow(_hwnd, UnmanagedMethods.MONITOR.MONITOR_DEFAULTTONEAREST), UnmanagedMethods.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY) == 0L)
		{
			return new Vector(dpiX, dpiY);
		}
		return new Vector(96.0, 96.0);
	}

	private static FramebufferData AllocateFramebufferData(int width, int height)
	{
		return new FramebufferData(new UnmanagedBlob(width * height * 4), width, height);
	}

	private static void DrawToDevice(FramebufferData framebufferData, IntPtr hDC, int destX = 0, int destY = 0, int srcX = 0, int srcY = 0, int width = -1, int height = -1)
	{
		if (width == -1)
		{
			width = framebufferData.Size.Width;
		}
		if (height == -1)
		{
			height = framebufferData.Size.Height;
		}
		UnmanagedMethods.BITMAPINFOHEADER lpbmi = framebufferData.Header;
		UnmanagedMethods.SetDIBitsToDevice(hDC, destX, destY, (uint)width, (uint)height, srcX, srcY, 0u, (uint)framebufferData.Size.Height, framebufferData.Data.Address, ref lpbmi, 0u);
	}

	private static bool DrawToWindow(IntPtr hWnd, FramebufferData framebufferData, int destX = 0, int destY = 0, int srcX = 0, int srcY = 0, int width = -1, int height = -1)
	{
		if (framebufferData.Data.IsDisposed)
		{
			throw new ObjectDisposedException("Framebuffer");
		}
		if (hWnd == IntPtr.Zero)
		{
			return false;
		}
		IntPtr dC = UnmanagedMethods.GetDC(hWnd);
		if (dC == IntPtr.Zero)
		{
			return false;
		}
		try
		{
			DrawToDevice(framebufferData, dC, destX, destY, srcX, srcY, width, height);
		}
		finally
		{
			UnmanagedMethods.ReleaseDC(hWnd, dC);
		}
		return true;
	}
}
