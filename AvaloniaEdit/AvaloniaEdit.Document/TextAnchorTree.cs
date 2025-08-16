using System.Collections.Generic;
using System.Diagnostics;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

internal sealed class TextAnchorTree
{
	private readonly TextDocument _document;

	private readonly List<TextAnchorNode> _nodesToDelete = new List<TextAnchorNode>();

	private TextAnchorNode _root;

	internal const bool Red = true;

	internal const bool Black = false;

	public TextAnchorTree(TextDocument document)
	{
		_document = document;
	}

	[Conditional("DEBUG")]
	private static void Log(string text)
	{
	}

	private void InsertText(int offset, int length, bool defaultAnchorMovementIsBeforeInsertion)
	{
		if (length == 0 || _root == null || offset > _root.TotalLength)
		{
			return;
		}
		if (offset == _root.TotalLength)
		{
			PerformInsertText(FindActualBeginNode(_root.RightMost), null, length, defaultAnchorMovementIsBeforeInsertion);
		}
		else
		{
			TextAnchorNode textAnchorNode = FindNode(ref offset);
			if (offset > 0)
			{
				textAnchorNode.Length += length;
				UpdateAugmentedData(textAnchorNode);
			}
			else
			{
				PerformInsertText(FindActualBeginNode(textAnchorNode.Predecessor), textAnchorNode, length, defaultAnchorMovementIsBeforeInsertion);
			}
		}
		DeleteMarkedNodes();
	}

	private TextAnchorNode FindActualBeginNode(TextAnchorNode node)
	{
		while (node != null && node.Length == 0)
		{
			node = node.Predecessor;
		}
		return node ?? _root.LeftMost;
	}

	private void PerformInsertText(TextAnchorNode beginNode, TextAnchorNode endNode, int length, bool defaultAnchorMovementIsBeforeInsertion)
	{
		List<TextAnchorNode> list = new List<TextAnchorNode>();
		TextAnchorNode textAnchorNode;
		for (textAnchorNode = beginNode; textAnchorNode != endNode; textAnchorNode = textAnchorNode.Successor)
		{
			TextAnchor textAnchor = (TextAnchor)textAnchorNode.Target;
			if (textAnchor == null)
			{
				MarkNodeForDelete(textAnchorNode);
			}
			else if (defaultAnchorMovementIsBeforeInsertion ? (textAnchor.MovementType != AnchorMovementType.AfterInsertion) : (textAnchor.MovementType == AnchorMovementType.BeforeInsertion))
			{
				list.Add(textAnchorNode);
			}
		}
		textAnchorNode = beginNode;
		foreach (TextAnchorNode item in list)
		{
			SwapAnchors(item, textAnchorNode);
			textAnchorNode = textAnchorNode.Successor;
		}
		if (textAnchorNode != null)
		{
			textAnchorNode.Length += length;
			UpdateAugmentedData(textAnchorNode);
		}
	}

	private void SwapAnchors(TextAnchorNode n1, TextAnchorNode n2)
	{
		if (n1 == n2)
		{
			return;
		}
		TextAnchor textAnchor = (TextAnchor)n1.Target;
		TextAnchor textAnchor2 = (TextAnchor)n2.Target;
		if (textAnchor != null || textAnchor2 != null)
		{
			n1.Target = textAnchor2;
			n2.Target = textAnchor;
			if (textAnchor == null)
			{
				_nodesToDelete.Remove(n1);
				MarkNodeForDelete(n2);
				textAnchor2.Node = n1;
			}
			else if (textAnchor2 == null)
			{
				_nodesToDelete.Remove(n2);
				MarkNodeForDelete(n1);
				textAnchor.Node = n2;
			}
			else
			{
				textAnchor.Node = n2;
				textAnchor2.Node = n1;
			}
		}
	}

