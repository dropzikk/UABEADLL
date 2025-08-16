using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

internal sealed class VisualLineTextParagraphProperties : TextParagraphProperties
{
	internal TextRunProperties defaultTextRunProperties;

	internal TextWrapping textWrapping;

	internal double tabSize;

	internal double indent;

	internal bool firstLineInParagraph;

	public override double DefaultIncrementalTab => tabSize;

	public override FlowDirection FlowDirection => FlowDirection.LeftToRight;

	public override TextAlignment TextAlignment => TextAlignment.Left;

	public override double LineHeight => DefaultTextRunProperties.FontRenderingEmSize * 1.35;

	public override bool FirstLineInParagraph => firstLineInParagraph;

	public override TextRunProperties DefaultTextRunProperties => defaultTextRunProperties;

	public override TextWrapping TextWrapping => textWrapping;

	public override double Indent => indent;
}
