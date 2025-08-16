using System;
using System.Buffers;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using TextMateSharp.Grammars;
using TextMateSharp.Model;
using TextMateSharp.Themes;

namespace AvaloniaEdit.TextMate;

public class TextMateColoringTransformer : GenericLineTransformer, IModelTokensChangedListener, ForegroundTextTransformation.IColorMap
{
	private Theme _theme;

	private IGrammar _grammar;

	private TMModel _model;

	private TextDocument _document;

	private TextView _textView;

	private Action<Exception> _exceptionHandler;

	private volatile bool _areVisualLinesValid;

	private volatile int _firstVisibleLineIndex = -1;

	private volatile int _lastVisibleLineIndex = -1;

	private readonly Dictionary<int, IBrush> _brushes;

	public TextMateColoringTransformer(TextView textView, Action<Exception> exceptionHandler)
		: base(exceptionHandler)
	{
		_textView = textView;
		_exceptionHandler = exceptionHandler;
		_brushes = new Dictionary<int, IBrush>();
		_textView.VisualLinesChanged += TextView_VisualLinesChanged;
	}

	public void SetModel(TextDocument document, TMModel model)
	{
		_areVisualLinesValid = false;
		_document = document;
		_model = model;
		if (_grammar != null)
		{
			_model.SetGrammar(_grammar);
		}
	}

	private void TextView_VisualLinesChanged(object sender, EventArgs e)
	{
		try
		{
			if (_textView.VisualLinesValid && _textView.VisualLines.Count != 0)
			{
				_areVisualLinesValid = true;
				_firstVisibleLineIndex = _textView.VisualLines[0].FirstDocumentLine.LineNumber - 1;
				_lastVisibleLineIndex = _textView.VisualLines[_textView.VisualLines.Count - 1].LastDocumentLine.LineNumber - 1;
			}
		}
		catch (Exception obj)
		{
			_exceptionHandler?.Invoke(obj);
		}
	}

	public void Dispose()
	{
		_textView.VisualLinesChanged -= TextView_VisualLinesChanged;
	}

	public void SetTheme(Theme theme)
	{
		_theme = theme;
		_brushes.Clear();
		foreach (string item in _theme.GetColorMap())
		{
			int colorId = _theme.GetColorId(item);
			_brushes[colorId] = new ImmutableSolidColorBrush(Color.Parse(NormalizeColor(item)));
		}
	}

	public void SetGrammar(IGrammar grammar)
	{
		_grammar = grammar;
		if (_model != null)
		{
			_model.SetGrammar(grammar);
		}
	}

	IBrush ForegroundTextTransformation.IColorMap.GetBrush(int colorId)
	{
		if (_brushes == null)
		{
			return null;
		}
		_brushes.TryGetValue(colorId, out var value);
		return value;
	}

	protected override void TransformLine(DocumentLine line, ITextRunConstructionContext context)
	{
		try
		{
			if (_model == null)
			{
				return;
			}
			int lineNumber = line.LineNumber;
			List<TMToken> lineTokens = _model.GetLineTokens(lineNumber - 1);
			if (lineTokens == null)
			{
				return;
			}
			ForegroundTextTransformation[] array = ArrayPool<ForegroundTextTransformation>.Shared.Rent(lineTokens.Count);
			try
			{
				GetLineTransformations(lineNumber, lineTokens, array);
				for (int i = 0; i < lineTokens.Count; i++)
				{
					if (array[i] != null)
					{
						array[i].Transform(this, line);
					}
				}
			}
			finally
			{
				ArrayPool<ForegroundTextTransformation>.Shared.Return(array);
			}
		}
		catch (Exception obj)
		{
			_exceptionHandler?.Invoke(obj);
		}
	}

	private void GetLineTransformations(int lineNumber, List<TMToken> tokens, ForegroundTextTransformation[] transformations)
	{
		for (int i = 0; i < tokens.Count; i++)
		{
			TMToken tMToken = tokens[i];
			TMToken obj = ((i + 1 < tokens.Count) ? tokens[i + 1] : null);
			int startIndex = tMToken.StartIndex;
			int num = obj?.StartIndex ?? _model.GetLines().GetLineLength(lineNumber - 1);
			if (startIndex >= num || tMToken.Scopes == null || tMToken.Scopes.Count == 0)
			{
				transformations[i] = null;
				continue;
			}
			int offset = _document.GetLineByNumber(lineNumber).Offset;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (ThemeTrieElementRule item in _theme.Match(tMToken.Scopes))
			{
				if (num2 == 0 && item.foreground > 0)
				{
					num2 = item.foreground;
				}
				if (num3 == 0 && item.background > 0)
				{
					num3 = item.background;
				}
				if (num4 == 0 && item.fontStyle > 0)
				{
					num4 = item.fontStyle;
				}
			}
			if (transformations[i] == null)
			{
				transformations[i] = new ForegroundTextTransformation();
			}
			transformations[i].ColorMap = this;
			transformations[i].ExceptionHandler = _exceptionHandler;
			transformations[i].StartOffset = offset + startIndex;
			transformations[i].EndOffset = offset + num;
			transformations[i].ForegroundColor = num2;
			transformations[i].BackgroundColor = num3;
			transformations[i].FontStyle = num4;
		}
	}

	public void ModelTokensChanged(ModelTokensChangedEvent e)
	{
		if (e.Ranges == null || _model == null || _model.IsStopped)
		{
			return;
		}
		int firstChangedLineIndex = int.MaxValue;
		int lastChangedLineIndex = -1;
		foreach (TextMateSharp.Model.Range range in e.Ranges)
		{
			firstChangedLineIndex = Math.Min(range.FromLineNumber - 1, firstChangedLineIndex);
			lastChangedLineIndex = Math.Max(range.ToLineNumber - 1, lastChangedLineIndex);
		}
		if (!_areVisualLinesValid || ((firstChangedLineIndex >= _firstVisibleLineIndex || lastChangedLineIndex >= _firstVisibleLineIndex) && (firstChangedLineIndex <= _lastVisibleLineIndex || lastChangedLineIndex <= _lastVisibleLineIndex)))
		{
			Dispatcher.UIThread.Post(delegate
			{
				int value = Math.Max(firstChangedLineIndex, _firstVisibleLineIndex);
				int value2 = Math.Min(lastChangedLineIndex, _lastVisibleLineIndex);
				int max = _document.Lines.Count - 1;
				value = Clamp(value, 0, max);
				value2 = Clamp(value2, 0, max);
				DocumentLine documentLine = _document.Lines[value];
				DocumentLine documentLine2 = _document.Lines[value2];
				_textView.Redraw(documentLine.Offset, documentLine2.Offset + documentLine2.TotalLength - documentLine.Offset);
			});
		}
	}

	private static int Clamp(int value, int min, int max)
	{
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	private static string NormalizeColor(string color)
	{
		if (color.Length == 9)
		{
			return stackalloc char[9]
			{
				'#',
				color[7],
				color[8],
				color[1],
				color[2],
				color[3],
				color[4],
				color[5],
				color[6]
			}.ToString();
		}
		return color;
	}
}
