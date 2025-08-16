using System;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Snippets;

internal sealed class BoundActiveElement : IActiveElement
{
	private readonly InsertionContext _context;

	private readonly SnippetReplaceableTextElement _targetSnippetElement;

	private readonly SnippetBoundElement _boundElement;

	internal IReplaceableActiveElement TargetElement;

	private AnchorSegment _segment;

	public bool IsEditable => false;

	public ISegment Segment => _segment;

	public BoundActiveElement(InsertionContext context, SnippetReplaceableTextElement targetSnippetElement, SnippetBoundElement boundElement, AnchorSegment segment)
	{
		_context = context;
		_targetSnippetElement = targetSnippetElement;
		_boundElement = boundElement;
		_segment = segment;
	}

	public void OnInsertionCompleted()
	{
		TargetElement = _context.GetActiveElement(_targetSnippetElement) as IReplaceableActiveElement;
		if (TargetElement != null)
		{
			TargetElement.TextChanged += targetElement_TextChanged;
		}
	}

	private void targetElement_TextChanged(object sender, EventArgs e)
	{
		if (!(SimpleSegment.GetOverlap(_segment, TargetElement.Segment) == SimpleSegment.Invalid))
		{
			return;
		}
		int offset = _segment.Offset;
		int length = _segment.Length;
		string text = _boundElement.ConvertText(TargetElement.Text);
		if (length != text.Length || text != _context.Document.GetText(offset, length))
		{
			_context.Document.Replace(offset, length, text);
			if (length == 0)
			{
				_segment = new AnchorSegment(_context.Document, offset, text.Length);
			}
		}
	}

	public void Deactivate(SnippetEventArgs e)
	{
		TargetElement.TextChanged -= targetElement_TextChanged;
	}
}
