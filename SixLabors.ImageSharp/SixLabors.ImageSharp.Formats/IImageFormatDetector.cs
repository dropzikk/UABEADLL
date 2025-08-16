using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Formats;

public interface IImageFormatDetector
{
	int HeaderSize { get; }

	bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format);
}
