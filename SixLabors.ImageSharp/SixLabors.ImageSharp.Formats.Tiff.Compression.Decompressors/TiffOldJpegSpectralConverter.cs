using SixLabors.ImageSharp.Formats.Jpeg.Components;
using SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class TiffOldJpegSpectralConverter<TPixel> : SpectralConverter<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly TiffPhotometricInterpretation photometricInterpretation;

	public TiffOldJpegSpectralConverter(Configuration configuration, TiffPhotometricInterpretation photometricInterpretation)
		: base(configuration, (Size?)null)
	{
		this.photometricInterpretation = photometricInterpretation;
	}

	protected override JpegColorConverterBase GetColorConverter(JpegFrame frame, IRawJpegData jpegData)
	{
		return JpegColorConverterBase.GetConverter(GetJpegColorSpaceFromPhotometricInterpretation(photometricInterpretation), frame.Precision);
	}

	private static JpegColorSpace GetJpegColorSpaceFromPhotometricInterpretation(TiffPhotometricInterpretation interpretation)
	{
		return interpretation switch
		{
			TiffPhotometricInterpretation.Rgb => JpegColorSpace.YCbCr, 
			TiffPhotometricInterpretation.YCbCr => JpegColorSpace.YCbCr, 
			_ => throw new InvalidImageContentException($"Invalid tiff photometric interpretation for jpeg encoding: {interpretation}"), 
		};
	}
}
