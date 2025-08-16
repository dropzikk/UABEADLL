using System;
using Avalonia.Collections;

namespace Avalonia.Styling;

[Obsolete("This interface may be removed in 12.0. Use StyledElement, or override StyledElement.StyleKeyOverride to override the StyleKey for a class.")]
public interface IStyleable : INamed
{
	IAvaloniaReadOnlyList<string> Classes { get; }

	[Obsolete("Override StyledElement.StyleKeyOverride instead.")]
	Type StyleKey { get; }

	AvaloniaObject? TemplatedParent { get; }
}
