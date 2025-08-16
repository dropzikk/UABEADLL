using System.Text.RegularExpressions;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Search;

internal class SearchResult : TextSegment, ISearchResult, ISegment
{
	public Match Data { get; set; }

	public string ReplaceWith(string replacement)
	{
		return Data.Result(replacement);
	}
}
