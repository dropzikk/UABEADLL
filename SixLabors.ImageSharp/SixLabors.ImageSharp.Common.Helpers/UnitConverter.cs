using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace SixLabors.ImageSharp.Common.Helpers;

internal static class UnitConverter
{
	private const double CmsInMeter = 100.0;

	private const double CmsInInch = 2.54;

	private const double InchesInMeter = 39.37007874015748;

	private const PixelResolutionUnit DefaultResolutionUnit = PixelResolutionUnit.PixelsPerInch;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double CmToMeter(double x)
	{
		return x * 100.0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double MeterToCm(double x)
	{
		return x / 100.0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double MeterToInch(double x)
	{
		return x / 39.37007874015748;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double InchToMeter(double x)
	{
		return x * 39.37007874015748;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double CmToInch(double x)
	{
		return x / 2.54;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double InchToCm(double x)
	{
		return x * 2.54;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PixelResolutionUnit ExifProfileToResolutionUnit(ExifProfile profile)
	{
		if (profile.TryGetValue(ExifTag.ResolutionUnit, out IExifValue<ushort> exifValue))
		{
			return (PixelResolutionUnit)(exifValue.Value - 1);
		}
		return PixelResolutionUnit.PixelsPerInch;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ExifResolutionValues GetExifResolutionValues(PixelResolutionUnit unit, double horizontal, double vertical)
	{
		switch (unit)
		{
		case PixelResolutionUnit.PixelsPerMeter:
			unit = PixelResolutionUnit.PixelsPerCentimeter;
			horizontal = MeterToCm(horizontal);
			vertical = MeterToCm(vertical);
			break;
		default:
			unit = PixelResolutionUnit.PixelsPerInch;
			break;
		case PixelResolutionUnit.AspectRatio:
		case PixelResolutionUnit.PixelsPerInch:
		case PixelResolutionUnit.PixelsPerCentimeter:
			break;
		}
		ushort resolutionUnit = (ushort)(unit + 1);
		if (unit == PixelResolutionUnit.AspectRatio)
		{
			return new ExifResolutionValues(resolutionUnit, null, null);
		}
		return new ExifResolutionValues(resolutionUnit, horizontal, vertical);
	}
}
