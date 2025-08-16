using System.Collections.Generic;
using System.Text;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace Avalonia.Controls.Documents;

public abstract class Inline : TextElement
{
	public static readonly StyledProperty<TextDecorationCollection?> TextDecorationsProperty = AvaloniaProperty.Register<Inline, TextDecorationCollection>("TextDecorations");

	public static readonly StyledProperty<BaselineAlignment> BaselineAlignmentProperty = AvaloniaProperty.Register<Inline, BaselineAlignment>("BaselineAlignment", BaselineAlignment.Baseline);

	public TextDecorationCollection? TextDecorations
	{
		get
		{
			return GetValue(TextDecorationsProperty);
		}
		set
		{
			SetValue(TextDecorationsProperty, value);
		}
	}

	public BaselineAlignment BaselineAlignment
	{
		get
		{
			return GetValue(BaselineAlignmentProperty);
		}
		set
		{
			SetValue(BaselineAlignmentProperty, value);
		}
	}

	internal abstract void BuildTextRun(IList<TextRun> textRuns);

	internal abstract void AppendText(StringBuilder stringBuilder);

	protected TextRunProperties CreateTextRunProperties()
	{
		TextDecorationCollection textDecorations = TextDecorations;
		IBrush background = base.Background;
		if (base.Parent is Inline inline)
		{
			if (textDecorations == null)
			{
				textDecorations = inline.TextDecorations;
			}
			if (background == null)
			{
				background = inline.Background;
			}
		}
		FontStyle style = base.FontStyle;
		if (base.Parent is Italic)
		{
			style = FontStyle.Italic;
		}
		FontWeight weight = base.FontWeight;
		if (base.Parent is Bold)
		{
			weight = FontWeight.Bold;
		}
		return new GenericTextRunProperties(new Typeface(base.FontFamily, style, weight), base.FontSize, textDecorations, base.Foreground, background, BaselineAlignment);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		string name = change.Property.Name;
		if (name == "TextDecorations" || name == "BaselineAlignment")
		{
			base.InlineHost?.Invalidate();
		}
	}
}
