using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Formats.Pbm;

public sealed class PbmImageFormatDetector : IImageFormatDetector
{
	private const byte P = 80;

	private const byte Zero = 48;

	private const byte Seven = 55;

	public int HeaderSize => 2;

	public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
	{
		format = (IsSupportedFileFormat(header) ? PbmFormat.Instance : null);
		return format != null;
	}

	private static bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
	{
		if ((uint)header.Length > 1u)
		{
			if (header[0] == 80)
			{
				return (uint)(header[1] - 48 - 1) < 6u;
			}
			return false;
		}
		return false;
	}
}
