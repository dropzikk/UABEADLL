using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Formats.Webp;

public sealed class WebpImageFormatDetector : IImageFormatDetector
{
	public int HeaderSize => 12;

	public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
	{
		format = (IsSupportedFileFormat(header) ? WebpFormat.Instance : null);
		return format != null;
	}

	private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
	{
		if (header.Length >= HeaderSize && IsRiffContainer(header))
		{
			return IsWebpFile(header);
		}
		return false;
	}

	private static bool IsRiffContainer(ReadOnlySpan<byte> header)
	{
		return header.Slice(0, 4).SequenceEqual(WebpConstants.RiffFourCc);
	}

	private static bool IsWebpFile(ReadOnlySpan<byte> header)
	{
		return header.Slice(8, 4).SequenceEqual(WebpConstants.WebpHeader);
	}
}
