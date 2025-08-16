using System;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting;

public class HighlightingColorizer : DocumentColorizingTransformer
{
	private readonly IHighlightingDefinition _definition;

	private TextView _textView;

	private IHighlighter _highlighter;

	private readonly bool _isFixedHighlighter;

	private bool _isInHighlightingGroup;

	private DocumentLine _lastColorizedLine;

	private int _lineNumberBeingColorized;

	public HighlightingColorizer(IHighlightingDefinition definition)
	{
		_definition = definition ?? throw new ArgumentNullException("definition");
	}

	public HighlightingColorizer(IHighlighter highlighter)
	{
		_highlighter = highlighter ?? throw new ArgumentNullException("highlighter");
		_isFixedHighlighter = true;
	}

	protected HighlightingColorizer()
	{
	}

	private void TextView_DocumentChanged(object sender, EventArgs e)
	{
		TextView textView = (TextView)sender;
		DeregisterServices(textView);
		RegisterServices(textView);
	}

	protected virtual void DeregisterServices(TextView textView)
	{
		if (_highlighter != null)
		{
			if (_isInHighlightingGroup)
			{
				_highlighter.EndHighlighting();
				_isInHighlightingGroup = false;
			}
			_highlighter.HighlightingStateChanged -= OnHighlightStateChanged;
			if (textView.Services.GetService(typeof(IHighlighter)) == _highlighter)
			{
				textView.Services.RemoveService<IHighlighter>();
			}
			if (!_isFixedHighlighter)
			{
				_highlighter?.Dispose();
				_highlighter = null;
			}
		}
	}

	protected virtual void RegisterServices(TextView textView)
	{
		if (textView.Document == null)
		{
			return;
		}
		if (!_isFixedHighlighter)
		{
			_highlighter = ((textView.Document != null) ? CreateHighlighter(textView, textView.Document) : null);
		}
		if (_highlighter != null && _highlighter.Document == textView.Document)
		{
			if (textView.Services.GetService<IHighlighter>() == null)
			{
				textView.Services.AddService(typeof(IHighlighter), _highlighter);
			}
			_highlighter.HighlightingStateChanged += OnHighlightStateChanged;
		}
	}

	protected virtual IHighlighter CreateHighlighter(TextView textView, TextDocument document)
	{
		if (_definition != null)
		{
			return new DocumentHighlighter(document, _definition);
		}
		throw new NotSupportedException("Cannot create a highlighter because no IHighlightingDefinition was specified, and the CreateHighlighter() method was not overridden.");
	}

	protected override void OnAddToTextView(TextView textView)
	{
		if (_textView != null)
		{
			throw new InvalidOperationException("Cannot use a HighlightingColorizer instance in multiple text views. Please create a separate instance for each text view.");
		}
		base.OnAddToTextView(textView);
		_textView = textView;
		textView.DocumentChanged += TextView_DocumentChanged;
		textView.VisualLineConstructionStarting += TextView_VisualLineConstructionStarting;
		textView.VisualLinesChanged += TextView_VisualLinesChanged;
		RegisterServices(textView);
	}

	protected override void OnRemoveFromTextView(TextView textView)
	{
		DeregisterServices(textView);
		textView.DocumentChanged -= TextView_DocumentChanged;
		textView.VisualLineConstructionStarting -= TextView_VisualLineConstructionStarting;
		textView.VisualLinesChanged -= TextView_VisualLinesChanged;
		base.OnRemoveFromTextView(textView);
		_textView = null;
	}

	private void TextView_VisualLineConstructionStarting(object sender, VisualLineConstructionStartEventArgs e)
	{
		if (_highlighter != null)
		{
			_lineNumberBeingColorized = e.FirstLineInView.LineNumber - 1;
			if (!_isInHighlightingGroup)
			{
				_highlighter.BeginHighlighting();
				_isInHighlightingGroup = true;
			}
			_highlighter.UpdateHighlightingState(_lineNumberBeingColorized);
			_lineNumberBeingColorized = 0;
		}
	}

