using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Formats.Bmp;

public sealed class BmpImageFormatDetector : IImageFormatDetector
{
	public int HeaderSize => 2;

	public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
	{
		format = (IsSupportedFileFormat(header) ? BmpFormat.Instance : null);
		return format != null;
	}

	private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
	{
		if (header.Length >= HeaderSize)
		{
			short num = BinaryPrimitives.ReadInt16LittleEndian(header);
			if (num != 19778)
			{
				return num == 16706;
			}
			return true;
		}
		return false;
	}
}
