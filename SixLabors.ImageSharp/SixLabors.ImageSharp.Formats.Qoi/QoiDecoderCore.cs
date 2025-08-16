using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Qoi;

internal class QoiDecoderCore : IImageDecoderInternals
{
	private readonly Configuration configuration;

	private readonly MemoryAllocator memoryAllocator;

	private QoiHeader header;

	public DecoderOptions Options { get; }

	public Size Dimensions { get; }

	public QoiDecoderCore(DecoderOptions options)
	{
		Options = options;
		configuration = options.Configuration;
		memoryAllocator = configuration.MemoryAllocator;
	}

	public Image<TPixel> Decode<TPixel>(BufferedReadStream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		ProcessHeader(stream);
		ImageMetadata metadata = new ImageMetadata
		{
			DecodedImageFormat = QoiFormat.Instance,
			HorizontalResolution = header.Width,
			VerticalResolution = header.Height,
			ResolutionUnits = PixelResolutionUnit.AspectRatio
		};
		QoiMetadata qoiMetadata = metadata.GetQoiMetadata();
		qoiMetadata.Channels = header.Channels;
		qoiMetadata.ColorSpace = header.ColorSpace;
		Image<TPixel> image = new Image<TPixel>(configuration, (int)header.Width, (int)header.Height, metadata);
		Buffer2D<TPixel> rootFramePixelBuffer = image.GetRootFramePixelBuffer();
		ProcessPixels(stream, rootFramePixelBuffer);
		return image;
	}

	public ImageInfo Identify(BufferedReadStream stream, CancellationToken cancellationToken)
	{
		ProcessHeader(stream);
		PixelTypeInfo pixelType = new PixelTypeInfo(8 * (int)header.Channels);
		Size size = new Size((int)header.Width, (int)header.Height);
		ImageMetadata metadata = new ImageMetadata();
		QoiMetadata qoiMetadata = metadata.GetQoiMetadata();
		qoiMetadata.Channels = header.Channels;
		qoiMetadata.ColorSpace = header.ColorSpace;
		return new ImageInfo(pixelType, size, metadata);
	}

	private void ProcessHeader(BufferedReadStream stream)
	{
		Span<byte> span = stackalloc byte[4];
		Span<byte> span2 = stackalloc byte[4];
		Span<byte> span3 = stackalloc byte[4];
		if (stream.Read(span) != 4 || !span.SequenceEqual(QoiConstants.Magic.ToArray()))
		{
			ThrowInvalidImageContentException();
		}
		if (stream.Read(span2) != 4)
		{
			ThrowInvalidImageContentException();
		}
		if (stream.Read(span3) != 4)
		{
			ThrowInvalidImageContentException();
		}
		uint num = BinaryPrimitives.ReadUInt32BigEndian(span2);
		uint num2 = BinaryPrimitives.ReadUInt32BigEndian(span3);
		if (num == 0 || num2 == 0)
		{
			throw new InvalidImageContentException($"The image has an invalid size: width = {num}, height = {num2}");
		}
		int num3 = stream.ReadByte();
		if ((num3 == -1 || (uint)(num3 - 3) > 1u) ? true : false)
		{
			ThrowInvalidImageContentException();
		}
		int num4 = stream.ReadByte();
		if ((num4 == -1 || (uint)num4 > 1u) ? true : false)
		{
			ThrowInvalidImageContentException();
		}
		header = new QoiHeader(num, num2, (QoiChannels)num3, (QoiColorSpace)num4);
	}

	[DoesNotReturn]
	private static void ThrowInvalidImageContentException()
	{
		throw new InvalidImageContentException("The image is not a valid QOI image.");
	}

