using System.Collections.Generic;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Snippets;

public class SnippetContainerElement : SnippetElement
{
	private readonly NullSafeCollection<SnippetElement> _elements = new NullSafeCollection<SnippetElement>();

	public IList<SnippetElement> Elements => _elements;

	public override void Insert(InsertionContext context)
	{
		foreach (SnippetElement element in Elements)
		{
			element.Insert(context);
		}
	}
}
