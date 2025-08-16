using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Presenters;

public class TextPresenter : Control
{
	public static readonly StyledProperty<int> CaretIndexProperty;

	public static readonly StyledProperty<bool> RevealPasswordProperty;

	public static readonly StyledProperty<char> PasswordCharProperty;

	public static readonly StyledProperty<IBrush?> SelectionBrushProperty;

	public static readonly StyledProperty<IBrush?> SelectionForegroundBrushProperty;

	public static readonly StyledProperty<IBrush?> CaretBrushProperty;

	public static readonly StyledProperty<int> SelectionStartProperty;

	public static readonly StyledProperty<int> SelectionEndProperty;

	public static readonly StyledProperty<string?> TextProperty;

	public static readonly StyledProperty<string?> PreeditTextProperty;

	public static readonly StyledProperty<TextAlignment> TextAlignmentProperty;

	public static readonly StyledProperty<TextWrapping> TextWrappingProperty;

	public static readonly StyledProperty<double> LineHeightProperty;

	public static readonly StyledProperty<double> LetterSpacingProperty;

	public static readonly StyledProperty<IBrush?> BackgroundProperty;

	private readonly DispatcherTimer _caretTimer;

	private bool _caretBlink;

	private TextLayout? _textLayout;

	private Size _constraint;

	private CharacterHit _lastCharacterHit;

	private Rect _caretBounds;

	private Point _navigationPosition;

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

	[Content]
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

	public string? PreeditText
	{
		get
		{
			return GetValue(PreeditTextProperty);
		}
		set
		{
			SetValue(PreeditTextProperty, value);
		}
	}

	public FontFamily FontFamily
	{
		get
		{
			return TextElement.GetFontFamily(this);
		}
		set
		{
			TextElement.SetFontFamily(this, value);
		}
	}

	public double FontSize
	{
		get
		{
			return TextElement.GetFontSize(this);
		}
		set
		{
			TextElement.SetFontSize(this, value);
		}
	}

	public FontStyle FontStyle
	{
		get
		{
			return TextElement.GetFontStyle(this);
		}
		set
		{
			TextElement.SetFontStyle(this, value);
		}
	}

	public FontWeight FontWeight
	{
		get
		{
			return TextElement.GetFontWeight(this);
		}
		set
		{
			TextElement.SetFontWeight(this, value);
		}
	}

	public FontStretch FontStretch
	{
		get
		{
			return TextElement.GetFontStretch(this);
		}
		set
		{
			TextElement.SetFontStretch(this, value);
		}
	}

