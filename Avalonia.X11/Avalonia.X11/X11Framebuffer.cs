using System;
using Avalonia.Platform;
using Avalonia.Platform.Internal;

namespace Avalonia.X11;

internal class X11Framebuffer : ILockedFramebuffer, IDisposable
{
	private readonly IntPtr _display;

	private readonly IntPtr _xid;

	private readonly int _depth;

	private UnmanagedBlob _blob;

	public IntPtr Address { get; }

	public PixelSize Size { get; }

	public int RowBytes { get; }

	public Vector Dpi { get; }

	public PixelFormat Format { get; }

	public X11Framebuffer(IntPtr display, IntPtr xid, int depth, int width, int height, double factor)
	{
		width = Math.Max(1, width);
		height = Math.Max(1, height);
		_display = display;
		_xid = xid;
		_depth = depth;
		Size = new PixelSize(width, height);
		RowBytes = width * 4;
		Dpi = new Vector(96.0, 96.0) * factor;
		Format = PixelFormat.Bgra8888;
		_blob = new UnmanagedBlob(RowBytes * height);
		Address = _blob.Address;
	}

	public void Dispose()
	{
		XImage image = default(XImage);
		int num = 32;
		image.width = Size.Width;
		image.height = Size.Height;
		image.format = 2;
		image.data = Address;
		image.byte_order = 0;
		image.bitmap_unit = num;
		image.bitmap_bit_order = 0;
		image.bitmap_pad = num;
		image.depth = _depth;
		image.bytes_per_line = RowBytes;
		image.bits_per_pixel = num;
		XLib.XLockDisplay(_display);
		XLib.XInitImage(ref image);
		IntPtr gc = XLib.XCreateGC(_display, _xid, 0uL, IntPtr.Zero);
		XLib.XPutImage(_display, _xid, gc, ref image, 0, 0, 0, 0, (uint)Size.Width, (uint)Size.Height);
		XLib.XFreeGC(_display, gc);
		XLib.XSync(_display, discard: true);
		XLib.XUnlockDisplay(_display);
		_blob.Dispose();
	}
}
