using System;

namespace SixLabors.ImageSharp;

internal interface IComponentShuffle
{
	void ShuffleReduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest);

	void RunFallbackShuffle(ReadOnlySpan<byte> source, Span<byte> dest);
}
