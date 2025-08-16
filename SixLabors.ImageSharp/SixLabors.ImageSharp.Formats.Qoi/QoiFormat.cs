using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Qoi;

public sealed class QoiFormat : IImageFormat<QoiMetadata>, IImageFormat
{
	public static QoiFormat Instance { get; } = new QoiFormat();

	public string DefaultMimeType => "image/qoi";

	public string Name => "QOI";

	public IEnumerable<string> MimeTypes => QoiConstants.MimeTypes;

	public IEnumerable<string> FileExtensions => QoiConstants.FileExtensions;

	private QoiFormat()
	{
	}

	public QoiMetadata CreateDefaultFormatMetadata()
	{
		return new QoiMetadata();
	}
}
