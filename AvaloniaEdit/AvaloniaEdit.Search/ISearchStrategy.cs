using System;
using System.Collections.Generic;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Search;

public interface ISearchStrategy : IEquatable<ISearchStrategy>
{
	IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length);

	ISearchResult FindNext(ITextSource document, int offset, int length);
}
