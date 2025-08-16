using System.Collections.Generic;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Editing;

public interface IReadOnlySectionProvider
{
	bool CanInsert(int offset);

	IEnumerable<ISegment> GetDeletableSegments(ISegment segment);
}
