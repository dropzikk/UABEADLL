using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

internal static class TransformProcessorHelpers
{
	public static void UpdateDimensionalMetadata<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		ExifProfile exifProfile = image.Metadata.ExifProfile;
		if (exifProfile != null)
		{
			if (exifProfile.TryGetValue(ExifTag.PixelXDimension, out IExifValue<Number> exifValue))
			{
				exifProfile.SetValue(ExifTag.PixelXDimension, image.Width);
			}
			if (exifProfile.TryGetValue(ExifTag.PixelYDimension, out exifValue))
			{
				exifProfile.SetValue(ExifTag.PixelYDimension, image.Height);
			}
		}
	}
}
