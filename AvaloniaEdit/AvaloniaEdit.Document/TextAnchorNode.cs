using System;

namespace AvaloniaEdit.Document;

internal sealed class TextAnchorNode : WeakReference
{
	internal TextAnchorNode Left { get; set; }

	internal TextAnchorNode Right { get; set; }

	internal TextAnchorNode Parent { get; set; }

	internal bool Color { get; set; }

	internal int Length { get; set; }

	internal int TotalLength { get; set; }

	internal TextAnchorNode LeftMost
	{
		get
		{
			TextAnchorNode textAnchorNode = this;
			while (textAnchorNode.Left != null)
			{
				textAnchorNode = textAnchorNode.Left;
			}
			return textAnchorNode;
		}
	}

	internal TextAnchorNode RightMost
	{
		get
		{
			TextAnchorNode textAnchorNode = this;
			while (textAnchorNode.Right != null)
			{
				textAnchorNode = textAnchorNode.Right;
			}
			return textAnchorNode;
		}
	}

	internal TextAnchorNode Successor
	{
		get
		{
			if (Right != null)
			{
				return Right.LeftMost;
			}
			TextAnchorNode textAnchorNode = this;
			TextAnchorNode textAnchorNode2;
			do
			{
				textAnchorNode2 = textAnchorNode;
				textAnchorNode = textAnchorNode.Parent;
			}
			while (textAnchorNode != null && textAnchorNode.Right == textAnchorNode2);
			return textAnchorNode;
		}
	}

	internal TextAnchorNode Predecessor
	{
		get
		{
			if (Left != null)
			{
				return Left.RightMost;
			}
			TextAnchorNode textAnchorNode = this;
			TextAnchorNode textAnchorNode2;
			do
			{
				textAnchorNode2 = textAnchorNode;
				textAnchorNode = textAnchorNode.Parent;
			}
			while (textAnchorNode != null && textAnchorNode.Left == textAnchorNode2);
			return textAnchorNode;
		}
	}

	public TextAnchorNode(TextAnchor anchor)
		: base(anchor)
	{
	}

	public override string ToString()
	{
		return "[TextAnchorNode Length=" + Length + " TotalLength=" + TotalLength + " Target=" + Target?.ToString() + "]";
	}
}
