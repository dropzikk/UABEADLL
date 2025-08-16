using System;
using System.Buffers.Binary;

namespace SixLabors.ImageSharp.Formats.Png.Chunks;

internal readonly struct FrameControl
{
	public const int Size = 26;

	public uint SequenceNumber { get; }

	public uint Width { get; }

	public uint Height { get; }

	public uint XOffset { get; }

	public uint YOffset { get; }

	public uint XMax => XOffset + Width;

	public uint YMax => YOffset + Height;

	public ushort DelayNumerator { get; }

	public ushort DelayDenominator { get; }

	public PngDisposalMethod DisposeOperation { get; }

	public PngBlendMethod BlendOperation { get; }

	public Rectangle Bounds => new Rectangle((int)XOffset, (int)YOffset, (int)Width, (int)Height);

	public FrameControl(uint width, uint height)
		: this(0u, width, height, 0u, 0u, 0, 0, PngDisposalMethod.DoNotDispose, PngBlendMethod.Source)
	{
	}

	public FrameControl(uint sequenceNumber, uint width, uint height, uint xOffset, uint yOffset, ushort delayNumerator, ushort delayDenominator, PngDisposalMethod disposeOperation, PngBlendMethod blendOperation)
	{
		SequenceNumber = sequenceNumber;
		Width = width;
		Height = height;
		XOffset = xOffset;
		YOffset = yOffset;
		DelayNumerator = delayNumerator;
		DelayDenominator = delayDenominator;
		DisposeOperation = disposeOperation;
		BlendOperation = blendOperation;
	}

	public void Validate(PngHeader header)
	{
		if (Width == 0)
		{
			PngThrowHelper.ThrowInvalidParameter(Width, "Expected > 0", "this.Width");
		}
		if (Height == 0)
		{
			PngThrowHelper.ThrowInvalidParameter(Height, "Expected > 0", "this.Height");
		}
		if (XMax > header.Width)
		{
			PngThrowHelper.ThrowInvalidParameter(XOffset, Width, "The x-offset plus width > PngHeader.Width", "this.XOffset", "this.XOffset");
		}
		if (YMax > header.Height)
		{
			PngThrowHelper.ThrowInvalidParameter(YOffset, Height, "The y-offset plus height > PngHeader.Height", "this.YOffset", "this.YOffset");
		}
	}

	public void WriteTo(Span<byte> buffer)
	{
		BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(0, 4), SequenceNumber);
		BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(4, 4), Width);
		BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(8, 4), Height);
		BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(12, 4), XOffset);
		BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(16, 4), YOffset);
		BinaryPrimitives.WriteUInt16BigEndian(buffer.Slice(20, 2), DelayNumerator);
		BinaryPrimitives.WriteUInt16BigEndian(buffer.Slice(22, 2), DelayDenominator);
		buffer[24] = (byte)DisposeOperation;
		buffer[25] = (byte)BlendOperation;
	}

	public static FrameControl Parse(ReadOnlySpan<byte> data)
	{
		return new FrameControl(BinaryPrimitives.ReadUInt32BigEndian(data.Slice(0, 4)), BinaryPrimitives.ReadUInt32BigEndian(data.Slice(4, 4)), BinaryPrimitives.ReadUInt32BigEndian(data.Slice(8, 4)), BinaryPrimitives.ReadUInt32BigEndian(data.Slice(12, 4)), BinaryPrimitives.ReadUInt32BigEndian(data.Slice(16, 4)), BinaryPrimitives.ReadUInt16BigEndian(data.Slice(20, 2)), BinaryPrimitives.ReadUInt16BigEndian(data.Slice(22, 2)), (PngDisposalMethod)data[24], (PngBlendMethod)data[25]);
	}
}
