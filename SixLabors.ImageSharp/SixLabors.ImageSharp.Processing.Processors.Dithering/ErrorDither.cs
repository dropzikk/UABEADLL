using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Processing.Processors.Dithering;

public readonly struct ErrorDither : IDither, IEquatable<ErrorDither>, IEquatable<IDither>
{
	public static readonly ErrorDither Atkinson = CreateAtkinson();

	public static readonly ErrorDither Burkes = CreateBurks();

	public static readonly ErrorDither FloydSteinberg = CreateFloydSteinberg();

	public static readonly ErrorDither JarvisJudiceNinke = CreateJarvisJudiceNinke();

	public static readonly ErrorDither Sierra2 = CreateSierra2();

	public static readonly ErrorDither Sierra3 = CreateSierra3();

	public static readonly ErrorDither SierraLite = CreateSierraLite();

	public static readonly ErrorDither StevensonArce = CreateStevensonArce();

	public static readonly ErrorDither Stucki = CreateStucki();

	private readonly int offset;

	private readonly DenseMatrix<float> matrix;

	private static ErrorDither CreateAtkinson()
	{
		return new ErrorDither((DenseMatrix<float>)new float[3, 4]
		{
			{ 0f, 0f, 0.125f, 0.125f },
			{ 0.125f, 0.125f, 0.125f, 0f },
			{ 0f, 0.125f, 0f, 0f }
		}, 1);
	}

	private static ErrorDither CreateBurks()
	{
		return new ErrorDither((DenseMatrix<float>)new float[2, 5]
		{
			{ 0f, 0f, 0f, 0.25f, 0.125f },
			{ 0.0625f, 0.125f, 0.25f, 0.125f, 0.0625f }
		}, 2);
	}

	private static ErrorDither CreateFloydSteinberg()
	{
		return new ErrorDither((DenseMatrix<float>)new float[2, 3]
		{
			{ 0f, 0f, 0.4375f },
			{ 0.1875f, 0.3125f, 0.0625f }
		}, 1);
	}

	private static ErrorDither CreateJarvisJudiceNinke()
	{
		return new ErrorDither((DenseMatrix<float>)new float[3, 5]
		{
			{
				0f,
				0f,
				0f,
				7f / 48f,
				5f / 48f
			},
			{
				0.0625f,
				5f / 48f,
				7f / 48f,
				5f / 48f,
				0.0625f
			},
			{
				1f / 48f,
				0.0625f,
				5f / 48f,
				0.0625f,
				1f / 48f
			}
		}, 2);
	}

	private static ErrorDither CreateSierra2()
	{
		return new ErrorDither((DenseMatrix<float>)new float[2, 5]
		{
			{ 0f, 0f, 0f, 0.25f, 0.1875f },
			{ 0.0625f, 0.125f, 0.1875f, 0.125f, 0.0625f }
		}, 2);
	}

	private static ErrorDither CreateSierra3()
	{
		return new ErrorDither((DenseMatrix<float>)new float[3, 5]
		{
			{
				0f,
				0f,
				0f,
				5f / 32f,
				3f / 32f
			},
			{
				0.0625f,
				0.125f,
				5f / 32f,
				0.125f,
				0.0625f
			},
			{
				0f,
				0.0625f,
				3f / 32f,
				0.0625f,
				0f
			}
		}, 2);
	}

	private static ErrorDither CreateSierraLite()
	{
		return new ErrorDither((DenseMatrix<float>)new float[2, 3]
		{
			{ 0f, 0f, 0.5f },
			{ 0.25f, 0.25f, 0f }
		}, 1);
	}

	private static ErrorDither CreateStevensonArce()
	{
		return new ErrorDither((DenseMatrix<float>)new float[4, 7]
		{
			{ 0f, 0f, 0f, 0f, 0f, 0.16f, 0f },
			{ 0.06f, 0f, 0.13f, 0f, 0.15f, 0f, 0.08f },
			{ 0f, 0.06f, 0f, 0.13f, 0f, 0.06f, 0f },
			{ 0.025f, 0f, 0.06f, 0f, 0.06f, 0f, 0.025f }
		}, 3);
	}

	private static ErrorDither CreateStucki()
	{
		return new ErrorDither((DenseMatrix<float>)new float[3, 5]
		{
			{
				0f,
				0f,
				0f,
				4f / 21f,
				2f / 21f
			},
			{
				1f / 21f,
				2f / 21f,
				4f / 21f,
				2f / 21f,
				1f / 21f
			},
			{
				1f / 42f,
				1f / 21f,
				2f / 21f,
				1f / 21f,
				1f / 42f
			}
		}, 2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ErrorDither(in DenseMatrix<float> matrix, int offset)
	{
		Guard.MustBeGreaterThan(offset, 0, "offset");
		this.matrix = matrix;
		this.offset = offset;
	}

	public static bool operator ==(IDither left, ErrorDither right)
	{
		return right == left;
	}

	public static bool operator !=(IDither left, ErrorDither right)
	{
		return !(right == left);
	}

	public static bool operator ==(ErrorDither left, IDither right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ErrorDither left, IDither right)
	{
		return !(left == right);
	}

	public static bool operator ==(ErrorDither left, ErrorDither right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ErrorDither left, ErrorDither right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyQuantizationDither<TFrameQuantizer, TPixel>(ref TFrameQuantizer quantizer, ImageFrame<TPixel> source, IndexedImageFrame<TPixel> destination, Rectangle bounds) where TFrameQuantizer : struct, IQuantizer<TPixel> where TPixel : unmanaged, IPixel<TPixel>
	{
		if (this == default(ErrorDither))
		{
			ThrowDefaultInstance();
		}
		int top = bounds.Top;
		int left = bounds.Left;
		float ditherScale = quantizer.Options.DitherScale;
		Buffer2D<TPixel> pixelBuffer = source.PixelBuffer;
		for (int i = 0; i < destination.Height; i++)
		{
			ReadOnlySpan<TPixel> readOnlySpan = pixelBuffer.DangerousGetRowSpan(i + top);
			Span<byte> writablePixelRowSpanUnsafe = destination.GetWritablePixelRowSpanUnsafe(i);
			for (int j = 0; j < writablePixelRowSpanUnsafe.Length; j++)
			{
				TPixel val = readOnlySpan[j + left];
				writablePixelRowSpanUnsafe[j] = quantizer.GetQuantizedColor(val, out var match);
				Dither(source, bounds, val, match, j, i, ditherScale);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyPaletteDither<TPaletteDitherImageProcessor, TPixel>(in TPaletteDitherImageProcessor processor, ImageFrame<TPixel> source, Rectangle bounds) where TPaletteDitherImageProcessor : struct, IPaletteDitherImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
	{
		if (this == default(ErrorDither))
		{
			ThrowDefaultInstance();
		}
		Buffer2D<TPixel> pixelBuffer = source.PixelBuffer;
		float ditherScale = processor.DitherScale;
		for (int i = bounds.Top; i < bounds.Bottom; i++)
		{
			ref TPixel reference = ref MemoryMarshal.GetReference(pixelBuffer.DangerousGetRowSpan(i));
			for (int j = bounds.Left; j < bounds.Right; j++)
			{
				ref TPixel reference2 = ref Unsafe.Add(ref reference, (uint)j);
				TPixel paletteColor = Unsafe.AsRef(in processor).GetPaletteColor(reference2);
				Dither(source, bounds, reference2, paletteColor, j, i, ditherScale);
				reference2 = paletteColor;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal TPixel Dither<TPixel>(ImageFrame<TPixel> image, Rectangle bounds, TPixel source, TPixel transformed, int x, int y, float scale) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (source.Equals(transformed))
		{
			return transformed;
		}
		Vector4 vector = (source.ToVector4() - transformed.ToVector4()) * scale;
		int num = offset;
		DenseMatrix<float> denseMatrix = matrix;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		int num2 = 0;
		int num3 = y;
		while (num2 < denseMatrix.Rows)
		{
			if (num3 < bounds.Bottom)
			{
				Span<TPixel> span = pixelBuffer.DangerousGetRowSpan(num3);
				for (int i = 0; i < denseMatrix.Columns; i++)
				{
					int num4 = x + (i - num);
					if (num4 >= bounds.Left && num4 < bounds.Right)
					{
						float num5 = denseMatrix[num2, i];
						if (num5 != 0f)
						{
							ref TPixel reference = ref span[num4];
							Vector4 vector2 = reference.ToVector4();
							vector2 += vector * num5;
							reference.FromVector4(vector2);
						}
					}
				}
			}
			num2++;
			num3++;
		}
		return transformed;
	}

	public override bool Equals(object? obj)
	{
		if (obj is ErrorDither other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(ErrorDither other)
	{
		if (offset == other.offset)
		{
			return matrix.Equals(other.matrix);
		}
		return false;
	}

	public bool Equals(IDither? other)
	{
		return Equals((object?)other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(offset, matrix);
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
