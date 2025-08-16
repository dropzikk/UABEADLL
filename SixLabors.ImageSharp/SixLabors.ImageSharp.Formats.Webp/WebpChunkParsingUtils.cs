using System;
using System.Buffers.Binary;
using System.IO;
using SixLabors.ImageSharp.Formats.Webp.BitReader;
using SixLabors.ImageSharp.Formats.Webp.Lossy;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;

namespace SixLabors.ImageSharp.Formats.Webp;

internal static class WebpChunkParsingUtils
{
	public static WebpImageInfo ReadVp8Header(MemoryAllocator memoryAllocator, BufferedReadStream stream, Span<byte> buffer, WebpFeatures features)
	{
		if (stream.Read(buffer, 0, 4) != 4)
		{
			WebpThrowHelper.ThrowInvalidImageContentException("Not enough data to read the VP8 header");
		}
		uint num = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
		uint num2 = num;
		if (stream.Read(buffer, 0, 3) != 3)
		{
			WebpThrowHelper.ThrowInvalidImageContentException("Not enough data to read the VP8 header");
		}
		int num3 = buffer[0] | (buffer[1] << 8) | (buffer[2] << 16);
		num2 -= 3;
		if ((num3 & 1) == 1)
		{
			WebpThrowHelper.ThrowImageFormatException("VP8 header indicates the image is not a key frame");
		}
		uint num4 = (uint)((num3 >>> 1) & 7);
		if (num4 > 3)
		{
			WebpThrowHelper.ThrowImageFormatException($"VP8 header indicates unknown profile {num4}");
		}
		if (((num3 >>> 4) & 1) == 0)
		{
			WebpThrowHelper.ThrowImageFormatException("VP8 header indicates that the first frame is invisible");
		}
		uint num5 = (uint)num3 >> 5;
		if (num5 > num)
		{
			WebpThrowHelper.ThrowImageFormatException("VP8 header contains inconsistent size information");
		}
		if (stream.Read(buffer, 0, 3) != 3)
		{
			WebpThrowHelper.ThrowInvalidImageContentException("Not enough data to read the VP8 magic bytes");
		}
		if (!buffer.Slice(0, 3).SequenceEqual(WebpConstants.Vp8HeaderMagicBytes))
		{
			WebpThrowHelper.ThrowImageFormatException("VP8 magic bytes not found");
		}
		if (stream.Read(buffer, 0, 4) != 4)
		{
			WebpThrowHelper.ThrowInvalidImageContentException("Not enough data to read the VP8 header, could not read width and height");
		}
		ushort num6 = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
		uint num7 = (uint)(num6 & 0x3FFF);
		sbyte xScale = (sbyte)(num6 >>> 6);
		ushort num8 = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(2, buffer.Length - 2));
		uint num9 = (uint)(num8 & 0x3FFF);
		sbyte yScale = (sbyte)(num8 >>> 6);
		num2 -= 7;
		if (num7 == 0 || num9 == 0)
		{
			WebpThrowHelper.ThrowImageFormatException("width or height can not be zero");
		}
		if (num5 > num2)
		{
			WebpThrowHelper.ThrowImageFormatException("bad partition length");
		}
		Vp8FrameHeader vp8FrameHeader = new Vp8FrameHeader
		{
			KeyFrame = true,
			Profile = (sbyte)num4,
			PartitionLength = num5
		};
		Vp8BitReader vp8BitReader = new Vp8BitReader(stream, num2, memoryAllocator, num5)
		{
			Remaining = num2
		};
		return new WebpImageInfo
		{
			Width = num7,
			Height = num9,
			XScale = xScale,
			YScale = yScale,
			BitsPerPixel = ((features != null && features.Alpha) ? WebpBitsPerPixel.Pixel32 : WebpBitsPerPixel.Pixel24),
			IsLossless = false,
			Features = features,
			Vp8Profile = (sbyte)num4,
			Vp8FrameHeader = vp8FrameHeader,
			Vp8BitReader = vp8BitReader
		};
	}

	public static WebpImageInfo ReadVp8LHeader(MemoryAllocator memoryAllocator, BufferedReadStream stream, Span<byte> buffer, WebpFeatures features)
	{
		uint imageDataSize = ReadChunkSize(stream, buffer);
		Vp8LBitReader vp8LBitReader = new Vp8LBitReader(stream, imageDataSize, memoryAllocator);
		if (vp8LBitReader.ReadValue(8) != 47)
		{
			WebpThrowHelper.ThrowImageFormatException("Invalid VP8L signature");
		}
		uint num = vp8LBitReader.ReadValue(14) + 1;
		uint num2 = vp8LBitReader.ReadValue(14) + 1;
		if (num == 0 || num2 == 0)
		{
			WebpThrowHelper.ThrowImageFormatException("invalid width or height read");
		}
		vp8LBitReader.ReadBit();
		uint num3 = vp8LBitReader.ReadValue(3);
		if (num3 != 0)
		{
			WebpThrowHelper.ThrowNotSupportedException($"Unexpected version number {num3} found in VP8L header");
		}
		return new WebpImageInfo
		{
			Width = num,
			Height = num2,
			BitsPerPixel = WebpBitsPerPixel.Pixel32,
			IsLossless = true,
			Features = features,
			Vp8LBitReader = vp8LBitReader
		};
	}

	public static WebpImageInfo ReadVp8XHeader(BufferedReadStream stream, Span<byte> buffer, WebpFeatures features)
	{
		ReadChunkSize(stream, buffer);
		byte b = (byte)stream.ReadByte();
		if (b >> 6 != 0)
		{
			WebpThrowHelper.ThrowImageFormatException("first two bits of the VP8X header are expected to be zero");
		}
		features.IccProfile = (b & 0x20) != 0;
		features.Alpha = (b & 0x10) != 0;
		features.ExifProfile = (b & 8) != 0;
		features.XmpMetaData = (b & 4) != 0;
		features.Animation = (b & 2) != 0;
		stream.Read(buffer, 0, 3);
		if (buffer[0] != 0 || buffer[1] != 0 || buffer[2] != 0)
		{
			WebpThrowHelper.ThrowImageFormatException("reserved bytes should be zero");
		}
		uint width = ReadUInt24LittleEndian(stream, buffer) + 1;
		uint height = ReadUInt24LittleEndian(stream, buffer) + 1;
		return new WebpImageInfo
		{
			Width = width,
			Height = height,
			Features = features
		};
	}

	public static uint ReadUInt24LittleEndian(Stream stream, Span<byte> buffer)
	{
		if (stream.Read(buffer, 0, 3) == 3)
		{
			buffer[3] = 0;
			return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
		}
		throw new ImageFormatException("Invalid Webp data, could not read unsigned 24 bit integer.");
	}

	public unsafe static void WriteUInt24LittleEndian(Stream stream, uint data)
	{
		if (data >= 16777216)
		{
			throw new InvalidDataException($"Invalid data, {data} is not a unsigned 24 bit integer.");
		}
		byte* ptr = (byte*)(&data);
		stream.WriteByte(*ptr);
		stream.WriteByte(ptr[1]);
		stream.WriteByte(ptr[2]);
	}

	public static uint ReadChunkSize(Stream stream, Span<byte> buffer)
	{
		if (stream.Read(buffer) == 4)
		{
			uint num = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
			if (num % 2 != 0)
			{
				return num + 1;
			}
			return num;
		}
		throw new ImageFormatException("Invalid Webp data, could not read chunk size.");
	}

	public static WebpChunkType ReadChunkType(BufferedReadStream stream, Span<byte> buffer)
	{
		if (stream.Read(buffer) == 4)
		{
			return (WebpChunkType)BinaryPrimitives.ReadUInt32BigEndian(buffer);
		}
		throw new ImageFormatException("Invalid Webp data, could not read chunk type.");
	}

	public static void ParseOptionalChunks(BufferedReadStream stream, WebpChunkType chunkType, ImageMetadata metadata, bool ignoreMetaData, Span<byte> buffer)
	{
		long length = stream.Length;
		while (stream.Position < length)
		{
			uint num = ReadChunkSize(stream, buffer);
			if (ignoreMetaData)
			{
				stream.Skip((int)num);
			}
			switch (chunkType)
			{
			case WebpChunkType.Exif:
			{
				byte[] array2 = new byte[num];
				if (stream.Read(array2, 0, (int)num) != num)
				{
					WebpThrowHelper.ThrowImageFormatException("Could not read enough data for the EXIF profile");
				}
				if (metadata.ExifProfile != null)
				{
					metadata.ExifProfile = new ExifProfile(array2);
				}
				break;
			}
			case WebpChunkType.Xmp:
			{
				byte[] array = new byte[num];
				if (stream.Read(array, 0, (int)num) != num)
				{
					WebpThrowHelper.ThrowImageFormatException("Could not read enough data for the XMP profile");
				}
				if (metadata.XmpProfile != null)
				{
					metadata.XmpProfile = new XmpProfile(array);
				}
				break;
			}
			default:
				stream.Skip((int)num);
				break;
			}
		}
	}

	public static bool IsOptionalVp8XChunk(WebpChunkType chunkType)
	{
		return chunkType switch
		{
			WebpChunkType.Alpha => true, 
			WebpChunkType.AnimationParameter => true, 
			WebpChunkType.Exif => true, 
			WebpChunkType.Iccp => true, 
			WebpChunkType.Xmp => true, 
			_ => false, 
		};
	}
}
