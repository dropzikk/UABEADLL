namespace SixLabors.ImageSharp.Formats.Pbm;

public sealed class PbmConfigurationModule : IImageFormatConfigurationModule
{
	public void Configure(Configuration configuration)
	{
		configuration.ImageFormatsManager.SetEncoder(PbmFormat.Instance, new PbmEncoder());
		configuration.ImageFormatsManager.SetDecoder(PbmFormat.Instance, PbmDecoder.Instance);
		configuration.ImageFormatsManager.AddImageFormatDetector(new PbmImageFormatDetector());
	}
}
