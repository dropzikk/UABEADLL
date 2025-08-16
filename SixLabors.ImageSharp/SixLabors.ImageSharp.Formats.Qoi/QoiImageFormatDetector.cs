using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Formats.Qoi;

public class QoiImageFormatDetector : IImageFormatDetector
{
	public int HeaderSize => 14;

	public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
	{
		format = (IsSupportedFileFormat(header) ? QoiFormat.Instance : null);
		return format != null;
	}

	private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
	{
		if (header.Length >= HeaderSize)
		{
			return QoiConstants.Magic.SequenceEqual(header.Slice(0, 4));
		}
		return false;
	}
}
