using System;

namespace AvaloniaEdit.Document;

public class TextSegment : ISegment
{
	internal ISegmentTree OwnerTree { get; set; }

	internal TextSegment Left { get; set; }

	internal TextSegment Right { get; set; }

	internal TextSegment Parent { get; set; }

	internal bool Color { get; set; }

	internal int NodeLength { get; set; }

	internal int TotalNodeLength { get; set; }

	internal int SegmentLength { get; set; }

	internal int DistanceToMaxEnd { get; set; }

	int ISegment.Offset => StartOffset;

	protected bool IsConnectedToCollection => OwnerTree != null;

	public int StartOffset
	{
		get
		{
			TextSegment textSegment = this;
			int num = textSegment.NodeLength;
			if (textSegment.Left != null)
			{
				num += textSegment.Left.TotalNodeLength;
			}
			while (textSegment.Parent != null)
			{
				if (textSegment == textSegment.Parent.Right)
				{
					if (textSegment.Parent.Left != null)
					{
						num += textSegment.Parent.Left.TotalNodeLength;
					}
					num += textSegment.Parent.NodeLength;
				}
				textSegment = textSegment.Parent;
			}
			return num;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value", "Offset must not be negative");
			}
			if (StartOffset != value)
			{
				ISegmentTree ownerTree = OwnerTree;
				if (ownerTree != null)
				{
					ownerTree.Remove(this);
					NodeLength = value;
					ownerTree.Add(this);
				}
				else
				{
					NodeLength = value;
				}
				OnSegmentChanged();
			}
		}
	}

	public int EndOffset
	{
		get
		{
			return StartOffset + Length;
		}
		set
		{
			int num = value - StartOffset;
			if (num < 0)
			{
				throw new ArgumentOutOfRangeException("value", "EndOffset must be greater or equal to StartOffset");
			}
			Length = num;
		}
	}

	public int Length
	{
		get
		{
			return SegmentLength;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value", "Length must not be negative");
			}
			if (SegmentLength != value)
			{
				SegmentLength = value;
				OwnerTree?.UpdateAugmentedData(this);
				OnSegmentChanged();
			}
		}
	}

	internal TextSegment LeftMost
	{
		get
		{
			TextSegment textSegment = this;
			while (textSegment.Left != null)
			{
				textSegment = textSegment.Left;
			}
			return textSegment;
		}
	}

	internal TextSegment RightMost
	{
		get
		{
			TextSegment textSegment = this;
			while (textSegment.Right != null)
			{
				textSegment = textSegment.Right;
			}
			return textSegment;
		}
	}

	internal TextSegment Successor
	{
		get
		{
			if (Right != null)
			{
				return Right.LeftMost;
			}
			TextSegment textSegment = this;
			TextSegment textSegment2;
			do
			{
				textSegment2 = textSegment;
				textSegment = textSegment.Parent;
			}
			while (textSegment != null && textSegment.Right == textSegment2);
			return textSegment;
		}
	}

	internal TextSegment Predecessor
	{
		get
		{
			if (Left != null)
			{
				return Left.RightMost;
			}
			TextSegment textSegment = this;
			TextSegment textSegment2;
			do
			{
				textSegment2 = textSegment;
				textSegment = textSegment.Parent;
			}
			while (textSegment != null && textSegment.Left == textSegment2);
			return textSegment;
		}
	}

	protected virtual void OnSegmentChanged()
	{
	}

	public override string ToString()
	{
		return $"[{GetType().Name} Offset={StartOffset} Length={Length} EndOffset={EndOffset}]";
	}
}
