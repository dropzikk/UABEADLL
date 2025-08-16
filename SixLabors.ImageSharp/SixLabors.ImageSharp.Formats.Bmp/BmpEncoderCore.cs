using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Formats.Bmp;

internal sealed class BmpEncoderCore : IImageEncoderInternals
{
	private int padding;

	private const int Rgba32AlphaMask = -16777216;

	private const int Rgba32RedMask = 16711680;

	private const int Rgba32GreenMask = 65280;

	private const int Rgba32BlueMask = 255;

	private const int ColorPaletteSize8Bit = 1024;

	private const int ColorPaletteSize4Bit = 64;

	private const int ColorPaletteSize2Bit = 16;

	private const int ColorPaletteSize1Bit = 8;

	private readonly MemoryAllocator memoryAllocator;

	private BmpBitsPerPixel? bitsPerPixel;

	private BmpInfoHeaderType infoHeaderType;

	private readonly IQuantizer quantizer;

	private readonly IPixelSamplingStrategy pixelSamplingStrategy;

	public BmpEncoderCore(BmpEncoder encoder, MemoryAllocator memoryAllocator)
	{
		this.memoryAllocator = memoryAllocator;
		bitsPerPixel = encoder.BitsPerPixel;
		quantizer = encoder.Quantizer ?? KnownQuantizers.Octree;
		pixelSamplingStrategy = encoder.PixelSamplingStrategy;
		infoHeaderType = (encoder.SupportTransparency ? BmpInfoHeaderType.WinVersion4 : BmpInfoHeaderType.WinVersion3);
	}

