using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp;

internal static class WebpCommonUtils
{
	public static WebpMetadata GetWebpMetadata<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (image.Metadata.TryGetWebpMetadata(out WebpMetadata metadata))
		{
			return (WebpMetadata)metadata.DeepClone();
		}
		if (image.Metadata.TryGetGifMetadata(out GifMetadata metadata2))
		{
			return WebpMetadata.FromAnimatedMetadata(metadata2.ToAnimatedImageMetadata());
		}
		if (image.Metadata.TryGetPngMetadata(out PngMetadata metadata3))
		{
			return WebpMetadata.FromAnimatedMetadata(metadata3.ToAnimatedImageMetadata());
		}
		return new WebpMetadata();
	}

	public static WebpFrameMetadata GetWebpFrameMetadata<TPixel>(ImageFrame<TPixel> frame) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (frame.Metadata.TryGetWebpFrameMetadata(out WebpFrameMetadata metadata))
		{
			return (WebpFrameMetadata)metadata.DeepClone();
		}
		if (frame.Metadata.TryGetGifMetadata(out GifFrameMetadata metadata2))
		{
			return WebpFrameMetadata.FromAnimatedMetadata(metadata2.ToAnimatedImageFrameMetadata());
		}
		if (frame.Metadata.TryGetPngMetadata(out PngFrameMetadata metadata3))
		{
			return WebpFrameMetadata.FromAnimatedMetadata(metadata3.ToAnimatedImageFrameMetadata());
		}
		return new WebpFrameMetadata();
	}

	public unsafe static bool CheckNonOpaque(Span<Bgra32> row)
	{
		if (Avx2.IsSupported)
		{
			ReadOnlySpan<byte> readOnlySpan = MemoryMarshal.AsBytes(row);
			int i = 0;
			int num = row.Length * 4 - 3;
			fixed (byte* ptr = readOnlySpan)
			{
				Vector256<byte> right = Vector256.Create(0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue);
				Vector256<byte> right2 = Vector256.Create((byte)128).AsByte();
				for (; i + 128 <= num; i += 128)
				{
					Vector256<byte> left = Avx.LoadVector256(ptr + i).AsByte();
					Vector256<byte> left2 = Avx.LoadVector256(ptr + i + 32).AsByte();
					Vector256<byte> left3 = Avx.LoadVector256(ptr + i + 64).AsByte();
					Vector256<byte> left4 = Avx.LoadVector256(ptr + i + 96).AsByte();
					Vector256<int> left5 = Avx2.And(left, right).AsInt32();
					Vector256<int> right3 = Avx2.And(left2, right).AsInt32();
					Vector256<int> left6 = Avx2.And(left3, right).AsInt32();
					Vector256<int> right4 = Avx2.And(left4, right).AsInt32();
					Vector256<short> left7 = Avx2.PackSignedSaturate(left5, right3).AsInt16();
					Vector256<short> right5 = Avx2.PackSignedSaturate(left6, right4).AsInt16();
					if (Avx2.MoveMask(Avx2.CompareEqual(Avx2.PackSignedSaturate(left7, right5).AsByte(), right2)) != -1)
					{
						return true;
					}
				}
				for (; i + 64 <= num; i += 64)
				{
					if (IsNoneOpaque64Bytes(ptr, i))
					{
						return true;
					}
				}
				for (; i + 32 <= num; i += 32)
				{
					if (IsNoneOpaque32Bytes(ptr, i))
					{
						return true;
					}
				}
				for (; i <= num; i += 4)
				{
					if (ptr[i + 3] != byte.MaxValue)
					{
						return true;
					}
				}
			}
		}
		else if (Sse2.IsSupported)
		{
			ReadOnlySpan<byte> readOnlySpan2 = MemoryMarshal.AsBytes(row);
			int j = 0;
			int num2 = row.Length * 4 - 3;
			fixed (byte* ptr2 = readOnlySpan2)
			{
				for (; j + 64 <= num2; j += 64)
				{
					if (IsNoneOpaque64Bytes(ptr2, j))
					{
						return true;
					}
				}
				for (; j + 32 <= num2; j += 32)
				{
					if (IsNoneOpaque32Bytes(ptr2, j))
					{
						return true;
					}
				}
				for (; j <= num2; j += 4)
				{
					if (ptr2[j + 3] != byte.MaxValue)
					{
						return true;
					}
				}
			}
		}
		else
		{
			for (int k = 0; k < row.Length; k++)
			{
				if (row[k].A != byte.MaxValue)
				{
					return true;
				}
			}
		}
		return false;
	}

	private unsafe static bool IsNoneOpaque64Bytes(byte* src, int i)
	{
		Vector128<byte> right = Vector128.Create(0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue);
		Vector128<byte> left = Sse2.LoadVector128(src + i).AsByte();
		Vector128<byte> left2 = Sse2.LoadVector128(src + i + 16).AsByte();
		Vector128<byte> left3 = Sse2.LoadVector128(src + i + 32).AsByte();
		Vector128<byte> left4 = Sse2.LoadVector128(src + i + 48).AsByte();
		Vector128<int> left5 = Sse2.And(left, right).AsInt32();
		Vector128<int> right2 = Sse2.And(left2, right).AsInt32();
		Vector128<int> left6 = Sse2.And(left3, right).AsInt32();
		Vector128<int> right3 = Sse2.And(left4, right).AsInt32();
		Vector128<short> left7 = Sse2.PackSignedSaturate(left5, right2).AsInt16();
		Vector128<short> right4 = Sse2.PackSignedSaturate(left6, right3).AsInt16();
		return Sse2.MoveMask(Sse2.CompareEqual(Sse2.PackSignedSaturate(left7, right4).AsByte(), Vector128.Create((byte)128).AsByte())) != 65535;
	}

	private unsafe static bool IsNoneOpaque32Bytes(byte* src, int i)
	{
		Vector128<byte> right = Vector128.Create(0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue, 0, 0, 0, byte.MaxValue);
		Vector128<byte> left = Sse2.LoadVector128(src + i).AsByte();
		Vector128<byte> left2 = Sse2.LoadVector128(src + i + 16).AsByte();
		Vector128<int> left3 = Sse2.And(left, right).AsInt32();
		Vector128<int> right2 = Sse2.And(left2, right).AsInt32();
		Vector128<short> vector = Sse2.PackSignedSaturate(left3, right2).AsInt16();
		return Sse2.MoveMask(Sse2.CompareEqual(Sse2.PackSignedSaturate(vector, vector).AsByte(), Vector128.Create((byte)128).AsByte())) != 65535;
	}
}
