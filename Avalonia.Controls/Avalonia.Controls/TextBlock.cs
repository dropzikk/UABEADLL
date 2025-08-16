using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;

namespace Avalonia.Controls;

[DebuggerDisplay("Text = {DebugText}")]
public class TextBlock : Control, IInlineHost, ILogical
{
	private readonly record struct SimpleTextSource(string text, TextRunProperties defaultProperties) : ITextSource
	{
		private readonly string _text = text;

		private readonly TextRunProperties _defaultProperties = defaultProperties;

		public TextRun? GetTextRun(int textSourceIndex)
		{
			if (textSourceIndex > text.Length)
			{
				return new TextEndOfParagraph();
			}
			ReadOnlyMemory<char> text = text.AsMemory(textSourceIndex);
			if (text.IsEmpty)
			{
				return new TextEndOfParagraph();
			}
			return new TextCharacters(text, defaultProperties);
		}

		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}
	}

	private readonly struct InlinesTextSource : ITextSource
	{
		private readonly IReadOnlyList<TextRun> _textRuns;

		public IReadOnlyList<TextRun> TextRuns => _textRuns;

		public InlinesTextSource(IReadOnlyList<TextRun> textRuns)
		{
			_textRuns = textRuns;
		}

		public TextRun? GetTextRun(int textSourceIndex)
		{
			int num = 0;
			foreach (TextRun textRun in _textRuns)
			{
				if (textRun.Length == 0)
				{
					continue;
				}
				if (textSourceIndex >= num + textRun.Length)
				{
					num += textRun.Length;
					continue;
				}
				if (textRun is TextCharacters)
				{
					int start = Math.Max(0, textSourceIndex - num);
					return new TextCharacters(textRun.Text.Slice(start), textRun.Properties);
				}
				return textRun;
			}
			return new TextEndOfParagraph();
		}
	}

	public static readonly StyledProperty<IBrush?> BackgroundProperty;

	public static readonly StyledProperty<Thickness> PaddingProperty;

	public static readonly StyledProperty<FontFamily> FontFamilyProperty;

	public static readonly StyledProperty<double> FontSizeProperty;

	public static readonly StyledProperty<FontStyle> FontStyleProperty;

	public static readonly StyledProperty<FontWeight> FontWeightProperty;

	public static readonly StyledProperty<FontStretch> FontStretchProperty;

	public static readonly StyledProperty<IBrush?> ForegroundProperty;

	public static readonly AttachedProperty<double> BaselineOffsetProperty;

	public static readonly AttachedProperty<double> LineHeightProperty;

	public static readonly AttachedProperty<double> LetterSpacingProperty;

	public static readonly AttachedProperty<int> MaxLinesProperty;

	public static readonly StyledProperty<string?> TextProperty;

	public static readonly AttachedProperty<TextAlignment> TextAlignmentProperty;

	public static readonly AttachedProperty<TextWrapping> TextWrappingProperty;

	public static readonly AttachedProperty<TextTrimming> TextTrimmingProperty;

	public static readonly StyledProperty<TextDecorationCollection?> TextDecorationsProperty;

	public static readonly DirectProperty<TextBlock, InlineCollection?> InlinesProperty;

	private TextLayout? _textLayout;

	private Size _constraint;

	private IReadOnlyList<TextRun>? _textRuns;

	private InlineCollection? _inlines;

	private bool _clearTextInternal;

	public TextLayout TextLayout => _textLayout ?? (_textLayout = CreateTextLayout(Text));

	public Thickness Padding
	{
		get
		{
			return GetValue(PaddingProperty);
		}
		set
		{
			SetValue(PaddingProperty, value);
		}
	}

	public IBrush? Background
	{
		get
		{
			return GetValue(BackgroundProperty);
		}
		set
		{
			SetValue(BackgroundProperty, value);
		}
	}

	public string? Text
	{
		get
		{
			return GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	private string? DebugText
	{
		get
		{
			string? text = Text;
			if (text == null)
			{
				InlineCollection? inlines = Inlines;
				if (inlines == null)
				{
					return null;
				}
				text = inlines.Text;
			}
			return text;
		}
	}

	public FontFamily FontFamily
	{
		get
		{
			return GetValue(FontFamilyProperty);
		}
		set
		{
			SetValue(FontFamilyProperty, value);
		}
	}

	public double FontSize
	{
		get
		{
			return GetValue(FontSizeProperty);
		}
		set
		{
			SetValue(FontSizeProperty, value);
		}
	}

	public FontStyle FontStyle
	{
		get
		{
			return GetValue(FontStyleProperty);
		}
		set
		{
			SetValue(FontStyleProperty, value);
		}
	}

	public FontWeight FontWeight
	{
		get
		{
			return GetValue(FontWeightProperty);
		}
		set
		{
			SetValue(FontWeightProperty, value);
		}
	}

	public FontStretch FontStretch
	{
		get
		{
			return GetValue(FontStretchProperty);
		}
		set
		{
			SetValue(FontStretchProperty, value);
		}
	}

	public IBrush? Foreground
	{
		get
		{
			return GetValue(ForegroundProperty);
		}
		set
		{
			SetValue(ForegroundProperty, value);
		}
	}

	public double LineHeight
	{
		get
		{
			return GetValue(LineHeightProperty);
		}
		set
		{
			SetValue(LineHeightProperty, value);
		}
	}

	public double LetterSpacing
	{
		get
		{
			return GetValue(LetterSpacingProperty);
		}
		set
		{
			SetValue(LetterSpacingProperty, value);
		}
	}

	public int MaxLines
	{
		get
		{
			return GetValue(MaxLinesProperty);
		}
		set
		{
			SetValue(MaxLinesProperty, value);
		}
	}

	public TextWrapping TextWrapping
	{
		get
		{
			return GetValue(TextWrappingProperty);
		}
		set
		{
			SetValue(TextWrappingProperty, value);
		}
	}

	public TextTrimming TextTrimming
	{
		get
		{
			return GetValue(TextTrimmingProperty);
		}
		set
		{
			SetValue(TextTrimmingProperty, value);
		}
	}

	public TextAlignment TextAlignment
	{
		get
		{
			return GetValue(TextAlignmentProperty);
		}
		set
		{
			SetValue(TextAlignmentProperty, value);
		}
	}

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

	[Content]
	public InlineCollection? Inlines
	{
		get
		{
			return _inlines;
		}
		set
		{
			SetAndRaise(InlinesProperty, ref _inlines, value);
		}
	}

	protected override bool BypassFlowDirectionPolicies => true;

	internal bool HasComplexContent
	{
		get
		{
			if (Inlines != null)
			{
				return Inlines.Count > 0;
			}
			return false;
		}
	}

	public double BaselineOffset
	{
		get
		{
			return GetValue(BaselineOffsetProperty);
		}
		set
		{
			SetValue(BaselineOffsetProperty, value);
		}
	}

	static TextBlock()
	{
		BackgroundProperty = Border.BackgroundProperty.AddOwner<TextBlock>();
		PaddingProperty = Decorator.PaddingProperty.AddOwner<TextBlock>();
		FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner<TextBlock>();
		FontSizeProperty = TextElement.FontSizeProperty.AddOwner<TextBlock>();
		FontStyleProperty = TextElement.FontStyleProperty.AddOwner<TextBlock>();
		FontWeightProperty = TextElement.FontWeightProperty.AddOwner<TextBlock>();
		FontStretchProperty = TextElement.FontStretchProperty.AddOwner<TextBlock>();
		ForegroundProperty = TextElement.ForegroundProperty.AddOwner<TextBlock>();
		BaselineOffsetProperty = AvaloniaProperty.RegisterAttached<TextBlock, Control, double>("BaselineOffset", 0.0, inherits: true);
		LineHeightProperty = AvaloniaProperty.RegisterAttached<TextBlock, Control, double>("LineHeight", double.NaN, inherits: true, BindingMode.OneWay, IsValidLineHeight);
		LetterSpacingProperty = AvaloniaProperty.RegisterAttached<TextBlock, Control, double>("LetterSpacing", 0.0, inherits: true);
		MaxLinesProperty = AvaloniaProperty.RegisterAttached<TextBlock, Control, int>("MaxLines", 0, inherits: true, BindingMode.OneWay, IsValidMaxLines);
		TextProperty = AvaloniaProperty.Register<TextBlock, string>("Text");
		TextAlignmentProperty = AvaloniaProperty.RegisterAttached<TextBlock, Control, TextAlignment>("TextAlignment", TextAlignment.Start, inherits: true);
		TextWrappingProperty = AvaloniaProperty.RegisterAttached<TextBlock, Control, TextWrapping>("TextWrapping", TextWrapping.NoWrap, inherits: true);
		TextTrimmingProperty = AvaloniaProperty.RegisterAttached<TextBlock, Control, TextTrimming>("TextTrimming", Avalonia.Media.TextTrimming.None, inherits: true);
		TextDecorationsProperty = AvaloniaProperty.Register<TextBlock, TextDecorationCollection>("TextDecorations");
		InlinesProperty = AvaloniaProperty.RegisterDirect("Inlines", (TextBlock t) => t.Inlines, delegate(TextBlock t, InlineCollection? v)
		{
			t.Inlines = v;
		});
		Visual.ClipToBoundsProperty.OverrideDefaultValue<TextBlock>(defaultValue: true);
		Visual.AffectsRender<TextBlock>(new AvaloniaProperty[2] { BackgroundProperty, ForegroundProperty });
	}

	public TextBlock()
	{
		Inlines = new InlineCollection
		{
			LogicalChildren = base.LogicalChildren,
			InlineHost = this
		};
	}

	public static double GetBaselineOffset(Control control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		return control.GetValue(BaselineOffsetProperty);
	}

	public static void SetBaselineOffset(Control control, double value)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		control.SetValue(BaselineOffsetProperty, value);
	}

	public static TextAlignment GetTextAlignment(Control control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		return control.GetValue(TextAlignmentProperty);
	}

	public static void SetTextAlignment(Control control, TextAlignment alignment)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		control.SetValue(TextAlignmentProperty, alignment);
	}

	public static TextWrapping GetTextWrapping(Control control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		return control.GetValue(TextWrappingProperty);
	}

	public static void SetTextWrapping(Control control, TextWrapping wrapping)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		control.SetValue(TextWrappingProperty, wrapping);
	}

	public static TextTrimming GetTextTrimming(Control control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		return control.GetValue(TextTrimmingProperty);
	}

	public static void SetTextTrimming(Control control, TextTrimming trimming)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		control.SetValue(TextTrimmingProperty, trimming);
	}

	public static double GetLineHeight(Control control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		return control.GetValue(LineHeightProperty);
	}

	public static void SetLineHeight(Control control, double height)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		control.SetValue(LineHeightProperty, height);
	}

	public static double GetLetterSpacing(Control control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		return control.GetValue(LetterSpacingProperty);
	}

	public static void SetLetterSpacing(Control control, double letterSpacing)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		control.SetValue(LetterSpacingProperty, letterSpacing);
	}

	public static int GetMaxLines(Control control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		return control.GetValue(MaxLinesProperty);
	}

	public static void SetMaxLines(Control control, int maxLines)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		control.SetValue(MaxLinesProperty, maxLines);
	}

	public sealed override void Render(DrawingContext context)
	{
		RenderCore(context);
	}

	private protected virtual void RenderCore(DrawingContext context)
	{
		IBrush background = Background;
		if (background != null)
		{
			context.FillRectangle(background, new Rect(base.Bounds.Size));
		}
		double layoutScale = LayoutHelper.GetLayoutScale(this);
		Thickness thickness = LayoutHelper.RoundLayoutThickness(Padding, layoutScale, layoutScale);
		double num = thickness.Top;
		double height = TextLayout.Height;
		if (base.Bounds.Height < height)
		{
			switch (base.VerticalAlignment)
			{
			case VerticalAlignment.Center:
				num += (base.Bounds.Height - height) / 2.0;
				break;
			case VerticalAlignment.Bottom:
				num += base.Bounds.Height - height;
				break;
			}
		}
		RenderTextLayout(context, new Point(thickness.Left, num));
	}

	protected virtual void RenderTextLayout(DrawingContext context, Point origin)
	{
		TextLayout.Draw(context, origin + new Point(TextLayout.OverhangLeading, 0.0));
	}

	internal void ClearTextInternal()
	{
		_clearTextInternal = true;
		try
		{
			SetCurrentValue(TextProperty, null);
		}
		finally
		{
			_clearTextInternal = false;
		}
	}

	protected virtual TextLayout CreateTextLayout(string? text)
	{
		GenericTextRunProperties genericTextRunProperties = new GenericTextRunProperties(new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, TextDecorations, Foreground);
		GenericTextParagraphProperties paragraphProperties = new GenericTextParagraphProperties(base.FlowDirection, TextAlignment, firstLineInParagraph: true, alwaysCollapsible: false, genericTextRunProperties, TextWrapping, LineHeight, 0.0, LetterSpacing);
		ITextSource textSource = (ITextSource)((_textRuns == null) ? ((object)new SimpleTextSource(text ?? "", genericTextRunProperties)) : ((object)new InlinesTextSource(_textRuns)));
		return new TextLayout(textSource, paragraphProperties, TextTrimming, _constraint.Width, _constraint.Height, MaxLines);
	}

	protected void InvalidateTextLayout()
	{
		_textLayout?.Dispose();
		_textLayout = null;
		InvalidateVisual();
		InvalidateMeasure();
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		double layoutScale = LayoutHelper.GetLayoutScale(this);
		Thickness thickness = LayoutHelper.RoundLayoutThickness(Padding, layoutScale, layoutScale);
		_constraint = availableSize.Deflate(thickness);
		_textLayout?.Dispose();
		_textLayout = null;
		InlineCollection inlines = Inlines;
		if (HasComplexContent)
		{
			base.VisualChildren.Clear();
			List<TextRun> textRuns = new List<TextRun>();
			foreach (Inline item in inlines)
			{
				item.BuildTextRun(textRuns);
			}
			_textRuns = textRuns;
			foreach (TextRun textRun in _textRuns)
			{
				if (textRun is EmbeddedControlRun embeddedControlRun)
				{
					Control control = embeddedControlRun.Control;
					if (control != null)
					{
						base.VisualChildren.Add(control);
						control.Measure(Size.Infinity);
					}
				}
			}
		}
		return new Size(TextLayout.OverhangLeading + TextLayout.WidthIncludingTrailingWhitespace + TextLayout.OverhangTrailing, TextLayout.Height).Inflate(thickness);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		double layoutScale = LayoutHelper.GetLayoutScale(this);
		Thickness thickness = LayoutHelper.RoundLayoutThickness(Padding, layoutScale, layoutScale);
		if (finalSize.Width < _constraint.Width)
		{
			_textLayout?.Dispose();
			_textLayout = null;
			_constraint = finalSize.Deflate(thickness);
		}
		if (HasComplexContent)
		{
			double num = thickness.Top;
			foreach (TextLine textLine in TextLayout.TextLines)
			{
				double num2 = thickness.Left + textLine.Start;
				foreach (TextRun textRun in textLine.TextRuns)
				{
					if (textRun is DrawableTextRun drawableTextRun)
					{
						if (drawableTextRun is EmbeddedControlRun embeddedControlRun)
						{
							Control control = embeddedControlRun.Control;
							control?.Arrange(new Rect(new Point(num2, num), new Size(control.DesiredSize.Width, textLine.Height)));
						}
						num2 += drawableTextRun.Size.Width;
					}
				}
				num += textLine.Height;
			}
		}
		return finalSize;
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new TextBlockAutomationPeer(this);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == TextProperty && HasComplexContent && !_clearTextInternal)
		{
			Inlines?.Clear();
		}
		switch (change.Property.Name)
		{
		case "FontSize":
		case "FontWeight":
		case "FontStyle":
		case "FontFamily":
		case "FontStretch":
		case "TextWrapping":
		case "TextTrimming":
		case "TextAlignment":
		case "FlowDirection":
		case "Padding":
		case "LineHeight":
		case "LetterSpacing":
		case "MaxLines":
		case "Text":
		case "TextDecorations":
		case "Foreground":
			InvalidateTextLayout();
			break;
		case "Inlines":
			OnInlinesChanged(change.OldValue as InlineCollection, change.NewValue as InlineCollection);
			InvalidateTextLayout();
			break;
		}
	}

	private static bool IsValidMaxLines(int maxLines)
	{
		return maxLines >= 0;
	}

	private static bool IsValidLineHeight(double lineHeight)
	{
		if (!double.IsNaN(lineHeight))
		{
			return lineHeight > 0.0;
		}
		return true;
	}

	private void OnInlinesChanged(InlineCollection? oldValue, InlineCollection? newValue)
	{
		if (oldValue != null)
		{
			oldValue.LogicalChildren = null;
			oldValue.InlineHost = null;
			oldValue.Invalidated -= delegate
			{
				InvalidateTextLayout();
			};
		}
		if (newValue != null)
		{
			newValue.LogicalChildren = base.LogicalChildren;
			newValue.InlineHost = this;
			newValue.Invalidated += delegate
			{
				InvalidateTextLayout();
			};
		}
	}

	void IInlineHost.Invalidate()
	{
		InvalidateTextLayout();
	}
}
