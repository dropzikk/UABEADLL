using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Media.Imaging;

public class Bitmap : IBitmap, IImage, IDisposable, IImageBrushSource
{
	private bool _isTranscoded;

	public Vector Dpi => PlatformImpl.Item.Dpi;

	public Size Size => PlatformImpl.Item.PixelSize.ToSizeWithDpi(Dpi);

	public PixelSize PixelSize => PlatformImpl.Item.PixelSize;

	internal IRef<IBitmapImpl> PlatformImpl { get; }

	IRef<IBitmapImpl> IBitmap.PlatformImpl => PlatformImpl;

	public virtual PixelFormat? Format => (PlatformImpl.Item as IReadableBitmapImpl)?.Format;

	IRef<IBitmapImpl> IImageBrushSource.Bitmap => PlatformImpl;

	public static Bitmap DecodeToWidth(Stream stream, int width, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		return new Bitmap(GetFactory().LoadBitmapToWidth(stream, width, interpolationMode));
	}

	public static Bitmap DecodeToHeight(Stream stream, int height, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		return new Bitmap(GetFactory().LoadBitmapToHeight(stream, height, interpolationMode));
	}

	public Bitmap CreateScaledBitmap(PixelSize destinationSize, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		return new Bitmap(GetFactory().ResizeBitmap(PlatformImpl.Item, destinationSize, interpolationMode));
	}

	public Bitmap(string fileName)
	{
		PlatformImpl = RefCountable.Create(GetFactory().LoadBitmap(fileName));
	}

	public Bitmap(Stream stream)
	{
		PlatformImpl = RefCountable.Create(GetFactory().LoadBitmap(stream));
	}

	internal Bitmap(IRef<IBitmapImpl> impl)
	{
		PlatformImpl = impl.Clone();
	}

	protected Bitmap(IBitmapImpl impl)
	{
		PlatformImpl = RefCountable.Create(impl);
	}

	public virtual void Dispose()
	{
		PlatformImpl.Dispose();
	}

	public Bitmap(PixelFormat format, AlphaFormat alphaFormat, IntPtr data, PixelSize size, Vector dpi, int stride)
	{
		IPlatformRenderInterface factory = GetFactory();
		if (factory.IsSupportedBitmapPixelFormat(format))
		{
			PlatformImpl = RefCountable.Create(factory.LoadBitmap(format, alphaFormat, data, size, dpi, stride));
			return;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(size.Width * size.Height * 4);
		int num = size.Width * 4;
		try
		{
			PixelFormatReader.Transcode(intPtr, data, size, stride, num, format);
			AlphaFormat alphaFormat2 = (format.HasAlpha ? alphaFormat : AlphaFormat.Opaque);
			PlatformImpl = RefCountable.Create(factory.LoadBitmap(PixelFormat.Rgba8888, alphaFormat2, intPtr, size, dpi, num));
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		_isTranscoded = true;
	}

	public void Save(string fileName, int? quality = null)
	{
		PlatformImpl.Item.Save(fileName, quality);
	}

	public void Save(Stream stream, int? quality = null)
	{
		PlatformImpl.Item.Save(stream, quality);
	}

	private protected unsafe void CopyPixelsCore(PixelRect sourceRect, IntPtr buffer, int bufferSize, int stride, ILockedFramebuffer fb)
	{
		if ((sourceRect.Width <= 0 || sourceRect.Height <= 0) && (sourceRect.X != 0 || sourceRect.Y != 0))
		{
			throw new ArgumentOutOfRangeException("sourceRect");
		}
		if (sourceRect.X < 0 || sourceRect.Y < 0)
		{
			throw new ArgumentOutOfRangeException("sourceRect");
		}
		if (sourceRect.Width <= 0)
		{
			sourceRect = sourceRect.WithWidth(PixelSize.Width);
		}
		if (sourceRect.Height <= 0)
		{
			sourceRect = sourceRect.WithHeight(PixelSize.Height);
		}
		if (sourceRect.Right > PixelSize.Width || sourceRect.Bottom > PixelSize.Height)
		{
			throw new ArgumentOutOfRangeException("sourceRect");
		}
		int num = checked(sourceRect.Width * fb.Format.BitsPerPixel + 7) / 8;
		if (stride < num)
		{
			throw new ArgumentOutOfRangeException("stride");
		}
		if (stride * sourceRect.Height > bufferSize)
		{
			throw new ArgumentOutOfRangeException("bufferSize");
		}
		for (int i = 0; i < sourceRect.Height; i++)
		{
			IntPtr intPtr = fb.Address + fb.RowBytes * i;
			Unsafe.CopyBlock((buffer + stride * i).ToPointer(), intPtr.ToPointer(), (uint)num);
		}
	}

	public virtual void CopyPixels(PixelRect sourceRect, IntPtr buffer, int bufferSize, int stride)
	{
		if (!Format.HasValue || !(PlatformImpl.Item is IReadableBitmapImpl readableBitmapImpl) || Format != readableBitmapImpl.Format)
		{
			throw new NotSupportedException("CopyPixels is not supported for this bitmap type");
		}
		if (_isTranscoded)
		{
			throw new NotSupportedException("CopyPixels is not supported for transcoded bitmaps");
		}
		using ILockedFramebuffer fb = readableBitmapImpl.Lock();
		CopyPixelsCore(sourceRect, buffer, bufferSize, stride, fb);
	}

	void IImage.Draw(DrawingContext context, Rect sourceRect, Rect destRect)
	{
		context.DrawBitmap(PlatformImpl, 1.0, sourceRect, destRect);
	}

	private static IPlatformRenderInterface GetFactory()
	{
		return AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();
	}
}
