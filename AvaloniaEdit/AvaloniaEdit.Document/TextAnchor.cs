using System;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public sealed class TextAnchor : ITextAnchor
{
	internal TextAnchorNode Node { get; set; }

	public TextDocument Document { get; }

	public AnchorMovementType MovementType { get; set; }

	public bool SurviveDeletion { get; set; }

	public bool IsDeleted => Node == null;

	public int Offset
	{
		get
		{
			TextAnchorNode textAnchorNode = Node;
			if (textAnchorNode == null)
			{
				throw new InvalidOperationException();
			}
			int num = textAnchorNode.Length;
			if (textAnchorNode.Left != null)
			{
				num += textAnchorNode.Left.TotalLength;
			}
			while (textAnchorNode.Parent != null)
			{
				if (textAnchorNode == textAnchorNode.Parent.Right)
				{
					if (textAnchorNode.Parent.Left != null)
					{
						num += textAnchorNode.Parent.Left.TotalLength;
					}
					num += textAnchorNode.Parent.Length;
				}
				textAnchorNode = textAnchorNode.Parent;
			}
			return num;
		}
	}

	public int Line => Document.GetLineByOffset(Offset).LineNumber;

	public int Column
	{
		get
		{
			int offset = Offset;
			return offset - Document.GetLineByOffset(offset).Offset + 1;
		}
	}

	public TextLocation Location => Document.GetLocation(Offset);

	public event EventHandler Deleted;

	internal TextAnchor(TextDocument document)
	{
		Document = document;
	}

	internal void OnDeleted(DelayedEvents delayedEvents)
	{
		Node = null;
		delayedEvents.DelayedRaise(this.Deleted, this, EventArgs.Empty);
	}

	public override string ToString()
	{
		return "[TextAnchor Offset=" + Offset + "]";
	}
}