	public IBrush? Foreground
	{
		get
		{
			return TextElement.GetForeground(this);
		}
		set
		{
			TextElement.SetForeground(this, value);
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

	public TextLayout TextLayout
	{
		get
		{
			if (_textLayout != null)
			{
				return _textLayout;
			}
			_textLayout = CreateTextLayout();
			UpdateCaret(_lastCharacterHit, notify: false);
			return _textLayout;
		}
	}

	public int CaretIndex
	{
		get
		{
			return GetValue(CaretIndexProperty);
		}
		set
		{
			SetValue(CaretIndexProperty, value);
		}
	}

	public char PasswordChar
	{
		get
		{
			return GetValue(PasswordCharProperty);
		}
		set
		{
			SetValue(PasswordCharProperty, value);
		}
	}

	public bool RevealPassword
	{
		get
		{
			return GetValue(RevealPasswordProperty);
		}
		set
		{
			SetValue(RevealPasswordProperty, value);
		}
	}

	public IBrush? SelectionBrush
	{
		get
		{
			return GetValue(SelectionBrushProperty);
		}
		set
		{
			SetValue(SelectionBrushProperty, value);
		}
	}

	public IBrush? SelectionForegroundBrush
	{
		get
		{
			return GetValue(SelectionForegroundBrushProperty);
		}
		set
		{
			SetValue(SelectionForegroundBrushProperty, value);
		}
	}

	public IBrush? CaretBrush
	{
		get
		{
			return GetValue(CaretBrushProperty);
		}
		set
		{
			SetValue(CaretBrushProperty, value);
		}
	}

	public int SelectionStart
	{
		get
		{
			return GetValue(SelectionStartProperty);
		}
		set
		{
			SetValue(SelectionStartProperty, value);
		}
	}

	public int SelectionEnd
	{
		get
		{
			return GetValue(SelectionEndProperty);
		}
		set
		{
			SetValue(SelectionEndProperty, value);
		}
	}

	protected override bool BypassFlowDirectionPolicies => true;

	public event EventHandler? CaretBoundsChanged;

	static TextPresenter()
	{
		StyledProperty<int> caretIndexProperty = TextBox.CaretIndexProperty;
		Func<AvaloniaObject, int, int> coerce = TextBox.CoerceCaretIndex;
		CaretIndexProperty = caretIndexProperty.AddOwner<TextPresenter>(new StyledPropertyMetadata<int>(default(Optional<int>), BindingMode.Default, coerce));
		RevealPasswordProperty = AvaloniaProperty.Register<TextPresenter, bool>("RevealPassword", defaultValue: false);
		PasswordCharProperty = AvaloniaProperty.Register<TextPresenter, char>("PasswordChar", '\0');
		SelectionBrushProperty = AvaloniaProperty.Register<TextPresenter, IBrush>("SelectionBrush");
		SelectionForegroundBrushProperty = AvaloniaProperty.Register<TextPresenter, IBrush>("SelectionForegroundBrush");
		CaretBrushProperty = AvaloniaProperty.Register<TextPresenter, IBrush>("CaretBrush");
		StyledProperty<int> selectionStartProperty = TextBox.SelectionStartProperty;
		coerce = TextBox.CoerceCaretIndex;
		SelectionStartProperty = selectionStartProperty.AddOwner<TextPresenter>(new StyledPropertyMetadata<int>(default(Optional<int>), BindingMode.Default, coerce));
		StyledProperty<int> selectionEndProperty = TextBox.SelectionEndProperty;
		coerce = TextBox.CoerceCaretIndex;
		SelectionEndProperty = selectionEndProperty.AddOwner<TextPresenter>(new StyledPropertyMetadata<int>(default(Optional<int>), BindingMode.Default, coerce));
		TextProperty = TextBlock.TextProperty.AddOwner<TextPresenter>(new StyledPropertyMetadata<string>(string.Empty));
		PreeditTextProperty = AvaloniaProperty.Register<TextPresenter, string>("PreeditText");
		TextAlignmentProperty = TextBlock.TextAlignmentProperty.AddOwner<TextPresenter>();
		TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner<TextPresenter>();
		LineHeightProperty = TextBlock.LineHeightProperty.AddOwner<TextPresenter>();
		LetterSpacingProperty = TextBlock.LetterSpacingProperty.AddOwner<TextPresenter>();
		BackgroundProperty = Border.BackgroundProperty.AddOwner<TextPresenter>();
		Visual.AffectsRender<TextPresenter>(new AvaloniaProperty[3]
		{
			CaretBrushProperty,
			SelectionBrushProperty,
			TextElement.ForegroundProperty
		});
	}

	public TextPresenter()
	{
		_caretTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(500.0)
		};
		_caretTimer.Tick += CaretTimerTick;
	}

	private TextLayout CreateTextLayoutInternal(Size constraint, string? text, Typeface typeface, IReadOnlyList<ValueSpan<TextRunProperties>>? textStyleOverrides)
	{
		IBrush foreground = Foreground;
		double num = (MathUtilities.IsZero(constraint.Width) ? double.PositiveInfinity : constraint.Width);
		double num2 = (MathUtilities.IsZero(constraint.Height) ? double.PositiveInfinity : constraint.Height);
		double fontSize = FontSize;
		TextAlignment textAlignment = TextAlignment;
		TextWrapping textWrapping = TextWrapping;
		double maxWidth = num;
		double maxHeight = num2;
		return new TextLayout(text, typeface, fontSize, foreground, textAlignment, textWrapping, null, null, base.FlowDirection, maxWidth, maxHeight, LineHeight, LetterSpacing, 0, textStyleOverrides);
	}

	private void RenderInternal(DrawingContext context)
	{
		IBrush background = Background;
		if (background != null)
		{
			context.FillRectangle(background, new Rect(base.Bounds.Size));
		}
		double num = 0.0;
		double x = 0.0;
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
		TextLayout.Draw(context, new Point(x, num));
	}

	public sealed override void Render(DrawingContext context)
	{
		int selectionStart = SelectionStart;
		int selectionEnd = SelectionEnd;
		IBrush selectionBrush = SelectionBrush;
		if (selectionStart != selectionEnd && selectionBrush != null)
		{
			int num = Math.Min(selectionStart, selectionEnd);
			int length = Math.Max(selectionStart, selectionEnd) - num;
			foreach (Rect item in TextLayout.HitTestTextRange(num, length))
			{
				context.FillRectangle(selectionBrush, PixelRect.FromRect(item, 1.0).ToRect(1.0));
			}
		}
		RenderInternal(context);
		if (selectionStart != selectionEnd || !_caretBlink)
		{
			return;
		}
		IImmutableBrush immutableBrush = CaretBrush?.ToImmutable();
		if (immutableBrush == null)
		{
			Color? color = (Background as ISolidColorBrush)?.Color;
			if (color.HasValue)
			{
				byte r = (byte)(~color.Value.R);
				byte g = (byte)(~color.Value.G);
				byte b = (byte)(~color.Value.B);
				immutableBrush = new ImmutableSolidColorBrush(Color.FromRgb(r, g, b));
			}
			else
			{
				immutableBrush = Brushes.Black;
			}
		}
		var (p, p2) = GetCaretPoints();
		context.DrawLine(new ImmutablePen(immutableBrush), p, p2);
	}

	private (Point, Point) GetCaretPoints()
	{
		double num = Math.Floor(_caretBounds.X) + 0.5;
		double y = Math.Floor(_caretBounds.Y) + 0.5;
		double y2 = Math.Ceiling(_caretBounds.Bottom) - 0.5;
		int charIndex = _lastCharacterHit.FirstCharacterIndex + _lastCharacterHit.TrailingLength;
		int lineIndexFromCharacterIndex = TextLayout.GetLineIndexFromCharacterIndex(charIndex, _lastCharacterHit.TrailingLength > 0);
		TextLine textLine = TextLayout.TextLines[lineIndexFromCharacterIndex];
		if (_caretBounds.X > 0.0 && _caretBounds.X >= textLine.WidthIncludingTrailingWhitespace)
		{
			num -= 1.0;
		}
		return (new Point(num, y), new Point(num, y2));
	}

	public void ShowCaret()
	{
		_caretBlink = true;
		_caretTimer.Start();
		InvalidateVisual();
	}

	public void HideCaret()
	{
		_caretBlink = false;
		_caretTimer.Stop();
		InvalidateVisual();
	}

	internal void CaretChanged()
	{
		if (this.GetVisualParent() == null)
		{
			return;
		}
		if (_caretTimer.IsEnabled)
		{
			_caretBlink = true;
			_caretTimer.Stop();
			_caretTimer.Start();
			InvalidateVisual();
		}
		else
		{
			_caretTimer.Start();
			InvalidateVisual();
			_caretTimer.Stop();
		}
		if (base.IsMeasureValid)
		{
			this.BringIntoView(_caretBounds);
			return;
		}
		Dispatcher.UIThread.Post(delegate
		{
			this.BringIntoView(_caretBounds);
		}, DispatcherPriority.AfterRender);
	}

	protected virtual TextLayout CreateTextLayout()
	{
		int caretIndex = CaretIndex;
		string preeditText = PreeditText;
		string combinedText = GetCombinedText(Text, caretIndex, preeditText);
		Typeface typeface = new Typeface(FontFamily, FontStyle, FontWeight);
		int selectionStart = SelectionStart;
		int selectionEnd = SelectionEnd;
		int num = Math.Min(selectionStart, selectionEnd);
		int num2 = Math.Max(selectionStart, selectionEnd) - num;
		IReadOnlyList<ValueSpan<TextRunProperties>> textStyleOverrides = null;
		IBrush foreground = Foreground;
		if (!string.IsNullOrEmpty(preeditText))
		{
			int length = preeditText.Length;
			double fontSize = FontSize;
			IBrush foregroundBrush = foreground;
			ValueSpan<TextRunProperties> valueSpan = new ValueSpan<TextRunProperties>(caretIndex, length, new GenericTextRunProperties(typeface, fontSize, TextDecorations.Underline, foregroundBrush));
			textStyleOverrides = new ValueSpan<TextRunProperties>[1] { valueSpan };
		}
		else if (num2 > 0 && SelectionForegroundBrush != null)
		{
			textStyleOverrides = new ValueSpan<TextRunProperties>[1]
			{
				new ValueSpan<TextRunProperties>(num, num2, new GenericTextRunProperties(typeface, FontSize, null, SelectionForegroundBrush))
			};
		}
		if (PasswordChar != 0 && !RevealPassword)
		{
			return CreateTextLayoutInternal(_constraint, new string(PasswordChar, combinedText?.Length ?? 0), typeface, textStyleOverrides);
		}
		return CreateTextLayoutInternal(_constraint, combinedText, typeface, textStyleOverrides);
	}

	private static string? GetCombinedText(string? text, int caretIndex, string? preeditText)
	{
		if (string.IsNullOrEmpty(preeditText))
		{
			return text;
		}
		if (string.IsNullOrEmpty(text))
		{
			return preeditText;
		}
		StringBuilder stringBuilder = StringBuilderCache.Acquire(text.Length + preeditText.Length);
		stringBuilder.Append(text.Substring(0, caretIndex));
		stringBuilder.Insert(caretIndex, preeditText);
		stringBuilder.Append(text.Substring(caretIndex));
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	protected virtual void InvalidateTextLayout()
	{
		_textLayout?.Dispose();
		_textLayout = null;
		InvalidateMeasure();
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		_constraint = availableSize;
		_textLayout?.Dispose();
		_textLayout = null;
		InvalidateArrange();
		return new Size(TextLayout.OverhangLeading + TextLayout.WidthIncludingTrailingWhitespace + TextLayout.OverhangTrailing, TextLayout.Height);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		double num = Math.Ceiling(TextLayout.OverhangLeading + TextLayout.WidthIncludingTrailingWhitespace + TextLayout.OverhangTrailing);
		if (finalSize.Width < num)
		{
			finalSize = finalSize.WithWidth(num);
		}
		if (MathUtilities.AreClose(_constraint.Width, finalSize.Width))
		{
			return finalSize;
		}
		_constraint = new Size(Math.Ceiling(finalSize.Width), double.PositiveInfinity);
		_textLayout?.Dispose();
		_textLayout = null;
		return finalSize;
	}

	private void CaretTimerTick(object? sender, EventArgs e)
	{
		_caretBlink = !_caretBlink;
		InvalidateVisual();
	}

	public void MoveCaretToTextPosition(int textPosition, bool trailingEdge = false)
	{
		int lineIndexFromCharacterIndex = TextLayout.GetLineIndexFromCharacterIndex(textPosition, trailingEdge);
		TextLine textLine = TextLayout.TextLines[lineIndexFromCharacterIndex];
		CharacterHit characterHit = textLine.GetPreviousCaretCharacterHit(new CharacterHit(textPosition));
		CharacterHit nextCaretCharacterHit = textLine.GetNextCaretCharacterHit(characterHit);
		if (nextCaretCharacterHit.FirstCharacterIndex <= textPosition)
		{
			characterHit = nextCaretCharacterHit;
		}
		if (textPosition == characterHit.FirstCharacterIndex + characterHit.TrailingLength)
		{
			UpdateCaret(characterHit);
		}
		else
		{
			UpdateCaret(trailingEdge ? characterHit : new CharacterHit(characterHit.FirstCharacterIndex));
		}
		_navigationPosition = _caretBounds.Position;
		CaretChanged();
	}

	public void MoveCaretToPoint(Point point)
	{
		UpdateCaret(TextLayout.HitTestPoint(in point).CharacterHit);
		_navigationPosition = _caretBounds.Position;
		CaretChanged();
	}

	public void MoveCaretVertical(LogicalDirection direction = LogicalDirection.Forward)
	{
		int lineIndexFromCharacterIndex = TextLayout.GetLineIndexFromCharacterIndex(CaretIndex, _lastCharacterHit.TrailingLength > 0);
		if (lineIndexFromCharacterIndex < 0)
		{
			return;
		}
		Point navigationPosition = _navigationPosition;
		double x;
		double num3;
		(x, num3) = (Point)(ref navigationPosition);
		if (direction == LogicalDirection.Forward)
		{
			if (lineIndexFromCharacterIndex + 1 > TextLayout.TextLines.Count - 1)
			{
				return;
			}
			TextLine textLine = TextLayout.TextLines[lineIndexFromCharacterIndex];
			num3 += textLine.Height;
		}
		else
		{
			if (lineIndexFromCharacterIndex - 1 < 0)
			{
				return;
			}
			TextLine textLine2 = TextLayout.TextLines[--lineIndexFromCharacterIndex];
			num3 -= textLine2.Height;
		}
		Point navigationPosition2 = _navigationPosition;
		MoveCaretToPoint(new Point(x, num3));
		_navigationPosition = navigationPosition2.WithY(_caretBounds.Y);
		CaretChanged();
	}

	public CharacterHit GetNextCharacterHit(LogicalDirection direction = LogicalDirection.Forward)
	{
		if (Text == null)
		{
			return default(CharacterHit);
		}
		CharacterHit characterHit = _lastCharacterHit;
		int charIndex = characterHit.FirstCharacterIndex + characterHit.TrailingLength;
		int i = TextLayout.GetLineIndexFromCharacterIndex(charIndex, trailingEdge: false);
		if (i < 0)
		{
			return default(CharacterHit);
		}
		if (direction == LogicalDirection.Forward)
		{
			for (; i < TextLayout.TextLines.Count; i++)
			{
				TextLine textLine = TextLayout.TextLines[i];
				characterHit = textLine.GetNextCaretCharacterHit(characterHit);
				charIndex = characterHit.FirstCharacterIndex + characterHit.TrailingLength;
				if (textLine.TrailingWhitespaceLength > 0 && charIndex == textLine.FirstTextSourceIndex + textLine.Length)
				{
					characterHit = new CharacterHit(charIndex);
				}
				if (charIndex >= Text.Length)
				{
					characterHit = new CharacterHit(Text.Length);
					break;
				}
				if (charIndex - textLine.NewLineLength == textLine.FirstTextSourceIndex + textLine.Length || charIndex > CaretIndex)
				{
					break;
				}
			}
		}
		else
		{
			while (i >= 0)
			{
				characterHit = TextLayout.TextLines[i].GetPreviousCaretCharacterHit(characterHit);
				charIndex = characterHit.FirstCharacterIndex + characterHit.TrailingLength;
				if (charIndex < CaretIndex)
				{
					break;
				}
				i--;
			}
		}
		return characterHit;
	}

	public void MoveCaretHorizontal(LogicalDirection direction = LogicalDirection.Forward)
	{
		if (base.FlowDirection == FlowDirection.RightToLeft)
		{
			direction = ((direction != LogicalDirection.Forward) ? LogicalDirection.Forward : LogicalDirection.Backward);
		}
		CharacterHit nextCharacterHit = GetNextCharacterHit(direction);
		UpdateCaret(nextCharacterHit);
		_navigationPosition = _caretBounds.Position;
		CaretChanged();
	}

	internal void UpdateCaret(CharacterHit characterHit, bool notify = true)
	{
		_lastCharacterHit = characterHit;
		int num = characterHit.FirstCharacterIndex + characterHit.TrailingLength;
		int lineIndexFromCharacterIndex = TextLayout.GetLineIndexFromCharacterIndex(num, characterHit.TrailingLength > 0);
		TextLine textLine = TextLayout.TextLines[lineIndexFromCharacterIndex];
		double distanceFromCharacterHit = textLine.GetDistanceFromCharacterHit(characterHit);
		double num2 = 0.0;
		for (int i = 0; i < lineIndexFromCharacterIndex; i++)
		{
			TextLine textLine2 = TextLayout.TextLines[i];
			num2 += textLine2.Height;
		}
		Rect rect = new Rect(distanceFromCharacterHit, num2, 0.0, textLine.Height);
		if (rect != _caretBounds)
		{
			_caretBounds = rect;
			this.CaretBoundsChanged?.Invoke(this, EventArgs.Empty);
		}
		if (notify)
		{
			SetCurrentValue(CaretIndexProperty, num);
		}
	}

	internal Rect GetCursorRectangle()
	{
		return _caretBounds;
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		_caretTimer.Stop();
		_caretTimer.Tick -= CaretTimerTick;
	}

	private void OnPreeditTextChanged(string? preeditText)
	{
		if (string.IsNullOrEmpty(preeditText))
		{
			UpdateCaret(new CharacterHit(CaretIndex), notify: false);
		}
		else
		{
			UpdateCaret(new CharacterHit(CaretIndex + preeditText.Length), notify: false);
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == CaretIndexProperty)
		{
			MoveCaretToTextPosition(change.GetNewValue<int>());
		}
		if (change.Property == PreeditTextProperty)
		{
			OnPreeditTextChanged(change.NewValue as string);
		}
		if (change.Property == TextProperty && !string.IsNullOrEmpty(PreeditText))
		{
			SetCurrentValue(PreeditTextProperty, null);
		}
		if (change.Property == CaretIndexProperty && !string.IsNullOrEmpty(PreeditText))
		{
			SetCurrentValue(PreeditTextProperty, null);
		}
		switch (change.Property.Name)
		{
		case "PreeditText":
		case "Foreground":
		case "FontSize":
		case "FontStyle":
		case "FontWeight":
		case "FontFamily":
		case "FontStretch":
		case "Text":
		case "TextAlignment":
		case "TextWrapping":
		case "LineHeight":
		case "LetterSpacing":
		case "SelectionStart":
		case "SelectionEnd":
		case "SelectionForegroundBrush":
		case "PasswordChar":
		case "RevealPassword":
		case "FlowDirection":
			InvalidateTextLayout();
			break;
		}
	}
}
