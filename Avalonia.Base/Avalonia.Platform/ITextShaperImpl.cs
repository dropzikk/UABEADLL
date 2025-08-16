using System;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface ITextShaperImpl
{
	ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options);
}