	private void TextView_VisualLinesChanged(object sender, EventArgs e)
	{
		if (_highlighter != null && _isInHighlightingGroup)
		{
			_highlighter.EndHighlighting();
			_isInHighlightingGroup = false;
		}
	}

	protected override void Colorize(ITextRunConstructionContext context)
	{
		_lastColorizedLine = null;
		base.Colorize(context);
		if (_lastColorizedLine != context.VisualLine.LastDocumentLine && _highlighter != null)
		{
			_lineNumberBeingColorized = context.VisualLine.LastDocumentLine.LineNumber;
			_highlighter.UpdateHighlightingState(_lineNumberBeingColorized);
			_lineNumberBeingColorized = 0;
		}
		_lastColorizedLine = null;
	}

	protected override void ColorizeLine(DocumentLine line)
	{
		if (_highlighter != null)
		{
			_lineNumberBeingColorized = line.LineNumber;
			HighlightedLine highlightedLine = _highlighter.HighlightLine(_lineNumberBeingColorized);
			_lineNumberBeingColorized = 0;
			foreach (HighlightedSection section in highlightedLine.Sections)
			{
				if (!IsEmptyColor(section.Color))
				{
					ChangeLinePart(section.Offset, section.Offset + section.Length, delegate(VisualLineElement visualLineElement)
					{
						ApplyColorToElement(visualLineElement, section.Color);
					});
				}
			}
		}
		_lastColorizedLine = line;
	}

	internal static bool IsEmptyColor(HighlightingColor color)
	{
		if (color == null)
		{
			return true;
		}
		if (color.Background == null && color.Foreground == null && !color.FontStyle.HasValue && !color.FontWeight.HasValue)
		{
			return !color.Underline.HasValue;
		}
		return false;
	}

	protected virtual void ApplyColorToElement(VisualLineElement element, HighlightingColor color)
	{
		ApplyColorToElement(element, color, base.CurrentContext);
	}

	internal static void ApplyColorToElement(VisualLineElement element, HighlightingColor color, ITextRunConstructionContext context)
	{
		if (color.Foreground != null)
		{
			IBrush brush = color.Foreground.GetBrush(context);
			if (brush != null)
			{
				element.TextRunProperties.SetForegroundBrush(brush);
			}
		}
		if (color.Background != null)
		{
			IBrush brush2 = color.Background.GetBrush(context);
			if (brush2 != null)
			{
				element.BackgroundBrush = brush2;
			}
		}
		if (color.FontStyle.HasValue || color.FontWeight.HasValue || color.FontFamily != null)
		{
			Typeface typeface = element.TextRunProperties.Typeface;
			element.TextRunProperties.SetTypeface(new Typeface(color.FontFamily ?? typeface.FontFamily, color.FontStyle ?? typeface.Style, color.FontWeight ?? typeface.Weight, typeface.Stretch));
		}
		if (color.Underline == true)
		{
			element.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
		}
		if (color.Strikethrough == true)
		{
			element.TextRunProperties.SetTextDecorations(TextDecorations.Strikethrough);
		}
		if (color.FontSize.HasValue)
		{
			element.TextRunProperties.SetFontRenderingEmSize(color.FontSize.Value);
		}
	}

	private void OnHighlightStateChanged(int fromLineNumber, int toLineNumber)
	{
		if (_lineNumberBeingColorized == 0 || toLineNumber > _lineNumberBeingColorized)
		{
			if (fromLineNumber == toLineNumber)
			{
				_textView.Redraw(_textView.Document.GetLineByNumber(fromLineNumber));
				return;
			}
			DocumentLine lineByNumber = _textView.Document.GetLineByNumber(fromLineNumber);
			DocumentLine lineByNumber2 = _textView.Document.GetLineByNumber(toLineNumber);
			int offset = lineByNumber.Offset;
			_textView.Redraw(offset, lineByNumber2.EndOffset - offset);
		}
	}
}
