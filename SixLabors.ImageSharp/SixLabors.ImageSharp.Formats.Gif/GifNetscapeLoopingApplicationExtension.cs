using System;
using System.Buffers.Binary;

namespace SixLabors.ImageSharp.Formats.Gif;

internal readonly struct GifNetscapeLoopingApplicationExtension : IGifExtension
{
	public byte Label => byte.MaxValue;

	public int ContentLength => 16;

	public ushort RepeatCount { get; }

	public GifNetscapeLoopingApplicationExtension(ushort repeatCount)
	{
		RepeatCount = repeatCount;
	}

	public static GifNetscapeLoopingApplicationExtension Parse(ReadOnlySpan<byte> buffer)
	{
		return new GifNetscapeLoopingApplicationExtension(BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(0, 2)));
	}

	public int WriteTo(Span<byte> buffer)
	{
		buffer[0] = 11;
		GifConstants.NetscapeApplicationIdentificationBytes.CopyTo(buffer.Slice(1, 11));
		buffer[12] = 3;
		buffer[13] = 1;
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(14, 2), RepeatCount);
		return ContentLength;
	}
}
