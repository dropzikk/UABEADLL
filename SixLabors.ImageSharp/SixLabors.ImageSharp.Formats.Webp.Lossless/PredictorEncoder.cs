using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal static class PredictorEncoder
{
	private static readonly sbyte[][] Offset = new sbyte[8][]
	{
		new sbyte[2] { 0, -1 },
		new sbyte[2] { 0, 1 },
		new sbyte[2] { -1, 0 },
		new sbyte[2] { 1, 0 },
		new sbyte[2] { -1, -1 },
		new sbyte[2] { -1, 1 },
		new sbyte[2] { 1, -1 },
		new sbyte[2] { 1, 1 }
	};

	private const int GreenRedToBlueNumAxis = 8;

	private const int GreenRedToBlueMaxIters = 7;

	private const float MaxDiffCost = 1E+30f;

	private const uint MaskAlpha = 4278190080u;

	private const float SpatialPredictorBias = 15f;

	private const int PredLowEffort = 11;

	private static ReadOnlySpan<sbyte> DeltaLut => new sbyte[7] { 16, 16, 8, 4, 2, 2, 2 };

	public static void ResidualImage(int width, int height, int bits, Span<uint> bgra, Span<uint> bgraScratch, Span<uint> image, int[][] histoArgb, int[][] bestHisto, bool nearLossless, int nearLosslessQuality, WebpTransparentColorMode transparentColorMode, bool usedSubtractGreen, bool lowEffort)
	{
		int num = LosslessUtils.SubSampleSize(width, bits);
		int num2 = LosslessUtils.SubSampleSize(height, bits);
		int maxQuantization = 1 << LosslessUtils.NearLosslessBits(nearLosslessQuality);
		Span<short> scratch = stackalloc short[8];
		int[][] accumulated = new int[4][]
		{
			new int[256],
			new int[256],
			new int[256],
			new int[256]
		};
		if (lowEffort)
		{
			for (int i = 0; i < num * num2; i++)
			{
				image[i] = 4278192896u;
			}
		}
		else
		{
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < num; k++)
				{
					int bestPredictorForTile = GetBestPredictorForTile(width, height, k, j, bits, accumulated, bgraScratch, bgra, histoArgb, bestHisto, maxQuantization, transparentColorMode, usedSubtractGreen, nearLossless, image, scratch);
					image[j * num + k] = (uint)(0xFF000000u | (bestPredictorForTile << 8));
				}
			}
		}
		CopyImageWithPrediction(width, height, bits, image, bgraScratch, bgra, maxQuantization, transparentColorMode, usedSubtractGreen, nearLossless, lowEffort);
	}

	public static void ColorSpaceTransform(int width, int height, int bits, uint quality, Span<uint> bgra, Span<uint> image, Span<int> scratch)
	{
		int num = 1 << bits;
		int num2 = LosslessUtils.SubSampleSize(width, bits);
		int num3 = LosslessUtils.SubSampleSize(height, bits);
		int[] array = new int[256];
		int[] array2 = new int[256];
		Vp8LMultipliers vp8LMultipliers = default(Vp8LMultipliers);
		Vp8LMultipliers m = default(Vp8LMultipliers);
		for (int i = 0; i < num3; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				int num4 = j * num;
				int num5 = i * num;
				int min = GetMin(num4 + num, width);
				int min2 = GetMin(num5 + num, height);
				int num6 = i * num2 + j;
				if (i != 0)
				{
					LosslessUtils.ColorCodeToMultipliers(image[num6 - num2], ref m);
				}
				vp8LMultipliers = GetBestColorTransformForTile(j, i, bits, vp8LMultipliers, m, quality, width, height, array, array2, bgra, scratch);
				image[num6] = MultipliersToColorCode(vp8LMultipliers);
				CopyTileWithColorTransform(width, height, num4, num5, num, vp8LMultipliers, bgra);
				for (int k = num5; k < min2; k++)
				{
					int l = k * width + num4;
					for (int num7 = l + min - num4; l < num7; l++)
					{
						uint num8 = bgra[l];
						if ((l < 2 || num8 != bgra[l - 2] || num8 != bgra[l - 1]) && (l < width + 2 || bgra[l - 2] != bgra[l - width - 2] || bgra[l - 1] != bgra[l - width - 1] || num8 != bgra[l - width]))
						{
							array[(num8 >> 16) & 0xFF]++;
							array2[num8 & 0xFF]++;
						}
					}
				}
			}
		}
	}

	private static int GetBestPredictorForTile(int width, int height, int tileX, int tileY, int bits, int[][] accumulated, Span<uint> argbScratch, Span<uint> argb, int[][] histoArgb, int[][] bestHisto, int maxQuantization, WebpTransparentColorMode transparentColorMode, bool usedSubtractGreen, bool nearLossless, Span<uint> modes, Span<short> scratch)
	{
		int num = tileX << bits;
		int num2 = tileY << bits;
		int a = 1 << bits;
		int min = GetMin(a, height - num2);
		int min2 = GetMin(a, width - num);
		int num3 = ((num > 0) ? 1 : 0);
		int num4 = num - num3;
		int width2 = min2 + num3 + ((min2 < width) ? 1 : 0) - num;
		int num5 = LosslessUtils.SubSampleSize(width, bits);
		int num6 = (int)((tileX > 0) ? ((modes[tileY * num5 + tileX - 1] >> 8) & 0xFF) : 255);
		int num7 = (int)((tileY > 0) ? ((modes[(tileY - 1) * num5 + tileX] >> 8) & 0xFF) : 255);
		Span<uint> span = argbScratch;
		ref Span<uint> reference = ref span;
		int num8 = width + 1;
		Span<uint> span2 = reference.Slice(num8, reference.Length - num8);
		reference = ref span2;
		num8 = width + 1;
		Span<byte> maxDiffs = MemoryMarshal.Cast<uint, byte>(reference.Slice(num8, reference.Length - num8));
		float num9 = 1E+30f;
		int result = 0;
		Span<uint> output = stackalloc uint[64];
		for (int i = 0; i < 4; i++)
		{
			histoArgb[i].AsSpan().Clear();
			bestHisto[i].AsSpan().Clear();
		}
		for (int j = 0; j < 14; j++)
		{
			if (num2 > 0)
			{
				Span<uint> span3 = argb.Slice((num2 - 1) * width + num4, min2 + num3 + 1);
				reference = ref span2;
				num8 = num4;
				Span<uint> destination = reference.Slice(num8, reference.Length - num8);
				span3.CopyTo(destination);
			}
			for (int k = 0; k < min; k++)
			{
				int num10 = num2 + k;
				Span<uint> span4 = span;
				span = span2;
				span2 = span4;
				int num11 = num10 * width + num4;
				Span<uint> span5 = argb.Slice(num11, min2 + num3 + ((num10 + 1 < height) ? 1 : 0));
				reference = ref span2;
				num8 = num4;
				Span<uint> destination2 = reference.Slice(num8, reference.Length - num8);
				span5.CopyTo(destination2);
				if (nearLossless && maxQuantization > 1 && num10 >= 1 && num10 + 1 < height)
				{
					Span<uint> argb2 = argb;
					num8 = num4;
					MaxDiffsForRow(width2, width, argb2, num11, maxDiffs.Slice(num8, maxDiffs.Length - num8), usedSubtractGreen);
				}
				GetResidual(width, height, span, span2, maxDiffs, j, num, num + min2, num10, maxQuantization, transparentColorMode, usedSubtractGreen, nearLossless, output, scratch);
				for (int l = 0; l < min2; l++)
				{
					UpdateHisto(histoArgb, output[l]);
				}
			}
			float num12 = PredictionCostSpatialHistogram(accumulated, histoArgb);
			if (j == num6)
			{
				num12 -= 15f;
			}
			if (j == num7)
			{
				num12 -= 15f;
			}
			if (num12 < num9)
			{
				int[][] array = histoArgb;
				histoArgb = bestHisto;
				bestHisto = array;
				num9 = num12;
				result = j;
			}
			for (int m = 0; m < 4; m++)
			{
				histoArgb[m].AsSpan().Clear();
			}
		}
		for (int n = 0; n < 4; n++)
		{
			for (int num13 = 0; num13 < 256; num13++)
			{
				accumulated[n][num13] += bestHisto[n][num13];
			}
		}
		return result;
	}

	private unsafe static void GetResidual(int width, int height, Span<uint> upperRowSpan, Span<uint> currentRowSpan, Span<byte> maxDiffs, int mode, int xStart, int xEnd, int y, int maxQuantization, WebpTransparentColorMode transparentColorMode, bool usedSubtractGreen, bool nearLossless, Span<uint> output, Span<short> scratch)
	{
		if (transparentColorMode == WebpTransparentColorMode.Preserve)
		{
			PredictBatch(mode, xStart, y, xEnd - xStart, currentRowSpan, upperRowSpan, output, scratch);
			return;
		}
		fixed (uint* ptr = currentRowSpan)
		{
			fixed (uint* ptr2 = upperRowSpan)
			{
				for (int i = xStart; i < xEnd; i++)
				{
					uint num = 0u;
					if (y == 0)
					{
						num = ((i == 0) ? 4278190080u : ptr[i - 1]);
					}
					else if (i == 0)
					{
						num = ptr2[i];
					}
					else
					{
						switch (mode)
						{
						case 0:
							num = 4278190080u;
							break;
						case 1:
							num = ptr[i - 1];
							break;
						case 2:
							num = LosslessUtils.Predictor2(ptr[i - 1], ptr2 + i);
							break;
						case 3:
							num = LosslessUtils.Predictor3(ptr[i - 1], ptr2 + i);
							break;
						case 4:
							num = LosslessUtils.Predictor4(ptr[i - 1], ptr2 + i);
							break;
						case 5:
							num = LosslessUtils.Predictor5(ptr[i - 1], ptr2 + i);
							break;
						case 6:
							num = LosslessUtils.Predictor6(ptr[i - 1], ptr2 + i);
							break;
						case 7:
							num = LosslessUtils.Predictor7(ptr[i - 1], ptr2 + i);
							break;
						case 8:
							num = LosslessUtils.Predictor8(ptr[i - 1], ptr2 + i);
							break;
						case 9:
							num = LosslessUtils.Predictor9(ptr[i - 1], ptr2 + i);
							break;
						case 10:
							num = LosslessUtils.Predictor10(ptr[i - 1], ptr2 + i);
							break;
						case 11:
							num = LosslessUtils.Predictor11(ptr[i - 1], ptr2 + i, scratch);
							break;
						case 12:
							num = LosslessUtils.Predictor12(ptr[i - 1], ptr2 + i);
							break;
						case 13:
							num = LosslessUtils.Predictor13(ptr[i - 1], ptr2 + i);
							break;
						}
					}
					uint num2;
					if (nearLossless)
					{
						if (maxQuantization == 1 || mode == 0 || y == 0 || y == height - 1 || i == 0 || i == width - 1)
						{
							num2 = LosslessUtils.SubPixels(ptr[i], num);
						}
						else
						{
							num2 = NearLossless(ptr[i], num, maxQuantization, maxDiffs[i], usedSubtractGreen);
							ptr[i] = LosslessUtils.AddPixels(num, num2);
						}
					}
					else
					{
						num2 = LosslessUtils.SubPixels(ptr[i], num);
					}
					if ((ptr[i] & 0xFF000000u) == 0)
					{
						num2 &= 0xFF000000u;
						ptr[i] = num & 0xFFFFFF;
						if (i == 0 && y != 0)
						{
							ptr2[width] = *ptr;
						}
					}
					output[i - xStart] = num2;
				}
			}
		}
	}

	private static uint NearLossless(uint value, uint predict, int maxQuantization, int maxDiff, bool usedSubtractGreen)
	{
		byte b = 0;
		byte b2 = 0;
		if (maxDiff <= 2)
		{
			return LosslessUtils.SubPixels(value, predict);
		}
		int num;
		for (num = maxQuantization; num >= maxDiff; num >>= 1)
		{
		}
		uint num2 = value >> 24;
		bool flag = ((num2 == 0 || num2 == 255) ? true : false);
		byte b3 = ((!flag) ? NearLosslessComponent((byte)(value >> 24), (byte)(predict >> 24), byte.MaxValue, num) : NearLosslessDiff((byte)((value >> 24) & 0xFF), (byte)((predict >> 24) & 0xFF)));
		byte b4 = NearLosslessComponent((byte)((value >> 8) & 0xFF), (byte)((predict >> 8) & 0xFF), byte.MaxValue, num);
		if (usedSubtractGreen)
		{
			b = (byte)(((predict >> 8) + b4) & 0xFF);
			b2 = NearLosslessDiff(b, (byte)((value >> 8) & 0xFF));
		}
		byte b5 = NearLosslessComponent(NearLosslessDiff((byte)((value >> 16) & 0xFF), b2), (byte)((predict >> 16) & 0xFF), (byte)(255 - b), num);
		byte b6 = NearLosslessComponent(NearLosslessDiff((byte)(value & 0xFF), b2), (byte)(predict & 0xFF), (byte)(255 - b), num);
		return (uint)((b3 << 24) | (b5 << 16) | (b4 << 8) | b6);
	}

	private static byte NearLosslessComponent(byte value, byte predict, byte boundary, int quantization)
	{
		int num = (value - predict) & 0xFF;
		int num2 = (boundary - predict) & 0xFF;
		int num3 = num & ~(quantization - 1);
		int num4 = num3 + quantization;
		int num5 = ((((boundary - value) & 0xFF) < num2) ? 1 : 0);
		if (num - num3 < num4 - num + num5)
		{
			if (num > num2 && num3 <= num2)
			{
				return (byte)(num3 + (quantization >> 1));
			}
			return (byte)num3;
		}
		if (num <= num2 && num4 > num2)
		{
			return (byte)(num3 + (quantization >> 1));
		}
		return (byte)num4;
	}

	private static void CopyImageWithPrediction(int width, int height, int bits, Span<uint> modes, Span<uint> argbScratch, Span<uint> argb, int maxQuantization, WebpTransparentColorMode transparentColorMode, bool usedSubtractGreen, bool nearLossless, bool lowEffort)
	{
		int num = LosslessUtils.SubSampleSize(width, bits);
		Span<uint> span = argbScratch;
		ref Span<uint> reference = ref span;
		int num2 = width + 1;
		Span<uint> span2 = reference.Slice(num2, reference.Length - num2);
		reference = ref span2;
		num2 = width + 1;
		Span<byte> span3 = MemoryMarshal.Cast<uint, byte>(reference.Slice(num2, reference.Length - num2));
		num2 = width;
		Span<byte> span4 = span3.Slice(num2, span3.Length - num2);
		Span<short> scratch = stackalloc short[8];
		for (int i = 0; i < height; i++)
		{
			Span<uint> span5 = span;
			span = span2;
			span2 = span5;
			argb.Slice(i * width, width + ((i + 1 < height) ? 1 : 0)).CopyTo(span2);
			if (lowEffort)
			{
				int y = i;
				Span<uint> currentSpan = span2;
				Span<uint> upperSpan = span;
				reference = ref argb;
				num2 = i * width;
				PredictBatch(11, 0, y, width, currentSpan, upperSpan, reference.Slice(num2, reference.Length - num2), scratch);
				continue;
			}
			if (nearLossless && maxQuantization > 1)
			{
				Span<byte> span6 = span3;
				span3 = span4;
				span4 = span6;
				if (i + 2 < height)
				{
					MaxDiffsForRow(width, width, argb, (i + 1) * width, span4, usedSubtractGreen);
				}
			}
			int num3 = 0;
			while (num3 < width)
			{
				int mode = (int)((modes[(i >> bits) * num + (num3 >> bits)] >> 8) & 0xFF);
				int num4 = num3 + (1 << bits);
				if (num4 > width)
				{
					num4 = width;
				}
				Span<uint> upperRowSpan = span;
				Span<uint> currentRowSpan = span2;
				Span<byte> maxDiffs = span3;
				int xStart = num3;
				int xEnd = num4;
				int y2 = i;
				reference = ref argb;
				num2 = i * width + num3;
				GetResidual(width, height, upperRowSpan, currentRowSpan, maxDiffs, mode, xStart, xEnd, y2, maxQuantization, transparentColorMode, usedSubtractGreen, nearLossless, reference.Slice(num2, reference.Length - num2), scratch);
				num3 = num4;
			}
		}
	}

	private unsafe static void PredictBatch(int mode, int xStart, int y, int numPixels, Span<uint> currentSpan, Span<uint> upperSpan, Span<uint> outputSpan, Span<short> scratch)
	{
		fixed (uint* ptr3 = currentSpan)
		{
			fixed (uint* ptr4 = upperSpan)
			{
				fixed (uint* ptr = outputSpan)
				{
					uint* ptr2 = ptr;
					if (xStart == 0)
					{
						if (y == 0)
						{
							LosslessUtils.PredictorSub0(ptr3, 1, ptr2);
						}
						else
						{
							LosslessUtils.PredictorSub2(ptr3, ptr4, 1, ptr2);
						}
						xStart++;
						ptr2++;
						numPixels--;
					}
					if (y == 0)
					{
						LosslessUtils.PredictorSub1(ptr3 + xStart, numPixels, ptr2);
						return;
					}
					switch (mode)
					{
					case 0:
						LosslessUtils.PredictorSub0(ptr3 + xStart, numPixels, ptr2);
						break;
					case 1:
						LosslessUtils.PredictorSub1(ptr3 + xStart, numPixels, ptr2);
						break;
					case 2:
						LosslessUtils.PredictorSub2(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 3:
						LosslessUtils.PredictorSub3(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 4:
						LosslessUtils.PredictorSub4(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 5:
						LosslessUtils.PredictorSub5(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 6:
						LosslessUtils.PredictorSub6(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 7:
						LosslessUtils.PredictorSub7(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 8:
						LosslessUtils.PredictorSub8(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 9:
						LosslessUtils.PredictorSub9(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 10:
						LosslessUtils.PredictorSub10(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 11:
						LosslessUtils.PredictorSub11(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2, scratch);
						break;
					case 12:
						LosslessUtils.PredictorSub12(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					case 13:
						LosslessUtils.PredictorSub13(ptr3 + xStart, ptr4 + xStart, numPixels, ptr2);
						break;
					}
				}
			}
		}
	}

	private static void MaxDiffsForRow(int width, int stride, Span<uint> argb, int offset, Span<byte> maxDiffs, bool usedSubtractGreen)
	{
		if (width <= 2)
		{
			return;
		}
		uint num = argb[offset];
		uint num2 = argb[offset + 1];
		if (usedSubtractGreen)
		{
			num = AddGreenToBlueAndRed(num);
			num2 = AddGreenToBlueAndRed(num2);
		}
		for (int i = 1; i < width - 1; i++)
		{
			uint num3 = argb[offset - stride + i];
			uint num4 = argb[offset + stride + i];
			uint left = num;
			num = num2;
			num2 = argb[offset + i + 1];
			if (usedSubtractGreen)
			{
				num3 = AddGreenToBlueAndRed(num3);
				num4 = AddGreenToBlueAndRed(num4);
				num2 = AddGreenToBlueAndRed(num2);
			}
			maxDiffs[i] = (byte)MaxDiffAroundPixel(num, num3, num4, left, num2);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int MaxDiffBetweenPixels(uint p1, uint p2)
	{
		int a = Math.Abs((int)((p1 >> 24) - (p2 >> 24)));
		int b = Math.Abs((int)(((p1 >> 16) & 0xFF) - ((p2 >> 16) & 0xFF)));
		int a2 = Math.Abs((int)(((p1 >> 8) & 0xFF) - ((p2 >> 8) & 0xFF)));
		return GetMax(b: GetMax(a2, Math.Abs((int)((p1 & 0xFF) - (p2 & 0xFF)))), a: GetMax(a, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int MaxDiffAroundPixel(uint current, uint up, uint down, uint left, uint right)
	{
		int a = MaxDiffBetweenPixels(current, up);
		int b = MaxDiffBetweenPixels(current, down);
		int a2 = MaxDiffBetweenPixels(current, left);
		return GetMax(b: GetMax(a2, MaxDiffBetweenPixels(current, right)), a: GetMax(a, b));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void UpdateHisto(int[][] histoArgb, uint argb)
	{
		histoArgb[0][argb >> 24]++;
		histoArgb[1][(argb >> 16) & 0xFF]++;
		histoArgb[2][(argb >> 8) & 0xFF]++;
		histoArgb[3][argb & 0xFF]++;
	}

	private static uint AddGreenToBlueAndRed(uint argb)
	{
		uint num = (argb >> 8) & 0xFF;
		uint num2 = argb & 0xFF00FF;
		num2 += (num << 16) | num;
		num2 &= 0xFF00FF;
		return (argb & 0xFF00FF00u) | num2;
	}

	private static void CopyTileWithColorTransform(int xSize, int ySize, int tileX, int tileY, int maxTileSize, Vp8LMultipliers colorTransform, Span<uint> argb)
	{
		int min = GetMin(maxTileSize, xSize - tileX);
		int min2 = GetMin(maxTileSize, ySize - tileY);
		ref Span<uint> reference = ref argb;
		int num = tileY * xSize + tileX;
		argb = reference.Slice(num, reference.Length - num);
		while (min2-- > 0)
		{
			LosslessUtils.TransformColor(colorTransform, argb, min);
			if (argb.Length > xSize)
			{
				reference = ref argb;
				num = xSize;
				argb = reference.Slice(num, reference.Length - num);
			}
		}
	}

	private static Vp8LMultipliers GetBestColorTransformForTile(int tileX, int tileY, int bits, Vp8LMultipliers prevX, Vp8LMultipliers prevY, uint quality, int xSize, int ySize, int[] accumulatedRedHisto, int[] accumulatedBlueHisto, Span<uint> argb, Span<int> scratch)
	{
		int num = 1 << bits;
		int num2 = tileY * num;
		int num3 = tileX * num;
		int min = GetMin(num3 + num, xSize);
		int min2 = GetMin(num2 + num, ySize);
		int tileWidth = min - num3;
		int tileHeight = min2 - num2;
		int num4 = num2 * xSize + num3;
		Span<uint> argb2 = argb.Slice(num4, argb.Length - num4);
		Vp8LMultipliers bestTx = default(Vp8LMultipliers);
		GetBestGreenToRed(argb2, xSize, scratch, tileWidth, tileHeight, prevX, prevY, quality, accumulatedRedHisto, ref bestTx);
		GetBestGreenRedToBlue(argb2, xSize, scratch, tileWidth, tileHeight, prevX, prevY, quality, accumulatedBlueHisto, ref bestTx);
		return bestTx;
	}

	private static void GetBestGreenToRed(Span<uint> argb, int stride, Span<int> scratch, int tileWidth, int tileHeight, Vp8LMultipliers prevX, Vp8LMultipliers prevY, uint quality, int[] accumulatedRedHisto, ref Vp8LMultipliers bestTx)
	{
		uint num = 4 + 7 * quality / 256;
		int num2 = 0;
		double num3 = GetPredictionCostCrossColorRed(argb, stride, scratch, tileWidth, tileHeight, prevX, prevY, num2, accumulatedRedHisto);
		for (int i = 0; i < (int)num; i++)
		{
			int num4 = 32 >> i;
			for (int j = -num4; j <= num4; j += 2 * num4)
			{
				int num5 = j + num2;
				double predictionCostCrossColorRed = GetPredictionCostCrossColorRed(argb, stride, scratch, tileWidth, tileHeight, prevX, prevY, num5, accumulatedRedHisto);
				if (predictionCostCrossColorRed < num3)
				{
					num3 = predictionCostCrossColorRed;
					num2 = num5;
				}
			}
		}
		bestTx.GreenToRed = (byte)(num2 & 0xFF);
	}

	private static void GetBestGreenRedToBlue(Span<uint> argb, int stride, Span<int> scratch, int tileWidth, int tileHeight, Vp8LMultipliers prevX, Vp8LMultipliers prevY, uint quality, int[] accumulatedBlueHisto, ref Vp8LMultipliers bestTx)
	{
		int num;
		switch (quality)
		{
		case 25u:
		case 26u:
		case 27u:
		case 28u:
		case 29u:
		case 30u:
		case 31u:
		case 32u:
		case 33u:
		case 34u:
		case 35u:
		case 36u:
		case 37u:
		case 38u:
		case 39u:
		case 40u:
		case 41u:
		case 42u:
		case 43u:
		case 44u:
		case 45u:
		case 46u:
		case 47u:
		case 48u:
		case 49u:
		case 50u:
			num = 4;
			break;
		default:
			num = 7;
			break;
		case 0u:
		case 1u:
		case 2u:
		case 3u:
		case 4u:
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
		case 17u:
		case 18u:
		case 19u:
		case 20u:
		case 21u:
		case 22u:
		case 23u:
		case 24u:
			num = 1;
			break;
		}
		int num2 = num;
		int num3 = 0;
		int num4 = 0;
		double num5 = GetPredictionCostCrossColorBlue(argb, stride, scratch, tileWidth, tileHeight, prevX, prevY, num3, num4, accumulatedBlueHisto);
		for (int i = 0; i < num2; i++)
		{
			int num6 = DeltaLut[i];
			for (int j = 0; j < 8; j++)
			{
				int num7 = Offset[j][0] * num6 + num3;
				int num8 = Offset[j][1] * num6 + num4;
				double predictionCostCrossColorBlue = GetPredictionCostCrossColorBlue(argb, stride, scratch, tileWidth, tileHeight, prevX, prevY, num7, num8, accumulatedBlueHisto);
				if (predictionCostCrossColorBlue < num5)
				{
					num5 = predictionCostCrossColorBlue;
					num3 = num7;
					num4 = num8;
				}
				if (quality < 25 && i == 4)
				{
					break;
				}
			}
			if (num6 == 2 && num3 == 0 && num4 == 0)
			{
				break;
			}
		}
		bestTx.GreenToBlue = (byte)(num3 & 0xFF);
		bestTx.RedToBlue = (byte)(num4 & 0xFF);
	}

	private static double GetPredictionCostCrossColorRed(Span<uint> argb, int stride, Span<int> scratch, int tileWidth, int tileHeight, Vp8LMultipliers prevX, Vp8LMultipliers prevY, int greenToRed, int[] accumulatedRedHisto)
	{
		Span<int> span = scratch.Slice(0, 256);
		span.Clear();
		ColorSpaceTransformUtils.CollectColorRedTransforms(argb, stride, tileWidth, tileHeight, greenToRed, span);
		double num = PredictionCostCrossColor(accumulatedRedHisto, span);
		if ((byte)greenToRed == prevX.GreenToRed)
		{
			num -= 3.0;
		}
		if ((byte)greenToRed == prevY.GreenToRed)
		{
			num -= 3.0;
		}
		if (greenToRed == 0)
		{
			num -= 3.0;
		}
		return num;
	}

	private static double GetPredictionCostCrossColorBlue(Span<uint> argb, int stride, Span<int> scratch, int tileWidth, int tileHeight, Vp8LMultipliers prevX, Vp8LMultipliers prevY, int greenToBlue, int redToBlue, int[] accumulatedBlueHisto)
	{
		Span<int> span = scratch.Slice(0, 256);
		span.Clear();
		ColorSpaceTransformUtils.CollectColorBlueTransforms(argb, stride, tileWidth, tileHeight, greenToBlue, redToBlue, span);
		double num = PredictionCostCrossColor(accumulatedBlueHisto, span);
		if ((byte)greenToBlue == prevX.GreenToBlue)
		{
			num -= 3.0;
		}
		if ((byte)greenToBlue == prevY.GreenToBlue)
		{
			num -= 3.0;
		}
		if ((byte)redToBlue == prevX.RedToBlue)
		{
			num -= 3.0;
		}
		if ((byte)redToBlue == prevY.RedToBlue)
		{
			num -= 3.0;
		}
		if (greenToBlue == 0)
		{
			num -= 3.0;
		}
		if (redToBlue == 0)
		{
			num -= 3.0;
		}
		return num;
	}

	private static float PredictionCostSpatialHistogram(int[][] accumulated, int[][] tile)
	{
		double num = 0.0;
		for (int i = 0; i < 4; i++)
		{
			double expVal = 0.94;
			num += (double)PredictionCostSpatial(tile[i], 1, expVal);
			num += (double)LosslessUtils.CombinedShannonEntropy(tile[i], accumulated[i]);
		}
		return (float)num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double PredictionCostCrossColor(int[] accumulated, Span<int> counts)
	{
		return LosslessUtils.CombinedShannonEntropy(counts, accumulated) + PredictionCostSpatial(counts, 3, 2.4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float PredictionCostSpatial(Span<int> counts, int weight0, double expVal)
	{
		int num = 16;
		double num2 = 0.6;
		double num3 = weight0 * counts[0];
		for (int i = 1; i < num; i++)
		{
			num3 += expVal * (double)(counts[i] + counts[256 - i]);
			expVal *= num2;
		}
		return (float)(-0.1 * num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte NearLosslessDiff(byte a, byte b)
	{
		return (byte)((a - b) & 0xFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint MultipliersToColorCode(Vp8LMultipliers m)
	{
		return (uint)(-16777216 | (m.RedToBlue << 16) | (m.GreenToBlue << 8) | m.GreenToRed);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMin(int a, int b)
	{
		if (a <= b)
		{
			return a;
		}
		return b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMax(int a, int b)
	{
		if (a >= b)
		{
			return a;
		}
		return b;
	}
}
