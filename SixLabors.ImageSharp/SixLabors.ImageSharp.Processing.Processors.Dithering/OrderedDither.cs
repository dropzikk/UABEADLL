using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Processing.Processors.Dithering;

public readonly struct OrderedDither : IDither, IEquatable<OrderedDither>, IEquatable<IDither>
{
	private readonly DenseMatrix<float> thresholdMatrix;

	private readonly int modulusX;

	private readonly int modulusY;

	public static readonly OrderedDither Bayer2x2 = new OrderedDither(2u);

	public static readonly OrderedDither Bayer4x4 = new OrderedDither(4u);

	public static readonly OrderedDither Bayer8x8 = new OrderedDither(8u);

	public static readonly OrderedDither Bayer16x16 = new OrderedDither(16u);

	public static readonly OrderedDither Ordered3x3 = new OrderedDither(3u);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OrderedDither(uint length)
	{
		Guard.MustBeGreaterThan(length, 0u, "length");
		DenseMatrix<uint> denseMatrix = OrderedDitherFactory.CreateDitherMatrix(length);
		DenseMatrix<float> denseMatrix2 = new DenseMatrix<float>((int)length);
		float num = length * length;
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				denseMatrix2[i, j] = (float)(denseMatrix[i, j] + 1) / num - 0.5f;
			}
		}
		modulusX = denseMatrix.Columns;
		modulusY = denseMatrix.Rows;
		thresholdMatrix = denseMatrix2;
	}

	public static bool operator ==(IDither left, OrderedDither right)
	{
		return right == left;
	}

	public static bool operator !=(IDither left, OrderedDither right)
	{
		return !(right == left);
	}

	public static bool operator ==(OrderedDither left, IDither right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(OrderedDither left, IDither right)
	{
		return !(left == right);
	}

	public static bool operator ==(OrderedDither left, OrderedDither right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(OrderedDither left, OrderedDither right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyQuantizationDither<TFrameQuantizer, TPixel>(ref TFrameQuantizer quantizer, ImageFrame<TPixel> source, IndexedImageFrame<TPixel> destination, Rectangle bounds) where TFrameQuantizer : struct, IQuantizer<TPixel> where TPixel : unmanaged, IPixel<TPixel>
	{
		if (this == default(OrderedDither))
		{
			ThrowDefaultInstance();
		}
		int spread = CalculatePaletteSpread(destination.Palette.Length);
		float ditherScale = quantizer.Options.DitherScale;
		Buffer2D<TPixel> pixelBuffer = source.PixelBuffer;
		for (int i = bounds.Top; i < bounds.Bottom; i++)
		{
			ReadOnlySpan<TPixel> readOnlySpan = pixelBuffer.DangerousGetRowSpan(i).Slice(bounds.X, bounds.Width);
			Span<byte> span = destination.GetWritablePixelRowSpanUnsafe(i - bounds.Y).Slice(0, readOnlySpan.Length);
			for (int j = 0; j < readOnlySpan.Length; j++)
			{
				TPixel color = Dither(readOnlySpan[j], j, i, spread, ditherScale);
				span[j] = quantizer.GetQuantizedColor(color, out var _);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyPaletteDither<TPaletteDitherImageProcessor, TPixel>(in TPaletteDitherImageProcessor processor, ImageFrame<TPixel> source, Rectangle bounds) where TPaletteDitherImageProcessor : struct, IPaletteDitherImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
	{
		if (this == default(OrderedDither))
		{
			ThrowDefaultInstance();
		}
		int spread = CalculatePaletteSpread(processor.Palette.Length);
		float ditherScale = processor.DitherScale;
		Buffer2D<TPixel> pixelBuffer = source.PixelBuffer;
		for (int i = bounds.Top; i < bounds.Bottom; i++)
		{
			Span<TPixel> span = pixelBuffer.DangerousGetRowSpan(i).Slice(bounds.X, bounds.Width);
			for (int j = 0; j < span.Length; j++)
			{
				ref TPixel reference = ref span[j];
				TPixel color = Dither(reference, j, i, spread, ditherScale);
				reference = processor.GetPaletteColor(color);
			}
		}
	}

	internal static int CalculatePaletteSpread(int colors)
	{
		return (int)(255.0 / Math.Max(1.0, Math.Pow(colors, 1.0 / 3.0) - 1.0));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal TPixel Dither<TPixel>(TPixel source, int x, int y, int spread, float scale) where TPixel : unmanaged, IPixel<TPixel>
	{
		Unsafe.SkipInit<Rgba32>(out var value);
		source.ToRgba32(ref value);
		Unsafe.SkipInit<Rgba32>(out var value2);
		float num = (float)spread * thresholdMatrix[y % modulusY, x % modulusX] * scale;
		value2.R = (byte)Numerics.Clamp((float)(int)value.R + num, 0f, 255f);
		value2.G = (byte)Numerics.Clamp((float)(int)value.G + num, 0f, 255f);
		value2.B = (byte)Numerics.Clamp((float)(int)value.B + num, 0f, 255f);
		value2.A = (byte)Numerics.Clamp((float)(int)value.A + num, 0f, 255f);
		TPixel result = default(TPixel);
		result.FromRgba32(value2);
		return result;
	}

	public override bool Equals(object? obj)
	{
		if (obj is OrderedDither other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(OrderedDither other)
	{
		if (thresholdMatrix.Equals(other.thresholdMatrix) && modulusX == other.modulusX)
		{
			return modulusY == other.modulusY;
		}
		return false;
	}

	public bool Equals(IDither? other)
	{
		return Equals((object?)other);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return HashCode.Combine(thresholdMatrix, modulusX, modulusY);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowDefaultInstance()
	{
		throw new ImageProcessingException("Cannot use the default value type instance to dither.");
	}

	void IDither.ApplyPaletteDither<TPaletteDitherImageProcessor, TPixel>(in TPaletteDitherImageProcessor processor, ImageFrame<TPixel> source, Rectangle bounds)
	{
		ApplyPaletteDither(in processor, source, bounds);
	}
}
