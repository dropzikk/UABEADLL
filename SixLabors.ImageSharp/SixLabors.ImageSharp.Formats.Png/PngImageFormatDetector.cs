using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Formats.Png;

public sealed class PngImageFormatDetector : IImageFormatDetector
{
	public int HeaderSize => 8;

	public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
	{
		format = (IsSupportedFileFormat(header) ? PngFormat.Instance : null);
		return format != null;
	}

	private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
	{
		if (header.Length >= HeaderSize)
		{
			return BinaryPrimitives.ReadUInt64BigEndian(header) == 9894494448401390090uL;
		}
		return false;
	}
}
