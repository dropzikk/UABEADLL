namespace AvaloniaEdit.Document;

internal sealed class DocumentChangeOperation : IUndoableOperationWithContext, IUndoableOperation
{
	private readonly TextDocument _document;

	private readonly DocumentChangeEventArgs _change;

	public DocumentChangeOperation(TextDocument document, DocumentChangeEventArgs change)
	{
		_document = document;
		_change = change;
	}

	public void Undo(UndoStack stack)
	{
		stack.RegisterAffectedDocument(_document);
		stack.State = 2;
		Undo();
		stack.State = 1;
	}

	public void Redo(UndoStack stack)
	{
		stack.RegisterAffectedDocument(_document);
		stack.State = 2;
		Redo();
		stack.State = 1;
	}

	public void Undo()
	{
		OffsetChangeMap offsetChangeMapOrNull = _change.OffsetChangeMapOrNull;
		_document.Replace(_change.Offset, _change.InsertionLength, _change.RemovedText, offsetChangeMapOrNull?.Invert());
	}

	public void Redo()
	{
		_document.Replace(_change.Offset, _change.RemovalLength, _change.InsertedText, _change.OffsetChangeMapOrNull);
	}
}
