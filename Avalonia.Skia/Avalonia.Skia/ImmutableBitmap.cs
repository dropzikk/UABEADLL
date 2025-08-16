using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia.Helpers;
using SkiaSharp;

namespace Avalonia.Skia;

internal class ImmutableBitmap : IDrawableBitmapImpl, IBitmapImpl, IDisposable, IReadableBitmapImpl
{
	private readonly SKImage _image;

	private readonly SKBitmap? _bitmap;

	public Vector Dpi { get; }

	public PixelSize PixelSize { get; }

	public int Version { get; } = 1;

	public PixelFormat? Format => _bitmap?.ColorType.ToAvalonia();

	public ImmutableBitmap(Stream stream)
	{
		using SKManagedStream stream2 = new SKManagedStream(stream);
		using (SKData data = SKData.Create(stream2))
		{
			_bitmap = SKBitmap.Decode(data);
		}
		if (_bitmap == null)
		{
			throw new ArgumentException("Unable to load bitmap from provided data");
		}
		_bitmap.SetImmutable();
		_image = SKImage.FromBitmap(_bitmap);
		PixelSize = new PixelSize(_image.Width, _image.Height);
		Dpi = new Vector(96.0, 96.0);
	}

	public ImmutableBitmap(SKImage image)
	{
		_image = image;
		PixelSize = new PixelSize(image.Width, image.Height);
		Dpi = new Vector(96.0, 96.0);
	}

	public ImmutableBitmap(ImmutableBitmap src, PixelSize destinationSize, BitmapInterpolationMode interpolationMode)
	{
		SKImageInfo info = new SKImageInfo(destinationSize.Width, destinationSize.Height, SKColorType.Bgra8888);
		_bitmap = new SKBitmap(info);
		src._image.ScalePixels(_bitmap.PeekPixels(), interpolationMode.ToSKFilterQuality());
		_bitmap.SetImmutable();
		_image = SKImage.FromBitmap(_bitmap);
		PixelSize = new PixelSize(_image.Width, _image.Height);
		Dpi = new Vector(96.0, 96.0);
	}

	public ImmutableBitmap(Stream stream, int decodeSize, bool horizontal, BitmapInterpolationMode interpolationMode)
	{
		using SKManagedStream stream2 = new SKManagedStream(stream);
		using SKData data = SKData.Create(stream2);
		using SKCodec sKCodec = SKCodec.Create(data);
		SKImageInfo info = sKCodec.Info;
		SKSizeI scaledDimensions = sKCodec.GetScaledDimensions(horizontal ? ((float)decodeSize / (float)info.Width) : ((float)decodeSize / (float)info.Height));
		SKImageInfo bitmapInfo = new SKImageInfo(scaledDimensions.Width, scaledDimensions.Height);
		_bitmap = SKBitmap.Decode(sKCodec, bitmapInfo);
		if (_bitmap == null)
		{
			throw new ArgumentException("Unable to load bitmap from provided data");
		}
		double num = (horizontal ? ((double)info.Height / (double)info.Width) : ((double)info.Width / (double)info.Height));
		SKImageInfo info2 = ((!horizontal) ? new SKImageInfo((int)(num * (double)decodeSize), decodeSize) : new SKImageInfo(decodeSize, (int)(num * (double)decodeSize)));
		if (_bitmap.Width != info2.Width || _bitmap.Height != info2.Height)
		{
			SKBitmap bitmap = _bitmap.Resize(info2, interpolationMode.ToSKFilterQuality());
			_bitmap.Dispose();
			_bitmap = bitmap;
		}
		_bitmap.SetImmutable();
		_image = SKImage.FromBitmap(_bitmap);
		if (_image == null)
		{
			throw new ArgumentException("Unable to load bitmap from provided data");
		}
		PixelSize = new PixelSize(_image.Width, _image.Height);
		Dpi = new Vector(96.0, 96.0);
	}

	public ImmutableBitmap(PixelSize size, Vector dpi, int stride, PixelFormat format, AlphaFormat alphaFormat, IntPtr data)
	{
		using (SKBitmap sKBitmap = new SKBitmap())
		{
			sKBitmap.InstallPixels(new SKImageInfo(size.Width, size.Height, format.ToSkColorType(), alphaFormat.ToSkAlphaType()), data);
			_bitmap = sKBitmap.Copy();
		}
		_bitmap.SetImmutable();
		_image = SKImage.FromBitmap(_bitmap);
		if (_image == null)
		{
			throw new ArgumentException("Unable to create bitmap from provided data");
		}
		PixelSize = size;
		Dpi = dpi;
	}

	public void Dispose()
	{
		_image.Dispose();
		_bitmap?.Dispose();
	}

	public void Save(string fileName, int? quality = null)
	{
		ImageSavingHelper.SaveImage(_image, fileName, null);
	}

	public void Save(Stream stream, int? quality = null)
	{
		ImageSavingHelper.SaveImage(_image, stream, null);
	}

	public void Draw(DrawingContextImpl context, SKRect sourceRect, SKRect destRect, SKPaint paint)
	{
		context.Canvas.DrawImage(_image, sourceRect, destRect, paint);
	}

	public ILockedFramebuffer Lock()
	{
		if (_bitmap == null)
		{
			throw new NotSupportedException("A bitmap is needed for locking");
		}
		PixelFormat format = _bitmap.ColorType.ToAvalonia() ?? throw new NotSupportedException($"Unsupported format {_bitmap.ColorType}");
		return new LockedFramebuffer(_bitmap.GetPixels(), PixelSize, _bitmap.RowBytes, Dpi, format, null);
	}
}
