using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats.Webp.BitReader;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal sealed class WebpLosslessDecoder
{
	private readonly Vp8LBitReader bitReader;

	private readonly Configuration configuration;

	private readonly MemoryAllocator memoryAllocator;

	private const int BitsSpecialMarker = 256;

	private const uint PackedNonLiteralCode = 0u;

	private static readonly int CodeToPlaneCodes = WebpLookupTables.CodeToPlane.Length;

	private const int FixedTableSize = 2300;

	private static readonly int[] TableSize = new int[12]
	{
		2954, 2956, 2958, 2962, 2970, 2986, 3018, 3082, 3212, 3468,
		3980, 5004
	};

	private static readonly int NumCodeLengthCodes = CodeLengthCodeOrder.Length;

	private static ReadOnlySpan<byte> CodeLengthCodeOrder => new byte[19]
	{
		17, 18, 0, 1, 2, 3, 4, 5, 16, 6,
		7, 8, 9, 10, 11, 12, 13, 14, 15
	};

	private static ReadOnlySpan<byte> LiteralMap => new byte[5] { 0, 1, 1, 1, 0 };

	public WebpLosslessDecoder(Vp8LBitReader bitReader, MemoryAllocator memoryAllocator, Configuration configuration)
	{
		this.bitReader = bitReader;
		this.memoryAllocator = memoryAllocator;
		this.configuration = configuration;
	}

	public void Decode<TPixel>(Buffer2D<TPixel> pixels, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		using Vp8LDecoder vp8LDecoder = new Vp8LDecoder(width, height, memoryAllocator);
		DecodeImageStream(vp8LDecoder, width, height, isLevel0: true);
		DecodeImageData(vp8LDecoder, vp8LDecoder.Pixels.Memory.Span);
		DecodePixelValues(vp8LDecoder, pixels, width, height);
	}

	public IMemoryOwner<uint> DecodeImageStream(Vp8LDecoder decoder, int xSize, int ySize, bool isLevel0)
	{
		int num = xSize;
		int num2 = 0;
		if (isLevel0)
		{
			decoder.Transforms = new List<Vp8LTransform>(4);
			while (bitReader.ReadBit())
			{
				if (num2 > 4)
				{
					WebpThrowHelper.ThrowImageFormatException($"The maximum number of transforms of {4} was exceeded");
				}
				ReadTransformation(num, ySize, decoder);
				if (decoder.Transforms[num2].TransformType == Vp8LTransformType.ColorIndexingTransform)
				{
					num = LosslessUtils.SubSampleSize(num, decoder.Transforms[num2].Bits);
				}
				num2++;
			}
		}
		else
		{
			decoder.Metadata = new Vp8LMetadata();
		}
		bool num3 = bitReader.ReadBit();
		int num4 = 0;
		int colorCacheSize = 0;
		if (num3)
		{
			num4 = (int)bitReader.ReadValue(4);
			if (num4 < 1 || num4 > 11)
			{
				WebpThrowHelper.ThrowImageFormatException("Invalid color cache bits found");
			}
		}
		ReadHuffmanCodes(decoder, num, ySize, num4, isLevel0);
		decoder.Metadata.ColorCacheSize = colorCacheSize;
		if (num3)
		{
			decoder.Metadata.ColorCache = new ColorCache(num4);
			colorCacheSize = 1 << num4;
			decoder.Metadata.ColorCacheSize = colorCacheSize;
		}
		else
		{
			decoder.Metadata.ColorCacheSize = 0;
		}
		UpdateDecoder(decoder, num, ySize);
		if (isLevel0)
		{
			return null;
		}
		IMemoryOwner<uint> memoryOwner = memoryAllocator.Allocate<uint>(decoder.Width * decoder.Height, AllocationOptions.Clean);
		DecodeImageData(decoder, memoryOwner.GetSpan());
		return memoryOwner;
	}

	private void DecodePixelValues<TPixel>(Vp8LDecoder decoder, Buffer2D<TPixel> pixels, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		Span<uint> span = decoder.Pixels.GetSpan();
		ApplyInverseTransforms(decoder, span, memoryAllocator);
		Span<byte> span2 = MemoryMarshal.Cast<uint, byte>(span);
		int num = width * 4;
		for (int i = 0; i < height; i++)
		{
			Span<byte> span3 = span2.Slice(i * num, num);
			Span<TPixel> span4 = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromBgra32Bytes(configuration, span3.Slice(0, num), span4.Slice(0, width), width);
		}
	}

	public void DecodeImageData(Vp8LDecoder decoder, Span<uint> pixelData)
	{
		int width = decoder.Width;
		int height = decoder.Height;
		int row = 0 / width;
		int col = 0 % width;
		int colorCacheSize = decoder.Metadata.ColorCacheSize;
		ColorCache colorCache = decoder.Metadata.ColorCache;
		int num = 280 + colorCacheSize;
		int huffmanMask = decoder.Metadata.HuffmanMask;
		Span<HTreeGroup> hTreeGroupForPos = GetHTreeGroupForPos(decoder.Metadata, col, row);
		int num2 = width * height;
		int decodedPixels = 0;
		int i = decodedPixels;
		while (decodedPixels < num2)
		{
			if ((col & huffmanMask) == 0)
			{
				hTreeGroupForPos = GetHTreeGroupForPos(decoder.Metadata, col, row);
			}
			if (hTreeGroupForPos[0].IsTrivialCode)
			{
				pixelData[decodedPixels] = hTreeGroupForPos[0].LiteralArb;
				AdvanceByOne(ref col, ref row, width, colorCache, ref decodedPixels, pixelData, ref i);
				continue;
			}
			bitReader.FillBitWindow();
			int num3;
			if (hTreeGroupForPos[0].UsePackedTable)
			{
				num3 = (int)ReadPackedSymbols(hTreeGroupForPos, pixelData, decodedPixels);
				if (bitReader.IsEndOfStream())
				{
					break;
				}
				if (num3 == 0L)
				{
					AdvanceByOne(ref col, ref row, width, colorCache, ref decodedPixels, pixelData, ref i);
					continue;
				}
			}
			else
			{
				num3 = (int)ReadSymbol(hTreeGroupForPos[0].HTrees[0]);
			}
			if (bitReader.IsEndOfStream())
			{
				break;
			}
			if (num3 < 256)
			{
				if (hTreeGroupForPos[0].IsTrivialLiteral)
				{
					pixelData[decodedPixels] = hTreeGroupForPos[0].LiteralArb | (uint)(num3 << 8);
				}
				else
				{
					uint num4 = ReadSymbol(hTreeGroupForPos[0].HTrees[1]);
					bitReader.FillBitWindow();
					uint num5 = ReadSymbol(hTreeGroupForPos[0].HTrees[2]);
					uint num6 = ReadSymbol(hTreeGroupForPos[0].HTrees[3]);
					if (bitReader.IsEndOfStream())
					{
						break;
					}
					pixelData[decodedPixels] = (uint)(((byte)num6 << 24) | ((byte)num4 << 16) | ((byte)num3 << 8) | (byte)num5);
				}
				AdvanceByOne(ref col, ref row, width, colorCache, ref decodedPixels, pixelData, ref i);
			}
			else if (num3 < 280)
			{
				int lengthSymbol = num3 - 256;
				int copyLength = GetCopyLength(lengthSymbol);
				uint distanceSymbol = ReadSymbol(hTreeGroupForPos[0].HTrees[4]);
				bitReader.FillBitWindow();
				int copyDistance = GetCopyDistance((int)distanceSymbol);
				int dist = PlaneCodeToDistance(width, copyDistance);
				if (bitReader.IsEndOfStream())
				{
					break;
				}
				CopyBlock(pixelData, decodedPixels, dist, copyLength);
				decodedPixels += copyLength;
				col += copyLength;
				while (col >= width)
				{
					col -= width;
					row++;
				}
				if ((col & huffmanMask) != 0)
				{
					hTreeGroupForPos = GetHTreeGroupForPos(decoder.Metadata, col, row);
				}
				if (colorCache != null)
				{
					for (; i < decodedPixels; i++)
					{
						colorCache.Insert(pixelData[i]);
					}
				}
			}
			else if (num3 < num)
			{
				int key = num3 - 280;
				for (; i < decodedPixels; i++)
				{
					colorCache.Insert(pixelData[i]);
				}
				pixelData[decodedPixels] = colorCache.Lookup(key);
				AdvanceByOne(ref col, ref row, width, colorCache, ref decodedPixels, pixelData, ref i);
			}
			else
			{
				WebpThrowHelper.ThrowImageFormatException("Webp parsing error");
			}
		}
	}

	private static void AdvanceByOne(ref int col, ref int row, int width, ColorCache colorCache, ref int decodedPixels, Span<uint> pixelData, ref int lastCached)
	{
		col++;
		decodedPixels++;
		if (col < width)
		{
			return;
		}
		col = 0;
		row++;
		if (colorCache != null)
		{
			while (lastCached < decodedPixels)
			{
				colorCache.Insert(pixelData[lastCached]);
				lastCached++;
			}
		}
	}

	private void ReadHuffmanCodes(Vp8LDecoder decoder, int xSize, int ySize, int colorCacheBits, bool allowRecursion)
	{
		int num = 0;
		int num2 = 1;
		int num3 = 1;
		if (allowRecursion && bitReader.ReadBit())
		{
			int num4 = (int)(bitReader.ReadValue(3) + 2);
			int num5 = LosslessUtils.SubSampleSize(xSize, num4);
			int num6 = LosslessUtils.SubSampleSize(ySize, num4);
			int num7 = num5 * num6;
			IMemoryOwner<uint> memoryOwner = DecodeImageStream(decoder, num5, num6, isLevel0: false);
			Span<uint> span = memoryOwner.GetSpan();
			decoder.Metadata.HuffmanSubSampleBits = num4;
			for (int i = 0; i < num7; i++)
			{
				uint num8 = (span[i] >> 8) & 0xFFFF;
				span[i] = num8;
				if (num8 >= num3)
				{
					num3 = (int)(num8 + 1);
				}
			}
			num2 = num3;
			decoder.Metadata.HuffmanImage = memoryOwner;
		}
		for (int j = 0; j < 5; j++)
		{
			int num9 = WebpConstants.AlphabetSize[j];
			if (j == 0 && colorCacheBits > 0)
			{
				num9 += 1 << colorCacheBits;
			}
			if (num < num9)
			{
				num = num9;
			}
		}
		int num10 = TableSize[colorCacheBits];
		HuffmanCode[] array = new HuffmanCode[num2 * num10];
		HTreeGroup[] array2 = new HTreeGroup[num2];
		Span<HuffmanCode> table = array.AsSpan();
		int[] array3 = new int[num];
		for (int k = 0; k < num3; k++)
		{
			array2[k] = new HTreeGroup(64u);
			HTreeGroup hTreeGroup = array2[k];
			int num11 = 0;
			bool flag = true;
			int num12 = 0;
			array3.AsSpan().Clear();
			for (int l = 0; l < 5; l++)
			{
				int num13 = WebpConstants.AlphabetSize[l];
				if (l == 0 && colorCacheBits > 0)
				{
					num13 += 1 << colorCacheBits;
				}
				int num14 = ReadHuffmanCode(num13, array3, table);
				if (num14 == 0)
				{
					WebpThrowHelper.ThrowImageFormatException("Huffman table size is zero");
				}
				hTreeGroup.HTrees.Add(table.Slice(0, num14).ToArray());
				HuffmanCode huffmanCode = table[0];
				if (flag && LiteralMap[l] == 1)
				{
					flag = huffmanCode.BitsUsed == 0;
				}
				num11 += huffmanCode.BitsUsed;
				int num15 = num14;
				table = table.Slice(num15, table.Length - num15);
				if (l > 3)
				{
					continue;
				}
				int num16 = array3[0];
				for (int m = 1; m < num13; m++)
				{
					int num17 = array3[m];
					if (num17 > num16)
					{
						num16 = num17;
					}
				}
				num12 += num16;
			}
			hTreeGroup.IsTrivialLiteral = flag;
			hTreeGroup.IsTrivialCode = false;
			if (flag)
			{
				uint value = hTreeGroup.HTrees[1][0].Value;
				uint value2 = hTreeGroup.HTrees[2][0].Value;
				uint value3 = hTreeGroup.HTrees[0][0].Value;
				uint value4 = hTreeGroup.HTrees[3][0].Value;
				hTreeGroup.LiteralArb = (value4 << 24) | (value << 16) | value2;
				if (num11 == 0 && value3 < 256)
				{
					hTreeGroup.IsTrivialCode = true;
					hTreeGroup.LiteralArb |= value3 << 8;
				}
			}
			hTreeGroup.UsePackedTable = !hTreeGroup.IsTrivialCode && num12 < 6;
			if (hTreeGroup.UsePackedTable)
			{
				BuildPackedTable(hTreeGroup);
			}
		}
		decoder.Metadata.NumHTreeGroups = num2;
		decoder.Metadata.HTreeGroups = array2;
		decoder.Metadata.HuffmanTables = array;
	}

	private int ReadHuffmanCode(int alphabetSize, int[] codeLengths, Span<HuffmanCode> table)
	{
		bool num = bitReader.ReadBit();
		codeLengths.AsSpan(0, alphabetSize).Clear();
		if (num)
		{
			uint num2 = bitReader.ReadValue(1) + 1;
			uint num3 = bitReader.ReadValue(1);
			uint num4 = bitReader.ReadValue((num3 == 0) ? 1 : 8);
			codeLengths[num4] = 1;
			if (num2 == 2)
			{
				num4 = bitReader.ReadValue(8);
				codeLengths[num4] = 1;
			}
		}
		else
		{
			int[] array = new int[NumCodeLengthCodes];
			uint num5 = bitReader.ReadValue(4) + 4;
			if (num5 > NumCodeLengthCodes)
			{
				WebpThrowHelper.ThrowImageFormatException("Bitstream error, numCodes has an invalid value");
			}
			for (int i = 0; i < num5; i++)
			{
				array[CodeLengthCodeOrder[i]] = (int)bitReader.ReadValue(3);
			}
			ReadHuffmanCodeLengths(table, array, alphabetSize, codeLengths);
		}
		return HuffmanUtils.BuildHuffmanTable(table, 8, codeLengths, alphabetSize);
	}

	private void ReadHuffmanCodeLengths(Span<HuffmanCode> table, int[] codeLengthCodeLengths, int numSymbols, int[] codeLengths)
	{
		int num = 0;
		int num2 = 8;
		if (HuffmanUtils.BuildHuffmanTable(table, 7, codeLengthCodeLengths, NumCodeLengthCodes) == 0)
		{
			WebpThrowHelper.ThrowImageFormatException("Error building huffman table");
		}
		int num3;
		if (bitReader.ReadBit())
		{
			int nBits = (int)(2 + 2 * bitReader.ReadValue(3));
			num3 = (int)(2 + bitReader.ReadValue(nBits));
		}
		else
		{
			num3 = numSymbols;
		}
		while (num < numSymbols && num3-- != 0)
		{
			bitReader.FillBitWindow();
			int index = (int)(bitReader.PrefetchBits() & 0x7F);
			HuffmanCode huffmanCode = table[index];
			bitReader.AdvanceBitPosition(huffmanCode.BitsUsed);
			uint value = huffmanCode.Value;
			if (value < 16)
			{
				codeLengths[num++] = (int)value;
				if (value != 0)
				{
					num2 = (int)value;
				}
				continue;
			}
			bool flag = value == 16;
			uint num4 = value - 16;
			int nBits2 = WebpConstants.CodeLengthExtraBits[num4];
			int num5 = WebpConstants.CodeLengthRepeatOffsets[num4];
			int num6 = (int)(bitReader.ReadValue(nBits2) + num5);
			if (num + num6 > numSymbols)
			{
				break;
			}
			int num7 = (flag ? num2 : 0);
			while (num6-- > 0)
			{
				codeLengths[num++] = num7;
			}
		}
	}

	private void ReadTransformation(int xSize, int ySize, Vp8LDecoder decoder)
	{
		Vp8LTransformType vp8LTransformType = (Vp8LTransformType)bitReader.ReadValue(2);
		Vp8LTransform transform = new Vp8LTransform(vp8LTransformType, xSize, ySize);
		if (decoder.Transforms.Any((Vp8LTransform decoderTransform) => decoderTransform.TransformType == transform.TransformType))
		{
			WebpThrowHelper.ThrowImageFormatException("Each transform can only be present once");
		}
		switch (vp8LTransformType)
		{
		case Vp8LTransformType.ColorIndexingTransform:
		{
			uint num = bitReader.ReadValue(8) + 1;
			switch (num)
			{
			default:
				transform.Bits = 0;
				break;
			case 5u:
			case 6u:
			case 7u:
			case 8u:
			case 9u:
			case 10u:
			case 11u:
			case 12u:
			case 13u:
			case 14u:
			case 15u:
			case 16u:
				transform.Bits = 1;
				break;
			case 3u:
			case 4u:
				transform.Bits = 2;
				break;
			case 0u:
			case 1u:
			case 2u:
				transform.Bits = 3;
				break;
			}
			using (IMemoryOwner<uint> buffer = DecodeImageStream(decoder, (int)num, 1, isLevel0: false))
			{
				int length = 1 << (8 >> transform.Bits);
				IMemoryOwner<uint> memoryOwner = memoryAllocator.Allocate<uint>(length, AllocationOptions.Clean);
				LosslessUtils.ExpandColorMap((int)num, buffer.GetSpan(), memoryOwner.GetSpan());
				transform.Data = memoryOwner;
			}
			break;
		}
		case Vp8LTransformType.PredictorTransform:
		case Vp8LTransformType.CrossColorTransform:
		{
			transform.Bits = (int)(bitReader.ReadValue(3) + 2);
			int xSize2 = LosslessUtils.SubSampleSize(transform.XSize, transform.Bits);
			int ySize2 = LosslessUtils.SubSampleSize(transform.YSize, transform.Bits);
			transform.Data = DecodeImageStream(decoder, xSize2, ySize2, isLevel0: false);
			break;
		}
		}
		decoder.Transforms.Add(transform);
	}

	public static void ApplyInverseTransforms(Vp8LDecoder decoder, Span<uint> pixelData, MemoryAllocator memoryAllocator)
	{
		List<Vp8LTransform> transforms = decoder.Transforms;
		for (int num = transforms.Count - 1; num >= 0; num--)
		{
			Vp8LTransform vp8LTransform = transforms[num];
			switch (vp8LTransform.TransformType)
			{
			case Vp8LTransformType.PredictorTransform:
			{
				using (IMemoryOwner<uint> buffer = memoryAllocator.Allocate<uint>(pixelData.Length, AllocationOptions.Clean))
				{
					LosslessUtils.PredictorInverseTransform(vp8LTransform, pixelData, buffer.GetSpan());
				}
				break;
			}
			case Vp8LTransformType.SubtractGreen:
				LosslessUtils.AddGreenToBlueAndRed(pixelData);
				break;
			case Vp8LTransformType.CrossColorTransform:
				LosslessUtils.ColorSpaceInverseTransform(vp8LTransform, pixelData);
				break;
			case Vp8LTransformType.ColorIndexingTransform:
				LosslessUtils.ColorIndexInverseTransform(vp8LTransform, pixelData);
				break;
			}
		}
	}

	public void DecodeAlphaData(AlphaDecoder dec)
	{
		Span<byte> data = MemoryMarshal.Cast<uint, byte>(dec.Vp8LDec.Pixels.Memory.Span);
		int num = 0;
		int num2 = 0;
		Vp8LDecoder vp8LDec = dec.Vp8LDec;
		int width = vp8LDec.Width;
		int height = vp8LDec.Height;
		Vp8LMetadata metadata = vp8LDec.Metadata;
		int num3 = 0;
		int num4 = width * height;
		int num5 = num4;
		int num6 = height;
		int huffmanMask = metadata.HuffmanMask;
		Span<HTreeGroup> span = ((num3 < num5) ? GetHTreeGroupForPos(metadata, num2, num) : ((Span<HTreeGroup>)null));
		while (!bitReader.Eos && num3 < num5)
		{
			if ((num2 & huffmanMask) == 0)
			{
				span = GetHTreeGroupForPos(metadata, num2, num);
			}
			bitReader.FillBitWindow();
			int num7 = (int)ReadSymbol(span[0].HTrees[0]);
			if (num7 >= 256)
			{
				if (num7 < 280)
				{
					int lengthSymbol = num7 - 256;
					int copyLength = GetCopyLength(lengthSymbol);
					int distanceSymbol = (int)ReadSymbol(span[0].HTrees[4]);
					bitReader.FillBitWindow();
					int copyDistance = GetCopyDistance(distanceSymbol);
					int num8 = PlaneCodeToDistance(width, copyDistance);
					if (num3 >= num8 && num4 - num3 >= copyLength)
					{
						CopyBlock8B(data, num3, num8, copyLength);
					}
					else
					{
						WebpThrowHelper.ThrowImageFormatException("error while decoding alpha data");
					}
					num3 += copyLength;
					num2 += copyLength;
					while (num2 >= width)
					{
						num2 -= width;
						num++;
						if (num <= num6 && num % 16 == 0)
						{
							dec.ExtractPalettedAlphaRows(num);
						}
					}
					if (num3 < num5 && (num2 & huffmanMask) > 0)
					{
						span = GetHTreeGroupForPos(metadata, num2, num);
					}
				}
				else
				{
					WebpThrowHelper.ThrowImageFormatException("bitstream error while parsing alpha data");
				}
			}
			else
			{
				data[num3] = (byte)num7;
				num3++;
				num2++;
				if (num2 >= width)
				{
					num2 = 0;
					num++;
					if (num <= num6 && num % 16 == 0)
					{
						dec.ExtractPalettedAlphaRows(num);
					}
				}
			}
			bitReader.Eos = bitReader.IsEndOfStream();
		}
		dec.ExtractPalettedAlphaRows((num > num6) ? num6 : num);
	}

	private static void UpdateDecoder(Vp8LDecoder decoder, int width, int height)
	{
		int huffmanSubSampleBits = decoder.Metadata.HuffmanSubSampleBits;
		decoder.Width = width;
		decoder.Height = height;
		decoder.Metadata.HuffmanXSize = LosslessUtils.SubSampleSize(width, huffmanSubSampleBits);
		decoder.Metadata.HuffmanMask = ((huffmanSubSampleBits == 0) ? (-1) : ((1 << huffmanSubSampleBits) - 1));
	}

	private uint ReadPackedSymbols(Span<HTreeGroup> group, Span<uint> pixelData, int decodedPixels)
	{
		uint num = (uint)(bitReader.PrefetchBits() & 0x3F);
		HuffmanCode huffmanCode = group[0].PackedTable[num];
		if (huffmanCode.BitsUsed < 256)
		{
			bitReader.AdvanceBitPosition(huffmanCode.BitsUsed);
			pixelData[decodedPixels] = huffmanCode.Value;
			return 0u;
		}
		bitReader.AdvanceBitPosition(huffmanCode.BitsUsed - 256);
		return huffmanCode.Value;
	}

	private static void BuildPackedTable(HTreeGroup hTreeGroup)
	{
		for (uint num = 0u; num < 64; num++)
		{
			uint num2 = num;
			ref HuffmanCode reference = ref hTreeGroup.PackedTable[num2];
			HuffmanCode hCode = hTreeGroup.HTrees[0][num2];
			if (hCode.Value >= 256)
			{
				reference.BitsUsed = hCode.BitsUsed + 256;
				reference.Value = hCode.Value;
				continue;
			}
			reference.BitsUsed = 0;
			reference.Value = 0u;
			num2 >>= AccumulateHCode(hCode, 8, ref reference);
			num2 >>= AccumulateHCode(hTreeGroup.HTrees[1][num2], 16, ref reference);
			num2 >>= AccumulateHCode(hTreeGroup.HTrees[2][num2], 0, ref reference);
			num2 >>= AccumulateHCode(hTreeGroup.HTrees[3][num2], 24, ref reference);
		}
	}

	private uint ReadSymbol(Span<HuffmanCode> table)
	{
		int num = (int)bitReader.PrefetchBits();
		ref Span<HuffmanCode> reference = ref table;
		int num2 = num & 0xFF;
		Span<HuffmanCode> span = reference.Slice(num2, reference.Length - num2);
		int num3 = span[0].BitsUsed - 8;
		if (num3 > 0)
		{
			bitReader.AdvanceBitPosition(8);
			int num4 = (int)bitReader.PrefetchBits();
			reference = ref span;
			num2 = (int)span[0].Value;
			span = reference.Slice(num2, reference.Length - num2);
			reference = ref span;
			num2 = num4 & ((1 << num3) - 1);
			span = reference.Slice(num2, reference.Length - num2);
		}
		bitReader.AdvanceBitPosition(span[0].BitsUsed);
		return span[0].Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetCopyLength(int lengthSymbol)
	{
		return GetCopyDistance(lengthSymbol);
	}

	private int GetCopyDistance(int distanceSymbol)
	{
		if (distanceSymbol < 4)
		{
			return distanceSymbol + 1;
		}
		int num = distanceSymbol - 2 >> 1;
		return (int)((2 + (distanceSymbol & 1) << num) + bitReader.ReadValue(num) + 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Span<HTreeGroup> GetHTreeGroupForPos(Vp8LMetadata metadata, int x, int y)
	{
		uint metaIndex = GetMetaIndex(metadata.HuffmanImage, metadata.HuffmanXSize, metadata.HuffmanSubSampleBits, x, y);
		return metadata.HTreeGroups.AsSpan((int)metaIndex);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint GetMetaIndex(IMemoryOwner<uint> huffmanImage, int xSize, int bits, int x, int y)
	{
		if (bits == 0)
		{
			return 0u;
		}
		return huffmanImage.GetSpan()[xSize * (y >> bits) + (x >> bits)];
	}

	private static int PlaneCodeToDistance(int xSize, int planeCode)
	{
		if (planeCode > CodeToPlaneCodes)
		{
			return planeCode - CodeToPlaneCodes;
		}
		int num = WebpLookupTables.CodeToPlane[planeCode - 1];
		int num2 = num >> 4;
		int num3 = 8 - (num & 0xF);
		int num4 = num2 * xSize + num3;
		if (num4 < 1)
		{
			return 1;
		}
		return num4;
	}

	private static void CopyBlock(Span<uint> pixelData, int decodedPixels, int dist, int length)
	{
		int num = decodedPixels - dist;
		if (num < 0)
		{
			WebpThrowHelper.ThrowImageFormatException("webp image data seems to be invalid");
		}
		ref Span<uint> reference;
		int num2;
		if (dist >= length)
		{
			Span<uint> span = pixelData.Slice(num, length);
			reference = ref pixelData;
			num2 = decodedPixels;
			Span<uint> destination = reference.Slice(num2, reference.Length - num2);
			span.CopyTo(destination);
			return;
		}
		reference = ref pixelData;
		num2 = num;
		Span<uint> span2 = reference.Slice(num2, reference.Length - num2);
		reference = ref pixelData;
		num2 = decodedPixels;
		Span<uint> span3 = reference.Slice(num2, reference.Length - num2);
		for (int i = 0; i < length; i++)
		{
			span3[i] = span2[i];
		}
	}

	private static void CopyBlock8B(Span<byte> data, int pos, int dist, int length)
	{
		ref Span<byte> reference;
		int num;
		if (dist >= length)
		{
			Span<byte> span = data.Slice(pos - dist, length);
			reference = ref data;
			num = pos;
			span.CopyTo(reference.Slice(num, reference.Length - num));
			return;
		}
		reference = ref data;
		num = pos;
		Span<byte> span2 = reference.Slice(num, reference.Length - num);
		reference = ref data;
		num = pos - dist;
		Span<byte> span3 = reference.Slice(num, reference.Length - num);
		for (int i = 0; i < length; i++)
		{
			span2[i] = span3[i];
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int AccumulateHCode(HuffmanCode hCode, int shift, ref HuffmanCode huff)
	{
		huff.BitsUsed += hCode.BitsUsed;
		huff.Value |= hCode.Value << shift;
		return hCode.BitsUsed;
	}
}
