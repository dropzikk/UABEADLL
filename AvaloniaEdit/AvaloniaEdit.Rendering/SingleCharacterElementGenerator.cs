using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

internal sealed class SingleCharacterElementGenerator : VisualLineElementGenerator, IBuiltinElementGenerator
{
	private sealed class SpaceTextElement : FormattedTextElement
	{
		public SpaceTextElement(TextLine textLine)
			: base(textLine, 1)
		{
		}

		public override int GetNextCaretPosition(int visualColumn, AvaloniaEdit.Document.LogicalDirection direction, CaretPositioningMode mode)
		{
			if (mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint)
			{
				return base.GetNextCaretPosition(visualColumn, direction, mode);
			}
			return -1;
		}

		public override bool IsWhitespace(int visualColumn)
		{
			return true;
		}
	}

	private sealed class TabTextElement : VisualLineElement
	{
		internal readonly TextLine Text;

		public TabTextElement(TextLine text)
			: base(2, 1)
		{
			Text = text;
		}

		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			if (startVisualColumn == base.VisualColumn)
			{
				return new TabGlyphRun(this, base.TextRunProperties);
			}
			if (startVisualColumn == base.VisualColumn + 1)
			{
				return new TextCharacters("\t".AsMemory(), base.TextRunProperties);
			}
			throw new ArgumentOutOfRangeException("startVisualColumn");
		}

		public override int GetNextCaretPosition(int visualColumn, AvaloniaEdit.Document.LogicalDirection direction, CaretPositioningMode mode)
		{
			if (mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint)
			{
				return base.GetNextCaretPosition(visualColumn, direction, mode);
			}
			return -1;
		}

		public override bool IsWhitespace(int visualColumn)
		{
			return true;
		}
	}

	private sealed class TabGlyphRun : DrawableTextRun
	{
		private readonly TabTextElement _element;

		public override TextRunProperties Properties { get; }

		public override double Baseline => _element.Text.Baseline;

		public override Size Size => default(Size);

		public TabGlyphRun(TabTextElement element, TextRunProperties properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			Properties = properties;
			_element = element;
		}

		public override void Draw(DrawingContext drawingContext, Point origin)
		{
			_element.Text.Draw(drawingContext, origin);
		}
	}

	private sealed class SpecialCharacterBoxElement : FormattedTextElement
	{
		public SpecialCharacterBoxElement(TextLine text)
			: base(text, 1)
		{
		}

		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			return new SpecialCharacterTextRun(this, base.TextRunProperties);
		}
	}

	internal sealed class SpecialCharacterTextRun : FormattedTextRun
	{
		private static readonly ISolidColorBrush DarkGrayBrush;

		internal const double BoxMargin = 3.0;

		public override Size Size
		{
			get
			{
				Size size = base.Size;
				return size.WithWidth(size.Width + 3.0);
			}
		}

		static SpecialCharacterTextRun()
		{
			DarkGrayBrush = new ImmutableSolidColorBrush(Color.FromArgb(200, 128, 128, 128));
		}

		public SpecialCharacterTextRun(FormattedTextElement element, TextRunProperties properties)
			: base(element, properties)
		{
		}

		public override void Draw(DrawingContext drawingContext, Point origin)
		{
			Point point = origin;
			point.Deconstruct(out var x, out var y);
			double num = x;
			double y2 = y;
			Point origin2 = new Point(num + 1.5, y2);
			Size.Deconstruct(out y, out x);
			double width = y;
			double height = x;
			drawingContext.FillRectangle(rect: new Rect(num, y2, width, height), brush: DarkGrayBrush, cornerRadius: 2.5f);
			base.Draw(drawingContext, origin2);
		}
	}

	public bool ShowSpaces { get; set; }

	public bool ShowTabs { get; set; }

	public bool ShowBoxForControlCharacters { get; set; }

	public SingleCharacterElementGenerator()
	{
		ShowSpaces = true;
		ShowTabs = true;
		ShowBoxForControlCharacters = true;
	}

	void IBuiltinElementGenerator.FetchOptions(TextEditorOptions options)
	{
		ShowSpaces = options.ShowSpaces;
		ShowTabs = options.ShowTabs;
		ShowBoxForControlCharacters = options.ShowBoxForControlCharacters;
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		DocumentLine lastDocumentLine = base.CurrentContext.VisualLine.LastDocumentLine;
		StringSegment text = base.CurrentContext.GetText(startOffset, lastDocumentLine.EndOffset - startOffset);
		for (int i = 0; i < text.Count; i++)
		{
			char c = text.Text[text.Offset + i];
			switch (c)
			{
			case ' ':
				if (ShowSpaces)
				{
					return startOffset + i;
				}
				break;
			case '\t':
				if (ShowTabs)
				{
					return startOffset + i;
				}
				break;
			default:
				if (ShowBoxForControlCharacters && char.IsControl(c))
				{
					return startOffset + i;
				}
				break;
			}
		}
		return -1;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		char charAt = base.CurrentContext.Document.GetCharAt(offset);
		if (ShowSpaces && charAt == ' ')
		{
			VisualLineElementTextRunProperties visualLineElementTextRunProperties = new VisualLineElementTextRunProperties(base.CurrentContext.GlobalTextRunProperties);
			visualLineElementTextRunProperties.SetForegroundBrush(base.CurrentContext.TextView.NonPrintableCharacterBrush);
			return new SpaceTextElement(base.CurrentContext.TextView.CachedElements.GetTextForNonPrintableCharacter(base.CurrentContext.TextView.Options.ShowSpacesGlyph, visualLineElementTextRunProperties));
		}
		if (ShowTabs && charAt == '\t')
		{
			VisualLineElementTextRunProperties visualLineElementTextRunProperties2 = new VisualLineElementTextRunProperties(base.CurrentContext.GlobalTextRunProperties);
			visualLineElementTextRunProperties2.SetForegroundBrush(base.CurrentContext.TextView.NonPrintableCharacterBrush);
			return new TabTextElement(base.CurrentContext.TextView.CachedElements.GetTextForNonPrintableCharacter(base.CurrentContext.TextView.Options.ShowTabsGlyph, visualLineElementTextRunProperties2));
		}
		if (ShowBoxForControlCharacters && char.IsControl(charAt))
		{
			VisualLineElementTextRunProperties visualLineElementTextRunProperties3 = new VisualLineElementTextRunProperties(base.CurrentContext.GlobalTextRunProperties);
			visualLineElementTextRunProperties3.SetForegroundBrush(Brushes.White);
			return new SpecialCharacterBoxElement(FormattedTextElement.PrepareText(TextFormatterFactory.Create(base.CurrentContext.TextView), TextUtilities.GetControlCharacterName(charAt), visualLineElementTextRunProperties3));
		}
		return null;
	}
}
