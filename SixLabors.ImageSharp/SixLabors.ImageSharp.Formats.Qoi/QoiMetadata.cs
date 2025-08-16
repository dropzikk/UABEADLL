namespace SixLabors.ImageSharp.Formats.Qoi;

public class QoiMetadata : IDeepCloneable
{
	public QoiChannels Channels { get; set; }

	public QoiColorSpace ColorSpace { get; set; }

	public QoiMetadata()
	{
	}

	private QoiMetadata(QoiMetadata other)
	{
		Channels = other.Channels;
		ColorSpace = other.ColorSpace;
	}

	public IDeepCloneable DeepClone()
	{
		return new QoiMetadata(this);
	}
}
