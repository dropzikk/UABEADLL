using AvaloniaEdit.Document;

namespace AvaloniaEdit.Search;

public interface ISearchResult : ISegment
{
	string ReplaceWith(string replacement);
}
