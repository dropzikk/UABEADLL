namespace SixLabors.ImageSharp.Formats.Jpeg;

public sealed class JpegConfigurationModule : IImageFormatConfigurationModule
{
	public void Configure(Configuration configuration)
	{
		configuration.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder());
		configuration.ImageFormatsManager.SetDecoder(JpegFormat.Instance, JpegDecoder.Instance);
		configuration.ImageFormatsManager.AddImageFormatDetector(new JpegImageFormatDetector());
	}
}
