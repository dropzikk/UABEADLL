using System;

namespace AvaloniaEdit.Search;

public class SearchOptionsChangedEventArgs : EventArgs
{
	public string SearchPattern { get; }

	public bool MatchCase { get; }

	public bool UseRegex { get; }

	public bool WholeWords { get; }

	public SearchOptionsChangedEventArgs(string searchPattern, bool matchCase, bool useRegex, bool wholeWords)
	{
		SearchPattern = searchPattern;
		MatchCase = matchCase;
		UseRegex = useRegex;
		WholeWords = wholeWords;
	}
}
