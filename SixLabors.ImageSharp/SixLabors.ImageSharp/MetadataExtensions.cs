using System;
using System.Diagnostics.CodeAnalysis;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Qoi;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Metadata;

namespace SixLabors.ImageSharp;

public static class MetadataExtensions
{
	public static BmpMetadata GetBmpMetadata(this ImageMetadata metadata)
	{
		return metadata.GetFormatMetadata(BmpFormat.Instance);
	}

	public static GifMetadata GetGifMetadata(this ImageMetadata source)
	{
		return source.GetFormatMetadata(GifFormat.Instance);
	}

	public static bool TryGetGifMetadata(this ImageMetadata source, [NotNullWhen(true)] out GifMetadata? metadata)
	{
		return source.TryGetFormatMetadata(GifFormat.Instance, out metadata);
	}

	public static GifFrameMetadata GetGifMetadata(this ImageFrameMetadata source)
	{
		return source.GetFormatMetadata(GifFormat.Instance);
	}

	public static bool TryGetGifMetadata(this ImageFrameMetadata source, [NotNullWhen(true)] out GifFrameMetadata? metadata)
	{
		return source.TryGetFormatMetadata(GifFormat.Instance, out metadata);
	}

	internal static AnimatedImageMetadata ToAnimatedImageMetadata(this GifMetadata source)
	{
		Color backgroundColor = Color.Transparent;
		if (source.GlobalColorTable.HasValue)
		{
			backgroundColor = source.GlobalColorTable.Value.Span[source.BackgroundColorIndex];
		}
		return new AnimatedImageMetadata
		{
			ColorTable = source.GlobalColorTable,
			ColorTableMode = ((source.ColorTableMode != 0) ? FrameColorTableMode.Local : FrameColorTableMode.Global),
			RepeatCount = source.RepeatCount,
			BackgroundColor = backgroundColor
		};
	}

	internal static AnimatedImageFrameMetadata ToAnimatedImageFrameMetadata(this GifFrameMetadata source)
	{
		int num;
		if (source.DisposalMethod != GifDisposalMethod.RestoreToBackground)
		{
			ReadOnlyMemory<Color>? localColorTable = source.LocalColorTable;
			num = ((localColorTable.HasValue && localColorTable.GetValueOrDefault().Length == 256 && !source.HasTransparency) ? 1 : 0);
		}
		else
		{
			num = 1;
		}
		bool flag = (byte)num != 0;
		flag |= source.ColorTableMode == GifColorTableMode.Global && !source.HasTransparency;
		return new AnimatedImageFrameMetadata
		{
			ColorTable = source.LocalColorTable,
			ColorTableMode = ((source.ColorTableMode != 0) ? FrameColorTableMode.Local : FrameColorTableMode.Global),
			Duration = TimeSpan.FromMilliseconds(source.FrameDelay * 10),
			DisposalMode = GetMode(source.DisposalMethod),
			BlendMode = ((!flag) ? FrameBlendMode.Over : FrameBlendMode.Source)
		};
	}

	private static FrameDisposalMode GetMode(GifDisposalMethod method)
	{
		return method switch
		{
			GifDisposalMethod.NotDispose => FrameDisposalMode.DoNotDispose, 
			GifDisposalMethod.RestoreToBackground => FrameDisposalMode.RestoreToBackground, 
			GifDisposalMethod.RestoreToPrevious => FrameDisposalMode.RestoreToPrevious, 
			_ => FrameDisposalMode.Unspecified, 
		};
	}

	public static JpegMetadata GetJpegMetadata(this ImageMetadata metadata)
	{
		return metadata.GetFormatMetadata(JpegFormat.Instance);
	}

	public static PbmMetadata GetPbmMetadata(this ImageMetadata metadata)
	{
		return metadata.GetFormatMetadata(PbmFormat.Instance);
	}

	public static PngMetadata GetPngMetadata(this ImageMetadata source)
	{
		return source.GetFormatMetadata(PngFormat.Instance);
	}

	public static bool TryGetPngMetadata(this ImageMetadata source, [NotNullWhen(true)] out PngMetadata? metadata)
	{
		return source.TryGetFormatMetadata(PngFormat.Instance, out metadata);
	}

	public static PngFrameMetadata GetPngMetadata(this ImageFrameMetadata source)
	{
		return source.GetFormatMetadata(PngFormat.Instance);
	}

