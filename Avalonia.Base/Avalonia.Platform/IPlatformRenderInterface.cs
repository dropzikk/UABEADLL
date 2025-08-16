using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
[PrivateApi]
public interface IPlatformRenderInterface
{
	bool SupportsIndividualRoundRects { get; }

	AlphaFormat DefaultAlphaFormat { get; }

	PixelFormat DefaultPixelFormat { get; }

	IGeometryImpl CreateEllipseGeometry(Rect rect);

	IGeometryImpl CreateLineGeometry(Point p1, Point p2);

	IGeometryImpl CreateRectangleGeometry(Rect rect);

	IStreamGeometryImpl CreateStreamGeometry();

	IGeometryImpl CreateGeometryGroup(FillRule fillRule, IReadOnlyList<IGeometryImpl> children);

	IGeometryImpl CreateCombinedGeometry(GeometryCombineMode combineMode, IGeometryImpl g1, IGeometryImpl g2);

	IGeometryImpl BuildGlyphRunGeometry(GlyphRun glyphRun);

	IRenderTargetBitmapImpl CreateRenderTargetBitmap(PixelSize size, Vector dpi);

	IWriteableBitmapImpl CreateWriteableBitmap(PixelSize size, Vector dpi, PixelFormat format, AlphaFormat alphaFormat);

	IBitmapImpl LoadBitmap(string fileName);

	IBitmapImpl LoadBitmap(Stream stream);

	IWriteableBitmapImpl LoadWriteableBitmapToWidth(Stream stream, int width, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality);

	IWriteableBitmapImpl LoadWriteableBitmapToHeight(Stream stream, int height, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality);

	IWriteableBitmapImpl LoadWriteableBitmap(string fileName);

	IWriteableBitmapImpl LoadWriteableBitmap(Stream stream);

	IBitmapImpl LoadBitmapToWidth(Stream stream, int width, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality);

	IBitmapImpl LoadBitmapToHeight(Stream stream, int height, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality);

	IBitmapImpl ResizeBitmap(IBitmapImpl bitmapImpl, PixelSize destinationSize, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality);

	IBitmapImpl LoadBitmap(PixelFormat format, AlphaFormat alphaFormat, IntPtr data, PixelSize size, Vector dpi, int stride);

	IGlyphRunImpl CreateGlyphRun(IGlyphTypeface glyphTypeface, double fontRenderingEmSize, IReadOnlyList<GlyphInfo> glyphInfos, Point baselineOrigin);

	IPlatformRenderInterfaceContext CreateBackendContext(IPlatformGraphicsContext? graphicsApiContext);

	bool IsSupportedBitmapPixelFormat(PixelFormat format);
}
