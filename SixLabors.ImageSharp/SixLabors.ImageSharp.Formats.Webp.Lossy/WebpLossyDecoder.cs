using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats.Webp.BitReader;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal sealed class WebpLossyDecoder
{
	private readonly Vp8BitReader bitReader;

	private readonly MemoryAllocator memoryAllocator;

	private readonly Configuration configuration;

	public WebpLossyDecoder(Vp8BitReader bitReader, MemoryAllocator memoryAllocator, Configuration configuration)
	{
		this.bitReader = bitReader;
		this.memoryAllocator = memoryAllocator;
		this.configuration = configuration;
	}

	public void Decode<TPixel>(Buffer2D<TPixel> pixels, int width, int height, WebpImageInfo info, IMemoryOwner<byte> alphaData) where TPixel : unmanaged, IPixel<TPixel>
	{
		sbyte colorSpace = (sbyte)bitReader.ReadValue(1);
		sbyte clampType = (sbyte)bitReader.ReadValue(1);
		Vp8PictureHeader pictureHeader = new Vp8PictureHeader
		{
			Width = (uint)width,
			Height = (uint)height,
			XScale = info.XScale,
			YScale = info.YScale,
			ColorSpace = colorSpace,
			ClampType = clampType
		};
		Vp8Proba vp8Proba = new Vp8Proba();
		Vp8SegmentHeader segmentHeader = ParseSegmentHeader(vp8Proba);
		using Vp8Decoder vp8Decoder = new Vp8Decoder(info.Vp8FrameHeader, pictureHeader, segmentHeader, vp8Proba, memoryAllocator);
		Vp8Io io = InitializeVp8Io(vp8Decoder, pictureHeader);
		ParseFilterHeader(vp8Decoder);
		vp8Decoder.PrecomputeFilterStrengths();
		ParsePartitions(vp8Decoder);
		ParseDequantizationIndices(vp8Decoder);
		bitReader.ReadBool();
		ParseProbabilities(vp8Decoder);
		ParseFrame(vp8Decoder, io);
		WebpFeatures? features = info.Features;
		if (features != null && features.Alpha)
		{
			using (AlphaDecoder alphaDecoder = new AlphaDecoder(width, height, alphaData, info.Features.AlphaChunkHeader, memoryAllocator, configuration))
			{
				alphaDecoder.Decode();
				DecodePixelValues(width, height, vp8Decoder.Pixels.Memory.Span, pixels, alphaDecoder.Alpha);
				return;
			}
		}
		DecodePixelValues(width, height, vp8Decoder.Pixels.Memory.Span, pixels);
	}

	private void DecodePixelValues<TPixel>(int width, int height, Span<byte> pixelData, Buffer2D<TPixel> decodedPixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = width * 3;
		for (int i = 0; i < height; i++)
		{
			Span<byte> span = pixelData.Slice(i * num, num);
			Span<TPixel> destinationPixels = decodedPixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromBgr24Bytes(configuration, span, destinationPixels, width);
		}
	}

	private static void DecodePixelValues<TPixel>(int width, int height, Span<byte> pixelData, Buffer2D<TPixel> decodedPixels, IMemoryOwner<byte> alpha) where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel val = default(TPixel);
		Span<byte> span = alpha.Memory.Span;
		Span<Bgr24> span2 = MemoryMarshal.Cast<byte, Bgr24>(pixelData);
		for (int i = 0; i < height; i++)
		{
			int num = i * width;
			Span<TPixel> span3 = decodedPixels.DangerousGetRowSpan(i);
			for (int j = 0; j < width; j++)
			{
				int index = num + j;
				Bgr24 bgr = span2[index];
				val.FromBgra32(new Bgra32(bgr.R, bgr.G, bgr.B, span[index]));
				span3[j] = val;
			}
		}
	}

	private void ParseFrame(Vp8Decoder dec, Vp8Io io)
	{
		dec.MbY = 0;
		while (dec.MbY < dec.BottomRightMbY)
		{
			long num = dec.MbY & dec.NumPartsMinusOne;
			Vp8BitReader bitreader = dec.Vp8BitReaders[num];
			for (int i = 0; i < dec.MbWidth; i++)
			{
				ParseIntraMode(dec, i);
			}
			int mbX;
			while (dec.MbX < dec.MbWidth)
			{
				DecodeMacroBlock(dec, bitreader);
				mbX = dec.MbX + 1;
				dec.MbX = mbX;
			}
			InitScanline(dec);
			ProcessRow(dec, io);
			mbX = dec.MbY + 1;
			dec.MbY = mbX;
		}
	}

	private void ParseIntraMode(Vp8Decoder dec, int mbX)
	{
		Vp8MacroBlockData vp8MacroBlockData = dec.MacroBlockData[mbX];
		Span<byte> span = dec.IntraT.AsSpan(4 * mbX, 4);
		byte[] intraL = dec.IntraL;
		if (dec.SegmentHeader.UpdateMap)
		{
			vp8MacroBlockData.Segment = ((bitReader.GetBit((int)dec.Probabilities.Segments[0]) == 0) ? ((byte)bitReader.GetBit((int)dec.Probabilities.Segments[1])) : ((byte)(bitReader.GetBit((int)dec.Probabilities.Segments[2]) + 2)));
		}
		else
		{
			vp8MacroBlockData.Segment = 0;
		}
		if (dec.UseSkipProbability)
		{
			vp8MacroBlockData.Skip = bitReader.GetBit(dec.SkipProbability) == 1;
		}
		vp8MacroBlockData.IsI4x4 = bitReader.GetBit(145) == 0;
		if (!vp8MacroBlockData.IsI4x4)
		{
			int num = ((bitReader.GetBit(156) != 0) ? ((bitReader.GetBit(128) != 0) ? 1 : 3) : ((bitReader.GetBit(163) != 0) ? 2 : 0));
			vp8MacroBlockData.Modes[0] = (byte)num;
			for (int i = 0; i < intraL.Length; i++)
			{
				intraL[i] = (byte)num;
				span[i] = (byte)num;
			}
		}
		else
		{
			Span<byte> destination = vp8MacroBlockData.Modes.AsSpan();
			for (int j = 0; j < 4; j++)
			{
				int num2 = intraL[j];
				for (int k = 0; k < 4; k++)
				{
					byte[] array = WebpLookupTables.ModesProba[span[k], num2];
					int num3;
					for (num3 = WebpConstants.YModesIntra4[bitReader.GetBit(array[0])]; num3 > 0; num3 = WebpConstants.YModesIntra4[2 * num3 + bitReader.GetBit(array[num3])])
					{
					}
					num2 = -num3;
					span[k] = (byte)num2;
				}
				span.CopyTo(destination);
				destination = destination.Slice(4, destination.Length - 4);
				intraL[j] = (byte)num2;
			}
		}
		if (bitReader.GetBit(142) == 0)
		{
			vp8MacroBlockData.UvMode = 0;
		}
		else if (bitReader.GetBit(114) == 0)
		{
			vp8MacroBlockData.UvMode = 2;
		}
		else if (bitReader.GetBit(183) != 0)
		{
			vp8MacroBlockData.UvMode = 1;
		}
		else
		{
			vp8MacroBlockData.UvMode = 3;
		}
	}

	private static void InitScanline(Vp8Decoder dec)
	{
		Vp8MacroBlock leftMacroBlock = dec.LeftMacroBlock;
		leftMacroBlock.NoneZeroAcDcCoeffs = 0u;
		leftMacroBlock.NoneZeroDcCoeffs = 0u;
		for (int i = 0; i < dec.IntraL.Length; i++)
		{
			dec.IntraL[i] = 0;
		}
		dec.MbX = 0;
	}

	private void ProcessRow(Vp8Decoder dec, Vp8Io io)
	{
		ReconstructRow(dec);
		FinishRow(dec, io);
	}

	private void ReconstructRow(Vp8Decoder dec)
	{
		int mbY = dec.MbY;
		Memory<byte> memory = dec.YuvBuffer.Memory;
		Span<byte> span = memory.Span;
		ref Span<byte> reference = ref span;
		Span<byte> dst = reference.Slice(40, reference.Length - 40);
		reference = ref span;
		Span<byte> dst2 = reference.Slice(584, reference.Length - 584);
		reference = ref span;
		Span<byte> dst3 = reference.Slice(600, reference.Length - 600);
		int num = 512;
		for (int i = 0; i < num; i += 32)
		{
			span[i - 1 + 40] = 129;
		}
		num = 256;
		for (int j = 0; j < num; j += 32)
		{
			span[j - 1 + 584] = 129;
			span[j - 1 + 600] = 129;
		}
		if (mbY > 0)
		{
			span[7] = (span[551] = (span[567] = 129));
		}
		else
		{
			Span<byte> span2 = span.Slice(7, 21);
			for (int k = 0; k < span2.Length; k++)
			{
				span2[k] = 127;
			}
			span2 = span.Slice(551, 9);
			for (int l = 0; l < span2.Length; l++)
			{
				span2[l] = 127;
			}
			span2 = span.Slice(567, 9);
			for (int m = 0; m < span2.Length; m++)
			{
				span2[m] = 127;
			}
		}
		Span<int> scratch = stackalloc int[16];
		Span<byte> vals = stackalloc byte[4];
		for (int n = 0; n < dec.MbWidth; n++)
		{
			Vp8MacroBlockData vp8MacroBlockData = dec.MacroBlockData[n];
			Span<byte> span3;
			int num4;
			if (n > 0)
			{
				for (int num2 = -1; num2 < 16; num2++)
				{
					int start = num2 * 32 + 12 + 40;
					int num3 = num2 * 32 - 4 + 40;
					span3 = span.Slice(start, 4);
					reference = ref span;
					num4 = num3;
					span3.CopyTo(reference.Slice(num4, reference.Length - num4));
				}
				for (int num5 = -1; num5 < 8; num5++)
				{
					int start2 = num5 * 32 + 4 + 584;
					int num6 = num5 * 32 - 4 + 584;
					span3 = span.Slice(start2, 4);
					reference = ref span;
					num4 = num6;
					span3.CopyTo(reference.Slice(num4, reference.Length - num4));
					start2 = num5 * 32 + 4 + 600;
					num6 = num5 * 32 - 4 + 600;
					span3 = span.Slice(start2, 4);
					reference = ref span;
					num4 = num6;
					span3.CopyTo(reference.Slice(num4, reference.Length - num4));
				}
			}
			Vp8TopSamples vp8TopSamples = dec.YuvTopSamples[n];
			short[] coeffs = vp8MacroBlockData.Coeffs;
			uint num7 = vp8MacroBlockData.NonZeroY;
			if (mbY > 0)
			{
				byte[] y = vp8TopSamples.Y;
				reference = ref span;
				y.CopyTo(reference.Slice(8, reference.Length - 8));
				byte[] u = vp8TopSamples.U;
				reference = ref span;
				u.CopyTo(reference.Slice(552, reference.Length - 552));
				byte[] v = vp8TopSamples.V;
				reference = ref span;
				v.CopyTo(reference.Slice(568, reference.Length - 568));
			}
			if (vp8MacroBlockData.IsI4x4)
			{
				reference = ref span;
				Span<byte> destination = reference.Slice(24, reference.Length - 24);
				if (mbY > 0)
				{
					if (n >= dec.MbWidth - 1)
					{
						byte b = vp8TopSamples.Y[15];
						destination[0] = b;
						destination[1] = b;
						destination[2] = b;
						destination[3] = b;
					}
					else
					{
						span3 = dec.YuvTopSamples[n + 1].Y.AsSpan(0, 4);
						span3.CopyTo(destination);
					}
				}
				reference = ref span;
				Span<uint> span4 = MemoryMarshal.Cast<byte, uint>(reference.Slice(24, reference.Length - 24));
				span4[32] = (span4[64] = (span4[96] = span4[0]));
				int num8 = 0;
				while (num8 < 16)
				{
					int num9 = 40 + WebpConstants.Scan[num8];
					reference = ref span;
					num4 = num9;
					Span<byte> dst4 = reference.Slice(num4, reference.Length - num4);
					switch (vp8MacroBlockData.Modes[num8])
					{
					case 0:
						LossyUtils.DC4(dst4, span, num9);
						break;
					case 1:
						LossyUtils.TM4(dst4, span, num9);
						break;
					case 2:
						LossyUtils.VE4(dst4, span, num9, vals);
						break;
					case 3:
						LossyUtils.HE4(dst4, span, num9);
						break;
					case 4:
						LossyUtils.RD4(dst4, span, num9);
						break;
					case 5:
						LossyUtils.VR4(dst4, span, num9);
						break;
					case 6:
						LossyUtils.LD4(dst4, span, num9);
						break;
					case 7:
						LossyUtils.VL4(dst4, span, num9);
						break;
					case 8:
						LossyUtils.HD4(dst4, span, num9);
						break;
					case 9:
						LossyUtils.HU4(dst4, span, num9);
						break;
					}
					DoTransform(num7, coeffs.AsSpan(num8 * 16), dst4, scratch);
					num8++;
					num7 <<= 2;
				}
			}
			else
			{
				switch (CheckMode(n, mbY, vp8MacroBlockData.Modes[0]))
				{
				case 0:
					LossyUtils.DC16(dst, span, 40);
					break;
				case 1:
					LossyUtils.TM16(dst, span, 40);
					break;
				case 2:
					LossyUtils.VE16(dst, span, 40);
					break;
				case 3:
					LossyUtils.HE16(dst, span, 40);
					break;
				case 4:
					LossyUtils.DC16NoTop(dst, span, 40);
					break;
				case 5:
					LossyUtils.DC16NoLeft(dst, span, 40);
					break;
				case 6:
					LossyUtils.DC16NoTopLeft(dst);
					break;
				}
				if (num7 != 0)
				{
					int num10 = 0;
					while (num10 < 16)
					{
						uint bits = num7;
						Span<short> src = coeffs.AsSpan(num10 * 16);
						reference = ref dst;
						num4 = WebpConstants.Scan[num10];
						DoTransform(bits, src, reference.Slice(num4, reference.Length - num4), scratch);
						num10++;
						num7 <<= 2;
					}
				}
			}
			uint nonZeroUv = vp8MacroBlockData.NonZeroUv;
			switch (CheckMode(n, mbY, vp8MacroBlockData.UvMode))
			{
			case 0:
				LossyUtils.DC8uv(dst2, span, 584);
				LossyUtils.DC8uv(dst3, span, 600);
				break;
			case 1:
				LossyUtils.TM8uv(dst2, span, 584);
				LossyUtils.TM8uv(dst3, span, 600);
				break;
			case 2:
				LossyUtils.VE8uv(dst2, span, 584);
				LossyUtils.VE8uv(dst3, span, 600);
				break;
			case 3:
				LossyUtils.HE8uv(dst2, span, 584);
				LossyUtils.HE8uv(dst3, span, 600);
				break;
			case 4:
				LossyUtils.DC8uvNoTop(dst2, span, 584);
				LossyUtils.DC8uvNoTop(dst3, span, 600);
				break;
			case 5:
				LossyUtils.DC8uvNoLeft(dst2, span, 584);
				LossyUtils.DC8uvNoLeft(dst3, span, 600);
				break;
			case 6:
				LossyUtils.DC8uvNoTopLeft(dst2);
				LossyUtils.DC8uvNoTopLeft(dst3);
				break;
			}
			DoUVTransform(nonZeroUv, coeffs.AsSpan(256), dst2, scratch);
			DoUVTransform(nonZeroUv >> 8, coeffs.AsSpan(320), dst3, scratch);
			if (mbY < dec.MbHeight - 1)
			{
				span3 = dst.Slice(480, 16);
				span3.CopyTo(vp8TopSamples.Y);
				span3 = dst2.Slice(224, 8);
				span3.CopyTo(vp8TopSamples.U);
				span3 = dst3.Slice(224, 8);
				span3.CopyTo(vp8TopSamples.V);
			}
			memory = dec.CacheY.Memory;
			span3 = memory.Span;
			reference = ref span3;
			num4 = dec.CacheYOffset + n * 16;
			Span<byte> span5 = reference.Slice(num4, reference.Length - num4);
			memory = dec.CacheU.Memory;
			span3 = memory.Span;
			reference = ref span3;
			num4 = dec.CacheUvOffset + n * 8;
			Span<byte> span6 = reference.Slice(num4, reference.Length - num4);
			memory = dec.CacheV.Memory;
			span3 = memory.Span;
			reference = ref span3;
			num4 = dec.CacheUvOffset + n * 8;
			Span<byte> span7 = reference.Slice(num4, reference.Length - num4);
			for (int num11 = 0; num11 < 16; num11++)
			{
				span3 = dst.Slice(num11 * 32, Math.Min(16, span5.Length));
				reference = ref span5;
				num4 = num11 * dec.CacheYStride;
				span3.CopyTo(reference.Slice(num4, reference.Length - num4));
			}
			for (int num12 = 0; num12 < 8; num12++)
			{
				int num13 = num12 * dec.CacheUvStride;
				span3 = dst2.Slice(num12 * 32, Math.Min(8, span6.Length));
				reference = ref span6;
				num4 = num13;
				span3.CopyTo(reference.Slice(num4, reference.Length - num4));
				span3 = dst3.Slice(num12 * 32, Math.Min(8, span7.Length));
				reference = ref span7;
				num4 = num13;
				span3.CopyTo(reference.Slice(num4, reference.Length - num4));
			}
		}
	}

	private static void FilterRow(Vp8Decoder dec)
	{
		int mbY = dec.MbY;
		for (int i = dec.TopLeftMbX; i < dec.BottomRightMbX; i++)
		{
			DoFilter(dec, i, mbY);
		}
	}

	private static void DoFilter(Vp8Decoder dec, int mbx, int mby)
	{
		int cacheYStride = dec.CacheYStride;
		Vp8FilterInfo vp8FilterInfo = dec.FilterInfo[mbx];
		int innerLevel = vp8FilterInfo.InnerLevel;
		int limit = vp8FilterInfo.Limit;
		if (limit == 0)
		{
			return;
		}
		Memory<byte> memory;
		switch (dec.Filter)
		{
		case LoopFilter.Simple:
		{
			int offset3 = dec.CacheYOffset + mbx * 16;
			if (mbx > 0)
			{
				memory = dec.CacheY.Memory;
				LossyUtils.SimpleHFilter16(memory.Span, offset3, cacheYStride, limit + 4);
			}
			if (vp8FilterInfo.UseInnerFiltering)
			{
				memory = dec.CacheY.Memory;
				LossyUtils.SimpleHFilter16i(memory.Span, offset3, cacheYStride, limit);
			}
			if (mby > 0)
			{
				memory = dec.CacheY.Memory;
				LossyUtils.SimpleVFilter16(memory.Span, offset3, cacheYStride, limit + 4);
			}
			if (vp8FilterInfo.UseInnerFiltering)
			{
				memory = dec.CacheY.Memory;
				LossyUtils.SimpleVFilter16i(memory.Span, offset3, cacheYStride, limit);
			}
			break;
		}
		case LoopFilter.Complex:
		{
			int cacheUvStride = dec.CacheUvStride;
			int offset = dec.CacheYOffset + mbx * 16;
			int offset2 = dec.CacheUvOffset + mbx * 8;
			int highEdgeVarianceThreshold = vp8FilterInfo.HighEdgeVarianceThreshold;
			if (mbx > 0)
			{
				memory = dec.CacheY.Memory;
				LossyUtils.HFilter16(memory.Span, offset, cacheYStride, limit + 4, innerLevel, highEdgeVarianceThreshold);
				memory = dec.CacheU.Memory;
				Span<byte> span = memory.Span;
				memory = dec.CacheV.Memory;
				LossyUtils.HFilter8(span, memory.Span, offset2, cacheUvStride, limit + 4, innerLevel, highEdgeVarianceThreshold);
			}
			if (vp8FilterInfo.UseInnerFiltering)
			{
				memory = dec.CacheY.Memory;
				LossyUtils.HFilter16i(memory.Span, offset, cacheYStride, limit, innerLevel, highEdgeVarianceThreshold);
				memory = dec.CacheU.Memory;
				Span<byte> span2 = memory.Span;
				memory = dec.CacheV.Memory;
				LossyUtils.HFilter8i(span2, memory.Span, offset2, cacheUvStride, limit, innerLevel, highEdgeVarianceThreshold);
			}
			if (mby > 0)
			{
				memory = dec.CacheY.Memory;
				LossyUtils.VFilter16(memory.Span, offset, cacheYStride, limit + 4, innerLevel, highEdgeVarianceThreshold);
				memory = dec.CacheU.Memory;
				Span<byte> span3 = memory.Span;
				memory = dec.CacheV.Memory;
				LossyUtils.VFilter8(span3, memory.Span, offset2, cacheUvStride, limit + 4, innerLevel, highEdgeVarianceThreshold);
			}
			if (vp8FilterInfo.UseInnerFiltering)
			{
				memory = dec.CacheY.Memory;
				LossyUtils.VFilter16i(memory.Span, offset, cacheYStride, limit, innerLevel, highEdgeVarianceThreshold);
				memory = dec.CacheU.Memory;
				Span<byte> span4 = memory.Span;
				memory = dec.CacheV.Memory;
				LossyUtils.VFilter8i(span4, memory.Span, offset2, cacheUvStride, limit, innerLevel, highEdgeVarianceThreshold);
			}
			break;
		}
		}
	}

	private static void FinishRow(Vp8Decoder dec, Vp8Io io)
	{
		int num = WebpConstants.FilterExtraRows[(int)dec.Filter];
		int length = num * dec.CacheYStride;
		int length2 = num / 2 * dec.CacheUvStride;
		Memory<byte> memory = dec.CacheY.Memory;
		Span<byte> span = memory.Span;
		memory = dec.CacheU.Memory;
		Span<byte> span2 = memory.Span;
		memory = dec.CacheV.Memory;
		Span<byte> span3 = memory.Span;
		int mbY = dec.MbY;
		bool flag = mbY == 0;
		bool flag2 = mbY >= dec.BottomRightMbY - 1;
		if (dec.Filter != 0 && dec.MbY >= dec.TopLeftMbY && dec.MbY <= dec.BottomRightMbY)
		{
			FilterRow(dec);
		}
		int num2 = mbY * 16;
		int num3 = (mbY + 1) * 16;
		if (!flag)
		{
			num2 -= num;
			io.Y = span;
			io.U = span2;
			io.V = span3;
		}
		else
		{
			memory = dec.CacheY.Memory;
			Span<byte> span4 = memory.Span;
			ref Span<byte> reference = ref span4;
			int cacheYOffset = dec.CacheYOffset;
			io.Y = reference.Slice(cacheYOffset, reference.Length - cacheYOffset);
			memory = dec.CacheU.Memory;
			span4 = memory.Span;
			reference = ref span4;
			cacheYOffset = dec.CacheUvOffset;
			io.U = reference.Slice(cacheYOffset, reference.Length - cacheYOffset);
			memory = dec.CacheV.Memory;
			span4 = memory.Span;
			reference = ref span4;
			cacheYOffset = dec.CacheUvOffset;
			io.V = reference.Slice(cacheYOffset, reference.Length - cacheYOffset);
		}
		if (!flag2)
		{
			num3 -= num;
		}
		if (num3 > io.Height)
		{
			num3 = io.Height;
		}
		if (num2 < num3)
		{
			io.MbY = num2;
			io.MbW = io.Width;
			io.MbH = num3 - num2;
			EmitRgb(dec, io);
		}
		if (!flag2)
		{
			Span<byte> span4 = span.Slice(16 * dec.CacheYStride, length);
			memory = dec.CacheY.Memory;
			span4.CopyTo(memory.Span);
			span4 = span2.Slice(8 * dec.CacheUvStride, length2);
			memory = dec.CacheU.Memory;
			span4.CopyTo(memory.Span);
			span4 = span3.Slice(8 * dec.CacheUvStride, length2);
			memory = dec.CacheV.Memory;
			span4.CopyTo(memory.Span);
		}
	}

	private static int EmitRgb(Vp8Decoder dec, Vp8Io io)
	{
		Memory<byte> memory = dec.Pixels.Memory;
		Span<byte> span = memory.Span;
		int num = io.MbH;
		Span<byte> span2 = io.Y;
		Span<byte> span3 = io.U;
		Span<byte> span4 = io.V;
		memory = dec.TmpYBuffer.Memory;
		Span<byte> span5 = memory.Span;
		memory = dec.TmpUBuffer.Memory;
		Span<byte> span6 = memory.Span;
		memory = dec.TmpVBuffer.Memory;
		Span<byte> span7 = memory.Span;
		Span<byte> span8 = span6;
		Span<byte> span9 = span7;
		int num2 = 3 * io.Width;
		int num3 = io.MbY * num2;
		ref Span<byte> reference = ref span;
		int num4 = num3;
		Span<byte> span10 = reference.Slice(num4, reference.Length - num4);
		int num5 = io.MbY + io.MbH;
		int mbW = io.MbW;
		int length = mbW + 1 >> 1;
		int i = io.MbY;
		byte[] uvBuffer = new byte[463];
		if (i == 0)
		{
			YuvConversion.UpSample(span2, default(Span<byte>), span3, span4, span3, span4, span10, default(Span<byte>), mbW, uvBuffer);
		}
		else
		{
			Span<byte> bottomY = span2;
			Span<byte> topU = span8;
			Span<byte> topV = span9;
			Span<byte> curU = span3;
			Span<byte> curV = span4;
			reference = ref span;
			num4 = num3 - num2;
			YuvConversion.UpSample(span5, bottomY, topU, topV, curU, curV, reference.Slice(num4, reference.Length - num4), span10, mbW, uvBuffer);
			num++;
		}
		int num6 = 2 * num2;
		int num7 = 2 * io.YStride;
		for (; i + 2 < num5; i += 2)
		{
			span8 = span3;
			span9 = span4;
			reference = ref span3;
			num4 = io.UvStride;
			span3 = reference.Slice(num4, reference.Length - num4);
			reference = ref span4;
			num4 = io.UvStride;
			span4 = reference.Slice(num4, reference.Length - num4);
			reference = ref span2;
			num4 = io.YStride;
			Span<byte> topY = reference.Slice(num4, reference.Length - num4);
			reference = ref span2;
			num4 = num7;
			Span<byte> bottomY2 = reference.Slice(num4, reference.Length - num4);
			Span<byte> topU2 = span8;
			Span<byte> topV2 = span9;
			Span<byte> curU2 = span3;
			Span<byte> curV2 = span4;
			reference = ref span10;
			num4 = num2;
			Span<byte> topDst = reference.Slice(num4, reference.Length - num4);
			reference = ref span10;
			num4 = num6;
			YuvConversion.UpSample(topY, bottomY2, topU2, topV2, curU2, curV2, topDst, reference.Slice(num4, reference.Length - num4), mbW, uvBuffer);
			reference = ref span2;
			num4 = num7;
			span2 = reference.Slice(num4, reference.Length - num4);
			reference = ref span10;
			num4 = num6;
			span10 = reference.Slice(num4, reference.Length - num4);
		}
		reference = ref span2;
		num4 = io.YStride;
		span2 = reference.Slice(num4, reference.Length - num4);
		if (num5 < io.Height)
		{
			span2.Slice(0, mbW).CopyTo(span5);
			span3.Slice(0, length).CopyTo(span6);
			span4.Slice(0, length).CopyTo(span7);
			num--;
		}
		else if ((num5 & 1) == 0)
		{
			Span<byte> topY2 = span2;
			Span<byte> topU3 = span3;
			Span<byte> topV3 = span4;
			Span<byte> curU3 = span3;
			Span<byte> curV3 = span4;
			reference = ref span10;
			num4 = num2;
			YuvConversion.UpSample(topY2, default(Span<byte>), topU3, topV3, curU3, curV3, reference.Slice(num4, reference.Length - num4), default(Span<byte>), mbW, uvBuffer);
		}
		return num;
	}

	private static void DoTransform(uint bits, Span<short> src, Span<byte> dst, Span<int> scratch)
	{
		switch (bits >> 30)
		{
		case 3u:
			LossyUtils.TransformOne(src, dst, scratch);
			break;
		case 2u:
			LossyUtils.TransformAc3(src, dst);
			break;
		case 1u:
			LossyUtils.TransformDc(src, dst);
			break;
		}
	}

	private static void DoUVTransform(uint bits, Span<short> src, Span<byte> dst, Span<int> scratch)
	{
		if ((bits & 0xFF) != 0)
		{
			if ((bits & 0xAA) != 0)
			{
				LossyUtils.TransformUv(src, dst, scratch);
			}
			else
			{
				LossyUtils.TransformDcuv(src, dst);
			}
		}
	}

	private void DecodeMacroBlock(Vp8Decoder dec, Vp8BitReader bitreader)
	{
		Vp8MacroBlock leftMacroBlock = dec.LeftMacroBlock;
		Vp8MacroBlock currentMacroBlock = dec.CurrentMacroBlock;
		Vp8MacroBlockData currentBlockData = dec.CurrentBlockData;
		bool flag = dec.UseSkipProbability && currentBlockData.Skip;
		if (!flag)
		{
			flag = ParseResiduals(dec, bitreader, currentMacroBlock);
		}
		else
		{
			uint noneZeroAcDcCoeffs = (currentMacroBlock.NoneZeroAcDcCoeffs = 0u);
			leftMacroBlock.NoneZeroAcDcCoeffs = noneZeroAcDcCoeffs;
			if (!currentBlockData.IsI4x4)
			{
				noneZeroAcDcCoeffs = (currentMacroBlock.NoneZeroDcCoeffs = 0u);
				leftMacroBlock.NoneZeroDcCoeffs = noneZeroAcDcCoeffs;
			}
			currentBlockData.NonZeroY = 0u;
			currentBlockData.NonZeroUv = 0u;
		}
		if (dec.Filter != 0)
		{
			Vp8FilterInfo vp8FilterInfo = dec.FilterStrength[currentBlockData.Segment, currentBlockData.IsI4x4 ? 1u : 0u];
			dec.FilterInfo[dec.MbX] = (Vp8FilterInfo)vp8FilterInfo.DeepClone();
			dec.FilterInfo[dec.MbX].UseInnerFiltering |= !flag;
		}
	}

	private bool ParseResiduals(Vp8Decoder dec, Vp8BitReader br, Vp8MacroBlock mb)
	{
		uint num = 0u;
		uint num2 = 0u;
		int num3 = 0;
		Vp8MacroBlockData currentBlockData = dec.CurrentBlockData;
		Vp8QuantMatrix vp8QuantMatrix = dec.DeQuantMatrices[currentBlockData.Segment];
		Vp8BandProbas[][] bandsPtr = dec.Probabilities.BandsPtr;
		Vp8MacroBlock leftMacroBlock = dec.LeftMacroBlock;
		short[] coeffs = currentBlockData.Coeffs;
		for (int i = 0; i < coeffs.Length; i++)
		{
			coeffs[i] = 0;
		}
		int num4;
		Vp8BandProbas[] prob;
		if (currentBlockData.IsI4x4)
		{
			num4 = 0;
			prob = bandsPtr[3];
		}
		else
		{
			Span<short> span = stackalloc short[16];
			int ctx = (int)(mb.NoneZeroDcCoeffs + leftMacroBlock.NoneZeroDcCoeffs);
			int coeffs2 = GetCoeffs(br, bandsPtr[1], ctx, vp8QuantMatrix.Y2Mat, 0, span);
			uint noneZeroDcCoeffs = (leftMacroBlock.NoneZeroDcCoeffs = ((coeffs2 > 0) ? 1u : 0u));
			mb.NoneZeroDcCoeffs = noneZeroDcCoeffs;
			if (coeffs2 > 1)
			{
				Span<short> input = span;
				Span<short> output = coeffs;
				Span<int> scratch = stackalloc int[16];
				LossyUtils.TransformWht(input, output, scratch);
			}
			else
			{
				int num6 = span[0] + 3 >> 3;
				for (int j = 0; j < 256; j += 16)
				{
					coeffs[j] = (short)num6;
				}
			}
			num4 = 1;
			prob = bandsPtr[0];
		}
		byte b = (byte)(mb.NoneZeroAcDcCoeffs & 0xF);
		byte b2 = (byte)(leftMacroBlock.NoneZeroAcDcCoeffs & 0xF);
		for (int k = 0; k < 4; k++)
		{
			int num7 = b2 & 1;
			uint num8 = 0u;
			for (int l = 0; l < 4; l++)
			{
				int ctx2 = num7 + (b & 1);
				int coeffs3 = GetCoeffs(br, prob, ctx2, vp8QuantMatrix.Y1Mat, num4, coeffs.AsSpan(num3));
				num7 = ((coeffs3 > num4) ? 1 : 0);
				b = (byte)((b >> 1) | (num7 << 7));
				num8 = NzCodeBits(num8, coeffs3, (coeffs[num3] != 0) ? 1 : 0);
				num3 += 16;
			}
			b >>= 4;
			b2 = (byte)((b2 >> 1) | (num7 << 7));
			num = (num << 8) | num8;
		}
		uint num9 = b;
		uint num10 = (uint)(b2 >> 4);
		for (int m = 0; m < 4; m += 2)
		{
			uint num11 = 0u;
			int num12 = 4 + m;
			b = (byte)(mb.NoneZeroAcDcCoeffs >> num12);
			b2 = (byte)(leftMacroBlock.NoneZeroAcDcCoeffs >> num12);
			for (int n = 0; n < 2; n++)
			{
				int num13 = b2 & 1;
				for (int num14 = 0; num14 < 2; num14++)
				{
					int ctx3 = num13 + (b & 1);
					int coeffs4 = GetCoeffs(br, bandsPtr[2], ctx3, vp8QuantMatrix.UvMat, 0, coeffs.AsSpan(num3));
					num13 = ((coeffs4 > 0) ? 1 : 0);
					b = (byte)((b >> 1) | (num13 << 3));
					num11 = NzCodeBits(num11, coeffs4, (coeffs[num3] != 0) ? 1 : 0);
					num3 += 16;
				}
				b >>= 2;
				b2 = (byte)((b2 >> 1) | (num13 << 5));
			}
			num2 |= num11 << 4 * m;
			num9 |= (uint)(b << 4 << m);
			num10 |= (uint)((b2 & 0xF0) << m);
		}
		mb.NoneZeroAcDcCoeffs = num9;
		leftMacroBlock.NoneZeroAcDcCoeffs = num10;
		currentBlockData.NonZeroY = num;
		currentBlockData.NonZeroUv = num2;
		return (num | num2) == 0;
	}

	private static int GetCoeffs(Vp8BitReader br, Vp8BandProbas[] prob, int ctx, int[] dq, int n, Span<short> coeffs)
	{
		Vp8ProbaArray vp8ProbaArray = prob[n].Probabilities[ctx];
		while (n < 16)
		{
			if (br.GetBit(vp8ProbaArray.Probabilities[0]) == 0)
			{
				return n;
			}
			while (br.GetBit(vp8ProbaArray.Probabilities[1]) == 0)
			{
				vp8ProbaArray = prob[++n].Probabilities[0];
				if (n == 16)
				{
					return 16;
				}
			}
			int v;
			if (br.GetBit(vp8ProbaArray.Probabilities[2]) == 0)
			{
				v = 1;
				vp8ProbaArray = prob[n + 1].Probabilities[1];
			}
			else
			{
				v = GetLargeValue(br, vp8ProbaArray.Probabilities);
				vp8ProbaArray = prob[n + 1].Probabilities[2];
			}
			int num = ((n > 0) ? 1 : 0);
			coeffs[WebpConstants.Zigzag[n]] = (short)(br.GetSigned(v) * dq[num]);
			n++;
		}
		return 16;
	}

	private static int GetLargeValue(Vp8BitReader br, byte[] p)
	{
		if (br.GetBit(p[3]) == 0)
		{
			if (br.GetBit(p[4]) == 0)
			{
				return 2;
			}
			return 3 + br.GetBit(p[5]);
		}
		int num;
		if (br.GetBit(p[6]) == 0)
		{
			if (br.GetBit(p[7]) == 0)
			{
				return 5 + br.GetBit(159);
			}
			num = 7 + 2 * br.GetBit(165);
			return num + br.GetBit(145);
		}
		int bit = br.GetBit(p[8]);
		int bit2 = br.GetBit(p[9 + bit]);
		int num2 = 2 * bit + bit2;
		num = 0;
		byte[] array = null;
		switch (num2)
		{
		case 0:
			array = WebpConstants.Cat3;
			break;
		case 1:
			array = WebpConstants.Cat4;
			break;
		case 2:
			array = WebpConstants.Cat5;
			break;
		case 3:
			array = WebpConstants.Cat6;
			break;
		default:
			WebpThrowHelper.ThrowImageFormatException("VP8 parsing error");
			break;
		}
		for (int i = 0; i < array.Length; i++)
		{
			num += num + br.GetBit(array[i]);
		}
		return num + (3 + (8 << num2));
	}

	private Vp8SegmentHeader ParseSegmentHeader(Vp8Proba proba)
	{
		Vp8SegmentHeader vp8SegmentHeader = new Vp8SegmentHeader
		{
			UseSegment = bitReader.ReadBool()
		};
		if (vp8SegmentHeader.UseSegment)
		{
			vp8SegmentHeader.UpdateMap = bitReader.ReadBool();
			if (bitReader.ReadBool())
			{
				vp8SegmentHeader.Delta = bitReader.ReadBool();
				for (int i = 0; i < vp8SegmentHeader.Quantizer.Length; i++)
				{
					bool flag = bitReader.ReadBool();
					vp8SegmentHeader.Quantizer[i] = (byte)(flag ? ((uint)bitReader.ReadSignedValue(7)) : 0u);
				}
				for (int j = 0; j < vp8SegmentHeader.FilterStrength.Length; j++)
				{
					bool flag = bitReader.ReadBool();
					vp8SegmentHeader.FilterStrength[j] = (byte)(flag ? ((uint)bitReader.ReadSignedValue(6)) : 0u);
				}
				if (vp8SegmentHeader.UpdateMap)
				{
					for (int k = 0; k < proba.Segments.Length; k++)
					{
						bool flag = bitReader.ReadBool();
						proba.Segments[k] = (flag ? bitReader.ReadValue(8) : 255u);
					}
				}
			}
		}
		else
		{
			vp8SegmentHeader.UpdateMap = false;
		}
		return vp8SegmentHeader;
	}

	private void ParseFilterHeader(Vp8Decoder dec)
	{
		Vp8FilterHeader filterHeader = dec.FilterHeader;
		filterHeader.LoopFilter = (bitReader.ReadBool() ? LoopFilter.Simple : LoopFilter.Complex);
		filterHeader.FilterLevel = (int)bitReader.ReadValue(6);
		filterHeader.Sharpness = (int)bitReader.ReadValue(3);
		filterHeader.UseLfDelta = bitReader.ReadBool();
		dec.Filter = ((filterHeader.FilterLevel != 0) ? filterHeader.LoopFilter : LoopFilter.None);
		if (filterHeader.UseLfDelta && bitReader.ReadBool())
		{
			for (int i = 0; i < filterHeader.RefLfDelta.Length; i++)
			{
				if (bitReader.ReadBool())
				{
					filterHeader.RefLfDelta[i] = bitReader.ReadSignedValue(6);
				}
			}
			for (int j = 0; j < filterHeader.ModeLfDelta.Length; j++)
			{
				if (bitReader.ReadBool())
				{
					filterHeader.ModeLfDelta[j] = bitReader.ReadSignedValue(6);
				}
			}
		}
		byte num = WebpConstants.FilterExtraRows[(int)dec.Filter];
		int cacheYOffset = num * dec.CacheYStride;
		int cacheUvOffset = num / 2 * dec.CacheUvStride;
		dec.CacheYOffset = cacheYOffset;
		dec.CacheUvOffset = cacheUvOffset;
	}

	private void ParsePartitions(Vp8Decoder dec)
	{
		uint num = bitReader.Remaining - bitReader.PartitionLength;
		int partitionLength = (int)bitReader.PartitionLength;
		Span<byte> span = bitReader.Data.Slice(partitionLength);
		int num2 = (int)num;
		dec.NumPartsMinusOne = (1 << (int)bitReader.ReadValue(2)) - 1;
		int numPartsMinusOne = dec.NumPartsMinusOne;
		int num3 = numPartsMinusOne * 3;
		int num4 = partitionLength + num3;
		num2 -= num3;
		for (int i = 0; i < numPartsMinusOne; i++)
		{
			int num5 = span[0] | (span[1] << 8) | (span[2] << 16);
			if (num5 > num2)
			{
				num5 = num2;
			}
			dec.Vp8BitReaders[i] = new Vp8BitReader(bitReader.Data, (uint)num5, num4);
			num4 += num5;
			num2 -= num5;
			span = span.Slice(3, span.Length - 3);
		}
		dec.Vp8BitReaders[numPartsMinusOne] = new Vp8BitReader(bitReader.Data, (uint)num2, num4);
	}

	private void ParseDequantizationIndices(Vp8Decoder decoder)
	{
		Vp8SegmentHeader segmentHeader = decoder.SegmentHeader;
		int num = (int)bitReader.ReadValue(7);
		int num2 = (bitReader.ReadBool() ? bitReader.ReadSignedValue(4) : 0);
		int num3 = (bitReader.ReadBool() ? bitReader.ReadSignedValue(4) : 0);
		int num4 = (bitReader.ReadBool() ? bitReader.ReadSignedValue(4) : 0);
		int num5 = (bitReader.ReadBool() ? bitReader.ReadSignedValue(4) : 0);
		int num6 = (bitReader.ReadBool() ? bitReader.ReadSignedValue(4) : 0);
		for (int i = 0; i < 4; i++)
		{
			int num7;
			if (segmentHeader.UseSegment)
			{
				num7 = segmentHeader.Quantizer[i];
				if (!segmentHeader.Delta)
				{
					num7 += num;
				}
			}
			else
			{
				if (i > 0)
				{
					decoder.DeQuantMatrices[i] = decoder.DeQuantMatrices[0];
					continue;
				}
				num7 = num;
			}
			Vp8QuantMatrix vp8QuantMatrix = decoder.DeQuantMatrices[i];
			vp8QuantMatrix.Y1Mat[0] = WebpLookupTables.DcTable[Clip(num7 + num2, 127)];
			vp8QuantMatrix.Y1Mat[1] = WebpLookupTables.AcTable[Clip(num7, 127)];
			vp8QuantMatrix.Y2Mat[0] = WebpLookupTables.DcTable[Clip(num7 + num3, 127)] * 2;
			vp8QuantMatrix.Y2Mat[1] = WebpLookupTables.AcTable[Clip(num7 + num4, 127)] * 101581 >> 16;
			if (vp8QuantMatrix.Y2Mat[1] < 8)
			{
				vp8QuantMatrix.Y2Mat[1] = 8;
			}
			vp8QuantMatrix.UvMat[0] = WebpLookupTables.DcTable[Clip(num7 + num5, 117)];
			vp8QuantMatrix.UvMat[1] = WebpLookupTables.AcTable[Clip(num7 + num6, 127)];
			vp8QuantMatrix.UvQuant = num7 + num6;
		}
	}

	private void ParseProbabilities(Vp8Decoder dec)
	{
		Vp8Proba probabilities = dec.Probabilities;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					for (int l = 0; l < 11; l++)
					{
						byte prob = WebpLookupTables.CoeffsUpdateProba[i, j, k, l];
						probabilities.Bands[i, j].Probabilities[k].Probabilities[l] = (byte)((bitReader.GetBit(prob) != 0) ? bitReader.ReadValue(8) : WebpLookupTables.DefaultCoeffsProba[i, j, k, l]);
					}
				}
			}
			for (int m = 0; m < 17; m++)
			{
				probabilities.BandsPtr[i][m] = probabilities.Bands[i, WebpConstants.Vp8EncBands[m]];
			}
		}
		dec.UseSkipProbability = bitReader.ReadBool();
		if (dec.UseSkipProbability)
		{
			dec.SkipProbability = (byte)bitReader.ReadValue(8);
		}
	}

	private static Vp8Io InitializeVp8Io(Vp8Decoder dec, Vp8PictureHeader pictureHeader)
	{
		Vp8Io result = default(Vp8Io);
		result.Width = (int)pictureHeader.Width;
		result.Height = (int)pictureHeader.Height;
		result.UseScaling = false;
		result.ScaledWidth = result.Width;
		result.ScaledHeight = result.ScaledHeight;
		result.MbW = result.Width;
		result.MbH = result.Height;
		uint num = pictureHeader.Width + 15 >> 4;
		result.YStride = (int)(16 * num);
		result.UvStride = (int)(8 * num);
		int num2 = 4 * dec.MbWidth;
		dec.IntraT = new byte[num2];
		int num3 = WebpConstants.FilterExtraRows[(int)dec.Filter];
		if (dec.Filter == LoopFilter.Complex)
		{
			dec.TopLeftMbX = 0;
			dec.TopLeftMbY = 0;
		}
		else
		{
			int topLeftMbY = (dec.TopLeftMbX = -num3 >> 4);
			dec.TopLeftMbY = topLeftMbY;
			if (dec.TopLeftMbX < 0)
			{
				dec.TopLeftMbX = 0;
			}
			if (dec.TopLeftMbY < 0)
			{
				dec.TopLeftMbY = 0;
			}
		}
		dec.BottomRightMbY = result.Height + 15 + num3 >> 4;
		dec.BottomRightMbX = result.Width + 15 + num3 >> 4;
		if (dec.BottomRightMbX > dec.MbWidth)
		{
			dec.BottomRightMbX = dec.MbWidth;
		}
		if (dec.BottomRightMbY > dec.MbHeight)
		{
			dec.BottomRightMbY = dec.MbHeight;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint NzCodeBits(uint nzCoeffs, int nz, int dcNz)
	{
		nzCoeffs <<= 2;
		uint num = nzCoeffs;
		uint num2 = ((nz > 3) ? 3u : ((nz <= 1) ? ((uint)dcNz) : 2u));
		nzCoeffs = num | num2;
		return nzCoeffs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int CheckMode(int mbx, int mby, int mode)
	{
		if (mode == 0)
		{
			if (mbx == 0)
			{
				if (mby != 0)
				{
					return 5;
				}
				return 6;
			}
			if (mby != 0)
			{
				return 0;
			}
			return 4;
		}
		return mode;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Clip(int value, int max)
	{
		return Math.Clamp(value, 0, max);
	}
}
