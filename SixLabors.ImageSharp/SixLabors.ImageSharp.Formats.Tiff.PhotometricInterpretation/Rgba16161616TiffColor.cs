using System;
using System.Buffers;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class Rgba16161616TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool isBigEndian;

	private readonly Configuration configuration;

	private readonly MemoryAllocator memoryAllocator;

	private readonly TiffExtraSampleType? extraSamplesType;

	public Rgba16161616TiffColor(Configuration configuration, MemoryAllocator memoryAllocator, TiffExtraSampleType? extraSamplesType, bool isBigEndian)
	{
		this.configuration = configuration;
		this.isBigEndian = isBigEndian;
		this.memoryAllocator = memoryAllocator;
		this.extraSamplesType = extraSamplesType;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		Rgba64 rgba64Default = TiffUtils.Rgba64Default;
		TPixel color = default(TPixel);
		color.FromScaledVector4(Vector4.Zero);
		bool flag = extraSamplesType.HasValue && extraSamplesType == TiffExtraSampleType.AssociatedAlphaData;
		int num = 0;
		using IMemoryOwner<Vector4> buffer = (flag ? memoryAllocator.Allocate<Vector4>(width) : null);
		Span<Vector4> span = (flag ? buffer.GetSpan() : Span<Vector4>.Empty);
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			if (isBigEndian)
			{
				for (int j = 0; j < span2.Length; j++)
				{
					ulong r = TiffUtils.ConvertToUShortBigEndian(data.Slice(num, 2));
					num += 2;
					ulong g = TiffUtils.ConvertToUShortBigEndian(data.Slice(num, 2));
					num += 2;
					ulong b = TiffUtils.ConvertToUShortBigEndian(data.Slice(num, 2));
					num += 2;
					ulong a = TiffUtils.ConvertToUShortBigEndian(data.Slice(num, 2));
					num += 2;
					span2[j] = (flag ? TiffUtils.ColorFromRgba64Premultiplied(rgba64Default, r, g, b, a, color) : TiffUtils.ColorFromRgba64(rgba64Default, r, g, b, a, color));
				}
			}
			else
			{
				int num2 = span2.Length * 8;
				PixelOperations<TPixel>.Instance.FromRgba64Bytes(configuration, data.Slice(num, num2), span2, span2.Length);
				if (flag)
				{
					PixelOperations<TPixel>.Instance.ToVector4(configuration, span2, span);
					PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, span2, PixelConversionModifiers.Scale | PixelConversionModifiers.Premultiply);
				}
				num += num2;
			}
		}
	}
}
