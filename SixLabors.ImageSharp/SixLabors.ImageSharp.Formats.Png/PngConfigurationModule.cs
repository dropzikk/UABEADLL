namespace SixLabors.ImageSharp.Formats.Png;

public sealed class PngConfigurationModule : IImageFormatConfigurationModule
{
	public void Configure(Configuration configuration)
	{
		configuration.ImageFormatsManager.SetEncoder(PngFormat.Instance, new PngEncoder());
		configuration.ImageFormatsManager.SetDecoder(PngFormat.Instance, PngDecoder.Instance);
		configuration.ImageFormatsManager.AddImageFormatDetector(new PngImageFormatDetector());
	}
}