	private void ProcessPixels<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IMemoryOwner<Rgba32> buffer = memoryAllocator.Allocate<Rgba32>(64, AllocationOptions.Clean);
		Span<Rgba32> span = buffer.GetSpan();
		Rgba32 rgba = new Rgba32(0, 0, 0, byte.MaxValue);
		int arrayPosition = GetArrayPosition(rgba);
		span[arrayPosition] = rgba;
		Rgba32 source = default(Rgba32);
		Span<byte> buffer2 = MemoryMarshal.CreateSpan(ref Unsafe.As<Rgba32, byte>(ref source), 4);
		TPixel val = default(TPixel);
		for (int i = 0; i < header.Height; i++)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(i);
			for (int j = 0; j < span2.Length; j++)
			{
				byte b = (byte)stream.ReadByte();
				switch ((QoiChunk)b)
				{
				case QoiChunk.QoiOpRgb:
					if (stream.Read(buffer2.Slice(0, 3)) < 3)
					{
						ThrowInvalidImageContentException();
					}
					source.A = rgba.A;
					val.FromRgba32(source);
					arrayPosition = GetArrayPosition(source);
					span[arrayPosition] = source;
					break;
				case QoiChunk.QoiOpRgba:
					if (stream.Read(buffer2) < 4)
					{
						ThrowInvalidImageContentException();
					}
					val.FromRgba32(source);
					arrayPosition = GetArrayPosition(source);
					span[arrayPosition] = source;
					break;
				default:
					switch ((QoiChunk)(b & 0xC0))
					{
					case QoiChunk.QoiOpIndex:
						source = span[b];
						val.FromRgba32(source);
						break;
					case QoiChunk.QoiOpDiff:
					{
						int num3 = (b & 0x30) >> 4;
						int num4 = (b & 0xC) >> 2;
						int num5 = b & 3;
						Rgba32 rgba2 = rgba;
						rgba2.R = (byte)Numerics.Modulo256(rgba.R + (num3 - 2));
						rgba2.G = (byte)Numerics.Modulo256(rgba.G + (num4 - 2));
						rgba2.B = (byte)Numerics.Modulo256(rgba.B + (num5 - 2));
						source = rgba2;
						val.FromRgba32(source);
						arrayPosition = GetArrayPosition(source);
						span[arrayPosition] = source;
						break;
					}
					case QoiChunk.QoiOpLuma:
					{
						int num6 = b & 0x3F;
						int num7 = Numerics.Modulo256(rgba.G + (num6 - 32));
						int num8 = stream.ReadByte();
						int num9 = num8 >> 4;
						int num10 = num8 & 0xF;
						int num11 = Numerics.Modulo256(num9 - 8 + (num6 - 32) + rgba.R);
						int num12 = Numerics.Modulo256(num10 - 8 + (num6 - 32) + rgba.B);
						Rgba32 rgba2 = rgba;
						rgba2.R = (byte)num11;
						rgba2.B = (byte)num12;
						rgba2.G = (byte)num7;
						source = rgba2;
						val.FromRgba32(source);
						arrayPosition = GetArrayPosition(source);
						span[arrayPosition] = source;
						break;
					}
					case QoiChunk.QoiOpRun:
					{
						int num = b & 0x3F;
						if ((uint)(num - 62) <= 1u)
						{
							ThrowInvalidImageContentException();
						}
						source = rgba;
						val.FromRgba32(source);
						int num2 = -1;
						while (num2 < num)
						{
							if (j == span2.Length)
							{
								j = 0;
								i++;
								span2 = pixels.DangerousGetRowSpan(i);
							}
							span2[j] = val;
							num2++;
							j++;
						}
						j--;
						continue;
					}
					default:
						ThrowInvalidImageContentException();
						return;
					}
					break;
				}
				span2[j] = val;
				rgba = source;
			}
		}
		for (int k = 0; k < 7; k++)
		{
			if (stream.ReadByte() != 0)
			{
				ThrowInvalidImageContentException();
			}
		}
		if (stream.ReadByte() != 1)
		{
			ThrowInvalidImageContentException();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetArrayPosition(Rgba32 pixel)
	{
		return Numerics.Modulo64(pixel.R * 3 + pixel.G * 5 + pixel.B * 7 + pixel.A * 11);
	}
}
