using System;
using System.IO;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Formats.Webp.Chunks;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;

namespace SixLabors.ImageSharp.Formats.Webp.BitWriter;

internal abstract class BitWriterBase
{
	private const uint MaxDimension = 16777215u;

	private const ulong MaxCanvasPixels = 4294967295uL;

	private byte[] buffer;

	public byte[] Buffer => buffer;

	public abstract int NumBytes { get; }

	protected BitWriterBase(int expectedSize)
	{
		buffer = new byte[expectedSize];
	}

	private protected BitWriterBase(byte[] buffer)
	{
		this.buffer = buffer;
	}

	public void WriteToStream(Stream stream)
	{
		stream.Write(Buffer.AsSpan(0, NumBytes));
	}

	public void WriteToBuffer(Span<byte> dest)
	{
		Buffer.AsSpan(0, NumBytes).CopyTo(dest);
	}

	public abstract void BitWriterResize(int extraSize);

	public abstract void Finish();

	protected void ResizeBuffer(int maxBytes, int sizeRequired)
	{
		int num = 3 * maxBytes >> 1;
		if (num < sizeRequired)
		{
			num = sizeRequired;
		}
		num = (num >> 10) + 1 << 10;
		Array.Resize(ref buffer, num);
	}

	public static WebpVp8X WriteTrunksBeforeData(Stream stream, uint width, uint height, ExifProfile? exifProfile, XmpProfile? xmpProfile, IccProfile? iccProfile, bool hasAlpha, bool hasAnimation)
	{
		RiffHelper.BeginWriteRiffFile(stream, "WEBP");
		WebpVp8X result = default(WebpVp8X);
		if (exifProfile != null || xmpProfile != null || iccProfile != null || hasAlpha || hasAnimation)
		{
			result = WriteVp8XHeader(stream, exifProfile, xmpProfile, iccProfile, width, height, hasAlpha, hasAnimation);
			if (iccProfile != null)
			{
				RiffHelper.WriteChunk(stream, 1229144912u, iccProfile.ToByteArray());
			}
		}
		return result;
	}

	public abstract void WriteEncodedImageToStream(Stream stream);

	public static void WriteTrunksAfterData(Stream stream, in WebpVp8X vp8x, bool updateVp8x, long initialPosition, ExifProfile? exifProfile, XmpProfile? xmpProfile)
	{
		if (exifProfile != null)
		{
			RiffHelper.WriteChunk(stream, 1163413830u, exifProfile.ToByteArray());
		}
		if (xmpProfile != null)
		{
			RiffHelper.WriteChunk(stream, 1481461792u, xmpProfile.Data);
		}
		RiffHelper.EndWriteRiffFile(stream, in vp8x, updateVp8x, initialPosition);
	}

	public static void WriteAnimationParameter(Stream stream, Color background, ushort loopCount)
	{
		WebpAnimationParameter webpAnimationParameter = new WebpAnimationParameter(background.ToBgra32().PackedValue, loopCount);
		webpAnimationParameter.WriteTo(stream);
	}

	public static void WriteAlphaChunk(Stream stream, Span<byte> dataBytes, bool alphaDataIsCompressed)
	{
		long sizePosition = RiffHelper.BeginWriteChunk(stream, 1095520328u);
		byte value = 0;
		if (alphaDataIsCompressed)
		{
			value = 1;
		}
		stream.WriteByte(value);
		stream.Write(dataBytes);
		RiffHelper.EndWriteChunk(stream, sizePosition);
	}

	protected static WebpVp8X WriteVp8XHeader(Stream stream, ExifProfile? exifProfile, XmpProfile? xmpProfile, IccProfile? iccProfile, uint width, uint height, bool hasAlpha, bool hasAnimation)
	{
		WebpVp8X result = new WebpVp8X(hasAnimation, xmpProfile != null, exifProfile != null, hasAlpha, iccProfile != null, width, height);
		result.Validate(16777215u, 4294967295uL);
		result.WriteTo(stream);
		return result;
	}
}
