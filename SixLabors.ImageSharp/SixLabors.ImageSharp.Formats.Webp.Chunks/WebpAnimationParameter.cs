using System;
using System.Buffers.Binary;
using System.IO;
using SixLabors.ImageSharp.Common.Helpers;

namespace SixLabors.ImageSharp.Formats.Webp.Chunks;

internal readonly struct WebpAnimationParameter
{
	public uint Background { get; }

	public ushort LoopCount { get; }

	public WebpAnimationParameter(uint background, ushort loopCount)
	{
		Background = background;
		LoopCount = loopCount;
	}

	public void WriteTo(Stream stream)
	{
		Span<byte> span = stackalloc byte[6];
		BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(0, 4), Background);
		BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(4, span.Length - 4), LoopCount);
		RiffHelper.WriteChunk(stream, 1095649613u, span);
	}
}
