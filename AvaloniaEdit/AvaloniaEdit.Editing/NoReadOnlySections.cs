using System;
using System.Collections.Generic;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

internal sealed class NoReadOnlySections : IReadOnlySectionProvider
{
	public static readonly NoReadOnlySections Instance = new NoReadOnlySections();

	public bool CanInsert(int offset)
	{
		return true;
	}

	public IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		return ExtensionMethods.Sequence(segment);
	}
}
