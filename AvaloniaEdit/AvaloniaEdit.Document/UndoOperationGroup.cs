using System;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

internal sealed class UndoOperationGroup : IUndoableOperationWithContext, IUndoableOperation
{
	private readonly IUndoableOperation[] _undolist;

	public UndoOperationGroup(Deque<IUndoableOperation> stack, int numops)
	{
		if (stack == null)
		{
			throw new ArgumentNullException("stack");
		}
		_undolist = new IUndoableOperation[numops];
		for (int i = 0; i < numops; i++)
		{
			_undolist[i] = stack.PopBack();
		}
	}

	public void Undo()
	{
		IUndoableOperation[] undolist = _undolist;
		for (int i = 0; i < undolist.Length; i++)
		{
			undolist[i].Undo();
		}
	}

	public void Undo(UndoStack stack)
	{
		IUndoableOperation[] undolist = _undolist;
		foreach (IUndoableOperation op in undolist)
		{
			stack.RunUndo(op);
		}
	}

	public void Redo()
	{
		for (int num = _undolist.Length - 1; num >= 0; num--)
		{
			_undolist[num].Redo();
		}
	}

	public void Redo(UndoStack stack)
	{
		for (int num = _undolist.Length - 1; num >= 0; num--)
		{
			stack.RunRedo(_undolist[num]);
		}
	}
}
