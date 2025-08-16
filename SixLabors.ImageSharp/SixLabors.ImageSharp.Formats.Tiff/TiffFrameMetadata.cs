using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace SixLabors.ImageSharp.Formats.Tiff;

public class TiffFrameMetadata : IDeepCloneable
{
	public TiffBitsPerPixel? BitsPerPixel { get; set; }

	public TiffBitsPerSample? BitsPerSample { get; set; }

	public TiffCompression? Compression { get; set; }

	public TiffPhotometricInterpretation? PhotometricInterpretation { get; set; }

	public TiffPredictor? Predictor { get; set; }

	public TiffInkSet? InkSet { get; set; }

	public TiffFrameMetadata()
	{
	}

	private TiffFrameMetadata(TiffFrameMetadata other)
	{
		BitsPerPixel = other.BitsPerPixel;
		Compression = other.Compression;
		PhotometricInterpretation = other.PhotometricInterpretation;
		Predictor = other.Predictor;
		InkSet = other.InkSet;
	}

	internal static TiffFrameMetadata Parse(ExifProfile profile)
	{
		TiffFrameMetadata tiffFrameMetadata = new TiffFrameMetadata();
		Parse(tiffFrameMetadata, profile);
		return tiffFrameMetadata;
	}

	internal static void Parse(TiffFrameMetadata meta, ExifProfile profile)
	{
		if (profile != null)
		{
			if (profile.TryGetValue(ExifTag.BitsPerSample, out IExifValue<ushort[]> exifValue) && TiffBitsPerSample.TryParse(exifValue.Value, out var sample))
			{
				meta.BitsPerSample = sample;
			}
			meta.BitsPerPixel = meta.BitsPerSample?.BitsPerPixel();
			if (profile.TryGetValue(ExifTag.Compression, out IExifValue<ushort> exifValue2))
			{
				meta.Compression = (TiffCompression)exifValue2.Value;
			}
			if (profile.TryGetValue(ExifTag.PhotometricInterpretation, out IExifValue<ushort> exifValue3))
			{
				meta.PhotometricInterpretation = (TiffPhotometricInterpretation)exifValue3.Value;
			}
			if (profile.TryGetValue(ExifTag.Predictor, out IExifValue<ushort> exifValue4))
			{
				meta.Predictor = (TiffPredictor)exifValue4.Value;
			}
			if (profile.TryGetValue(ExifTag.InkSet, out IExifValue<ushort> exifValue5))
			{
				meta.InkSet = (TiffInkSet)exifValue5.Value;
			}
			profile.RemoveValue(ExifTag.BitsPerSample);
			profile.RemoveValue(ExifTag.Compression);
			profile.RemoveValue(ExifTag.PhotometricInterpretation);
			profile.RemoveValue(ExifTag.Predictor);
		}
	}

	public IDeepCloneable DeepClone()
	{
		return new TiffFrameMetadata(this);
	}
}
