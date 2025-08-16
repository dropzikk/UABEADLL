using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

public static class SkiaSharpExtensions
{
	public static SKFilterQuality ToSKFilterQuality(this BitmapInterpolationMode interpolationMode)
	{
		switch (interpolationMode)
		{
		case BitmapInterpolationMode.Unspecified:
		case BitmapInterpolationMode.LowQuality:
			return SKFilterQuality.Low;
		case BitmapInterpolationMode.MediumQuality:
			return SKFilterQuality.Medium;
		case BitmapInterpolationMode.HighQuality:
			return SKFilterQuality.High;
		case BitmapInterpolationMode.None:
			return SKFilterQuality.None;
		default:
			throw new ArgumentOutOfRangeException("interpolationMode", interpolationMode, null);
		}
	}

	public static SKBlendMode ToSKBlendMode(this BitmapBlendingMode blendingMode)
	{
		switch (blendingMode)
		{
		case BitmapBlendingMode.Unspecified:
		case BitmapBlendingMode.SourceOver:
			return SKBlendMode.SrcOver;
		case BitmapBlendingMode.Source:
			return SKBlendMode.Src;
		case BitmapBlendingMode.SourceIn:
			return SKBlendMode.SrcIn;
		case BitmapBlendingMode.SourceOut:
			return SKBlendMode.SrcOut;
		case BitmapBlendingMode.SourceAtop:
			return SKBlendMode.SrcATop;
		case BitmapBlendingMode.Destination:
			return SKBlendMode.Dst;
		case BitmapBlendingMode.DestinationIn:
			return SKBlendMode.DstIn;
		case BitmapBlendingMode.DestinationOut:
			return SKBlendMode.DstOut;
		case BitmapBlendingMode.DestinationOver:
			return SKBlendMode.DstOver;
		case BitmapBlendingMode.DestinationAtop:
			return SKBlendMode.DstATop;
		case BitmapBlendingMode.Xor:
			return SKBlendMode.Xor;
		case BitmapBlendingMode.Plus:
			return SKBlendMode.Plus;
		default:
			throw new ArgumentOutOfRangeException("blendingMode", blendingMode, null);
		}
	}

	public static SKPoint ToSKPoint(this Point p)
	{
		return new SKPoint((float)p.X, (float)p.Y);
	}

	public static SKPoint ToSKPoint(this Vector p)
	{
		return new SKPoint((float)p.X, (float)p.Y);
	}

	public static SKRect ToSKRect(this Rect r)
	{
		return new SKRect((float)r.X, (float)r.Y, (float)r.Right, (float)r.Bottom);
	}

	public static SKRoundRect ToSKRoundRect(this RoundedRect r)
	{
		SKRect rect = r.Rect.ToSKRect();
		SKRoundRect sKRoundRect = new SKRoundRect();
		sKRoundRect.SetRectRadii(rect, new SKPoint[4]
		{
			r.RadiiTopLeft.ToSKPoint(),
			r.RadiiTopRight.ToSKPoint(),
			r.RadiiBottomRight.ToSKPoint(),
			r.RadiiBottomLeft.ToSKPoint()
		});
		return sKRoundRect;
	}

	public static Rect ToAvaloniaRect(this SKRect r)
	{
		return new Rect(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
	}

	public static SKMatrix ToSKMatrix(this Matrix m)
	{
		SKMatrix result = default(SKMatrix);
		result.ScaleX = (float)m.M11;
		result.SkewX = (float)m.M21;
		result.TransX = (float)m.M31;
		result.SkewY = (float)m.M12;
		result.ScaleY = (float)m.M22;
		result.TransY = (float)m.M32;
		result.Persp0 = (float)m.M13;
		result.Persp1 = (float)m.M23;
		result.Persp2 = (float)m.M33;
		return result;
	}

	public static SKColor ToSKColor(this Color c)
	{
		return new SKColor(c.R, c.G, c.B, c.A);
	}

	public static SKColorType ToSkColorType(this PixelFormat fmt)
	{
		if (fmt == PixelFormat.Rgb565)
		{
			return SKColorType.Rgb565;
		}
		if (fmt == PixelFormat.Bgra8888)
		{
			return SKColorType.Bgra8888;
		}
		if (fmt == PixelFormat.Rgba8888)
		{
			return SKColorType.Rgba8888;
		}
		PixelFormat pixelFormat = fmt;
		throw new ArgumentException("Unknown pixel format: " + pixelFormat.ToString());
	}

	public static PixelFormat? ToAvalonia(this SKColorType colorType)
	{
		return colorType switch
		{
			SKColorType.Rgb565 => PixelFormats.Rgb565, 
			SKColorType.Bgra8888 => PixelFormats.Bgra8888, 
			SKColorType.Rgba8888 => PixelFormats.Rgba8888, 
			_ => null, 
		};
	}

	public static PixelFormat ToPixelFormat(this SKColorType fmt)
	{
		return fmt switch
		{
			SKColorType.Rgb565 => PixelFormat.Rgb565, 
			SKColorType.Bgra8888 => PixelFormat.Bgra8888, 
			SKColorType.Rgba8888 => PixelFormat.Rgba8888, 
			_ => throw new ArgumentException("Unknown pixel format: " + fmt), 
		};
	}

	public static SKAlphaType ToSkAlphaType(this AlphaFormat fmt)
	{
		return fmt switch
		{
			AlphaFormat.Premul => SKAlphaType.Premul, 
			AlphaFormat.Unpremul => SKAlphaType.Unpremul, 
			AlphaFormat.Opaque => SKAlphaType.Opaque, 
			_ => throw new ArgumentException($"Unknown alpha format: {fmt}"), 
		};
	}

	public static AlphaFormat ToAlphaFormat(this SKAlphaType fmt)
	{
		return fmt switch
		{
			SKAlphaType.Premul => AlphaFormat.Premul, 
			SKAlphaType.Unpremul => AlphaFormat.Unpremul, 
			SKAlphaType.Opaque => AlphaFormat.Opaque, 
			_ => throw new ArgumentException($"Unknown alpha format: {fmt}"), 
		};
	}

	public static SKShaderTileMode ToSKShaderTileMode(this GradientSpreadMethod m)
	{
		return m switch
		{
			GradientSpreadMethod.Reflect => SKShaderTileMode.Mirror, 
			GradientSpreadMethod.Repeat => SKShaderTileMode.Repeat, 
			_ => SKShaderTileMode.Clamp, 
		};
	}

	public static SKTextAlign ToSKTextAlign(this TextAlignment a)
	{
		return a switch
		{
			TextAlignment.Center => SKTextAlign.Center, 
			TextAlignment.Right => SKTextAlign.Right, 
			_ => SKTextAlign.Left, 
		};
	}

	public static TextAlignment ToAvalonia(this SKTextAlign a)
	{
		return a switch
		{
			SKTextAlign.Center => TextAlignment.Center, 
			SKTextAlign.Right => TextAlignment.Right, 
			_ => TextAlignment.Left, 
		};
	}

	public static FontStyle ToAvalonia(this SKFontStyleSlant slant)
	{
		return slant switch
		{
			SKFontStyleSlant.Upright => FontStyle.Normal, 
			SKFontStyleSlant.Italic => FontStyle.Italic, 
			SKFontStyleSlant.Oblique => FontStyle.Oblique, 
			_ => throw new ArgumentOutOfRangeException("slant", slant, null), 
		};
	}

	[return: NotNullIfNotNull("src")]
	public static SKPath? Clone(this SKPath? src)
	{
		if (src == null)
		{
			return null;
		}
		return new SKPath(src);
	}
}