	public void HandleTextChange(OffsetChangeMapEntry entry, DelayedEvents delayedEvents)
	{
		if (entry.RemovalLength == 0)
		{
			InsertText(entry.Offset, entry.InsertionLength, entry.DefaultAnchorMovementIsBeforeInsertion);
			return;
		}
		int offset = entry.Offset;
		int num = entry.RemovalLength;
		if (_root == null || offset >= _root.TotalLength)
		{
			return;
		}
		TextAnchorNode textAnchorNode = FindNode(ref offset);
		TextAnchorNode textAnchorNode2 = null;
		while (textAnchorNode != null && offset + num > textAnchorNode.Length)
		{
			TextAnchor textAnchor = (TextAnchor)textAnchorNode.Target;
			if (textAnchor != null && (textAnchor.SurviveDeletion || entry.RemovalNeverCausesAnchorDeletion))
			{
				if (textAnchorNode2 == null)
				{
					textAnchorNode2 = textAnchorNode;
				}
				num -= textAnchorNode.Length - offset;
				textAnchorNode.Length = offset;
				offset = 0;
				UpdateAugmentedData(textAnchorNode);
				textAnchorNode = textAnchorNode.Successor;
			}
			else
			{
				TextAnchorNode successor = textAnchorNode.Successor;
				num -= textAnchorNode.Length;
				RemoveNode(textAnchorNode);
				_nodesToDelete.Remove(textAnchorNode);
				textAnchor?.OnDeleted(delayedEvents);
				textAnchorNode = successor;
			}
		}
		if (textAnchorNode != null)
		{
			textAnchorNode.Length -= num;
		}
		if (entry.InsertionLength > 0)
		{
			if (textAnchorNode2 != null)
			{
				PerformInsertText(textAnchorNode2, textAnchorNode, entry.InsertionLength, entry.DefaultAnchorMovementIsBeforeInsertion);
			}
			else if (textAnchorNode != null)
			{
				textAnchorNode.Length += entry.InsertionLength;
			}
		}
		if (textAnchorNode != null)
		{
			UpdateAugmentedData(textAnchorNode);
		}
		DeleteMarkedNodes();
	}

	private void MarkNodeForDelete(TextAnchorNode node)
	{
		if (!_nodesToDelete.Contains(node))
		{
			_nodesToDelete.Add(node);
		}
	}

	private void DeleteMarkedNodes()
	{
		while (_nodesToDelete.Count > 0)
		{
			int index = _nodesToDelete.Count - 1;
			TextAnchorNode textAnchorNode = _nodesToDelete[index];
			TextAnchorNode successor = textAnchorNode.Successor;
			if (successor != null)
			{
				successor.Length += textAnchorNode.Length;
			}
			RemoveNode(textAnchorNode);
			if (successor != null)
			{
				UpdateAugmentedData(successor);
			}
			_nodesToDelete.RemoveAt(index);
		}
	}

	private TextAnchorNode FindNode(ref int offset)
	{
		TextAnchorNode textAnchorNode = _root;
		while (true)
		{
			if (textAnchorNode.Left != null)
			{
				if (offset < textAnchorNode.Left.TotalLength)
				{
					textAnchorNode = textAnchorNode.Left;
					continue;
				}
				offset -= textAnchorNode.Left.TotalLength;
			}
			if (!textAnchorNode.IsAlive)
			{
				MarkNodeForDelete(textAnchorNode);
			}
			if (offset < textAnchorNode.Length)
			{
				return textAnchorNode;
			}
			offset -= textAnchorNode.Length;
			if (textAnchorNode.Right == null)
			{
				break;
			}
			textAnchorNode = textAnchorNode.Right;
		}
		return null;
	}

	private void UpdateAugmentedData(TextAnchorNode n)
	{
		if (!n.IsAlive)
		{
			MarkNodeForDelete(n);
		}
		int num = n.Length;
		if (n.Left != null)
		{
			num += n.Left.TotalLength;
		}
		if (n.Right != null)
		{
			num += n.Right.TotalLength;
		}
		if (n.TotalLength != num)
		{
			n.TotalLength = num;
			if (n.Parent != null)
			{
				UpdateAugmentedData(n.Parent);
			}
		}
	}

