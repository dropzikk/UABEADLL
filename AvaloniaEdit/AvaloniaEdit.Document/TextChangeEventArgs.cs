using System;

namespace AvaloniaEdit.Document;

public class TextChangeEventArgs : EventArgs
{
	public int Offset { get; }

	public ITextSource RemovedText { get; }

	public int RemovalLength => RemovedText.TextLength;

	public ITextSource InsertedText { get; }

	public int InsertionLength => InsertedText.TextLength;

	public TextChangeEventArgs(int offset, string removedText, string insertedText)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "offset must not be negative");
		}
		Offset = offset;
		RemovedText = ((removedText != null) ? new StringTextSource(removedText) : StringTextSource.Empty);
		InsertedText = ((insertedText != null) ? new StringTextSource(insertedText) : StringTextSource.Empty);
	}

	public TextChangeEventArgs(int offset, ITextSource removedText, ITextSource insertedText)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "offset must not be negative");
		}
		Offset = offset;
		RemovedText = removedText ?? StringTextSource.Empty;
		InsertedText = insertedText ?? StringTextSource.Empty;
	}

	public virtual int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
	{
		if (offset >= Offset && offset <= Offset + RemovalLength)
		{
			if (movementType == AnchorMovementType.BeforeInsertion)
			{
				return Offset;
			}
			return Offset + InsertionLength;
		}
		if (offset > Offset)
		{
			return offset + InsertionLength - RemovalLength;
		}
		return offset;
	}

	public virtual TextChangeEventArgs Invert()
	{
		return new TextChangeEventArgs(Offset, InsertedText, RemovedText);
	}
}
