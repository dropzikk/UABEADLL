namespace SixLabors.ImageSharp.Formats.Webp;

public sealed class WebpConfigurationModule : IImageFormatConfigurationModule
{
	public void Configure(Configuration configuration)
	{
		configuration.ImageFormatsManager.SetDecoder(WebpFormat.Instance, WebpDecoder.Instance);
		configuration.ImageFormatsManager.SetEncoder(WebpFormat.Instance, new WebpEncoder());
		configuration.ImageFormatsManager.AddImageFormatDetector(new WebpImageFormatDetector());
	}
}
