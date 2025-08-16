using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Search;

internal class RegexSearchStrategy : ISearchStrategy, IEquatable<ISearchStrategy>
{
	private readonly Regex _searchPattern;

	private readonly bool _matchWholeWords;

	public RegexSearchStrategy(Regex searchPattern, bool matchWholeWords)
	{
		_searchPattern = searchPattern ?? throw new ArgumentNullException("searchPattern");
		_matchWholeWords = matchWholeWords;
	}

	public IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length)
	{
		int endOffset = offset + length;
		foreach (Match item in _searchPattern.Matches(document.Text))
		{
			int num = item.Length + item.Index;
			if (offset <= item.Index && endOffset >= num && (!_matchWholeWords || (IsWordBorder(document, item.Index) && IsWordBorder(document, num))))
			{
				yield return new SearchResult
				{
					StartOffset = item.Index,
					Length = item.Length,
					Data = item
				};
			}
		}
	}

	private static bool IsWordBorder(ITextSource document, int offset)
	{
		return TextUtilities.GetNextCaretPosition(document, offset - 1, LogicalDirection.Forward, CaretPositioningMode.WordBorder) == offset;
	}

	public ISearchResult FindNext(ITextSource document, int offset, int length)
	{
		return FindAll(document, offset, length).FirstOrDefault();
	}

	public bool Equals(ISearchStrategy other)
	{
		if (other is RegexSearchStrategy regexSearchStrategy && regexSearchStrategy._searchPattern.ToString() == _searchPattern.ToString() && regexSearchStrategy._searchPattern.Options == _searchPattern.Options)
		{
			return regexSearchStrategy._searchPattern.RightToLeft == _searchPattern.RightToLeft;
		}
		return false;
	}
}
