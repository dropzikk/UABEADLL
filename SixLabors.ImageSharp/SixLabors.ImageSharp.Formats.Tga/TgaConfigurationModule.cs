namespace SixLabors.ImageSharp.Formats.Tga;

public sealed class TgaConfigurationModule : IImageFormatConfigurationModule
{
	public void Configure(Configuration configuration)
	{
		configuration.ImageFormatsManager.SetEncoder(TgaFormat.Instance, new TgaEncoder());
		configuration.ImageFormatsManager.SetDecoder(TgaFormat.Instance, TgaDecoder.Instance);
		configuration.ImageFormatsManager.AddImageFormatDetector(new TgaImageFormatDetector());
	}
}