	public static bool TryGetPngMetadata(this ImageFrameMetadata source, [NotNullWhen(true)] out PngFrameMetadata? metadata)
	{
		return source.TryGetFormatMetadata(PngFormat.Instance, out metadata);
	}

	internal static AnimatedImageMetadata ToAnimatedImageMetadata(this PngMetadata source)
	{
		return new AnimatedImageMetadata
		{
			ColorTable = source.ColorTable,
			ColorTableMode = FrameColorTableMode.Global,
			RepeatCount = (ushort)Numerics.Clamp(source.RepeatCount, 0u, 65535u)
		};
	}

	internal static AnimatedImageFrameMetadata ToAnimatedImageFrameMetadata(this PngFrameMetadata source)
	{
		double num = source.FrameDelay.ToDouble();
		if (double.IsNaN(num))
		{
			num = 0.0;
		}
		return new AnimatedImageFrameMetadata
		{
			ColorTableMode = FrameColorTableMode.Global,
			Duration = TimeSpan.FromMilliseconds(num * 1000.0),
			DisposalMode = GetMode(source.DisposalMethod),
			BlendMode = ((source.BlendMethod != 0) ? FrameBlendMode.Over : FrameBlendMode.Source)
		};
	}

	private static FrameDisposalMode GetMode(PngDisposalMethod method)
	{
		return method switch
		{
			PngDisposalMethod.DoNotDispose => FrameDisposalMode.DoNotDispose, 
			PngDisposalMethod.RestoreToBackground => FrameDisposalMode.RestoreToBackground, 
			PngDisposalMethod.RestoreToPrevious => FrameDisposalMode.RestoreToPrevious, 
			_ => FrameDisposalMode.Unspecified, 
		};
	}

	public static QoiMetadata GetQoiMetadata(this ImageMetadata metadata)
	{
		return metadata.GetFormatMetadata(QoiFormat.Instance);
	}

	public static TgaMetadata GetTgaMetadata(this ImageMetadata metadata)
	{
		return metadata.GetFormatMetadata(TgaFormat.Instance);
	}

	public static TiffMetadata GetTiffMetadata(this ImageMetadata metadata)
	{
		return metadata.GetFormatMetadata(TiffFormat.Instance);
	}

	public static TiffFrameMetadata GetTiffMetadata(this ImageFrameMetadata metadata)
	{
		return metadata.GetFormatMetadata(TiffFormat.Instance);
	}

	public static WebpMetadata GetWebpMetadata(this ImageMetadata metadata)
	{
		return metadata.GetFormatMetadata(WebpFormat.Instance);
	}

	public static bool TryGetWebpMetadata(this ImageMetadata source, [NotNullWhen(true)] out WebpMetadata? metadata)
	{
		return source.TryGetFormatMetadata(WebpFormat.Instance, out metadata);
	}

	public static WebpFrameMetadata GetWebpMetadata(this ImageFrameMetadata metadata)
	{
		return metadata.GetFormatMetadata(WebpFormat.Instance);
	}

	public static bool TryGetWebpFrameMetadata(this ImageFrameMetadata source, [NotNullWhen(true)] out WebpFrameMetadata? metadata)
	{
		return source.TryGetFormatMetadata(WebpFormat.Instance, out metadata);
	}

	internal static AnimatedImageMetadata ToAnimatedImageMetadata(this WebpMetadata source)
	{
		return new AnimatedImageMetadata
		{
			ColorTableMode = FrameColorTableMode.Global,
			RepeatCount = source.RepeatCount,
			BackgroundColor = source.BackgroundColor
		};
	}

	internal static AnimatedImageFrameMetadata ToAnimatedImageFrameMetadata(this WebpFrameMetadata source)
	{
		return new AnimatedImageFrameMetadata
		{
			ColorTableMode = FrameColorTableMode.Global,
			Duration = TimeSpan.FromMilliseconds(source.FrameDelay),
			DisposalMode = GetMode(source.DisposalMethod),
			BlendMode = ((source.BlendMethod == WebpBlendMethod.Over) ? FrameBlendMode.Over : FrameBlendMode.Source)
		};
	}

	private static FrameDisposalMode GetMode(WebpDisposalMethod method)
	{
		return method switch
		{
			WebpDisposalMethod.RestoreToBackground => FrameDisposalMode.RestoreToBackground, 
			WebpDisposalMethod.DoNotDispose => FrameDisposalMode.DoNotDispose, 
			_ => FrameDisposalMode.DoNotDispose, 
		};
	}
}
