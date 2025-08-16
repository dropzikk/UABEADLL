using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting;

public class HighlightingEngine
{
	private readonly HighlightingRuleSet _mainRuleSet;

	private ImmutableStack<HighlightingSpan> _spanStack = ImmutableStack<HighlightingSpan>.Empty;

	private string _lineText;

	private int _lineStartOffset;

	private int _position;

	private HighlightedLine _highlightedLine;

	private static readonly HighlightingRuleSet EmptyRuleSet = new HighlightingRuleSet
	{
		Name = "EmptyRuleSet"
	};

	private Stack<HighlightedSection> _highlightedSectionStack;

	private HighlightedSection _lastPoppedSection;

	public ImmutableStack<HighlightingSpan> CurrentSpanStack
	{
		get
		{
			return _spanStack;
		}
		set
		{
			_spanStack = value ?? ImmutableStack<HighlightingSpan>.Empty;
		}
	}

	private HighlightingRuleSet CurrentRuleSet
	{
		get
		{
			if (_spanStack.IsEmpty)
			{
				return _mainRuleSet;
			}
			return _spanStack.Peek().RuleSet ?? EmptyRuleSet;
		}
	}

	public HighlightingEngine(HighlightingRuleSet mainRuleSet)
	{
		_mainRuleSet = mainRuleSet ?? throw new ArgumentNullException("mainRuleSet");
	}

	public HighlightedLine HighlightLine(IDocument document, IDocumentLine line)
	{
		_lineStartOffset = line.Offset;
		_lineText = document.GetText(line);
		try
		{
			_highlightedLine = new HighlightedLine(document, line);
			HighlightLineInternal();
			return _highlightedLine;
		}
		finally
		{
			_highlightedLine = null;
			_lineText = null;
			_lineStartOffset = 0;
		}
	}

	public void ScanLine(IDocument document, IDocumentLine line)
	{
		_lineText = document.GetText(line);
		try
		{
			HighlightLineInternal();
		}
		finally
		{
			_lineText = null;
		}
	}

