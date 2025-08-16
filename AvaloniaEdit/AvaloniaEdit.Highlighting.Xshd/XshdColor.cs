using Avalonia.Media;

namespace AvaloniaEdit.Highlighting.Xshd;

public class XshdColor : XshdElement
{
	public string Name { get; set; }

	public HighlightingBrush Foreground { get; set; }

	public HighlightingBrush Background { get; set; }

	public FontFamily FontFamily { get; set; }

	public int? FontSize { get; set; }

	public FontWeight? FontWeight { get; set; }

	public bool? Underline { get; set; }

	public bool? Strikethrough { get; set; }

	public FontStyle? FontStyle { get; set; }

	public string ExampleText { get; set; }

	public override object AcceptVisitor(IXshdVisitor visitor)
	{
		return visitor.VisitColor(this);
	}
}
