using System.Collections.Generic;
using System.Linq;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Editing;

internal sealed class ReadOnlySectionDocument : IReadOnlySectionProvider
{
	public static readonly ReadOnlySectionDocument Instance = new ReadOnlySectionDocument();

	public bool CanInsert(int offset)
	{
		return false;
	}

	public IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
	{
		return Enumerable.Empty<ISegment>();
	}
}
