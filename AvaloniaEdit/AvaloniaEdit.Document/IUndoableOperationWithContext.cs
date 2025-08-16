namespace AvaloniaEdit.Document;

internal interface IUndoableOperationWithContext : IUndoableOperation
{
	void Undo(UndoStack stack);

	void Redo(UndoStack stack);
}
