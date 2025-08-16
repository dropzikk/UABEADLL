using AvaloniaEdit.Document;

namespace AvaloniaEdit.Snippets;

public interface IActiveElement
{
	bool IsEditable { get; }

	ISegment Segment { get; }

	void OnInsertionCompleted();

	void Deactivate(SnippetEventArgs e);
}
