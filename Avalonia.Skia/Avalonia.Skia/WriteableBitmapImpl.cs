using System;
using System.IO;
using System.Threading;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Internal;
using Avalonia.Skia.Helpers;
using SkiaSharp;

namespace Avalonia.Skia;

internal class WriteableBitmapImpl : IWriteableBitmapImpl, IBitmapImpl, IDisposable, IReadableBitmapImpl, IDrawableBitmapImpl
{
	private class BitmapFramebuffer : ILockedFramebuffer, IDisposable
	{
		private WriteableBitmapImpl _parent;

		private SKBitmap _bitmap;

		public IntPtr Address => _bitmap.GetPixels();

		public PixelSize Size => new PixelSize(_bitmap.Width, _bitmap.Height);

		public int RowBytes => _bitmap.RowBytes;

		public Vector Dpi => _parent.Dpi;

		public PixelFormat Format => _bitmap.ColorType.ToPixelFormat();

		public BitmapFramebuffer(WriteableBitmapImpl parent, SKBitmap bitmap)
		{
			_parent = parent;
			_bitmap = bitmap;
			Monitor.Enter(parent._lock);
		}

		public void Dispose()
		{
			_bitmap.NotifyPixelsChanged();
			_parent.Version++;
			Monitor.Exit(_parent._lock);
			_bitmap = null;
			_parent = null;
		}
	}

	private static readonly SKBitmapReleaseDelegate s_releaseDelegate = ReleaseProc;

	private readonly SKBitmap _bitmap;

	private readonly object _lock = new object();

	public Vector Dpi { get; }

	public PixelSize PixelSize { get; }

	public int Version { get; private set; } = 1;

	public PixelFormat? Format => _bitmap.ColorType.ToAvalonia();

	public WriteableBitmapImpl(Stream stream)
	{
		using SKManagedStream stream2 = new SKManagedStream(stream);
		using SKData data = SKData.Create(stream2);
		_bitmap = SKBitmap.Decode(data);
		if (_bitmap == null)
		{
			throw new ArgumentException("Unable to load bitmap from provided data");
		}
		PixelSize = new PixelSize(_bitmap.Width, _bitmap.Height);
		Dpi = SkiaPlatform.DefaultDpi;
	}

	public WriteableBitmapImpl(Stream stream, int decodeSize, bool horizontal, BitmapInterpolationMode interpolationMode)
	{
		using SKManagedStream stream2 = new SKManagedStream(stream);
		using SKData data = SKData.Create(stream2);
		using SKCodec sKCodec = SKCodec.Create(data);
		SKImageInfo info = sKCodec.Info;
		SKSizeI scaledDimensions = sKCodec.GetScaledDimensions(horizontal ? ((float)decodeSize / (float)info.Width) : ((float)decodeSize / (float)info.Height));
		SKImageInfo bitmapInfo = new SKImageInfo(scaledDimensions.Width, scaledDimensions.Height);
		SKBitmap sKBitmap = SKBitmap.Decode(sKCodec, bitmapInfo);
		double num = (horizontal ? ((double)info.Height / (double)info.Width) : ((double)info.Width / (double)info.Height));
		SKImageInfo info2 = ((!horizontal) ? new SKImageInfo((int)(num * (double)decodeSize), decodeSize) : new SKImageInfo(decodeSize, (int)(num * (double)decodeSize)));
		if (sKBitmap.Width != info2.Width || sKBitmap.Height != info2.Height)
		{
			SKBitmap sKBitmap2 = sKBitmap.Resize(info2, interpolationMode.ToSKFilterQuality());
			sKBitmap.Dispose();
			sKBitmap = sKBitmap2;
		}
		_bitmap = sKBitmap;
		PixelSize = new PixelSize(sKBitmap.Width, sKBitmap.Height);
		Dpi = SkiaPlatform.DefaultDpi;
	}

	public WriteableBitmapImpl(PixelSize size, Vector dpi, PixelFormat format, AlphaFormat alphaFormat)
	{
		PixelSize = size;
		Dpi = dpi;
		SKColorType colorType = format.ToSkColorType();
		SKAlphaType alphaType = alphaFormat.ToSkAlphaType();
		_bitmap = new SKBitmap();
		SKImageInfo info = new SKImageInfo(size.Width, size.Height, colorType, alphaType);
		UnmanagedBlob unmanagedBlob = new UnmanagedBlob(info.BytesSize);
		_bitmap.InstallPixels(info, unmanagedBlob.Address, info.RowBytes, s_releaseDelegate, unmanagedBlob);
		_bitmap.Erase(SKColor.Empty);
	}

	public void Draw(DrawingContextImpl context, SKRect sourceRect, SKRect destRect, SKPaint paint)
	{
		lock (_lock)
		{
			context.Canvas.DrawBitmap(_bitmap, sourceRect, destRect, paint);
		}
	}

	public virtual void Dispose()
	{
		_bitmap.Dispose();
	}

	public void Save(Stream stream, int? quality = null)
	{
		using SKImage image = GetSnapshot();
		ImageSavingHelper.SaveImage(image, stream, quality);
	}

	public void Save(string fileName, int? quality = null)
	{
		using SKImage image = GetSnapshot();
		ImageSavingHelper.SaveImage(image, fileName, quality);
	}

	public ILockedFramebuffer Lock()
	{
		return new BitmapFramebuffer(this, _bitmap);
	}

	public SKImage GetSnapshot()
	{
		lock (_lock)
		{
			return SKImage.FromPixels(_bitmap.Info, _bitmap.GetPixels(), _bitmap.RowBytes);
		}
	}

	private static void ReleaseProc(IntPtr address, object ctx)
	{
		((UnmanagedBlob)ctx).Dispose();
	}
}
