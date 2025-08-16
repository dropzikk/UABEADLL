using AvaloniaEdit.Document;

namespace AvaloniaEdit.Snippets;

public sealed class AnchorElement : IActiveElement
{
	private AnchorSegment _segment;

	private readonly InsertionContext _context;

	public bool IsEditable => false;

	public ISegment Segment => _segment;

	public string Text
	{
		get
		{
			return _context.Document.GetText(_segment);
		}
		set
		{
			int offset = _segment.Offset;
			int length = _segment.Length;
			_context.Document.Replace(offset, length, value);
			if (length == 0)
			{
				_segment = new AnchorSegment(_context.Document, offset, value.Length);
			}
		}
	}

	public string Name { get; }

	public AnchorElement(AnchorSegment segment, string name, InsertionContext context)
	{
		_segment = segment;
		_context = context;
		Name = name;
	}

	public void OnInsertionCompleted()
	{
	}

	public void Deactivate(SnippetEventArgs e)
	{
	}
}