	public TextAnchor CreateAnchor(int offset)
	{
		TextAnchor textAnchor = new TextAnchor(_document);
		textAnchor.Node = new TextAnchorNode(textAnchor);
		if (_root == null)
		{
			_root = textAnchor.Node;
			TextAnchorNode root = _root;
			int totalLength = (_root.Length = offset);
			root.TotalLength = totalLength;
		}
		else if (offset >= _root.TotalLength)
		{
			TextAnchorNode node = textAnchor.Node;
			int totalLength = (textAnchor.Node.Length = offset - _root.TotalLength);
			node.TotalLength = totalLength;
			InsertAsRight(_root.RightMost, textAnchor.Node);
		}
		else
		{
			TextAnchorNode textAnchorNode = FindNode(ref offset);
			TextAnchorNode node2 = textAnchor.Node;
			int totalLength = (textAnchor.Node.Length = offset);
			node2.TotalLength = totalLength;
			textAnchorNode.Length -= offset;
			InsertBefore(textAnchorNode, textAnchor.Node);
		}
		DeleteMarkedNodes();
		return textAnchor;
	}

	private void InsertBefore(TextAnchorNode node, TextAnchorNode newNode)
	{
		if (node.Left == null)
		{
			InsertAsLeft(node, newNode);
		}
		else
		{
			InsertAsRight(node.Left.RightMost, newNode);
		}
	}

