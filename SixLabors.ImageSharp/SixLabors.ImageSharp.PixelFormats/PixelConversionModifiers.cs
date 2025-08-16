using System;

namespace SixLabors.ImageSharp.PixelFormats;

[Flags]
public enum PixelConversionModifiers
{
	None = 0,
	Scale = 1,
	Premultiply = 2,
	SRgbCompand = 4
}
