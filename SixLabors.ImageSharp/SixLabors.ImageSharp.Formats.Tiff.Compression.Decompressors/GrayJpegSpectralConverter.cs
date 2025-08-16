using SixLabors.ImageSharp.Formats.Jpeg.Components;
using SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class GrayJpegSpectralConverter<TPixel> : SpectralConverter<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public GrayJpegSpectralConverter(Configuration configuration)
		: base(configuration, (Size?)null)
	{
	}

	protected override JpegColorConverterBase GetColorConverter(JpegFrame frame, IRawJpegData jpegData)
	{
		return JpegColorConverterBase.GetConverter(JpegColorSpace.Grayscale, frame.Precision);
	}
}