	public void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(image, "image");
		Guard.NotNull(stream, "stream");
		Configuration configuration = image.Configuration;
		ImageMetadata metadata = image.Metadata;
		BmpMetadata bmpMetadata = metadata.GetBmpMetadata();
		BmpBitsPerPixel valueOrDefault = bitsPerPixel.GetValueOrDefault();
		if (!bitsPerPixel.HasValue)
		{
			valueOrDefault = bmpMetadata.BitsPerPixel;
			bitsPerPixel = valueOrDefault;
		}
		ushort num = (ushort)bitsPerPixel.Value;
		int num2 = (int)(4 * ((uint)(image.Width * num + 31) / 32u));
		padding = num2 - (int)((float)image.Width * ((float)(int)num / 8f));
		int colorPaletteSize = bitsPerPixel switch
		{
			BmpBitsPerPixel.Pixel8 => 1024, 
			BmpBitsPerPixel.Pixel4 => 64, 
			BmpBitsPerPixel.Pixel2 => 16, 
			BmpBitsPerPixel.Pixel1 => 8, 
			_ => 0, 
		};
		byte[] array = null;
		int iccProfileSize = 0;
		if (metadata.IccProfile != null)
		{
			infoHeaderType = BmpInfoHeaderType.WinVersion5;
			array = metadata.IccProfile.ToByteArray();
			iccProfileSize = array.Length;
		}
		int num3 = infoHeaderType switch
		{
			BmpInfoHeaderType.WinVersion3 => 40, 
			BmpInfoHeaderType.WinVersion4 => 108, 
			BmpInfoHeaderType.WinVersion5 => 124, 
			_ => 40, 
		};
		BmpInfoHeader infoHeader = CreateBmpInfoHeader(image.Width, image.Height, num3, num, num2, metadata, array);
		Span<byte> buffer = stackalloc byte[num3];
		WriteBitmapFileHeader(stream, num3, colorPaletteSize, iccProfileSize, infoHeader, buffer);
		WriteBitmapInfoHeader(stream, infoHeader, buffer, num3);
		WriteImage(configuration, stream, image);
		WriteColorProfile(stream, array, buffer);
		stream.Flush();
	}

	private BmpInfoHeader CreateBmpInfoHeader(int width, int height, int infoHeaderSize, ushort bpp, int bytesPerLine, ImageMetadata metadata, byte[]? iccProfileData)
	{
		int xPelsPerMeter = 0;
		int yPelsPerMeter = 0;
		if (metadata.ResolutionUnits != 0 && metadata.HorizontalResolution > 0.0 && metadata.VerticalResolution > 0.0)
		{
			switch (metadata.ResolutionUnits)
			{
			case PixelResolutionUnit.PixelsPerInch:
				xPelsPerMeter = (int)Math.Round(UnitConverter.InchToMeter(metadata.HorizontalResolution));
				yPelsPerMeter = (int)Math.Round(UnitConverter.InchToMeter(metadata.VerticalResolution));
				break;
			case PixelResolutionUnit.PixelsPerCentimeter:
				xPelsPerMeter = (int)Math.Round(UnitConverter.CmToMeter(metadata.HorizontalResolution));
				yPelsPerMeter = (int)Math.Round(UnitConverter.CmToMeter(metadata.VerticalResolution));
				break;
			case PixelResolutionUnit.PixelsPerMeter:
				xPelsPerMeter = (int)Math.Round(metadata.HorizontalResolution);
				yPelsPerMeter = (int)Math.Round(metadata.VerticalResolution);
				break;
			}
		}
		BmpInfoHeader result = new BmpInfoHeader(infoHeaderSize, width, height, 1, bpp, BmpCompression.RGB, height * bytesPerLine, xPelsPerMeter, yPelsPerMeter);
		BmpInfoHeaderType bmpInfoHeaderType = infoHeaderType;
		bool flag = ((bmpInfoHeaderType == BmpInfoHeaderType.WinVersion4 || bmpInfoHeaderType == BmpInfoHeaderType.WinVersion5) ? true : false);
		if (flag && bitsPerPixel == BmpBitsPerPixel.Pixel32)
		{
			result.AlphaMask = -16777216;
			result.RedMask = 16711680;
			result.GreenMask = 65280;
			result.BlueMask = 255;
			result.Compression = BmpCompression.BitFields;
		}
		if (infoHeaderType == BmpInfoHeaderType.WinVersion5 && iccProfileData != null)
		{
			result.ProfileSize = iccProfileData.Length;
			result.CsType = BmpColorSpace.PROFILE_EMBEDDED;
			result.Intent = BmpRenderingIntent.LCS_GM_IMAGES;
		}
		return result;
	}

	private static void WriteColorProfile(Stream stream, byte[]? iccProfileData, Span<byte> buffer)
	{
		if (iccProfileData != null)
		{
			int value = (int)stream.Position - 14;
			stream.Write(iccProfileData);
			BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
			stream.Position = 126L;
			stream.Write(buffer.Slice(0, 4));
		}
	}

	private static void WriteBitmapFileHeader(Stream stream, int infoHeaderSize, int colorPaletteSize, int iccProfileSize, BmpInfoHeader infoHeader, Span<byte> buffer)
	{
		new BmpFileHeader(19778, 14 + infoHeaderSize + colorPaletteSize + iccProfileSize + infoHeader.ImageSize, 0, 14 + infoHeaderSize + colorPaletteSize).WriteTo(buffer);
		stream.Write(buffer, 0, 14);
	}

	private void WriteBitmapInfoHeader(Stream stream, BmpInfoHeader infoHeader, Span<byte> buffer, int infoHeaderSize)
	{
		switch (infoHeaderType)
		{
		case BmpInfoHeaderType.WinVersion3:
			infoHeader.WriteV3Header(buffer);
			break;
		case BmpInfoHeaderType.WinVersion4:
			infoHeader.WriteV4Header(buffer);
			break;
		case BmpInfoHeaderType.WinVersion5:
			infoHeader.WriteV5Header(buffer);
			break;
		}
		stream.Write(buffer, 0, infoHeaderSize);
	}

	private void WriteImage<TPixel>(Configuration configuration, Stream stream, Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		Buffer2D<TPixel> pixelBuffer = image.Frames.RootFrame.PixelBuffer;
		BmpBitsPerPixel? bmpBitsPerPixel = bitsPerPixel;
		if (bmpBitsPerPixel.HasValue)
		{
			switch (bmpBitsPerPixel.GetValueOrDefault())
			{
			case BmpBitsPerPixel.Pixel32:
				Write32BitPixelData(configuration, stream, pixelBuffer);
				break;
			case BmpBitsPerPixel.Pixel24:
				Write24BitPixelData(configuration, stream, pixelBuffer);
				break;
			case BmpBitsPerPixel.Pixel16:
				Write16BitPixelData(configuration, stream, pixelBuffer);
				break;
			case BmpBitsPerPixel.Pixel8:
				Write8BitPixelData(configuration, stream, image);
				break;
			case BmpBitsPerPixel.Pixel4:
				Write4BitPixelData(configuration, stream, image);
				break;
			case BmpBitsPerPixel.Pixel2:
				Write2BitPixelData(configuration, stream, image);
				break;
			case BmpBitsPerPixel.Pixel1:
				Write1BitPixelData(configuration, stream, image);
				break;
			}
		}
	}

	private IMemoryOwner<byte> AllocateRow(int width, int bytesPerPixel)
	{
		return memoryAllocator.AllocatePaddedPixelRowBuffer(width, bytesPerPixel, padding);
	}

	private void Write32BitPixelData<TPixel>(Configuration configuration, Stream stream, Buffer2D<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IMemoryOwner<byte> buffer = AllocateRow(pixels.Width, 4);
		Span<byte> span = buffer.GetSpan();
		for (int num = pixels.Height - 1; num >= 0; num--)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(num);
			PixelOperations<TPixel>.Instance.ToBgra32Bytes(configuration, span2, span, span2.Length);
			stream.Write(span);
		}
	}

	private void Write24BitPixelData<TPixel>(Configuration configuration, Stream stream, Buffer2D<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int length = width * 3;
		using IMemoryOwner<byte> buffer = AllocateRow(width, 3);
		Span<byte> span = buffer.GetSpan();
		for (int num = pixels.Height - 1; num >= 0; num--)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(num);
			PixelOperations<TPixel>.Instance.ToBgr24Bytes(configuration, span2, buffer.Slice(0, length), width);
			stream.Write(span);
		}
	}

	private void Write16BitPixelData<TPixel>(Configuration configuration, Stream stream, Buffer2D<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int length = width * 2;
		using IMemoryOwner<byte> buffer = AllocateRow(width, 2);
		Span<byte> span = buffer.GetSpan();
		for (int num = pixels.Height - 1; num >= 0; num--)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(num);
			PixelOperations<TPixel>.Instance.ToBgra5551Bytes(configuration, span2, buffer.Slice(0, length), span2.Length);
			stream.Write(span);
		}
	}

	private void Write8BitPixelData<TPixel>(Configuration configuration, Stream stream, Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		bool flag = typeof(TPixel) == typeof(L8);
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(1024, AllocationOptions.Clean);
		Span<byte> span = buffer.GetSpan();
		if (flag)
		{
			Write8BitPixelData(stream, image, span);
		}
		else
		{
			Write8BitColor(configuration, stream, image, span);
		}
	}

	private void Write8BitColor<TPixel>(Configuration configuration, Stream stream, Image<TPixel> image, Span<byte> colorPalette) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IQuantizer<TPixel> quantizer = this.quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration);
		quantizer.BuildPalette(pixelSamplingStrategy, image);
		using IndexedImageFrame<TPixel> indexedImageFrame = quantizer.QuantizeFrame(image.Frames.RootFrame, image.Bounds);
		ReadOnlySpan<TPixel> span = indexedImageFrame.Palette.Span;
		WriteColorPalette(configuration, stream, span, colorPalette);
		for (int num = image.Height - 1; num >= 0; num--)
		{
			ReadOnlySpan<byte> buffer = indexedImageFrame.DangerousGetRowSpan(num);
			stream.Write(buffer);
			for (int i = 0; i < padding; i++)
			{
				stream.WriteByte(0);
			}
		}
	}

	private void Write8BitPixelData<TPixel>(Stream stream, Image<TPixel> image, Span<byte> colorPalette) where TPixel : unmanaged, IPixel<TPixel>
	{
		for (int i = 0; i <= 255; i++)
		{
			int num = i * 4;
			byte b = (byte)i;
			colorPalette[num] = b;
			colorPalette[num + 1] = b;
			colorPalette[num + 2] = b;
			colorPalette[num + 3] = 0;
		}
		stream.Write(colorPalette);
		Buffer2D<TPixel> rootFramePixelBuffer = image.GetRootFramePixelBuffer();
		for (int num2 = image.Height - 1; num2 >= 0; num2--)
		{
			ReadOnlySpan<byte> buffer = MemoryMarshal.AsBytes((ReadOnlySpan<TPixel>)rootFramePixelBuffer.DangerousGetRowSpan(num2));
			stream.Write(buffer);
			for (int j = 0; j < padding; j++)
			{
				stream.WriteByte(0);
			}
		}
	}

	private void Write4BitPixelData<TPixel>(Configuration configuration, Stream stream, Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IQuantizer<TPixel> quantizer = this.quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration, new QuantizerOptions
		{
			MaxColors = 16
		});
		quantizer.BuildPalette(pixelSamplingStrategy, image);
		using IndexedImageFrame<TPixel> indexedImageFrame = quantizer.QuantizeFrame(image.Frames.RootFrame, image.Bounds);
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(64, AllocationOptions.Clean);
		Span<byte> span = buffer.GetSpan();
		ReadOnlySpan<TPixel> span2 = indexedImageFrame.Palette.Span;
		WriteColorPalette(configuration, stream, span2, span);
		int num = ((indexedImageFrame.DangerousGetRowSpan(0).Length % 2 != 0) ? (padding - 1) : padding);
		for (int num2 = image.Height - 1; num2 >= 0; num2--)
		{
			ReadOnlySpan<byte> readOnlySpan = indexedImageFrame.DangerousGetRowSpan(num2);
			int num3 = ((readOnlySpan.Length % 2 == 0) ? readOnlySpan.Length : (readOnlySpan.Length - 1));
			for (int i = 0; i < num3; i += 2)
			{
				stream.WriteByte((byte)((readOnlySpan[i] << 4) | readOnlySpan[i + 1]));
			}
			if (readOnlySpan.Length % 2 != 0)
			{
				stream.WriteByte((byte)((readOnlySpan[readOnlySpan.Length - 1] << 4) | 0));
			}
			for (int j = 0; j < num; j++)
			{
				stream.WriteByte(0);
			}
		}
	}

	private void Write2BitPixelData<TPixel>(Configuration configuration, Stream stream, Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IQuantizer<TPixel> quantizer = this.quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration, new QuantizerOptions
		{
			MaxColors = 4
		});
		quantizer.BuildPalette(pixelSamplingStrategy, image);
		using IndexedImageFrame<TPixel> indexedImageFrame = quantizer.QuantizeFrame(image.Frames.RootFrame, image.Bounds);
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(16, AllocationOptions.Clean);
		Span<byte> span = buffer.GetSpan();
		ReadOnlySpan<TPixel> span2 = indexedImageFrame.Palette.Span;
		WriteColorPalette(configuration, stream, span2, span);
		int num = ((indexedImageFrame.DangerousGetRowSpan(0).Length % 4 != 0) ? (padding - 1) : padding);
		for (int num2 = image.Height - 1; num2 >= 0; num2--)
		{
			ReadOnlySpan<byte> readOnlySpan = indexedImageFrame.DangerousGetRowSpan(num2);
			int num3 = ((readOnlySpan.Length % 4 == 0) ? readOnlySpan.Length : (readOnlySpan.Length - 4));
			int num4 = 0;
			for (num4 = 0; num4 < num3; num4 += 4)
			{
				stream.WriteByte((byte)((readOnlySpan[num4] << 6) | (readOnlySpan[num4 + 1] << 4) | (readOnlySpan[num4 + 2] << 2) | readOnlySpan[num4 + 3]));
			}
			if (readOnlySpan.Length % 4 != 0)
			{
				int num5 = 6;
				byte b = 0;
				for (; num4 < readOnlySpan.Length; num4++)
				{
					b = (byte)(b | (readOnlySpan[num4] << num5));
					num5 -= 2;
				}
				stream.WriteByte(b);
			}
			for (num4 = 0; num4 < num; num4++)
			{
				stream.WriteByte(0);
			}
		}
	}

	private void Write1BitPixelData<TPixel>(Configuration configuration, Stream stream, Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IQuantizer<TPixel> quantizer = this.quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration, new QuantizerOptions
		{
			MaxColors = 2
		});
		quantizer.BuildPalette(pixelSamplingStrategy, image);
		using IndexedImageFrame<TPixel> indexedImageFrame = quantizer.QuantizeFrame(image.Frames.RootFrame, image.Bounds);
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(8, AllocationOptions.Clean);
		Span<byte> span = buffer.GetSpan();
		ReadOnlySpan<TPixel> span2 = indexedImageFrame.Palette.Span;
		WriteColorPalette(configuration, stream, span2, span);
		int num = ((indexedImageFrame.DangerousGetRowSpan(0).Length % 8 != 0) ? (padding - 1) : padding);
		for (int num2 = image.Height - 1; num2 >= 0; num2--)
		{
			ReadOnlySpan<byte> quantizedPixelRow = indexedImageFrame.DangerousGetRowSpan(num2);
			int num3 = ((quantizedPixelRow.Length % 8 == 0) ? quantizedPixelRow.Length : (quantizedPixelRow.Length - 8));
			for (int i = 0; i < num3; i += 8)
			{
				Write1BitPalette(stream, i, i + 8, quantizedPixelRow);
			}
			if (quantizedPixelRow.Length % 8 != 0)
			{
				int startIdx = quantizedPixelRow.Length - quantizedPixelRow.Length % 8;
				num3 = quantizedPixelRow.Length;
				Write1BitPalette(stream, startIdx, num3, quantizedPixelRow);
			}
			for (int j = 0; j < num; j++)
			{
				stream.WriteByte(0);
			}
		}
	}

	private static void WriteColorPalette<TPixel>(Configuration configuration, Stream stream, ReadOnlySpan<TPixel> quantizedColorPalette, Span<byte> colorPalette) where TPixel : unmanaged, IPixel<TPixel>
	{
		int length = quantizedColorPalette.Length * 4;
		PixelOperations<TPixel>.Instance.ToBgra32(configuration, quantizedColorPalette, MemoryMarshal.Cast<byte, Bgra32>(colorPalette.Slice(0, length)));
		Span<uint> span = MemoryMarshal.Cast<byte, uint>(colorPalette);
		for (int i = 0; i < span.Length; i++)
		{
			span[i] &= 16777215u;
		}
		stream.Write(colorPalette);
	}

	private static void Write1BitPalette(Stream stream, int startIdx, int endIdx, ReadOnlySpan<byte> quantizedPixelRow)
	{
		int num = 7;
		byte b = 0;
		for (int i = startIdx; i < endIdx; i++)
		{
			b = (byte)(b | ((byte)(quantizedPixelRow[i] & 1) << num));
			num--;
		}
		stream.WriteByte(b);
	}
}
