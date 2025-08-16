using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

[Flags]
public enum ExifParts
{
	None = 0,
	IfdTags = 1,
	ExifTags = 2,
	GpsTags = 4,
	All = 7
}
