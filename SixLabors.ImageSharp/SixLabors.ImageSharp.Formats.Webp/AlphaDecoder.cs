using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SixLabors.ImageSharp.Formats.Webp.BitReader;
using SixLabors.ImageSharp.Formats.Webp.Lossless;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp;

internal class AlphaDecoder : IDisposable
{
	private readonly MemoryAllocator memoryAllocator;

	public int Width { get; }

	public int Height { get; }

	public WebpAlphaFilterType AlphaFilterType { get; }

	public int LastRow { get; set; }

	public int PrevRow { get; set; }

	public Vp8LDecoder Vp8LDec { get; }

	public IMemoryOwner<byte> Alpha { get; }

	[MemberNotNullWhen(true, "LosslessDecoder")]
	private bool Compressed
	{
		[MemberNotNullWhen(true, "LosslessDecoder")]
		get;
	}

	private IMemoryOwner<byte> Data { get; }

	private WebpLosslessDecoder? LosslessDecoder { get; }

	public bool Use8BDecode { get; }

	public AlphaDecoder(int width, int height, IMemoryOwner<byte> data, byte alphaChunkHeader, MemoryAllocator memoryAllocator, Configuration configuration)
	{
		Width = width;
		Height = height;
		Data = data;
		this.memoryAllocator = memoryAllocator;
		LastRow = 0;
		int length = width * height;
		WebpAlphaCompressionMethod webpAlphaCompressionMethod = (WebpAlphaCompressionMethod)(alphaChunkHeader & 3);
		if (webpAlphaCompressionMethod != 0 && webpAlphaCompressionMethod != WebpAlphaCompressionMethod.WebpLosslessCompression)
		{
			WebpThrowHelper.ThrowImageFormatException($"unexpected alpha compression method {webpAlphaCompressionMethod} found");
		}
		Compressed = webpAlphaCompressionMethod == WebpAlphaCompressionMethod.WebpLosslessCompression;
		int num = (alphaChunkHeader >> 2) & 3;
		if ((num < 0 || num > 3) ? true : false)
		{
			WebpThrowHelper.ThrowImageFormatException($"unexpected alpha filter method {num} found");
		}
		Alpha = memoryAllocator.Allocate<byte>(length);
		AlphaFilterType = (WebpAlphaFilterType)num;
		Vp8LDec = new Vp8LDecoder(width, height, memoryAllocator);
		if (Compressed)
		{
			Vp8LBitReader bitReader = new Vp8LBitReader(data);
			LosslessDecoder = new WebpLosslessDecoder(bitReader, memoryAllocator, configuration);
			LosslessDecoder.DecodeImageStream(Vp8LDec, width, height, isLevel0: true);
			Use8BDecode = Vp8LDec.Transforms.Count == 1 && Vp8LDec.Transforms[0].TransformType == Vp8LTransformType.ColorIndexingTransform && Is8BOptimizable(Vp8LDec.Metadata);
		}
	}

	public void Decode()
	{
		if (!Compressed)
		{
			Memory<byte> memory = Data.Memory;
			Span<byte> span = memory.Span;
			int num = Width * Height;
			if (span.Length < num)
			{
				WebpThrowHelper.ThrowImageFormatException("not enough data in the ALPH chunk");
			}
			memory = Alpha.Memory;
			Span<byte> span2 = memory.Span;
			if (AlphaFilterType == WebpAlphaFilterType.None)
			{
				span.Slice(0, num).CopyTo(span2);
				return;
			}
			Span<byte> input = span;
			Span<byte> span3 = span2;
			Span<byte> prev = default(Span<byte>);
			for (int i = 0; i < Height; i++)
			{
				switch (AlphaFilterType)
				{
				case WebpAlphaFilterType.Horizontal:
					HorizontalUnfilter(prev, input, span3, Width);
					break;
				case WebpAlphaFilterType.Vertical:
					VerticalUnfilter(prev, input, span3, Width);
					break;
				case WebpAlphaFilterType.Gradient:
					GradientUnfilter(prev, input, span3, Width);
					break;
				}
				prev = span3;
				ref Span<byte> reference = ref input;
				int width = Width;
				input = reference.Slice(width, reference.Length - width);
				reference = ref span3;
				width = Width;
				span3 = reference.Slice(width, reference.Length - width);
			}
		}
		else if (Use8BDecode)
		{
			LosslessDecoder.DecodeAlphaData(this);
		}
		else
		{
			LosslessDecoder.DecodeImageData(Vp8LDec, Vp8LDec.Pixels.Memory.Span);
			ExtractAlphaRows(Vp8LDec);
		}
	}

