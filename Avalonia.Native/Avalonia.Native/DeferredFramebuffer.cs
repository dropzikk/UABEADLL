using System;
using System.Runtime.InteropServices;
using Avalonia.Native.Interop;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class DeferredFramebuffer : ILockedFramebuffer, IDisposable
{
	private readonly IAvnSoftwareRenderTarget _renderTarget;

	private readonly Action<Action<IAvnWindowBase>> _lockWindow;

	public IntPtr Address { get; set; }

	public PixelSize Size { get; set; }

	public int Height { get; set; }

	public int RowBytes { get; set; }

	public Vector Dpi { get; set; }

	public PixelFormat Format { get; set; }

	public DeferredFramebuffer(IAvnSoftwareRenderTarget renderTarget, Action<Action<IAvnWindowBase>> lockWindow, int width, int height, Vector dpi)
	{
		_renderTarget = renderTarget;
		_lockWindow = lockWindow;
		Address = Marshal.AllocHGlobal(width * height * 4);
		Size = new PixelSize(width, height);
		RowBytes = width * 4;
		Dpi = dpi;
		Format = PixelFormat.Bgra8888;
	}

	public unsafe void Dispose()
	{
		if (!(Address == IntPtr.Zero))
		{
			_lockWindow(delegate
			{
				AvnFramebuffer avnFramebuffer = default(AvnFramebuffer);
				avnFramebuffer.Data = Address.ToPointer();
				avnFramebuffer.Dpi = new AvnVector
				{
					X = Dpi.X,
					Y = Dpi.Y
				};
				avnFramebuffer.Width = Size.Width;
				avnFramebuffer.Height = Size.Height;
				avnFramebuffer.PixelFormat = (AvnPixelFormat)Format.FormatEnum;
				avnFramebuffer.Stride = RowBytes;
				AvnFramebuffer avnFramebuffer2 = avnFramebuffer;
				_renderTarget.SetFrame(&avnFramebuffer2);
			});
			Marshal.FreeHGlobal(Address);
			Address = IntPtr.Zero;
		}
	}
}
