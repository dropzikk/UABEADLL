using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Formats.Tga;

public sealed class TgaImageFormatDetector : IImageFormatDetector
{
	public int HeaderSize => 16;

	public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
	{
		format = (IsSupportedFileFormat(header) ? TgaFormat.Instance : null);
		return format != null;
	}

	private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
	{
		if (header.Length >= HeaderSize)
		{
			if (header[1] != 0 && header[1] != 1)
			{
				return false;
			}
			if (!((TgaImageType)header[2]).IsValid())
			{
				return false;
			}
			if (header[1] == 0 && (header[3] != 0 || header[4] != 0 || header[5] != 0 || header[6] != 0 || header[7] != 0))
			{
				return false;
			}
			if ((header[12] == 0 && header[13] == 0) || (header[14] == 0 && header[15] == 0))
			{
				return false;
			}
			return true;
		}
		return false;
	}
}
