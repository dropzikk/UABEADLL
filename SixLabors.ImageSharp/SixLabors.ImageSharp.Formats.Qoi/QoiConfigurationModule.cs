namespace SixLabors.ImageSharp.Formats.Qoi;

public sealed class QoiConfigurationModule : IImageFormatConfigurationModule
{
	public void Configure(Configuration configuration)
	{
		configuration.ImageFormatsManager.SetDecoder(QoiFormat.Instance, QoiDecoder.Instance);
		configuration.ImageFormatsManager.SetEncoder(QoiFormat.Instance, new QoiEncoder());
		configuration.ImageFormatsManager.AddImageFormatDetector(new QoiImageFormatDetector());
	}
}