	public void AlphaApplyFilter(int firstRow, int lastRow, Span<byte> dst, int stride)
	{
		if (AlphaFilterType == WebpAlphaFilterType.None)
		{
			return;
		}
		Span<byte> span = Alpha.Memory.Span;
		Span<byte> span2;
		if (PrevRow != 0)
		{
			ref Span<byte> reference = ref span;
			int num = Width * PrevRow;
			span2 = reference.Slice(num, reference.Length - num);
		}
		else
		{
			span2 = null;
		}
		Span<byte> prev = span2;
		for (int i = firstRow; i < lastRow; i++)
		{
			switch (AlphaFilterType)
			{
			case WebpAlphaFilterType.Horizontal:
				HorizontalUnfilter(prev, dst, dst, Width);
				break;
			case WebpAlphaFilterType.Vertical:
				VerticalUnfilter(prev, dst, dst, Width);
				break;
			case WebpAlphaFilterType.Gradient:
				GradientUnfilter(prev, dst, dst, Width);
				break;
			}
			prev = dst;
			ref Span<byte> reference = ref dst;
			int num = stride;
			dst = reference.Slice(num, reference.Length - num);
		}
		PrevRow = lastRow - 1;
	}

	public void ExtractPalettedAlphaRows(int lastRow)
	{
		WebpAlphaFilterType alphaFilterType = AlphaFilterType;
		bool flag = (uint)alphaFilterType <= 1u;
		int num = ((!flag) ? LastRow : 0);
		int num2 = ((LastRow < num) ? num : LastRow);
		if (lastRow > num2)
		{
			Span<byte> span = Alpha.Memory.Span;
			Span<byte> span2 = MemoryMarshal.Cast<uint, byte>(Vp8LDec.Pixels.Memory.Span);
			ref Span<byte> reference = ref span;
			int num3 = Width * num2;
			Span<byte> dst = reference.Slice(num3, reference.Length - num3);
			reference = ref span2;
			num3 = Vp8LDec.Width * num2;
			Span<byte> src = reference.Slice(num3, reference.Length - num3);
			if (Vp8LDec.Transforms.Count == 0 || Vp8LDec.Transforms[0].TransformType != Vp8LTransformType.ColorIndexingTransform)
			{
				WebpThrowHelper.ThrowImageFormatException("error while decoding alpha channel, expected color index transform data is missing");
			}
			ColorIndexInverseTransformAlpha(Vp8LDec.Transforms[0], num2, lastRow, src, dst);
			AlphaApplyFilter(num2, lastRow, dst, Width);
		}
		LastRow = lastRow;
	}

	private void ExtractAlphaRows(Vp8LDecoder dec)
	{
		int height = dec.Height;
		int width = dec.Width;
		Span<uint> span = dec.Pixels.Memory.Span;
		Span<byte> span2 = Alpha.Memory.Span;
		int size = width * height;
		WebpLosslessDecoder.ApplyInverseTransforms(dec, span, memoryAllocator);
		ExtractGreen(span, span2, size);
		AlphaApplyFilter(0, height, span2, width);
	}

	private static void ColorIndexInverseTransformAlpha(Vp8LTransform transform, int yStart, int yEnd, Span<byte> src, Span<byte> dst)
	{
		int num = 8 >> transform.Bits;
		int xSize = transform.XSize;
		Span<uint> span = transform.Data.Memory.Span;
		if (num < 8)
		{
			int num2 = 0;
			int num3 = 0;
			int num4 = (1 << transform.Bits) - 1;
			int num5 = (1 << num) - 1;
			for (int i = yStart; i < yEnd; i++)
			{
				int num6 = 0;
				for (int j = 0; j < xSize; j++)
				{
					if ((j & num4) == 0)
					{
						num6 = src[num2];
						num2++;
					}
					dst[num3] = GetAlphaValue((int)span[num6 & num5]);
					num3++;
					num6 >>= num;
				}
			}
		}
		else
		{
			MapAlpha(src, span, dst, yStart, yEnd, xSize);
		}
	}

