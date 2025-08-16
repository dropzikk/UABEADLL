using System;
using System.Buffers;
using System.Numerics;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class Rgba8888TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly Configuration configuration;

	private readonly MemoryAllocator memoryAllocator;

	private readonly TiffExtraSampleType? extraSamplesType;

	public Rgba8888TiffColor(Configuration configuration, MemoryAllocator memoryAllocator, TiffExtraSampleType? extraSamplesType)
	{
		this.configuration = configuration;
		this.memoryAllocator = memoryAllocator;
		this.extraSamplesType = extraSamplesType;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		int num = 0;
		bool flag = extraSamplesType.HasValue && extraSamplesType == TiffExtraSampleType.AssociatedAlphaData;
		default(TPixel).FromScaledVector4(Vector4.Zero);
		using IMemoryOwner<Vector4> buffer = (flag ? memoryAllocator.Allocate<Vector4>(width) : null);
		Span<Vector4> span = (flag ? buffer.GetSpan() : Span<Vector4>.Empty);
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			int num2 = span2.Length * 4;
			PixelOperations<TPixel>.Instance.FromRgba32Bytes(configuration, data.Slice(num, num2), span2, span2.Length);
			if (flag)
			{
				PixelOperations<TPixel>.Instance.ToVector4(configuration, span2, span);
				PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, span2, PixelConversionModifiers.Scale | PixelConversionModifiers.Premultiply);
			}
			num += num2;
		}
	}
}
