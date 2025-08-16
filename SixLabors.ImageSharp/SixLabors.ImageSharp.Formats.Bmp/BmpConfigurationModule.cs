namespace SixLabors.ImageSharp.Formats.Bmp;

public sealed class BmpConfigurationModule : IImageFormatConfigurationModule
{
	public void Configure(Configuration configuration)
	{
		configuration.ImageFormatsManager.SetEncoder(BmpFormat.Instance, new BmpEncoder());
		configuration.ImageFormatsManager.SetDecoder(BmpFormat.Instance, BmpDecoder.Instance);
		configuration.ImageFormatsManager.AddImageFormatDetector(new BmpImageFormatDetector());
	}
}
