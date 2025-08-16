using System;

namespace SixLabors.ImageSharp.Formats.Gif;

public interface IGifExtension
{
	byte Label { get; }

	int ContentLength { get; }

	int WriteTo(Span<byte> buffer);
}
