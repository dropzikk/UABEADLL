using System;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression;

[Flags]
public enum FaxCompressionOptions : uint
{
	None = 0u,
	TwoDimensionalCoding = 1u,
	UncompressedMode = 2u,
	EolPadding = 4u
}
