using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats.Png.Chunks;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Png;

internal static class PngScanlineProcessor
{
	public static void ProcessGrayscaleScanline<TPixel>(int bitDepth, in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, Color? transparentColor) where TPixel : unmanaged, IPixel<TPixel>
	{
		ProcessInterlacedGrayscaleScanline(bitDepth, in frameControl, scanlineSpan, rowSpan, 0u, 1u, transparentColor);
	}

	public static void ProcessInterlacedGrayscaleScanline<TPixel>(int bitDepth, in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, uint pixelOffset, uint increment, Color? transparentColor) where TPixel : unmanaged, IPixel<TPixel>
	{
		uint num = pixelOffset + frameControl.XOffset;
		TPixel val = default(TPixel);
		ref byte reference = ref MemoryMarshal.GetReference(scanlineSpan);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(rowSpan);
		int num2 = 255 / (ColorNumerics.GetColorCountForBitDepth(bitDepth) - 1);
		if (!transparentColor.HasValue)
		{
			if (bitDepth == 16)
			{
				int num3 = 0;
				nuint num4 = num;
				while (num4 < frameControl.XMax)
				{
					ushort source = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num3, 2));
					val.FromL16(Unsafe.As<ushort, L16>(ref source));
					Unsafe.Add(ref reference2, num4) = val;
					num4 += increment;
					num3 += 2;
				}
			}
			else
			{
				nuint num5 = num;
				nuint num6 = 0u;
				while (num5 < frameControl.XMax)
				{
					byte source2 = (byte)(Unsafe.Add(ref reference, num6) * num2);
					val.FromL8(Unsafe.As<byte, L8>(ref source2));
					Unsafe.Add(ref reference2, num5) = val;
					num5 += increment;
					num6++;
				}
			}
		}
		else if (bitDepth == 16)
		{
			L16 l = transparentColor.Value.ToPixel<L16>();
			La32 source3 = default(La32);
			int num7 = 0;
			nuint num8 = num;
			while (num8 < frameControl.XMax)
			{
				ushort num9 = (source3.L = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num7, 2)));
				source3.A = (ushort)((!num9.Equals(l.PackedValue)) ? ushort.MaxValue : 0);
				val.FromLa32(source3);
				Unsafe.Add(ref reference2, num8) = val;
				num8 += increment;
				num7 += 2;
			}
		}
		else
		{
			byte obj = (byte)(transparentColor.Value.ToPixel<L8>().PackedValue * num2);
			La16 source4 = default(La16);
			nuint num10 = num;
			nuint num11 = 0u;
			while (num10 < frameControl.XMax)
			{
				byte b = (source4.L = (byte)(Unsafe.Add(ref reference, num11) * num2));
				source4.A = (byte)((!b.Equals(obj)) ? byte.MaxValue : 0);
				val.FromLa16(source4);
				Unsafe.Add(ref reference2, num10) = val;
				num10 += increment;
				num11++;
			}
		}
	}

	public static void ProcessGrayscaleWithAlphaScanline<TPixel>(int bitDepth, in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, uint bytesPerPixel, uint bytesPerSample) where TPixel : unmanaged, IPixel<TPixel>
	{
		ProcessInterlacedGrayscaleWithAlphaScanline(bitDepth, in frameControl, scanlineSpan, rowSpan, 0u, 1u, bytesPerPixel, bytesPerSample);
	}

	public static void ProcessInterlacedGrayscaleWithAlphaScanline<TPixel>(int bitDepth, in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, uint pixelOffset, uint increment, uint bytesPerPixel, uint bytesPerSample) where TPixel : unmanaged, IPixel<TPixel>
	{
		uint num = pixelOffset + frameControl.XOffset;
		TPixel val = default(TPixel);
		ref byte reference = ref MemoryMarshal.GetReference(scanlineSpan);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(rowSpan);
		if (bitDepth == 16)
		{
			La32 source = default(La32);
			int num2 = 0;
			nuint num3 = num;
			while (num3 < frameControl.XMax)
			{
				source.L = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num2, 2));
				source.A = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num2 + 2, 2));
				val.FromLa32(source);
				Unsafe.Add(ref reference2, (uint)num3) = val;
				num3 += increment;
				num2 += 4;
			}
		}
		else
		{
			La16 source2 = default(La16);
			nuint num4 = 0u;
			for (nuint num5 = num; num5 < frameControl.XMax; num5 += increment)
			{
				source2.L = Unsafe.Add(ref reference, num4);
				source2.A = Unsafe.Add(ref reference, num4 + bytesPerSample);
				val.FromLa16(source2);
				Unsafe.Add(ref reference2, num5) = val;
				num4 += bytesPerPixel;
			}
		}
	}

	public static void ProcessPaletteScanline<TPixel>(in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, ReadOnlyMemory<Color>? palette) where TPixel : unmanaged, IPixel<TPixel>
	{
		ProcessInterlacedPaletteScanline(in frameControl, scanlineSpan, rowSpan, 0u, 1u, palette);
	}

	public static void ProcessInterlacedPaletteScanline<TPixel>(in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, uint pixelOffset, uint increment, ReadOnlyMemory<Color>? palette) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (!palette.HasValue)
		{
			PngThrowHelper.ThrowMissingPalette();
		}
		TPixel val = default(TPixel);
		ref byte reference = ref MemoryMarshal.GetReference(scanlineSpan);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(rowSpan);
		ReadOnlyMemory<Color> value = palette.Value;
		ref Color reference3 = ref MemoryMarshal.GetReference(value.Span);
		uint num = pixelOffset + frameControl.XOffset;
		value = palette.Value;
		int num2 = value.Length - 1;
		nuint num3 = num;
		nuint num4 = 0u;
		while (num3 < frameControl.XMax)
		{
			uint num5 = Unsafe.Add(ref reference, num4);
			val.FromRgba32(Unsafe.Add(ref reference3, (int)Math.Min(num5, num2)).ToRgba32());
			Unsafe.Add(ref reference2, num3) = val;
			num3 += increment;
			num4++;
		}
	}

	public static void ProcessRgbScanline<TPixel>(Configuration configuration, int bitDepth, in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, int bytesPerPixel, int bytesPerSample, Color? transparentColor) where TPixel : unmanaged, IPixel<TPixel>
	{
		ProcessInterlacedRgbScanline(configuration, bitDepth, in frameControl, scanlineSpan, rowSpan, 0u, 1u, bytesPerPixel, bytesPerSample, transparentColor);
	}

	public static void ProcessInterlacedRgbScanline<TPixel>(Configuration configuration, int bitDepth, in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, uint pixelOffset, uint increment, int bytesPerPixel, int bytesPerSample, Color? transparentColor) where TPixel : unmanaged, IPixel<TPixel>
	{
		uint num = pixelOffset + frameControl.XOffset;
		TPixel val = default(TPixel);
		ref byte reference = ref MemoryMarshal.GetReference(scanlineSpan);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(rowSpan);
		if (!transparentColor.HasValue)
		{
			if (bitDepth == 16)
			{
				Rgb48 source = default(Rgb48);
				int num2 = 0;
				nuint num3 = num;
				while (num3 < frameControl.XMax)
				{
					source.R = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num2, bytesPerSample));
					source.G = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num2 + bytesPerSample, bytesPerSample));
					source.B = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num2 + 2 * bytesPerSample, bytesPerSample));
					val.FromRgb48(source);
					Unsafe.Add(ref reference2, num3) = val;
					num3 += increment;
					num2 += bytesPerPixel;
				}
			}
			else if (pixelOffset == 0 && increment == 1)
			{
				PixelOperations<TPixel>.Instance.FromRgb24Bytes(configuration, scanlineSpan.Slice(0, (int)(frameControl.Width * bytesPerPixel)), rowSpan.Slice((int)frameControl.XOffset, (int)frameControl.Width), (int)frameControl.Width);
			}
			else
			{
				Rgb24 source2 = default(Rgb24);
				int num4 = 0;
				nuint num5 = num;
				while (num5 < frameControl.XMax)
				{
					source2.R = Unsafe.Add(ref reference, (uint)num4);
					source2.G = Unsafe.Add(ref reference, (uint)(num4 + bytesPerSample));
					source2.B = Unsafe.Add(ref reference, (uint)(num4 + 2 * bytesPerSample));
					val.FromRgb24(source2);
					Unsafe.Add(ref reference2, num5) = val;
					num5 += increment;
					num4 += bytesPerPixel;
				}
			}
		}
		else if (bitDepth == 16)
		{
			Rgb48 other = transparentColor.Value.ToPixel<Rgb48>();
			Rgb48 rgb = default(Rgb48);
			Rgba64 source3 = default(Rgba64);
			int num6 = 0;
			nuint num7 = num;
			while (num7 < frameControl.XMax)
			{
				rgb.R = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num6, bytesPerSample));
				rgb.G = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num6 + bytesPerSample, bytesPerSample));
				rgb.B = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num6 + 2 * bytesPerSample, bytesPerSample));
				source3.Rgb = rgb;
				source3.A = (ushort)((!rgb.Equals(other)) ? ushort.MaxValue : 0);
				val.FromRgba64(source3);
				Unsafe.Add(ref reference2, num7) = val;
				num7 += increment;
				num6 += bytesPerPixel;
			}
		}
		else
		{
			Rgb24 rgb2 = transparentColor.Value.ToPixel<Rgb24>();
			Rgba32 source4 = default(Rgba32);
			int num8 = 0;
			nuint num9 = num;
			while (num9 < frameControl.XMax)
			{
				source4.R = Unsafe.Add(ref reference, (uint)num8);
				source4.G = Unsafe.Add(ref reference, (uint)(num8 + bytesPerSample));
				source4.B = Unsafe.Add(ref reference, (uint)(num8 + 2 * bytesPerSample));
				source4.A = (byte)((!rgb2.Equals(source4.Rgb)) ? byte.MaxValue : 0);
				val.FromRgba32(source4);
				Unsafe.Add(ref reference2, num9) = val;
				num9 += increment;
				num8 += bytesPerPixel;
			}
		}
	}

	public static void ProcessRgbaScanline<TPixel>(Configuration configuration, int bitDepth, in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, int bytesPerPixel, int bytesPerSample) where TPixel : unmanaged, IPixel<TPixel>
	{
		ProcessInterlacedRgbaScanline(configuration, bitDepth, in frameControl, scanlineSpan, rowSpan, 0u, 1u, bytesPerPixel, bytesPerSample);
	}

	public static void ProcessInterlacedRgbaScanline<TPixel>(Configuration configuration, int bitDepth, in FrameControl frameControl, ReadOnlySpan<byte> scanlineSpan, Span<TPixel> rowSpan, uint pixelOffset, uint increment, int bytesPerPixel, int bytesPerSample) where TPixel : unmanaged, IPixel<TPixel>
	{
		uint num = pixelOffset + frameControl.XOffset;
		TPixel val = default(TPixel);
		ref TPixel reference = ref MemoryMarshal.GetReference(rowSpan);
		if (bitDepth == 16)
		{
			Rgba64 source = default(Rgba64);
			int num2 = 0;
			nuint num3 = num;
			while (num3 < frameControl.XMax)
			{
				source.R = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num2, bytesPerSample));
				source.G = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num2 + bytesPerSample, bytesPerSample));
				source.B = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num2 + 2 * bytesPerSample, bytesPerSample));
				source.A = BinaryPrimitives.ReadUInt16BigEndian(scanlineSpan.Slice(num2 + 3 * bytesPerSample, bytesPerSample));
				val.FromRgba64(source);
				Unsafe.Add(ref reference, num3) = val;
				num3 += increment;
				num2 += bytesPerPixel;
			}
			return;
		}
		if (pixelOffset == 0 && increment == 1)
		{
			PixelOperations<TPixel>.Instance.FromRgba32Bytes(configuration, scanlineSpan.Slice(0, (int)(frameControl.Width * bytesPerPixel)), rowSpan.Slice((int)frameControl.XOffset, (int)frameControl.Width), (int)frameControl.Width);
			return;
		}
		ref byte reference2 = ref MemoryMarshal.GetReference(scanlineSpan);
		Rgba32 source2 = default(Rgba32);
		int num4 = 0;
		nuint num5 = num;
		while (num5 < frameControl.XMax)
		{
			source2.R = Unsafe.Add(ref reference2, (uint)num4);
			source2.G = Unsafe.Add(ref reference2, (uint)(num4 + bytesPerSample));
			source2.B = Unsafe.Add(ref reference2, (uint)(num4 + 2 * bytesPerSample));
			source2.A = Unsafe.Add(ref reference2, (uint)(num4 + 3 * bytesPerSample));
			val.FromRgba32(source2);
			Unsafe.Add(ref reference, num5) = val;
			num5 += increment;
			num4 += bytesPerPixel;
		}
	}
}
