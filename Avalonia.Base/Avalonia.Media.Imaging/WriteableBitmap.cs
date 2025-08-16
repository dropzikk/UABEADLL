using System;
using System.IO;
using System.Runtime.CompilerServices;
using Avalonia.Platform;

namespace Avalonia.Media.Imaging;

public class WriteableBitmap : Bitmap
{
	private BitmapMemory? _pixelFormatMemory;

	public override PixelFormat? Format
	{
		get
		{
			BitmapMemory? pixelFormatMemory = _pixelFormatMemory;
			if (pixelFormatMemory == null)
			{
				return base.Format;
			}
			return pixelFormatMemory.Format;
		}
	}

	public WriteableBitmap(PixelSize size, Vector dpi, PixelFormat? format = null, AlphaFormat? alphaFormat = null)
		: this(CreatePlatformImpl(size, in dpi, format, alphaFormat))
	{
	}

	private WriteableBitmap((IBitmapImpl impl, BitmapMemory? mem) bitmapWithMem)
		: this(bitmapWithMem.impl, bitmapWithMem.mem)
	{
	}

	private WriteableBitmap(IBitmapImpl impl, BitmapMemory? pixelFormatMemory = null)
		: base(impl)
	{
		_pixelFormatMemory = pixelFormatMemory;
	}

	public unsafe WriteableBitmap(PixelFormat format, AlphaFormat alphaFormat, IntPtr data, PixelSize size, Vector dpi, int stride)
		: this(size, dpi, format, alphaFormat)
	{
		int num = (format.BitsPerPixel * size.Width + 7) / 8;
		if (num > stride)
		{
			throw new ArgumentOutOfRangeException("stride");
		}
		using ILockedFramebuffer lockedFramebuffer = Lock();
		for (int i = 0; i < size.Height; i++)
		{
			Unsafe.CopyBlock((lockedFramebuffer.Address + lockedFramebuffer.RowBytes * i).ToPointer(), (data + i * stride).ToPointer(), (uint)num);
		}
	}

	public ILockedFramebuffer Lock()
	{
		if (_pixelFormatMemory == null)
		{
			return ((IWriteableBitmapImpl)base.PlatformImpl.Item).Lock();
		}
		return new LockedFramebuffer(_pixelFormatMemory.Address, _pixelFormatMemory.Size, _pixelFormatMemory.RowBytes, base.Dpi, _pixelFormatMemory.Format, delegate
		{
			using ILockedFramebuffer lockedFramebuffer = ((IWriteableBitmapImpl)base.PlatformImpl.Item).Lock();
			_pixelFormatMemory.CopyToRgba(lockedFramebuffer.Address, lockedFramebuffer.RowBytes);
		});
	}

	public override void CopyPixels(PixelRect sourceRect, IntPtr buffer, int bufferSize, int stride)
	{
		using ILockedFramebuffer fb = Lock();
		CopyPixelsCore(sourceRect, buffer, bufferSize, stride, fb);
	}

	public static WriteableBitmap Decode(Stream stream)
	{
		return new WriteableBitmap(GetFactory().LoadWriteableBitmap(stream));
	}

	public new static WriteableBitmap DecodeToWidth(Stream stream, int width, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		return new WriteableBitmap(GetFactory().LoadWriteableBitmapToWidth(stream, width, interpolationMode));
	}

	public new static WriteableBitmap DecodeToHeight(Stream stream, int height, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		return new WriteableBitmap(GetFactory().LoadWriteableBitmapToHeight(stream, height, interpolationMode));
	}

	private static (IBitmapImpl, BitmapMemory?) CreatePlatformImpl(PixelSize size, in Vector dpi, PixelFormat? format, AlphaFormat? alphaFormat)
	{
		if (size.Width <= 0 || size.Height <= 0)
		{
			throw new ArgumentException("Size should be >= (1,1)", "size");
		}
		IPlatformRenderInterface factory = GetFactory();
		PixelFormat pixelFormat = format ?? factory.DefaultPixelFormat;
		AlphaFormat alphaFormat2 = alphaFormat ?? factory.DefaultAlphaFormat;
		if (factory.IsSupportedBitmapPixelFormat(pixelFormat))
		{
			return (factory.CreateWriteableBitmap(size, dpi, pixelFormat, alphaFormat2), null);
		}
		if (!PixelFormatReader.SupportsFormat(pixelFormat))
		{
			throw new NotSupportedException($"Pixel format {pixelFormat} is not supported");
		}
		return (factory.CreateWriteableBitmap(size, dpi, PixelFormat.Rgba8888, pixelFormat.HasAlpha ? alphaFormat2 : AlphaFormat.Opaque), new BitmapMemory(pixelFormat, size));
	}

	private static IPlatformRenderInterface GetFactory()
	{
		return AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();
	}
}
