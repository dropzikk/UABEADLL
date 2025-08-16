using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Formats.Webp.BitWriter;
using SixLabors.ImageSharp.Formats.Webp.Chunks;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal class Vp8LEncoder : IDisposable
{
	private struct ScratchBuffer
	{
		private const int Size = 256;

		private unsafe fixed int scratch[256];

		public unsafe Span<int> Span => MemoryMarshal.CreateSpan(ref scratch[0], 256);
	}

	private ScratchBuffer scratch;

	private readonly int[][] histoArgb = new int[4][]
	{
		new int[256],
		new int[256],
		new int[256],
		new int[256]
	};

	private readonly int[][] bestHisto = new int[4][]
	{
		new int[256],
		new int[256],
		new int[256],
		new int[256]
	};

	private readonly MemoryAllocator memoryAllocator;

	private readonly Configuration configuration;

	private const int MaxRefsBlockPerImage = 16;

	private const int MinBlockSize = 256;

	private Vp8LBitWriter bitWriter;

	private readonly uint quality;

	private readonly WebpEncodingMethod method;

	private readonly WebpTransparentColorMode transparentColorMode;

	private readonly bool skipMetadata;

	private readonly bool nearLossless;

	private readonly int nearLosslessQuality;

	private const int ApplyPaletteGreedyMax = 4;

	private const int PaletteInvSizeBits = 11;

	private const int PaletteInvSize = 2048;

	private static ReadOnlySpan<byte> StorageOrder => new byte[19]
	{
		17, 18, 0, 1, 2, 3, 4, 5, 16, 6,
		7, 8, 9, 10, 11, 12, 13, 14, 15
	};

	private static ReadOnlySpan<byte> Order => new byte[4] { 1, 2, 0, 3 };

	public IMemoryOwner<uint> Bgra { get; }

	public IMemoryOwner<uint> EncodedData { get; }

	public IMemoryOwner<uint> BgraScratch { get; set; }

	public int CurrentWidth { get; set; }

	public int HistoBits { get; set; }

	public int TransformBits { get; set; }

	public IMemoryOwner<uint> TransformData { get; set; }

	public int CacheBits { get; set; }

	public bool UseCrossColorTransform { get; set; }

	public bool UseSubtractGreenTransform { get; set; }

	public bool UsePredictorTransform { get; set; }

	public bool UsePalette { get; set; }

	public int PaletteSize { get; set; }

	public IMemoryOwner<uint> Palette { get; }

	public Vp8LBackwardRefs[] Refs { get; }

	public Vp8LHashChain HashChain { get; }

	public Vp8LEncoder(MemoryAllocator memoryAllocator, Configuration configuration, int width, int height, uint quality, bool skipMetadata, WebpEncodingMethod method, WebpTransparentColorMode transparentColorMode, bool nearLossless, int nearLosslessQuality)
	{
		int num = width * height;
		int expectedSize = num * 2;
		this.memoryAllocator = memoryAllocator;
		this.configuration = configuration;
		this.quality = Math.Min(quality, 100u);
		this.skipMetadata = skipMetadata;
		this.method = method;
		this.transparentColorMode = transparentColorMode;
		this.nearLossless = nearLossless;
		this.nearLosslessQuality = Numerics.Clamp(nearLosslessQuality, 0, 100);
		bitWriter = new Vp8LBitWriter(expectedSize);
		Bgra = memoryAllocator.Allocate<uint>(num);
		EncodedData = memoryAllocator.Allocate<uint>(num);
		Palette = memoryAllocator.Allocate<uint>(256);
		Refs = new Vp8LBackwardRefs[3];
		HashChain = new Vp8LHashChain(memoryAllocator, num);
		int num2 = (num - 1) / 16 + 1;
		for (int i = 0; i < Refs.Length; i++)
		{
			Refs[i] = new Vp8LBackwardRefs(num)
			{
				BlockSize = ((num2 < 256) ? 256 : num2)
			};
		}
	}

	public WebpVp8X EncodeHeader<TPixel>(Image<TPixel> image, Stream stream, bool hasAnimation) where TPixel : unmanaged, IPixel<TPixel>
	{
		ImageMetadata metadata = image.Metadata;
		metadata.SyncProfiles();
		ExifProfile exifProfile = (skipMetadata ? null : metadata.ExifProfile);
		XmpProfile xmpProfile = (skipMetadata ? null : metadata.XmpProfile);
		WebpVp8X result = BitWriterBase.WriteTrunksBeforeData(stream, (uint)image.Width, (uint)image.Height, exifProfile, xmpProfile, metadata.IccProfile, hasAlpha: false, hasAnimation);
		if (hasAnimation)
		{
			WebpMetadata webpMetadata = WebpCommonUtils.GetWebpMetadata(image);
			BitWriterBase.WriteAnimationParameter(stream, webpMetadata.BackgroundColor, webpMetadata.RepeatCount);
		}
		return result;
	}

	public void EncodeFooter<TPixel>(Image<TPixel> image, in WebpVp8X vp8x, bool hasAlpha, Stream stream, long initialPosition) where TPixel : unmanaged, IPixel<TPixel>
	{
		ImageMetadata metadata = image.Metadata;
		ExifProfile exifProfile = (skipMetadata ? null : metadata.ExifProfile);
		XmpProfile xmpProfile = (skipMetadata ? null : metadata.XmpProfile);
		bool flag = hasAlpha && vp8x != default(WebpVp8X);
		BitWriterBase.WriteTrunksAfterData(stream, flag ? vp8x.WithAlpha(hasAlpha: true) : vp8x, flag, initialPosition, exifProfile, xmpProfile);
	}

	public bool Encode<TPixel>(ImageFrame<TPixel> frame, Rectangle bounds, WebpFrameMetadata frameMetadata, Stream stream, bool hasAnimation) where TPixel : unmanaged, IPixel<TPixel>
	{
		bool flag = ConvertPixelsToBgra(frame.PixelBuffer.GetRegion(bounds));
		WriteImageSize(bounds.Width, bounds.Height);
		WriteAlphaAndVersion(flag);
		EncodeStream(bounds.Width, bounds.Height);
		bitWriter.Finish();
		long sizePosition = 0L;
		if (hasAnimation)
		{
			sizePosition = new WebpFrameData((uint)bounds.Left, (uint)bounds.Top, (uint)bounds.Width, (uint)bounds.Height, frameMetadata.FrameDelay, frameMetadata.BlendMethod, frameMetadata.DisposalMethod).WriteHeaderTo(stream);
		}
		bitWriter.WriteEncodedImageToStream(stream);
		if (hasAnimation)
		{
			RiffHelper.EndWriteChunk(stream, sizePosition);
		}
		return flag;
	}

	public int EncodeAlphaImageData<TPixel>(Buffer2DRegion<TPixel> frame, IMemoryOwner<byte> alphaData) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = frame.Width;
		int height = frame.Height;
		int num = width * height;
		ConvertPixelsToBgra(frame);
		EncodeStream(width, height);
		bitWriter.Finish();
		int numBytes = bitWriter.NumBytes;
		if (numBytes >= num)
		{
			return num;
		}
		bitWriter.WriteToBuffer(alphaData.GetSpan());
		return numBytes;
	}

	private void WriteImageSize(int inputImgWidth, int inputImgHeight)
	{
		Guard.MustBeLessThan(inputImgWidth, 16383, "inputImgWidth");
		Guard.MustBeLessThan(inputImgHeight, 16383, "inputImgHeight");
		uint bits = (uint)(inputImgWidth - 1);
		uint bits2 = (uint)(inputImgHeight - 1);
		bitWriter.PutBits(bits, 14);
		bitWriter.PutBits(bits2, 14);
	}

	private void WriteAlphaAndVersion(bool hasAlpha)
	{
		bitWriter.PutBits(hasAlpha ? 1u : 0u, 1);
		bitWriter.PutBits(0u, 3);
	}

	private void EncodeStream(int width, int height)
	{
		Span<uint> span = Bgra.GetSpan();
		Span<uint> span2 = EncodedData.GetSpan();
		bool flag = method == WebpEncodingMethod.Level0;
		bool redAndBlueAlwaysZero;
		CrunchConfig[] array = EncoderAnalyze(span, width, height, out redAndBlueAlwaysZero);
		int num = 0;
		Vp8LBitWriter bwInit = bitWriter;
		Vp8LBitWriter dst = bitWriter.Clone();
		bool flag2 = true;
		CrunchConfig[] array2 = array;
		foreach (CrunchConfig crunchConfig in array2)
		{
			span.CopyTo(span2);
			EntropyIx entropyIdx = crunchConfig.EntropyIdx;
			bool usePalette = entropyIdx - 4 <= EntropyIx.Spatial;
			UsePalette = usePalette;
			entropyIdx = crunchConfig.EntropyIdx;
			usePalette = entropyIdx - 2 <= EntropyIx.Spatial;
			UseSubtractGreenTransform = usePalette;
			entropyIdx = crunchConfig.EntropyIdx;
			usePalette = ((entropyIdx == EntropyIx.Spatial || entropyIdx == EntropyIx.SpatialSubGreen) ? true : false);
			UsePredictorTransform = usePalette;
			if (flag)
			{
				UseCrossColorTransform = false;
			}
			else
			{
				UseCrossColorTransform = !redAndBlueAlwaysZero && UsePredictorTransform;
			}
			AllocateTransformBuffer(width, height);
			CacheBits = 0;
			ClearRefs();
			if (nearLossless && nearLosslessQuality < 100 && !UsePalette && !UsePredictorTransform)
			{
				AllocateTransformBuffer(width, height);
				NearLosslessEnc.ApplyNearLossless(width, height, nearLosslessQuality, span, span, width);
			}
			if (UsePalette)
			{
				EncodePalette(flag);
				MapImageFromPalette(width, height);
				if (PaletteSize < 1024)
				{
					CacheBits = BitOperations.Log2((uint)PaletteSize) + 1;
				}
			}
			if (UseSubtractGreenTransform)
			{
				ApplySubtractGreen();
			}
			if (UsePredictorTransform)
			{
				ApplyPredictFilter(CurrentWidth, height, flag);
			}
			if (UseCrossColorTransform)
			{
				ApplyCrossColorFilter(CurrentWidth, height, flag);
			}
			bitWriter.PutBits(0u, 1);
			EncodeImage(CurrentWidth, height, useCache: true, crunchConfig, CacheBits, flag);
			if (flag2 || bitWriter.NumBytes < num)
			{
				num = bitWriter.NumBytes;
				BitWriterSwap(ref bitWriter, ref dst);
			}
			if (array.Length > 1)
			{
				bitWriter.Reset(bwInit);
			}
			flag2 = false;
		}
		BitWriterSwap(ref dst, ref bitWriter);
	}

	public bool ConvertPixelsToBgra<TPixel>(Buffer2DRegion<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		bool flag = false;
		Span<byte> span = MemoryMarshal.Cast<uint, byte>(Bgra.GetSpan());
		int num = pixels.Width * 4;
		for (int i = 0; i < pixels.Height; i++)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(i);
			Span<byte> span3 = span.Slice(i * num, num);
			PixelOperations<TPixel>.Instance.ToBgra32Bytes(configuration, span2, span3, pixels.Width);
			if (!flag)
			{
				flag = WebpCommonUtils.CheckNonOpaque(MemoryMarshal.Cast<byte, Bgra32>(span3));
			}
		}
		return flag;
	}

	private CrunchConfig[] EncoderAnalyze(ReadOnlySpan<uint> bgra, int width, int height, out bool redAndBlueAlwaysZero)
	{
		bool flag = AnalyzeAndCreatePalette(bgra, width, height);
		HistoBits = GetHistoBits(method, flag, width, height);
		TransformBits = GetTransformBits(method, HistoBits);
		int paletteSize = PaletteSize;
		int num = ((paletteSize <= 0 || paletteSize > 16) ? 1 : 2);
		EntropyIx entropyIx = AnalyzeEntropy(bgra, width, height, flag, PaletteSize, TransformBits, out redAndBlueAlwaysZero);
		bool doNotCache = false;
		List<CrunchConfig> list = new List<CrunchConfig>();
		if (method == WebpEncodingMethod.Level6 && quality == 100)
		{
			doNotCache = true;
			EntropyIx[] values = Enum.GetValues<EntropyIx>();
			foreach (EntropyIx entropyIx2 in values)
			{
				if ((entropyIx2 != EntropyIx.Palette && entropyIx2 != EntropyIx.PaletteAndSpatial) || flag)
				{
					list.Add(new CrunchConfig
					{
						EntropyIdx = entropyIx2
					});
				}
			}
		}
		else
		{
			list.Add(new CrunchConfig
			{
				EntropyIdx = entropyIx
			});
			if (quality >= 75 && method == WebpEncodingMethod.Level5)
			{
				doNotCache = true;
				if (entropyIx == EntropyIx.Palette)
				{
					list.Add(new CrunchConfig
					{
						EntropyIdx = EntropyIx.PaletteAndSpatial
					});
				}
			}
		}
		foreach (CrunchConfig item in list)
		{
			for (int i = 0; i < num; i++)
			{
				item.SubConfigs.Add(new CrunchSubConfig
				{
					Lz77 = ((i == 0) ? 3 : 4),
					DoNotCache = doNotCache
				});
			}
		}
		return list.ToArray();
	}

	private void EncodeImage(int width, int height, bool useCache, CrunchConfig config, int cacheBits, bool lowEffort)
	{
		Span<uint> span = EncodedData.GetSpan();
		int num = LosslessUtils.SubSampleSize(width, HistoBits) * LosslessUtils.SubSampleSize(height, HistoBits);
		Span<ushort> span2 = ((num > 64) ? ((Span<ushort>)new ushort[num]) : stackalloc ushort[num]);
		Span<ushort> histogramSymbols = span2;
		Span<HuffmanTree> huffTree = stackalloc HuffmanTree[57];
		if (useCache)
		{
			if (cacheBits == 0)
			{
				cacheBits = 10;
			}
		}
		else
		{
			cacheBits = 0;
		}
		HashChain.Fill(span, quality, width, height, lowEffort);
		Vp8LBitWriter vp8LBitWriter = ((config.SubConfigs.Count > 1) ? bitWriter.Clone() : bitWriter);
		Vp8LBitWriter bwInit = bitWriter;
		bool flag = true;
		foreach (CrunchSubConfig subConfig in config.SubConfigs)
		{
			Vp8LBackwardRefs backwardReferences = BackwardReferenceEncoder.GetBackwardReferences(width, height, span, quality, subConfig.Lz77, ref cacheBits, memoryAllocator, HashChain, Refs[0], Refs[1]);
			Vp8LBackwardRefs refsTmp = Refs[backwardReferences.Equals(Refs[0]) ? 1u : 0u];
			bitWriter.Reset(bwInit);
			using OwnedVp8LHistogram tmpHisto = OwnedVp8LHistogram.Create(memoryAllocator, cacheBits);
			using Vp8LHistogramSet vp8LHistogramSet = new Vp8LHistogramSet(memoryAllocator, num, cacheBits);
			HistogramEncoder.GetHistoImageSymbols(memoryAllocator, width, height, backwardReferences, quality, HistoBits, cacheBits, vp8LHistogramSet, tmpHisto, histogramSymbols);
			int count = vp8LHistogramSet.Count;
			HuffmanTreeCode[] array = new HuffmanTreeCode[5 * count];
			GetHuffBitLengthsAndCodes(vp8LHistogramSet, array);
			if (cacheBits > 0)
			{
				bitWriter.PutBits(1u, 1);
				bitWriter.PutBits((uint)cacheBits, 4);
			}
			else
			{
				bitWriter.PutBits(0u, 1);
			}
			bool flag2 = count > 1;
			bitWriter.PutBits(flag2 ? 1u : 0u, 1);
			if (flag2)
			{
				using IMemoryOwner<uint> buffer = memoryAllocator.Allocate<uint>(num);
				Span<uint> span3 = buffer.GetSpan();
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					int num3 = histogramSymbols[i] & 0xFFFF;
					span3[i] = (uint)(num3 << 8);
					if (num3 >= num2)
					{
						num2 = num3 + 1;
					}
				}
				bitWriter.PutBits((uint)(HistoBits - 2), 3);
				EncodeImageNoHuffman(span3, HashChain, refsTmp, Refs[2], LosslessUtils.SubSampleSize(width, HistoBits), LosslessUtils.SubSampleSize(height, HistoBits), quality, lowEffort);
			}
			int num4 = 0;
			for (int j = 0; j < 5 * vp8LHistogramSet.Count; j++)
			{
				HuffmanTreeCode huffmanTreeCode = array[j];
				if (num4 < huffmanTreeCode.NumSymbols)
				{
					num4 = huffmanTreeCode.NumSymbols;
				}
			}
			HuffmanTreeToken[] array2 = new HuffmanTreeToken[num4];
			for (int k = 0; k < array2.Length; k++)
			{
				array2[k] = new HuffmanTreeToken();
			}
			for (int l = 0; l < 5 * vp8LHistogramSet.Count; l++)
			{
				HuffmanTreeCode huffmanCode = array[l];
				StoreHuffmanCode(huffTree, array2, huffmanCode);
				ClearHuffmanTreeIfOnlyOneSymbol(huffmanCode);
			}
			StoreImageToBitMask(width, HistoBits, backwardReferences, histogramSymbols, array);
			if (flag || (vp8LBitWriter != null && bitWriter.NumBytes < vp8LBitWriter.NumBytes))
			{
				Vp8LBitWriter vp8LBitWriter2 = bitWriter;
				Vp8LBitWriter vp8LBitWriter3 = vp8LBitWriter;
				vp8LBitWriter = vp8LBitWriter2;
				bitWriter = vp8LBitWriter3;
			}
			flag = false;
		}
		bitWriter = vp8LBitWriter;
	}

	private void EncodePalette(bool lowEffort)
	{
		Span<uint> bgra = stackalloc uint[256];
		int paletteSize = PaletteSize;
		Span<uint> span = Palette.Memory.Span;
		bitWriter.PutBits(1u, 1);
		bitWriter.PutBits(3u, 2);
		bitWriter.PutBits((uint)(paletteSize - 1), 8);
		for (int num = paletteSize - 1; num >= 1; num--)
		{
			bgra[num] = LosslessUtils.SubPixels(span[num], span[num - 1]);
		}
		bgra[0] = span[0];
		EncodeImageNoHuffman(bgra, HashChain, Refs[0], Refs[1], paletteSize, 1, 20u, lowEffort);
	}

	private void ApplySubtractGreen()
	{
		bitWriter.PutBits(1u, 1);
		bitWriter.PutBits(2u, 2);
		LosslessUtils.SubtractGreenFromBlueAndRed(EncodedData.GetSpan());
	}

	private void ApplyPredictFilter(int width, int height, bool lowEffort)
	{
		int num = (UsePalette ? 100 : nearLosslessQuality);
		int transformBits = TransformBits;
		int width2 = LosslessUtils.SubSampleSize(width, transformBits);
		int height2 = LosslessUtils.SubSampleSize(height, transformBits);
		PredictorEncoder.ResidualImage(width, height, transformBits, EncodedData.GetSpan(), BgraScratch.GetSpan(), TransformData.GetSpan(), histoArgb, bestHisto, nearLossless, num, transparentColorMode, UseSubtractGreenTransform, lowEffort);
		bitWriter.PutBits(1u, 1);
		bitWriter.PutBits(0u, 2);
		bitWriter.PutBits((uint)(transformBits - 2), 3);
		EncodeImageNoHuffman(TransformData.GetSpan(), HashChain, Refs[0], Refs[1], width2, height2, quality, lowEffort);
	}

	private void ApplyCrossColorFilter(int width, int height, bool lowEffort)
	{
		int transformBits = TransformBits;
		int width2 = LosslessUtils.SubSampleSize(width, transformBits);
		int height2 = LosslessUtils.SubSampleSize(height, transformBits);
		PredictorEncoder.ColorSpaceTransform(width, height, transformBits, quality, EncodedData.GetSpan(), TransformData.GetSpan(), scratch.Span);
		bitWriter.PutBits(1u, 1);
		bitWriter.PutBits(1u, 2);
		bitWriter.PutBits((uint)(transformBits - 2), 3);
		EncodeImageNoHuffman(TransformData.GetSpan(), HashChain, Refs[0], Refs[1], width2, height2, quality, lowEffort);
	}

	private void EncodeImageNoHuffman(Span<uint> bgra, Vp8LHashChain hashChain, Vp8LBackwardRefs refsTmp1, Vp8LBackwardRefs refsTmp2, int width, int height, uint quality, bool lowEffort)
	{
		int cacheBits = 0;
		ushort[] array = new ushort[1];
		HuffmanTreeCode[] array2 = new HuffmanTreeCode[5];
		Span<HuffmanTree> huffTree = stackalloc HuffmanTree[57];
		hashChain.Fill(bgra, quality, width, height, lowEffort);
		Vp8LBackwardRefs backwardReferences = BackwardReferenceEncoder.GetBackwardReferences(width, height, bgra, quality, 3, ref cacheBits, memoryAllocator, hashChain, refsTmp1, refsTmp2);
		using Vp8LHistogramSet histogramImage = new Vp8LHistogramSet(memoryAllocator, backwardReferences, 1, cacheBits);
		GetHuffBitLengthsAndCodes(histogramImage, array2);
		bitWriter.PutBits(0u, 1);
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			HuffmanTreeCode huffmanTreeCode = array2[i];
			if (num < huffmanTreeCode.NumSymbols)
			{
				num = huffmanTreeCode.NumSymbols;
			}
		}
		HuffmanTreeToken[] array3 = new HuffmanTreeToken[num];
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j] = new HuffmanTreeToken();
		}
		for (int k = 0; k < 5; k++)
		{
			HuffmanTreeCode huffmanCode = array2[k];
			StoreHuffmanCode(huffTree, array3, huffmanCode);
			ClearHuffmanTreeIfOnlyOneSymbol(huffmanCode);
		}
		StoreImageToBitMask(width, 0, backwardReferences, array, array2);
	}

	private void StoreHuffmanCode(Span<HuffmanTree> huffTree, HuffmanTreeToken[] tokens, HuffmanTreeCode huffmanCode)
	{
		int num = 0;
		Span<int> span = scratch.Span.Slice(0, 2);
		span.Clear();
		for (int i = 0; i < huffmanCode.NumSymbols; i++)
		{
			if (num >= 3)
			{
				break;
			}
			if (huffmanCode.CodeLengths[i] != 0)
			{
				if (num < 2)
				{
					span[num] = i;
				}
				num++;
			}
		}
		if (num == 0)
		{
			bitWriter.PutBits(1u, 4);
		}
		else if (num <= 2 && span[0] < 256 && span[1] < 256)
		{
			bitWriter.PutBits(1u, 1);
			bitWriter.PutBits((uint)(num - 1), 1);
			if (span[0] <= 1)
			{
				bitWriter.PutBits(0u, 1);
				bitWriter.PutBits((uint)span[0], 1);
			}
			else
			{
				bitWriter.PutBits(1u, 1);
				bitWriter.PutBits((uint)span[0], 8);
			}
			if (num == 2)
			{
				bitWriter.PutBits((uint)span[1], 8);
			}
		}
		else
		{
			StoreFullHuffmanCode(huffTree, tokens, huffmanCode);
		}
	}

	private void StoreFullHuffmanCode(Span<HuffmanTree> huffTree, HuffmanTreeToken[] tokens, HuffmanTreeCode tree)
	{
		byte[] array = new byte[19];
		short[] codes = new short[19];
		HuffmanTreeCode huffmanTreeCode = default(HuffmanTreeCode);
		huffmanTreeCode.NumSymbols = 19;
		huffmanTreeCode.CodeLengths = array;
		huffmanTreeCode.Codes = codes;
		HuffmanTreeCode huffmanTreeCode2 = huffmanTreeCode;
		bitWriter.PutBits(0u, 1);
		int num = HuffmanUtils.CreateCompressedHuffmanTree(tree, tokens);
		uint[] array2 = new uint[20];
		bool[] bufRle = new bool[20];
		int i;
		for (i = 0; i < num; i++)
		{
			array2[tokens[i].Code]++;
		}
		HuffmanUtils.CreateHuffmanTree(array2, 7, bufRle, huffTree, huffmanTreeCode2);
		StoreHuffmanTreeOfHuffmanTreeToBitMask(array);
		ClearHuffmanTreeIfOnlyOneSymbol(huffmanTreeCode2);
		int num2 = 0;
		int num3 = num;
		i = num;
		while (i-- > 0)
		{
			int code = tokens[i].Code;
			if ((code != 0 && (uint)(code - 17) > 1u) || 1 == 0)
			{
				break;
			}
			num3--;
			num2 += array[code];
			switch (code)
			{
			case 17:
				num2 += 3;
				break;
			case 18:
				num2 += 7;
				break;
			}
		}
		bool flag = num3 > 1 && num2 > 12;
		int numTokens = (flag ? num3 : num);
		bitWriter.PutBits(flag ? 1u : 0u, 1);
		if (flag)
		{
			if (num3 == 2)
			{
				bitWriter.PutBits(0u, 5);
			}
			else
			{
				int num4 = (int)((uint)BitOperations.Log2((uint)(num3 - 2)) / 2u + 1);
				bitWriter.PutBits((uint)(num4 - 1), 3);
				bitWriter.PutBits((uint)(num3 - 2), num4 * 2);
			}
		}
		StoreHuffmanTreeToBitMask(tokens, numTokens, huffmanTreeCode2);
	}

	private void StoreHuffmanTreeToBitMask(HuffmanTreeToken[] tokens, int numTokens, HuffmanTreeCode huffmanCode)
	{
		for (int i = 0; i < numTokens; i++)
		{
			int code = tokens[i].Code;
			int extraBits = tokens[i].ExtraBits;
			bitWriter.PutBits((uint)huffmanCode.Codes[code], huffmanCode.CodeLengths[code]);
			switch (code)
			{
			case 16:
				bitWriter.PutBits((uint)extraBits, 2);
				break;
			case 17:
				bitWriter.PutBits((uint)extraBits, 3);
				break;
			case 18:
				bitWriter.PutBits((uint)extraBits, 7);
				break;
			}
		}
	}

	private void StoreHuffmanTreeOfHuffmanTreeToBitMask(byte[] codeLengthBitDepth)
	{
		int num = 19;
		while (num > 4 && codeLengthBitDepth[StorageOrder[num - 1]] == 0)
		{
			num--;
		}
		bitWriter.PutBits((uint)(num - 4), 4);
		for (int i = 0; i < num; i++)
		{
			bitWriter.PutBits(codeLengthBitDepth[StorageOrder[i]], 3);
		}
	}

	private void StoreImageToBitMask(int width, int histoBits, Vp8LBackwardRefs backwardRefs, Span<ushort> histogramSymbols, HuffmanTreeCode[] huffmanCodes)
	{
		int num = ((histoBits <= 0) ? 1 : LosslessUtils.SubSampleSize(width, histoBits));
		int num2 = ((histoBits != 0) ? (-(1 << histoBits)) : 0);
		int num3 = 0;
		int num4 = 0;
		int num5 = num3 & num2;
		int num6 = num4 & num2;
		int num7 = histogramSymbols[0];
		Span<HuffmanTreeCode> span = huffmanCodes.AsSpan(5 * num7);
		for (int i = 0; i < backwardRefs.Refs.Count; i++)
		{
			PixOrCopy pixOrCopy = backwardRefs.Refs[i];
			if (num5 != (num3 & num2) || num6 != (num4 & num2))
			{
				num5 = num3 & num2;
				num6 = num4 & num2;
				num7 = histogramSymbols[(num4 >> histoBits) * num + (num3 >> histoBits)];
				span = huffmanCodes.AsSpan(5 * num7);
			}
			if (pixOrCopy.IsLiteral())
			{
				for (int j = 0; j < 4; j++)
				{
					int codeIndex = pixOrCopy.Literal(Order[j]);
					bitWriter.WriteHuffmanCode(span[j], codeIndex);
				}
			}
			else if (pixOrCopy.IsCacheIdx())
			{
				int num8 = (int)pixOrCopy.CacheIdx();
				int codeIndex2 = 280 + num8;
				bitWriter.WriteHuffmanCode(span[0], codeIndex2);
			}
			else
			{
				int extraBitsValue = 0;
				int extraBits = 0;
				uint distance = pixOrCopy.Distance();
				int num9 = LosslessUtils.PrefixEncode(pixOrCopy.Len, ref extraBits, ref extraBitsValue);
				bitWriter.WriteHuffmanCodeWithExtraBits(span[0], 256 + num9, extraBitsValue, extraBits);
				num9 = LosslessUtils.PrefixEncode((int)distance, ref extraBits, ref extraBitsValue);
				bitWriter.WriteHuffmanCode(span[4], num9);
				bitWriter.PutBits((uint)extraBitsValue, extraBits);
			}
			num3 += pixOrCopy.Length();
			while (num3 >= width)
			{
				num3 -= width;
				num4++;
			}
		}
	}

	private EntropyIx AnalyzeEntropy(ReadOnlySpan<uint> bgra, int width, int height, bool usePalette, int paletteSize, int transformBits, out bool redAndBlueAlwaysZero)
	{
		if (usePalette && paletteSize <= 16)
		{
			redAndBlueAlwaysZero = true;
			return EntropyIx.Palette;
		}
		using IMemoryOwner<uint> memoryOwner = memoryAllocator.Allocate<uint>(3328, AllocationOptions.Clean);
		Span<uint> span = memoryOwner.Memory.Span;
		uint b = bgra[0];
		ReadOnlySpan<uint> readOnlySpan = null;
		ref Span<uint> reference;
		for (int i = 0; i < height; i++)
		{
			ReadOnlySpan<uint> readOnlySpan2 = bgra.Slice(i * width, width);
			for (int j = 0; j < width; j++)
			{
				uint num = readOnlySpan2[j];
				uint num2 = LosslessUtils.SubPixels(num, b);
				b = num;
				if (num2 != 0 && (readOnlySpan.Length <= 0 || num != readOnlySpan[j]))
				{
					reference = ref span;
					Span<uint> a = reference.Slice(0, reference.Length);
					reference = ref span;
					Span<uint> r = reference.Slice(1024, reference.Length - 1024);
					reference = ref span;
					Span<uint> g = reference.Slice(512, reference.Length - 512);
					reference = ref span;
					AddSingle(num, a, r, g, reference.Slice(1536, reference.Length - 1536));
					reference = ref span;
					Span<uint> a2 = reference.Slice(256, reference.Length - 256);
					reference = ref span;
					Span<uint> r2 = reference.Slice(1280, reference.Length - 1280);
					reference = ref span;
					Span<uint> g2 = reference.Slice(768, reference.Length - 768);
					reference = ref span;
					AddSingle(num2, a2, r2, g2, reference.Slice(1792, reference.Length - 1792));
					reference = ref span;
					Span<uint> r3 = reference.Slice(2048, reference.Length - 2048);
					reference = ref span;
					AddSingleSubGreen(num, r3, reference.Slice(2560, reference.Length - 2560));
					reference = ref span;
					Span<uint> r4 = reference.Slice(2304, reference.Length - 2304);
					reference = ref span;
					AddSingleSubGreen(num2, r4, reference.Slice(2816, reference.Length - 2816));
					uint num3 = HashPix(num);
					span[(int)(3072 + num3)]++;
				}
			}
			readOnlySpan = readOnlySpan2;
		}
		Span<double> span2 = stackalloc double[13];
		Span<double> span3 = stackalloc double[6];
		int num4 = (usePalette ? 4 : 3);
		span[2304]++;
		span[2816]++;
		span[1280]++;
		span[768]++;
		span[1792]++;
		span[256]++;
		Vp8LBitEntropy vp8LBitEntropy = new Vp8LBitEntropy();
		for (int k = 0; k < 13; k++)
		{
			vp8LBitEntropy.Init();
			Span<uint> array = span.Slice(k * 256, 256);
			vp8LBitEntropy.BitsEntropyUnrefined(array, 256);
			span2[k] = vp8LBitEntropy.BitsEntropyRefine();
		}
		span3[0] = span2[0] + span2[4] + span2[2] + span2[6];
		span3[1] = span2[1] + span2[5] + span2[3] + span2[7];
		span3[2] = span2[0] + span2[8] + span2[2] + span2[10];
		span3[3] = span2[1] + span2[9] + span2[3] + span2[11];
		span3[4] = span2[12];
		span3[1] += (float)(LosslessUtils.SubSampleSize(width, transformBits) * LosslessUtils.SubSampleSize(height, transformBits)) * LosslessUtils.FastLog2(14u);
		span3[3] += (float)(LosslessUtils.SubSampleSize(width, transformBits) * LosslessUtils.SubSampleSize(height, transformBits)) * LosslessUtils.FastLog2(24u);
		span3[4] += paletteSize * 8;
		EntropyIx entropyIx = EntropyIx.Direct;
		for (int l = 1; l <= num4; l++)
		{
			if (span3[(int)entropyIx] > span3[l])
			{
				entropyIx = (EntropyIx)l;
			}
		}
		redAndBlueAlwaysZero = true;
		byte[][] array2 = new byte[5][]
		{
			new byte[2] { 4, 6 },
			new byte[2] { 5, 7 },
			new byte[2] { 8, 10 },
			new byte[2] { 9, 11 },
			new byte[2] { 4, 6 }
		};
		reference = ref span;
		int num5 = 256 * array2[(uint)entropyIx][0];
		Span<uint> span4 = reference.Slice(num5, reference.Length - num5);
		reference = ref span;
		num5 = 256 * array2[(uint)entropyIx][1];
		Span<uint> span5 = reference.Slice(num5, reference.Length - num5);
		for (int m = 1; m < 256; m++)
		{
			if ((span4[m] | span5[m]) != 0)
			{
				redAndBlueAlwaysZero = false;
				break;
			}
		}
		return entropyIx;
	}

	private bool AnalyzeAndCreatePalette(ReadOnlySpan<uint> bgra, int width, int height)
	{
		Span<uint> span = Palette.Memory.Span;
		PaletteSize = GetColorPalette(bgra, width, height, span);
		if (PaletteSize > 256)
		{
			PaletteSize = 0;
			return false;
		}
		span.Slice(0, PaletteSize).Sort();
		if (PaletteHasNonMonotonousDeltas(span, PaletteSize))
		{
			GreedyMinimizeDeltas(span, PaletteSize);
		}
		return true;
	}

	private static int GetColorPalette(ReadOnlySpan<uint> bgra, int width, int height, Span<uint> palette)
	{
		HashSet<uint> hashSet = new HashSet<uint>();
		for (int i = 0; i < height; i++)
		{
			ReadOnlySpan<uint> readOnlySpan = bgra.Slice(i * width, width);
			for (int j = 0; j < width; j++)
			{
				hashSet.Add(readOnlySpan[j]);
				if (hashSet.Count > 256)
				{
					return 257;
				}
			}
		}
		using HashSet<uint>.Enumerator enumerator = hashSet.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			palette[num++] = enumerator.Current;
		}
		return hashSet.Count;
	}

	private void MapImageFromPalette(int width, int height)
	{
		Span<uint> span = EncodedData.GetSpan();
		int currentWidth = CurrentWidth;
		Span<uint> span2 = EncodedData.GetSpan();
		Span<uint> span3 = Palette.GetSpan();
		int paletteSize = PaletteSize;
		int num = ((paletteSize > 4) ? ((paletteSize <= 16) ? 1 : 0) : ((paletteSize <= 2) ? 3 : 2));
		CurrentWidth = LosslessUtils.SubSampleSize(width, num);
		ApplyPalette(span, currentWidth, span2, CurrentWidth, span3, paletteSize, width, height, num);
	}

	private void ApplyPalette(Span<uint> src, int srcStride, Span<uint> dst, int dstStride, Span<uint> palette, int paletteSize, int width, int height, int xBits)
	{
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(width);
		Span<byte> span = buffer.GetSpan();
		if (paletteSize < 4)
		{
			uint num = palette[0];
			uint num2 = 0u;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					uint num3 = src[j];
					if (num3 != num)
					{
						num2 = SearchColorGreedy(palette, num3);
						num = num3;
					}
					span[j] = (byte)num2;
				}
				BundleColorMap(span, width, xBits, dst);
				ref Span<uint> reference = ref src;
				int num4 = srcStride;
				src = reference.Slice(num4, reference.Length - num4);
				reference = ref dst;
				num4 = dstStride;
				dst = reference.Slice(num4, reference.Length - num4);
			}
			return;
		}
		uint[] array = new uint[2048];
		int k;
		for (k = 0; k < 3; k++)
		{
			bool flag = true;
			array.AsSpan().Fill(uint.MaxValue);
			for (int l = 0; l < paletteSize; l++)
			{
				uint num5 = 0u;
				switch (k)
				{
				case 0:
					num5 = ApplyPaletteHash0(palette[l]);
					break;
				case 1:
					num5 = ApplyPaletteHash1(palette[l]);
					break;
				case 2:
					num5 = ApplyPaletteHash2(palette[l]);
					break;
				}
				if (array[num5] != uint.MaxValue)
				{
					flag = false;
					break;
				}
				array[num5] = (uint)l;
			}
			if (flag)
			{
				break;
			}
		}
		if ((uint)k <= 2u)
		{
			ApplyPaletteFor(width, height, palette, k, src, srcStride, dst, dstStride, span, array, xBits);
			return;
		}
		uint[] idxMap = new uint[paletteSize];
		uint[] array2 = new uint[paletteSize];
		PrepareMapToPalette(palette, paletteSize, array2, idxMap);
		ApplyPaletteForWithIdxMap(width, height, palette, src, srcStride, dst, dstStride, span, idxMap, xBits, array2, paletteSize);
	}

	private static void ApplyPaletteFor(int width, int height, Span<uint> palette, int hashIdx, Span<uint> src, int srcStride, Span<uint> dst, int dstStride, Span<byte> tmpRow, uint[] buffer, int xBits)
	{
		uint num = palette[0];
		uint num2 = 0u;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				uint num3 = src[j];
				if (num3 != num)
				{
					switch (hashIdx)
					{
					case 0:
						num2 = buffer[ApplyPaletteHash0(num3)];
						break;
					case 1:
						num2 = buffer[ApplyPaletteHash1(num3)];
						break;
					case 2:
						num2 = buffer[ApplyPaletteHash2(num3)];
						break;
					}
					num = num3;
				}
				tmpRow[j] = (byte)num2;
			}
			LosslessUtils.BundleColorMap(tmpRow, width, xBits, dst);
			ref Span<uint> reference = ref src;
			int num4 = srcStride;
			src = reference.Slice(num4, reference.Length - num4);
			reference = ref dst;
			num4 = dstStride;
			dst = reference.Slice(num4, reference.Length - num4);
		}
	}

	private static void ApplyPaletteForWithIdxMap(int width, int height, Span<uint> palette, Span<uint> src, int srcStride, Span<uint> dst, int dstStride, Span<byte> tmpRow, uint[] idxMap, int xBits, uint[] paletteSorted, int paletteSize)
	{
		uint num = palette[0];
		uint num2 = 0u;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				uint num3 = src[j];
				if (num3 != num)
				{
					num2 = idxMap[SearchColorNoIdx(paletteSorted, num3, paletteSize)];
					num = num3;
				}
				tmpRow[j] = (byte)num2;
			}
			LosslessUtils.BundleColorMap(tmpRow, width, xBits, dst);
			ref Span<uint> reference = ref src;
			int num4 = srcStride;
			src = reference.Slice(num4, reference.Length - num4);
			reference = ref dst;
			num4 = dstStride;
			dst = reference.Slice(num4, reference.Length - num4);
		}
	}

	private static void PrepareMapToPalette(Span<uint> palette, int numColors, uint[] sorted, uint[] idxMap)
	{
		palette.Slice(0, numColors).CopyTo(sorted);
		Array.Sort(sorted, PaletteCompareColorsForSort);
		for (int i = 0; i < numColors; i++)
		{
			idxMap[SearchColorNoIdx(sorted, palette[i], numColors)] = (uint)i;
		}
	}

	private static int SearchColorNoIdx(uint[] sorted, uint color, int hi)
	{
		int num = 0;
		if (sorted[num] == color)
		{
			return num;
		}
		int num2;
		while (true)
		{
			num2 = num + hi >> 1;
			if (sorted[num2] == color)
			{
				break;
			}
			if (sorted[num2] < color)
			{
				num = num2;
			}
			else
			{
				hi = num2;
			}
		}
		return num2;
	}

	private static void ClearHuffmanTreeIfOnlyOneSymbol(HuffmanTreeCode huffmanCode)
	{
		int num = 0;
		for (int i = 0; i < huffmanCode.NumSymbols; i++)
		{
			if (huffmanCode.CodeLengths[i] != 0)
			{
				num++;
				if (num > 1)
				{
					return;
				}
			}
		}
		for (int j = 0; j < huffmanCode.NumSymbols; j++)
		{
			huffmanCode.CodeLengths[j] = 0;
			huffmanCode.Codes[j] = 0;
		}
	}

	private static bool PaletteHasNonMonotonousDeltas(Span<uint> palette, int numColors)
	{
		byte b = 0;
		for (int i = 0; i < numColors; i++)
		{
			uint num = LosslessUtils.SubPixels(palette[i], 0u);
			byte b2 = (byte)((num >> 16) & 0xFF);
			byte b3 = (byte)((num >> 8) & 0xFF);
			byte b4 = (byte)(num & 0xFF);
			if (b2 != 0)
			{
				b |= (byte)((b2 < 128) ? 1 : 2);
			}
			if (b3 != 0)
			{
				b |= (byte)((b3 < 128) ? 8 : 16);
			}
			if (b4 != 0)
			{
				b |= (byte)((b4 < 128) ? 64 : 128);
			}
		}
		return (b & (b << 1)) != 0;
	}

	private static void GreedyMinimizeDeltas(Span<uint> palette, int numColors)
	{
		uint col = 0u;
		for (int i = 0; i < numColors; i++)
		{
			int index = i;
			uint num = uint.MaxValue;
			for (int j = i; j < numColors; j++)
			{
				uint num2 = PaletteColorDistance(palette[j], col);
				if (num > num2)
				{
					num = num2;
					index = j;
				}
			}
			ref uint reference = ref palette[i];
			ref uint reference2 = ref palette[index];
			uint num3 = palette[index];
			uint num4 = palette[i];
			reference = num3;
			reference2 = num4;
			col = palette[i];
		}
	}

	private static void GetHuffBitLengthsAndCodes(Vp8LHistogramSet histogramImage, HuffmanTreeCode[] huffmanCodes)
	{
		int num = 0;
		for (int i = 0; i < histogramImage.Count; i++)
		{
			Vp8LHistogram vp8LHistogram = histogramImage[i];
			int num2 = 5 * i;
			for (int j = 0; j < 5; j++)
			{
				int numSymbols = j switch
				{
					0 => vp8LHistogram.NumCodes(), 
					4 => 40, 
					_ => 256, 
				};
				huffmanCodes[num2 + j].NumSymbols = numSymbols;
			}
		}
		int num3 = 5 * histogramImage.Count;
		for (int k = 0; k < num3; k++)
		{
			int numSymbols2 = huffmanCodes[k].NumSymbols;
			huffmanCodes[k].Codes = new short[numSymbols2];
			huffmanCodes[k].CodeLengths = new byte[numSymbols2];
			if (num < numSymbols2)
			{
				num = numSymbols2;
			}
		}
		bool[] bufRle = new bool[num];
		HuffmanTree[] array = new HuffmanTree[3 * num];
		for (int l = 0; l < histogramImage.Count; l++)
		{
			int num4 = 5 * l;
			Vp8LHistogram vp8LHistogram2 = histogramImage[l];
			HuffmanUtils.CreateHuffmanTree(vp8LHistogram2.Literal, 15, bufRle, array, huffmanCodes[num4]);
			HuffmanUtils.CreateHuffmanTree(vp8LHistogram2.Red, 15, bufRle, array, huffmanCodes[num4 + 1]);
			HuffmanUtils.CreateHuffmanTree(vp8LHistogram2.Blue, 15, bufRle, array, huffmanCodes[num4 + 2]);
			HuffmanUtils.CreateHuffmanTree(vp8LHistogram2.Alpha, 15, bufRle, array, huffmanCodes[num4 + 3]);
			HuffmanUtils.CreateHuffmanTree(vp8LHistogram2.Distance, 15, bufRle, array, huffmanCodes[num4 + 4]);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint PaletteColorDistance(uint col1, uint col2)
	{
		uint num = LosslessUtils.SubPixels(col1, col2);
		return (PaletteComponentDistance(num & 0xFF) + PaletteComponentDistance((num >> 8) & 0xFF) + PaletteComponentDistance((num >> 16) & 0xFF)) * 9 + PaletteComponentDistance((num >> 24) & 0xFF);
	}

	private static int GetHistoBits(WebpEncodingMethod method, bool usePalette, int width, int height)
	{
		int i;
		for (i = (int)((usePalette ? 9 : 7) - method); LosslessUtils.SubSampleSize(width, i) * LosslessUtils.SubSampleSize(height, i) > 2600; i++)
		{
		}
		if (i < 2)
		{
			return 2;
		}
		if (i > 9)
		{
			return 9;
		}
		return i;
	}

	private static void BundleColorMap(Span<byte> row, int width, int xBits, Span<uint> dst)
	{
		if (xBits > 0)
		{
			int num = 1 << 3 - xBits;
			int num2 = (1 << xBits) - 1;
			uint num3 = 4278190080u;
			for (int i = 0; i < width; i++)
			{
				int num4 = i & num2;
				if (num4 == 0)
				{
					num3 = 4278190080u;
				}
				num3 |= (uint)(row[i] << 8 + num * num4);
				dst[i >> xBits] = num3;
			}
		}
		else
		{
			for (int i = 0; i < width; i++)
			{
				dst[i] = (uint)(0xFF000000u | (row[i] << 8));
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void BitWriterSwap(ref Vp8LBitWriter src, ref Vp8LBitWriter dst)
	{
		Vp8LBitWriter vp8LBitWriter = src;
		Vp8LBitWriter vp8LBitWriter2 = dst;
		dst = vp8LBitWriter;
		src = vp8LBitWriter2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetTransformBits(WebpEncodingMethod method, int histoBits)
	{
		int num = ((method < WebpEncodingMethod.Level4) ? 6 : ((method <= WebpEncodingMethod.Level4) ? 5 : 4));
		if (histoBits <= num)
		{
			return histoBits;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void AddSingle(uint p, Span<uint> a, Span<uint> r, Span<uint> g, Span<uint> b)
	{
		a[(int)((p >> 24) & 0xFF)]++;
		r[(int)((p >> 16) & 0xFF)]++;
		g[(int)((p >> 8) & 0xFF)]++;
		b[(int)(p & 0xFF)]++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void AddSingleSubGreen(uint p, Span<uint> r, Span<uint> b)
	{
		int num = (int)p >> 8;
		r[(int)((p >> 16) - num) & 0xFF]++;
		b[(int)(p - num) & 0xFF]++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint SearchColorGreedy(Span<uint> palette, uint color)
	{
		if (color == palette[0])
		{
			return 0u;
		}
		if (color == palette[1])
		{
			return 1u;
		}
		if (color == palette[2])
		{
			return 2u;
		}
		return 3u;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ApplyPaletteHash0(uint color)
	{
		return (color >> 8) & 0xFF;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ApplyPaletteHash1(uint color)
	{
		return (uint)((long)(color & 0xFFFFFF) * 4222244071L) >> 21;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ApplyPaletteHash2(uint color)
	{
		return (uint)((long)(color & 0xFFFFFF) * 2147483647L) >> 21;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint HashPix(uint pix)
	{
		return (uint)((((long)pix + (long)(pix >> 19)) * 969276327) & 0xFFFFFFFFu) >> 24;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int PaletteCompareColorsForSort(uint p1, uint p2)
	{
		if (p1 >= p2)
		{
			return 1;
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint PaletteComponentDistance(uint v)
	{
		if (v > 128)
		{
			return 256 - v;
		}
		return v;
	}

	public void AllocateTransformBuffer(int width, int height)
	{
		int length = (UsePredictorTransform ? ((width + 1) * 2 + (width * 2 + 4 - 1) / 4) : 0);
		int length2 = ((UsePredictorTransform || UseCrossColorTransform) ? (LosslessUtils.SubSampleSize(width, TransformBits) * LosslessUtils.SubSampleSize(height, TransformBits)) : 0);
		BgraScratch = memoryAllocator.Allocate<uint>(length);
		TransformData = memoryAllocator.Allocate<uint>(length2);
		CurrentWidth = width;
	}

	public void ClearRefs()
	{
		Vp8LBackwardRefs[] refs = Refs;
		for (int i = 0; i < refs.Length; i++)
		{
			refs[i].Refs.Clear();
		}
	}

	public void Dispose()
	{
		Bgra.Dispose();
		EncodedData.Dispose();
		BgraScratch?.Dispose();
		Palette.Dispose();
		TransformData?.Dispose();
		HashChain.Dispose();
	}
}
