using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Avalonia.X11;

internal class X11IconData : IWindowIconImpl, IFramebufferPlatformSurface
{
	private int _width;

	private int _height;

	private uint[] _bdata;

	public UIntPtr[] Data { get; }

	public X11IconData(Bitmap bitmap)
	{
		_width = Math.Min(bitmap.PixelSize.Width, 128);
		_height = Math.Min(bitmap.PixelSize.Height, 128);
		_bdata = new uint[_width * _height];
		using (IPlatformRenderInterfaceContext platformRenderInterfaceContext = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>().CreateBackendContext(null))
		{
			using IRenderTarget renderTarget = platformRenderInterfaceContext.CreateRenderTarget(new X11IconData[1] { this });
			using IDrawingContextImpl drawingContextImpl = renderTarget.CreateDrawingContext();
			drawingContextImpl.DrawBitmap(bitmap.PlatformImpl.Item, 1.0, new Rect(bitmap.Size), new Rect(0.0, 0.0, _width, _height));
		}
		Data = new UIntPtr[_width * _height + 2];
		Data[0] = new UIntPtr((uint)_width);
		Data[1] = new UIntPtr((uint)_height);
		for (int i = 0; i < _height; i++)
		{
			int num = i * _width;
			for (int j = 0; j < _width; j++)
			{
				Data[num + j + 2] = new UIntPtr(_bdata[num + j]);
			}
		}
		_bdata = null;
	}

	public unsafe void Save(Stream outputStream)
	{
		using WriteableBitmap writeableBitmap = new WriteableBitmap(new PixelSize(_width, _height), new Vector(96.0, 96.0), PixelFormat.Bgra8888, null);
		using (ILockedFramebuffer lockedFramebuffer = writeableBitmap.Lock())
		{
			uint* ptr = (uint*)(void*)lockedFramebuffer.Address;
			for (int i = 0; i < _height; i++)
			{
				int num = i * _width;
				int num2 = i * lockedFramebuffer.RowBytes / 4;
				for (int j = 0; j < _width; j++)
				{
					ptr[num2 + j] = Data[num + j + 2].ToUInt32();
				}
			}
		}
		writeableBitmap.Save(outputStream, null);
	}

	public ILockedFramebuffer Lock()
	{
		GCHandle h = GCHandle.Alloc(_bdata, GCHandleType.Pinned);
		return new LockedFramebuffer(h.AddrOfPinnedObject(), new PixelSize(_width, _height), _width * 4, new Vector(96.0, 96.0), PixelFormat.Bgra8888, delegate
		{
			h.Free();
		});
	}

	public IFramebufferRenderTarget CreateFramebufferRenderTarget()
	{
		return new FuncFramebufferRenderTarget(Lock);
	}
}
