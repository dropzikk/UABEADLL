using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

[Flags]
public enum IccDeviceAttribute : long
{
	OpacityTransparent = 1L,
	OpacityReflective = 0L,
	ReflectivityMatte = 2L,
	ReflectivityGlossy = 0L,
	PolarityNegative = 4L,
	PolarityPositive = 0L,
	ChromaBlackWhite = 8L,
	ChromaColor = 0L
}
