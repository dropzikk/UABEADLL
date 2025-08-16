using SixLabors.ImageSharp.Formats.Jpeg.Components;
using SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class RgbJpegSpectralConverter<TPixel> : SpectralConverter<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public RgbJpegSpectralConverter(Configuration configuration)
		: base(configuration, (Size?)null)
	{
	}

	protected override JpegColorConverterBase GetColorConverter(JpegFrame frame, IRawJpegData jpegData)
	{
		return JpegColorConverterBase.GetConverter(JpegColorSpace.RGB, frame.Precision);
	}
}
