using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Metal;
using Avalonia.OpenGL;
using Avalonia.Platform;
using Avalonia.Skia.Metal;
using SkiaSharp;

namespace Avalonia.Skia;

internal class PlatformRenderInterface : IPlatformRenderInterface
{
	private readonly long? _maxResourceBytes;

	public bool SupportsIndividualRoundRects => true;

	public AlphaFormat DefaultAlphaFormat => AlphaFormat.Premul;

	public PixelFormat DefaultPixelFormat { get; }

	public PlatformRenderInterface(long? maxResourceBytes = null)
	{
		_maxResourceBytes = maxResourceBytes;
		DefaultPixelFormat = SKImageInfo.PlatformColorType.ToPixelFormat();
	}

	public IPlatformRenderInterfaceContext CreateBackendContext(IPlatformGraphicsContext? graphicsContext)
	{
		if (graphicsContext == null)
		{
			return new SkiaContext(null);
		}
		if (graphicsContext is ISkiaGpu gpu)
		{
			return new SkiaContext(gpu);
		}
		if (graphicsContext is IGlContext context)
		{
			return new SkiaContext(new GlSkiaGpu(context, _maxResourceBytes));
		}
		if (graphicsContext is IMetalDevice device)
		{
			return new SkiaContext(new SkiaMetalGpu(device, _maxResourceBytes));
		}
		throw new ArgumentException("Graphics context of type is not supported");
	}

	public bool IsSupportedBitmapPixelFormat(PixelFormat format)
	{
		if (!(format == PixelFormats.Rgb565) && !(format == PixelFormats.Bgra8888))
		{
			return format == PixelFormats.Rgba8888;
		}
		return true;
	}

	public IGeometryImpl CreateEllipseGeometry(Rect rect)
	{
		return new EllipseGeometryImpl(rect);
	}

	public IGeometryImpl CreateLineGeometry(Point p1, Point p2)
	{
		return new LineGeometryImpl(p1, p2);
	}

	public IGeometryImpl CreateRectangleGeometry(Rect rect)
	{
		return new RectangleGeometryImpl(rect);
	}

	public IStreamGeometryImpl CreateStreamGeometry()
	{
		return new StreamGeometryImpl();
	}

	public IGeometryImpl CreateGeometryGroup(FillRule fillRule, IReadOnlyList<IGeometryImpl> children)
	{
		return new GeometryGroupImpl(fillRule, children);
	}

	public IGeometryImpl CreateCombinedGeometry(GeometryCombineMode combineMode, IGeometryImpl g1, IGeometryImpl g2)
	{
		return CombinedGeometryImpl.ForceCreate(combineMode, g1, g2);
	}

	public IGeometryImpl BuildGlyphRunGeometry(GlyphRun glyphRun)
	{
		GlyphTypefaceImpl obj = (glyphRun.GlyphTypeface as GlyphTypefaceImpl) ?? throw new InvalidOperationException("PlatformImpl can't be null.");
		float size = (float)glyphRun.FontRenderingEmSize;
		SKFont sKFont = obj.SKFont;
		sKFont.Size = size;
		sKFont.Hinting = SKFontHinting.None;
		SKPath sKPath = new SKPath();
		glyphRun.BaselineOrigin.Deconstruct(out var x, out var y);
		double num = x;
		double num2 = y;
		for (int i = 0; i < glyphRun.GlyphInfos.Count; i++)
		{
			ushort glyphIndex = glyphRun.GlyphInfos[i].GlyphIndex;
			SKPath glyphPath = sKFont.GetGlyphPath(glyphIndex);
			if (!glyphPath.IsEmpty)
			{
				sKPath.AddPath(glyphPath, (float)num, (float)num2);
			}
			num += glyphRun.GlyphInfos[i].GlyphAdvance;
		}
		return new StreamGeometryImpl(sKPath, sKPath, null);
	}

	public IBitmapImpl LoadBitmap(string fileName)
	{
		using FileStream stream = File.OpenRead(fileName);
		return LoadBitmap(stream);
	}

	public IBitmapImpl LoadBitmap(Stream stream)
	{
		return new ImmutableBitmap(stream);
	}

	public IWriteableBitmapImpl LoadWriteableBitmapToWidth(Stream stream, int width, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		return new WriteableBitmapImpl(stream, width, horizontal: true, interpolationMode);
	}

	public IWriteableBitmapImpl LoadWriteableBitmapToHeight(Stream stream, int height, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		return new WriteableBitmapImpl(stream, height, horizontal: false, interpolationMode);
	}

	public IWriteableBitmapImpl LoadWriteableBitmap(string fileName)
	{
		using FileStream stream = File.OpenRead(fileName);
		return LoadWriteableBitmap(stream);
	}

	public IWriteableBitmapImpl LoadWriteableBitmap(Stream stream)
	{
		return new WriteableBitmapImpl(stream);
	}

	public IBitmapImpl LoadBitmap(PixelFormat format, AlphaFormat alphaFormat, IntPtr data, PixelSize size, Vector dpi, int stride)
	{
		return new ImmutableBitmap(size, dpi, stride, format, alphaFormat, data);
	}

	public IBitmapImpl LoadBitmapToWidth(Stream stream, int width, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		return new ImmutableBitmap(stream, width, horizontal: true, interpolationMode);
	}

	public IBitmapImpl LoadBitmapToHeight(Stream stream, int height, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		return new ImmutableBitmap(stream, height, horizontal: false, interpolationMode);
	}

	public IBitmapImpl ResizeBitmap(IBitmapImpl bitmapImpl, PixelSize destinationSize, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
	{
		if (bitmapImpl is ImmutableBitmap src)
		{
			return new ImmutableBitmap(src, destinationSize, interpolationMode);
		}
		throw new Exception("Invalid source bitmap type.");
	}

	public IRenderTargetBitmapImpl CreateRenderTargetBitmap(PixelSize size, Vector dpi)
	{
		if (size.Width < 1)
		{
			throw new ArgumentException("Width can't be less than 1", "size");
		}
		if (size.Height < 1)
		{
			throw new ArgumentException("Height can't be less than 1", "size");
		}
		return new RenderTargetBitmapImpl(size, dpi);
	}

	public IWriteableBitmapImpl CreateWriteableBitmap(PixelSize size, Vector dpi, PixelFormat format, AlphaFormat alphaFormat)
	{
		return new WriteableBitmapImpl(size, dpi, format, alphaFormat);
	}

	public IGlyphRunImpl CreateGlyphRun(IGlyphTypeface glyphTypeface, double fontRenderingEmSize, IReadOnlyList<GlyphInfo> glyphInfos, Point baselineOrigin)
	{
		return new GlyphRunImpl(glyphTypeface, fontRenderingEmSize, glyphInfos, baselineOrigin);
	}
}
