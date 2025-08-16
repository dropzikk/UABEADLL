namespace AvaloniaEdit.Document;

public interface IUndoableOperation
{
	void Undo();

	void Redo();
}
