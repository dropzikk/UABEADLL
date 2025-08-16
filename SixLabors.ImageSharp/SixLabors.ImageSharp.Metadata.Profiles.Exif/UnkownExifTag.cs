namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class UnkownExifTag : ExifTag
{
	internal UnkownExifTag(ExifTagValue value)
		: base((ushort)value)
	{
	}
}
