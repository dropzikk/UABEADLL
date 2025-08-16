using System;

namespace SixLabors.ImageSharp.Formats.Tiff.Constants;

[Flags]
public enum TiffNewSubfileType : uint
{
	FullImage = 0u,
	Preview = 1u,
	SinglePage = 2u,
	TransparencyMask = 4u,
	AlternativePreview = 0x10000u,
	MixedRasterContent = 8u
}
