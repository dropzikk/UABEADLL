using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

[Flags]
public enum IccProfileFlag
{
	None = 0,
	Embedded = 1,
	NotEmbedded = 0,
	NotIndependent = 2,
	Independent = 0
}
