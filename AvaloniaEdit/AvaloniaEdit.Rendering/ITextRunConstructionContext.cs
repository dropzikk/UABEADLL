using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

public interface ITextRunConstructionContext
{
	TextDocument Document { get; }

	TextView TextView { get; }

	VisualLine VisualLine { get; }

	TextRunProperties GlobalTextRunProperties { get; }

	StringSegment GetText(int offset, int length);
}
