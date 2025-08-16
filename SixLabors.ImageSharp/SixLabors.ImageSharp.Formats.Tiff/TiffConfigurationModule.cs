namespace SixLabors.ImageSharp.Formats.Tiff;

public sealed class TiffConfigurationModule : IImageFormatConfigurationModule
{
	public void Configure(Configuration configuration)
	{
		configuration.ImageFormatsManager.SetEncoder(TiffFormat.Instance, new TiffEncoder());
		configuration.ImageFormatsManager.SetDecoder(TiffFormat.Instance, TiffDecoder.Instance);
		configuration.ImageFormatsManager.AddImageFormatDetector(new TiffImageFormatDetector());
	}
}