	private void HighlightLineInternal()
	{
		_position = 0;
		ResetColorStack();
		HighlightingRuleSet currentRuleSet = CurrentRuleSet;
		Stack<Match[]> stack = new Stack<Match[]>();
		Match[] array = AllocateMatchArray(currentRuleSet.Spans.Count);
		Match match = null;
		while (true)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null || (array[i].Success && array[i].Index < _position))
				{
					array[i] = currentRuleSet.Spans[i].StartExpression.Match(_lineText, _position);
				}
			}
			if (!_spanStack.IsEmpty)
			{
				match = _spanStack.Peek().EndExpression.Match(_lineText, _position);
			}
			Match match2 = Minimum(array, match);
			if (match2 == null)
			{
				break;
			}
			HighlightNonSpans(match2.Index);
			if (match2 == match)
			{
				HighlightingSpan highlightingSpan = _spanStack.Peek();
				if (!highlightingSpan.SpanColorIncludesEnd)
				{
					PopColor();
				}
				PushColor(highlightingSpan.EndColor);
				_position = match2.Index + match2.Length;
				PopColor();
				if (highlightingSpan.SpanColorIncludesEnd)
				{
					PopColor();
				}
				_spanStack = _spanStack.Pop();
				currentRuleSet = CurrentRuleSet;
				if (stack.Count > 0)
				{
					array = stack.Pop();
					int num = currentRuleSet.Spans.IndexOf(highlightingSpan);
					if (array[num].Index == _position)
					{
						throw new InvalidOperationException("A highlighting span matched 0 characters, which would cause an endless loop.\nChange the highlighting definition so that either the start or the end regex matches at least one character.\nStart regex: " + highlightingSpan.StartExpression?.ToString() + "\nEnd regex: " + highlightingSpan.EndExpression);
					}
				}
				else
				{
					array = AllocateMatchArray(currentRuleSet.Spans.Count);
				}
			}
			else
			{
				int index = Array.IndexOf(array, match2);
				HighlightingSpan highlightingSpan2 = currentRuleSet.Spans[index];
				_spanStack = _spanStack.Push(highlightingSpan2);
				currentRuleSet = CurrentRuleSet;
				stack.Push(array);
				array = AllocateMatchArray(currentRuleSet.Spans.Count);
				if (highlightingSpan2.SpanColorIncludesStart)
				{
					PushColor(highlightingSpan2.SpanColor);
				}
				PushColor(highlightingSpan2.StartColor);
				_position = match2.Index + match2.Length;
				PopColor();
				if (!highlightingSpan2.SpanColorIncludesStart)
				{
					PushColor(highlightingSpan2.SpanColor);
				}
			}
			match = null;
		}
		HighlightNonSpans(_lineText.Length);
		PopAllColors();
	}

	private void HighlightNonSpans(int until)
	{
		if (_position == until)
		{
			return;
		}
		if (_highlightedLine != null)
		{
			IList<HighlightingRule> rules = CurrentRuleSet.Rules;
			Match[] array = AllocateMatchArray(rules.Count);
			while (true)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == null || (array[i].Success && array[i].Index < _position))
					{
						array[i] = rules[i].Regex.Match(_lineText, _position, until - _position);
					}
				}
				Match match = Minimum(array, null);
				if (match == null)
				{
					break;
				}
				_position = match.Index;
				int index = Array.IndexOf(array, match);
				if (match.Length == 0)
				{
					throw new InvalidOperationException("A highlighting rule matched 0 characters, which would cause an endless loop.\nChange the highlighting definition so that the rule matches at least one character.\nRegex: " + rules[index].Regex);
				}
				PushColor(rules[index].Color);
				_position = match.Index + match.Length;
				PopColor();
			}
		}
		_position = until;
	}

	private void ResetColorStack()
	{
		_lastPoppedSection = null;
		if (_highlightedLine == null)
		{
			_highlightedSectionStack = null;
			return;
		}
		_highlightedSectionStack = new Stack<HighlightedSection>();
		foreach (HighlightingSpan item in _spanStack.Reverse())
		{
			PushColor(item.SpanColor);
		}
	}

	private void PushColor(HighlightingColor color)
	{
		if (_highlightedLine != null)
		{
			if (color == null)
			{
				_highlightedSectionStack.Push(null);
				return;
			}
			if (_lastPoppedSection != null && object.Equals(_lastPoppedSection.Color, color) && _lastPoppedSection.Offset + _lastPoppedSection.Length == _position + _lineStartOffset)
			{
				_highlightedSectionStack.Push(_lastPoppedSection);
				_lastPoppedSection = null;
				return;
			}
			HighlightedSection item = new HighlightedSection
			{
				Offset = _position + _lineStartOffset,
				Color = color
			};
			_highlightedLine.Sections.Add(item);
			_highlightedSectionStack.Push(item);
			_lastPoppedSection = null;
		}
	}

	private void PopColor()
	{
		if (_highlightedLine == null)
		{
			return;
		}
		HighlightedSection highlightedSection = _highlightedSectionStack.Pop();
		if (highlightedSection != null)
		{
			highlightedSection.Length = _position + _lineStartOffset - highlightedSection.Offset;
			if (highlightedSection.Length == 0)
			{
				_highlightedLine.Sections.Remove(highlightedSection);
			}
			else
			{
				_lastPoppedSection = highlightedSection;
			}
		}
	}

	private void PopAllColors()
	{
		if (_highlightedSectionStack != null)
		{
			while (_highlightedSectionStack.Count > 0)
			{
				PopColor();
			}
		}
	}

	private static Match Minimum(Match[] arr, Match endSpanMatch)
	{
		Match match = null;
		foreach (Match match2 in arr)
		{
			if (match2.Success && (match == null || match2.Index < match.Index))
			{
				match = match2;
			}
		}
		if (endSpanMatch != null && endSpanMatch.Success && (match == null || endSpanMatch.Index < match.Index))
		{
			return endSpanMatch;
		}
		return match;
	}

	private static Match[] AllocateMatchArray(int count)
	{
		if (count == 0)
		{
			return Empty<Match>.Array;
		}
		return new Match[count];
	}
}
