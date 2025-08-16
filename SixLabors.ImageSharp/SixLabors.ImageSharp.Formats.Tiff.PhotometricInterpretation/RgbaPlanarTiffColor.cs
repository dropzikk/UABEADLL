using System;
using System.Buffers;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class RgbaPlanarTiffColor<TPixel> : TiffBasePlanarColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly float rFactor;

	private readonly float gFactor;

	private readonly float bFactor;

	private readonly float aFactor;

	private readonly ushort bitsPerSampleR;

	private readonly ushort bitsPerSampleG;

	private readonly ushort bitsPerSampleB;

	private readonly ushort bitsPerSampleA;

	private readonly TiffExtraSampleType? extraSampleType;

	public RgbaPlanarTiffColor(TiffExtraSampleType? extraSampleType, TiffBitsPerSample bitsPerSample)
	{
		bitsPerSampleR = bitsPerSample.Channel0;
		bitsPerSampleG = bitsPerSample.Channel1;
		bitsPerSampleB = bitsPerSample.Channel2;
		bitsPerSampleA = bitsPerSample.Channel3;
		rFactor = (float)(1 << (int)bitsPerSampleR) - 1f;
		gFactor = (float)(1 << (int)bitsPerSampleG) - 1f;
		bFactor = (float)(1 << (int)bitsPerSampleB) - 1f;
		aFactor = (float)(1 << (int)bitsPerSampleA) - 1f;
		this.extraSampleType = extraSampleType;
	}

	public override void Decode(IMemoryOwner<byte>[] data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel val = default(TPixel);
		bool flag = extraSampleType.HasValue && extraSampleType == TiffExtraSampleType.AssociatedAlphaData;
		BitReader bitReader = new BitReader(data[0].GetSpan());
		BitReader bitReader2 = new BitReader(data[1].GetSpan());
		BitReader bitReader3 = new BitReader(data[2].GetSpan());
		BitReader bitReader4 = new BitReader(data[3].GetSpan());
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span = pixels.DangerousGetRowSpan(i).Slice(left, width);
			for (int j = 0; j < span.Length; j++)
			{
				float x = (float)bitReader.ReadBits(bitsPerSampleR) / rFactor;
				float y = (float)bitReader2.ReadBits(bitsPerSampleG) / gFactor;
				float z = (float)bitReader3.ReadBits(bitsPerSampleB) / bFactor;
				float w = (float)bitReader4.ReadBits(bitsPerSampleA) / aFactor;
				Vector4 vector = new Vector4(x, y, z, w);
				if (flag)
				{
					val = TiffUtils.UnPremultiply(ref vector, val);
				}
				else
				{
					val.FromScaledVector4(vector);
				}
				span[j] = val;
			}
			bitReader.NextRow();
			bitReader2.NextRow();
			bitReader3.NextRow();
			bitReader4.NextRow();
		}
	}
}
