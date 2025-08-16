namespace Avalonia.Media.TextFormatting;

public abstract class TextParagraphProperties
{
	public abstract FlowDirection FlowDirection { get; }

	public abstract TextAlignment TextAlignment { get; }

	public abstract double LineHeight { get; }

	public abstract bool FirstLineInParagraph { get; }

	public virtual bool AlwaysCollapsible => false;

	public abstract TextRunProperties DefaultTextRunProperties { get; }

	public virtual TextDecorationCollection? TextDecorations => null;

	public abstract TextWrapping TextWrapping { get; }

	public abstract double Indent { get; }

	public virtual double ParagraphIndent => 0.0;

	public virtual double DefaultIncrementalTab => 0.0;

	public virtual double LetterSpacing { get; }
}