	private static void HorizontalUnfilter(Span<byte> prev, Span<byte> input, Span<byte> dst, int width)
	{
		if (Sse2.IsSupported && width >= 9)
		{
			dst[0] = (byte)(input[0] + ((!prev.IsEmpty) ? prev[0] : 0));
			Vector128<int> vector = Vector128<int>.Zero.WithElement(0, dst[0]);
			ref byte reference = ref MemoryMarshal.GetReference(input);
			ref byte reference2 = ref MemoryMarshal.GetReference(dst);
			nuint num;
			for (num = 1u; num <= (uint)(width - 8); num += 8)
			{
				Vector128<byte> vector2 = Sse2.Add(Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference, num)), 0L).AsByte(), vector.AsByte());
				Vector128<byte> right = Sse2.ShiftLeftLogical128BitLane(vector2, 1);
				Vector128<byte> vector3 = Sse2.Add(vector2, right);
				Vector128<byte> right2 = Sse2.ShiftLeftLogical128BitLane(vector3, 2);
				Vector128<byte> vector4 = Sse2.Add(vector3, right2);
				Vector128<byte> right3 = Sse2.ShiftLeftLogical128BitLane(vector4, 4);
				Vector128<byte> vector5 = Sse2.Add(vector4, right3);
				Unsafe.As<byte, Vector64<byte>>(ref Unsafe.Add(ref reference2, num)) = vector5.GetLower();
				vector = Sse2.ShiftRightLogical(vector5.AsInt64(), 56).AsInt32();
			}
			for (; num < (uint)width; num++)
			{
				dst[(int)num] = (byte)(input[(int)num] + dst[(int)num - 1]);
			}
		}
		else
		{
			byte b = (byte)((!prev.IsEmpty) ? prev[0] : 0);
			for (int i = 0; i < width; i++)
			{
				byte b2 = (byte)(b + input[i]);
				b = b2;
				dst[i] = b2;
			}
		}
	}

	private static void VerticalUnfilter(Span<byte> prev, Span<byte> input, Span<byte> dst, int width)
	{
		if (prev.IsEmpty)
		{
			HorizontalUnfilter(null, input, dst, width);
		}
		else if (Avx2.IsSupported)
		{
			ref byte reference = ref MemoryMarshal.GetReference(input);
			ref byte reference2 = ref MemoryMarshal.GetReference(prev);
			ref byte reference3 = ref MemoryMarshal.GetReference(dst);
			int num = width & -32;
			nuint num2;
			for (num2 = 0u; num2 < (uint)num; num2 += 32)
			{
				Vector256<int> vector = Unsafe.As<byte, Vector256<int>>(ref Unsafe.Add(ref reference, num2));
				Vector256<byte> vector2 = Avx2.Add(right: Unsafe.As<byte, Vector256<int>>(ref Unsafe.Add(ref reference2, num2)).AsByte(), left: vector.AsByte());
				Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference3, num2)) = vector2;
			}
			for (; num2 < (uint)width; num2++)
			{
				Unsafe.Add(ref reference3, num2) = (byte)(Unsafe.Add(ref reference2, num2) + Unsafe.Add(ref reference, num2));
			}
		}
		else
		{
			for (int i = 0; i < width; i++)
			{
				dst[i] = (byte)(prev[i] + input[i]);
			}
		}
	}

	private static void GradientUnfilter(Span<byte> prev, Span<byte> input, Span<byte> dst, int width)
	{
		if (prev.IsEmpty)
		{
			HorizontalUnfilter(null, input, dst, width);
			return;
		}
		byte c;
		byte b = (c = prev[0]);
		for (int i = 0; i < width; i++)
		{
			byte b2 = prev[i];
			b = (byte)(input[i] + GradientPredictor(b, b2, c));
			c = b2;
			dst[i] = b;
		}
	}

	private static bool Is8BOptimizable(Vp8LMetadata hdr)
	{
		if (hdr.ColorCacheSize > 0)
		{
			return false;
		}
		for (int i = 0; i < hdr.NumHTreeGroups; i++)
		{
			List<HuffmanCode[]> hTrees = hdr.HTreeGroups[i].HTrees;
			if (hTrees[1][0].BitsUsed > 0)
			{
				return false;
			}
			if (hTrees[2][0].BitsUsed > 0)
			{
				return false;
			}
			if (hTrees[3][0].BitsUsed > 0)
			{
				return false;
			}
		}
		return true;
	}

	private static void MapAlpha(Span<byte> src, Span<uint> colorMap, Span<byte> dst, int yStart, int yEnd, int width)
	{
		int num = 0;
		for (int i = yStart; i < yEnd; i++)
		{
			for (int j = 0; j < width; j++)
			{
				dst[num] = GetAlphaValue((int)colorMap[src[num]]);
				num++;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte GetAlphaValue(int val)
	{
		return (byte)((val >> 8) & 0xFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GradientPredictor(byte a, byte b, byte c)
	{
		int num = a + b - c;
		if ((num & -256) != 0)
		{
			if (num >= 0)
			{
				return 255;
			}
			return 0;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ExtractGreen(Span<uint> argb, Span<byte> alpha, int size)
	{
		for (int i = 0; i < size; i++)
		{
			alpha[i] = (byte)(argb[i] >> 8);
		}
	}

	public void Dispose()
	{
		Vp8LDec?.Dispose();
		Data.Dispose();
		Alpha?.Dispose();
	}
}
