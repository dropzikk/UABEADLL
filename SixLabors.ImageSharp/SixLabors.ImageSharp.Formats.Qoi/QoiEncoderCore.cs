using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Qoi;

internal class QoiEncoderCore : IImageEncoderInternals
{
	private readonly QoiEncoder encoder;

	private readonly MemoryAllocator memoryAllocator;

	private readonly Configuration configuration;

	public QoiEncoderCore(QoiEncoder encoder, Configuration configuration)
	{
		this.encoder = encoder;
		this.configuration = configuration;
		memoryAllocator = configuration.MemoryAllocator;
	}

	public void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(image, "image");
		Guard.NotNull(stream, "stream");
		WriteHeader(image, stream);
		WritePixels(image, stream);
		WriteEndOfStream(stream);
		stream.Flush();
	}

	private void WriteHeader(Image image, Stream stream)
	{
		Span<byte> span = stackalloc byte[4];
		Span<byte> span2 = stackalloc byte[4];
		BinaryPrimitives.WriteUInt32BigEndian(span, (uint)image.Width);
		BinaryPrimitives.WriteUInt32BigEndian(span2, (uint)image.Height);
		QoiChannels qoiChannels = encoder.Channels ?? QoiChannels.Rgba;
		QoiColorSpace valueOrDefault = encoder.ColorSpace.GetValueOrDefault();
		stream.Write(QoiConstants.Magic);
		stream.Write(span);
		stream.Write(span2);
		stream.WriteByte((byte)qoiChannels);
		stream.WriteByte((byte)valueOrDefault);
	}

	private void WritePixels<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IMemoryOwner<Rgba32> buffer = memoryAllocator.Allocate<Rgba32>(64, AllocationOptions.Clean);
		Span<Rgba32> span = buffer.GetSpan();
		Rgba32 other = new Rgba32(0, 0, 0, byte.MaxValue);
		Rgba32 rgba = default(Rgba32);
		Buffer2D<TPixel> pixelBuffer = image.Frames[0].PixelBuffer;
		using IMemoryOwner<Rgba32> buffer2 = memoryAllocator.Allocate<Rgba32>(pixelBuffer.Width);
		Span<Rgba32> span2 = buffer2.GetSpan();
		for (int i = 0; i < pixelBuffer.Height; i++)
		{
			Span<TPixel> span3 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToRgba32(configuration, span3, span2);
			for (int j = 0; j < span3.Length; j++)
			{
				if (i >= pixelBuffer.Height)
				{
					break;
				}
				rgba = span2[j];
				if (rgba.Equals(other))
				{
					int num = 0;
					do
					{
						num++;
						j++;
						if (j == span3.Length)
						{
							j = 0;
							i++;
							if (i == pixelBuffer.Height)
							{
								break;
							}
							span3 = pixelBuffer.DangerousGetRowSpan(i);
							PixelOperations<TPixel>.Instance.ToRgba32(configuration, span3, span2);
						}
						rgba = span2[j];
					}
					while (rgba.Equals(other) && num < 62);
					j--;
					stream.WriteByte((byte)(0xC0 | (num - 1)));
					continue;
				}
				int arrayPosition = GetArrayPosition(rgba);
				if (span[arrayPosition].Equals(rgba))
				{
					stream.WriteByte((byte)arrayPosition);
				}
				else
				{
					span[arrayPosition] = rgba;
					int num2 = rgba.R - other.R;
					int num3 = rgba.G - other.G;
					int num4 = rgba.B - other.B;
					if (num2 >= -2 && num2 <= 1 && num3 >= -2 && num3 <= 1 && num4 >= -2 && num4 <= 1 && rgba.A == other.A)
					{
						int num5 = num2 + 2;
						int num6 = num3 + 2;
						int num7 = num4 + 2;
						byte value = (byte)(0x40 | (num5 << 4) | (num6 << 2) | num7);
						stream.WriteByte(value);
					}
					else
					{
						int num8 = num2 - num3;
						int num9 = num4 - num3;
						if (num3 >= -32 && num3 <= 31 && num8 >= -8 && num8 <= 7 && num9 >= -8 && num9 <= 7 && rgba.A == other.A)
						{
							int num10 = num8 + 8;
							int num11 = num9 + 8;
							byte value2 = (byte)(0x80 | (num3 + 32));
							byte value3 = (byte)((num10 << 4) | num11);
							stream.WriteByte(value2);
							stream.WriteByte(value3);
						}
						else if (rgba.A == other.A)
						{
							stream.WriteByte(254);
							stream.WriteByte(rgba.R);
							stream.WriteByte(rgba.G);
							stream.WriteByte(rgba.B);
						}
						else
						{
							stream.WriteByte(byte.MaxValue);
							stream.WriteByte(rgba.R);
							stream.WriteByte(rgba.G);
							stream.WriteByte(rgba.B);
							stream.WriteByte(rgba.A);
						}
					}
				}
				other = rgba;
			}
		}
	}

	private static void WriteEndOfStream(Stream stream)
	{
		for (int i = 0; i < 7; i++)
		{
			stream.WriteByte(0);
		}
		stream.WriteByte(1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetArrayPosition(Rgba32 pixel)
	{
		return Numerics.Modulo64(pixel.R * 3 + pixel.G * 5 + pixel.B * 7 + pixel.A * 11);
	}
}