	private void InsertAsLeft(TextAnchorNode parentNode, TextAnchorNode newNode)
	{
		parentNode.Left = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(TextAnchorNode parentNode, TextAnchorNode newNode)
	{
		parentNode.Right = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(TextAnchorNode node)
	{
		TextAnchorNode parent = node.Parent;
		if (parent == null)
		{
			node.Color = false;
		}
		else
		{
			if (!parent.Color)
			{
				return;
			}
			TextAnchorNode parent2 = parent.Parent;
			TextAnchorNode textAnchorNode = Sibling(parent);
			if (textAnchorNode != null && textAnchorNode.Color)
			{
				parent.Color = false;
				textAnchorNode.Color = false;
				parent2.Color = true;
				FixTreeOnInsert(parent2);
				return;
			}
			if (node == parent.Right && parent == parent2.Left)
			{
				RotateLeft(parent);
				node = node.Left;
			}
			else if (node == parent.Left && parent == parent2.Right)
			{
				RotateRight(parent);
				node = node.Right;
			}
			parent = node.Parent;
			parent2 = parent.Parent;
			parent.Color = false;
			parent2.Color = true;
			if (node == parent.Left && parent == parent2.Left)
			{
				RotateRight(parent2);
			}
			else
			{
				RotateLeft(parent2);
			}
		}
	}

	private void RemoveNode(TextAnchorNode removedNode)
	{
		if (removedNode.Left != null && removedNode.Right != null)
		{
			TextAnchorNode leftMost = removedNode.Right.LeftMost;
			RemoveNode(leftMost);
			ReplaceNode(removedNode, leftMost);
			leftMost.Left = removedNode.Left;
			if (leftMost.Left != null)
			{
				leftMost.Left.Parent = leftMost;
			}
			leftMost.Right = removedNode.Right;
			if (leftMost.Right != null)
			{
				leftMost.Right.Parent = leftMost;
			}
			leftMost.Color = removedNode.Color;
			UpdateAugmentedData(leftMost);
			if (leftMost.Parent != null)
			{
				UpdateAugmentedData(leftMost.Parent);
			}
			return;
		}
		TextAnchorNode parent = removedNode.Parent;
		TextAnchorNode textAnchorNode = removedNode.Left ?? removedNode.Right;
		ReplaceNode(removedNode, textAnchorNode);
		if (parent != null)
		{
			UpdateAugmentedData(parent);
		}
		if (!removedNode.Color)
		{
			if (textAnchorNode != null && textAnchorNode.Color)
			{
				textAnchorNode.Color = false;
			}
			else
			{
				FixTreeOnDelete(textAnchorNode, parent);
			}
		}
	}

	private void FixTreeOnDelete(TextAnchorNode node, TextAnchorNode parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		TextAnchorNode textAnchorNode = Sibling(node, parentNode);
		if (textAnchorNode.Color)
		{
			parentNode.Color = true;
			textAnchorNode.Color = false;
			if (node == parentNode.Left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			textAnchorNode = Sibling(node, parentNode);
		}
		if (!parentNode.Color && !textAnchorNode.Color && !GetColor(textAnchorNode.Left) && !GetColor(textAnchorNode.Right))
		{
			textAnchorNode.Color = true;
			FixTreeOnDelete(parentNode, parentNode.Parent);
			return;
		}
		if (parentNode.Color && !textAnchorNode.Color && !GetColor(textAnchorNode.Left) && !GetColor(textAnchorNode.Right))
		{
			textAnchorNode.Color = true;
			parentNode.Color = false;
			return;
		}
		if (node == parentNode.Left && !textAnchorNode.Color && GetColor(textAnchorNode.Left) && !GetColor(textAnchorNode.Right))
		{
			textAnchorNode.Color = true;
			textAnchorNode.Left.Color = false;
			RotateRight(textAnchorNode);
		}
		else if (node == parentNode.Right && !textAnchorNode.Color && GetColor(textAnchorNode.Right) && !GetColor(textAnchorNode.Left))
		{
			textAnchorNode.Color = true;
			textAnchorNode.Right.Color = false;
			RotateLeft(textAnchorNode);
		}
		textAnchorNode = Sibling(node, parentNode);
		textAnchorNode.Color = parentNode.Color;
		parentNode.Color = false;
		if (node == parentNode.Left)
		{
			if (textAnchorNode.Right != null)
			{
				textAnchorNode.Right.Color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (textAnchorNode.Left != null)
			{
				textAnchorNode.Left.Color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(TextAnchorNode replacedNode, TextAnchorNode newNode)
	{
		if (replacedNode.Parent == null)
		{
			_root = newNode;
		}
		else if (replacedNode.Parent.Left == replacedNode)
		{
			replacedNode.Parent.Left = newNode;
		}
		else
		{
			replacedNode.Parent.Right = newNode;
		}
		if (newNode != null)
		{
			newNode.Parent = replacedNode.Parent;
		}
		replacedNode.Parent = null;
	}

	private void RotateLeft(TextAnchorNode p)
	{
		TextAnchorNode right = p.Right;
		ReplaceNode(p, right);
		p.Right = right.Left;
		if (p.Right != null)
		{
			p.Right.Parent = p;
		}
		right.Left = p;
		p.Parent = right;
		UpdateAugmentedData(p);
		UpdateAugmentedData(right);
	}

	private void RotateRight(TextAnchorNode p)
	{
		TextAnchorNode left = p.Left;
		ReplaceNode(p, left);
		p.Left = left.Right;
		if (p.Left != null)
		{
			p.Left.Parent = p;
		}
		left.Right = p;
		p.Parent = left;
		UpdateAugmentedData(p);
		UpdateAugmentedData(left);
	}

	private static TextAnchorNode Sibling(TextAnchorNode node)
	{
		if (node == node.Parent.Left)
		{
			return node.Parent.Right;
		}
		return node.Parent.Left;
	}

	private static TextAnchorNode Sibling(TextAnchorNode node, TextAnchorNode parentNode)
	{
		if (node == parentNode.Left)
		{
			return parentNode.Right;
		}
		return parentNode.Left;
	}

	private static bool GetColor(TextAnchorNode node)
	{
		return node?.Color ?? false;
	}

	[Conditional("DATACONSISTENCYTEST")]
	internal void CheckProperties()
	{
	}
}
