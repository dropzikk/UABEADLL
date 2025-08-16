namespace SixLabors.ImageSharp.Formats.Gif;

public sealed class GifConfigurationModule : IImageFormatConfigurationModule
{
	public void Configure(Configuration configuration)
	{
		configuration.ImageFormatsManager.SetEncoder(GifFormat.Instance, new GifEncoder());
		configuration.ImageFormatsManager.SetDecoder(GifFormat.Instance, GifDecoder.Instance);
		configuration.ImageFormatsManager.AddImageFormatDetector(new GifImageFormatDetector());
	}
}
