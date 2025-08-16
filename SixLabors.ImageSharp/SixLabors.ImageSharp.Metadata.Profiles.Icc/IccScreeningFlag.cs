using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

[Flags]
internal enum IccScreeningFlag
{
	None = 0,
	DefaultScreens = 1,
	NotDefaultScreens = 0,
	UnitLinesPerInch = 2,
	UnitLinesPerCm = 0
}
