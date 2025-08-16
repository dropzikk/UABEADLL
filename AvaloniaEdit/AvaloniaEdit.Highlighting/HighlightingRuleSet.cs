using System.Collections.Generic;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting;

public class HighlightingRuleSet
{
	public string Name { get; set; }

	public IList<HighlightingSpan> Spans { get; }

	public IList<HighlightingRule> Rules { get; }

	public HighlightingRuleSet()
	{
		Spans = new NullSafeCollection<HighlightingSpan>();
		Rules = new NullSafeCollection<HighlightingRule>();
	}

	public override string ToString()
	{
		return "[" + GetType().Name + " " + Name + "]";
	}
}
