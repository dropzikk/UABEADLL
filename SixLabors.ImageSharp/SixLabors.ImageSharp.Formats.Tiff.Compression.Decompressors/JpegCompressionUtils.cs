using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal static class JpegCompressionUtils
{
	public static void CopyImageBytesToBuffer(Configuration configuration, Span<byte> buffer, Buffer2D<Rgb24> pixelBuffer)
	{
		int num = 0;
		for (int i = 0; i < pixelBuffer.Height; i++)
		{
			Span<Rgb24> span = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<Rgb24> instance = PixelOperations<Rgb24>.Instance;
			ReadOnlySpan<Rgb24> sourcePixels = span;
			int num2 = num;
			instance.ToRgb24Bytes(configuration, sourcePixels, buffer.Slice(num2, buffer.Length - num2), span.Length);
			num += Unsafe.SizeOf<Rgb24>() * span.Length;
		}
	}

	public static void CopyImageBytesToBuffer(Configuration configuration, Span<byte> buffer, Buffer2D<L8> pixelBuffer)
	{
		int num = 0;
		for (int i = 0; i < pixelBuffer.Height; i++)
		{
			Span<L8> span = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<L8> instance = PixelOperations<L8>.Instance;
			ReadOnlySpan<L8> sourcePixels = span;
			int num2 = num;
			instance.ToL8Bytes(configuration, sourcePixels, buffer.Slice(num2, buffer.Length - num2), span.Length);
			num += Unsafe.SizeOf<L8>() * span.Length;
		}
	}
}
