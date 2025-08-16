using System;

namespace SixLabors.ImageSharp.Formats.Png;

[Flags]
public enum PngChunkFilter
{
	None = 0,
	ExcludePhysicalChunk = 1,
	ExcludeGammaChunk = 2,
	ExcludeExifChunk = 4,
	ExcludeTextChunks = 8,
	ExcludeAll = -1
}
