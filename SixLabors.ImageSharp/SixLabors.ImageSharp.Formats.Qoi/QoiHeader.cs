using System.Text;

namespace SixLabors.ImageSharp.Formats.Qoi;

internal readonly struct QoiHeader
{
	public byte[] Magic { get; }

	public uint Width { get; }

	public uint Height { get; }

	public QoiChannels Channels { get; }

	public QoiColorSpace ColorSpace { get; }

	public QoiHeader(uint width, uint height, QoiChannels channels, QoiColorSpace colorSpace)
	{
		Magic = Encoding.UTF8.GetBytes("qoif");
		Width = width;
		Height = height;
		Channels = channels;
		ColorSpace = colorSpace;
	}
}
