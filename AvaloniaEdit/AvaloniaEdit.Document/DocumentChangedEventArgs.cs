using System;

namespace AvaloniaEdit.Document;

public class DocumentChangedEventArgs : EventArgs
{
	public TextDocument OldDocument { get; private set; }

	public TextDocument NewDocument { get; private set; }

	public DocumentChangedEventArgs(TextDocument oldDocument, TextDocument newDocument)
	{
		OldDocument = oldDocument;
		NewDocument = newDocument;
	}
}
