namespace Avalonia.Media.TextFormatting;

public sealed class GenericTextParagraphProperties : TextParagraphProperties
{
	private FlowDirection _flowDirection;

	private TextAlignment _textAlignment;

	private TextWrapping _textWrap;

	private double _lineHeight;

	public override FlowDirection FlowDirection => _flowDirection;

	public override TextAlignment TextAlignment => _textAlignment;

	public override double LineHeight => _lineHeight;

	public override bool FirstLineInParagraph { get; }

	public override bool AlwaysCollapsible { get; }

	public override TextRunProperties DefaultTextRunProperties { get; }

	public override TextWrapping TextWrapping => _textWrap;

	public override double Indent { get; }

	public override double LetterSpacing { get; }

	public GenericTextParagraphProperties(TextRunProperties defaultTextRunProperties, TextAlignment textAlignment = TextAlignment.Left, TextWrapping textWrap = TextWrapping.NoWrap, double lineHeight = 0.0, double letterSpacing = 0.0)
	{
		DefaultTextRunProperties = defaultTextRunProperties;
		_textAlignment = textAlignment;
		_textWrap = textWrap;
		_lineHeight = lineHeight;
		LetterSpacing = letterSpacing;
	}

	public GenericTextParagraphProperties(FlowDirection flowDirection, TextAlignment textAlignment, bool firstLineInParagraph, bool alwaysCollapsible, TextRunProperties defaultTextRunProperties, TextWrapping textWrap, double lineHeight, double indent, double letterSpacing)
	{
		_flowDirection = flowDirection;
		_textAlignment = textAlignment;
		FirstLineInParagraph = firstLineInParagraph;
		AlwaysCollapsible = alwaysCollapsible;
		DefaultTextRunProperties = defaultTextRunProperties;
		_textWrap = textWrap;
		_lineHeight = lineHeight;
		LetterSpacing = letterSpacing;
		Indent = indent;
	}

	public GenericTextParagraphProperties(TextParagraphProperties textParagraphProperties)
		: this(textParagraphProperties.FlowDirection, textParagraphProperties.TextAlignment, textParagraphProperties.FirstLineInParagraph, textParagraphProperties.AlwaysCollapsible, textParagraphProperties.DefaultTextRunProperties, textParagraphProperties.TextWrapping, textParagraphProperties.LineHeight, textParagraphProperties.Indent, textParagraphProperties.LetterSpacing)
	{
	}

	internal void SetFlowDirection(FlowDirection flowDirection)
	{
		_flowDirection = flowDirection;
	}

	internal void SetTextAlignment(TextAlignment textAlignment)
	{
		_textAlignment = textAlignment;
	}

	internal void SetLineHeight(double lineHeight)
	{
		_lineHeight = lineHeight;
	}

	internal void SetTextWrapping(TextWrapping textWrap)
	{
		_textWrap = textWrap;
	}
}
