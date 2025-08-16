using AvaloniaEdit.Document;

namespace AvaloniaEdit.Indentation;

public interface IIndentationStrategy
{
	void IndentLine(TextDocument document, DocumentLine line);

	void IndentLines(TextDocument document, int beginLine, int endLine);
}
