using AvaloniaEdit.Document;

namespace AvaloniaEdit.TextMate;

public abstract class TextTransformation : TextSegment
{
	public abstract void Transform(GenericLineTransformer transformer, DocumentLine line);
}
