using System;
using System.Buffers.Binary;

namespace SixLabors.ImageSharp.Formats.Png.Chunks;

internal readonly struct AnimationControl
{
	public const int Size = 8;

	public uint NumberFrames { get; }

	public uint NumberPlays { get; }

	public AnimationControl(uint numberFrames, uint numberPlays)
	{
		NumberFrames = numberFrames;
		NumberPlays = numberPlays;
	}

	public void WriteTo(Span<byte> buffer)
	{
		BinaryPrimitives.WriteInt32BigEndian(buffer.Slice(0, 4), (int)NumberFrames);
		BinaryPrimitives.WriteInt32BigEndian(buffer.Slice(4, 4), (int)NumberPlays);
	}

	public static AnimationControl Parse(ReadOnlySpan<byte> data)
	{
		return new AnimationControl(BinaryPrimitives.ReadUInt32BigEndian(data.Slice(0, 4)), BinaryPrimitives.ReadUInt32BigEndian(data.Slice(4, 4)));
	}
}
