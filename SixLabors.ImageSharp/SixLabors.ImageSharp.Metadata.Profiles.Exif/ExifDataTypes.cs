using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal static class ExifDataTypes
{
	public static uint GetSize(ExifDataType dataType)
	{
		switch (dataType)
		{
		case ExifDataType.Byte:
		case ExifDataType.Ascii:
		case ExifDataType.SignedByte:
		case ExifDataType.Undefined:
			return 1u;
		case ExifDataType.Short:
		case ExifDataType.SignedShort:
			return 2u;
		case ExifDataType.Long:
		case ExifDataType.SignedLong:
		case ExifDataType.SingleFloat:
		case ExifDataType.Ifd:
			return 4u;
		case ExifDataType.Rational:
		case ExifDataType.SignedRational:
		case ExifDataType.DoubleFloat:
		case ExifDataType.Long8:
		case ExifDataType.SignedLong8:
		case ExifDataType.Ifd8:
			return 8u;
		default:
			throw new NotSupportedException(dataType.ToString());
		}
	}
}
