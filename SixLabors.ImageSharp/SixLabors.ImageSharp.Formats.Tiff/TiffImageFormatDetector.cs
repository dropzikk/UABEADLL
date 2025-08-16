using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Formats.Tiff;

public sealed class TiffImageFormatDetector : IImageFormatDetector
{
	public int HeaderSize => 8;

	public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
	{
		format = (IsSupportedFileFormat(header) ? TiffFormat.Instance : null);
		return format != null;
	}

	private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
	{
		if (header.Length >= HeaderSize)
		{
			if (header[0] == 73 && header[1] == 73)
			{
				if (header[2] == 42 && header[3] == 0)
				{
					return true;
				}
				if (header[2] == 43 && header[3] == 0 && header[4] == 8 && header[5] == 0 && header[6] == 0 && header[7] == 0)
				{
					return true;
				}
			}
			else if (header[0] == 77 && header[1] == 77)
			{
				if (header[2] == 0 && header[3] == 42)
				{
					return true;
				}
				if (header[2] == 0 && header[3] == 43 && header[4] == 0 && header[5] == 8 && header[6] == 0 && header[7] == 0)
				{
					return true;
				}
			}
		}
		return false;
	}
}
