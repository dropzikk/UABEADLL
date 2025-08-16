using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Threading;

namespace AvaloniaEdit.Document;

internal sealed class DocumentLineTree : IList<DocumentLine>, ICollection<DocumentLine>, IEnumerable<DocumentLine>, IEnumerable
{
	private readonly TextDocument _document;

	private DocumentLine _root;

	internal const bool Red = true;

	internal const bool Black = false;

	public int LineCount => _root.NodeTotalCount;

	DocumentLine IList<DocumentLine>.this[int index]
	{
		get
		{
			Dispatcher.UIThread.VerifyAccess();
			return GetByNumber(1 + index);
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	int ICollection<DocumentLine>.Count
	{
		get
		{
			Dispatcher.UIThread.VerifyAccess();
			return LineCount;
		}
	}

	bool ICollection<DocumentLine>.IsReadOnly => true;

	public DocumentLineTree(TextDocument document)
	{
		_document = document;
		DocumentLine documentLine = new DocumentLine(document);
		_root = documentLine.InitLineNode();
	}

	internal static void UpdateAfterChildrenChange(DocumentLine node)
	{
		int num = 1;
		int num2 = node.TotalLength;
		if (node.Left != null)
		{
			num += node.Left.NodeTotalCount;
			num2 += node.Left.NodeTotalLength;
		}
		if (node.Right != null)
		{
			num += node.Right.NodeTotalCount;
			num2 += node.Right.NodeTotalLength;
		}
		if (num != node.NodeTotalCount || num2 != node.NodeTotalLength)
		{
			node.NodeTotalCount = num;
			node.NodeTotalLength = num2;
			if (node.Parent != null)
			{
				UpdateAfterChildrenChange(node.Parent);
			}
		}
	}

	private static void UpdateAfterRotateLeft(DocumentLine node)
	{
		UpdateAfterChildrenChange(node);
	}

	private static void UpdateAfterRotateRight(DocumentLine node)
	{
		UpdateAfterChildrenChange(node);
	}

	public void RebuildTree(List<DocumentLine> documentLines)
	{
		DocumentLine[] array = new DocumentLine[documentLines.Count];
		for (int i = 0; i < documentLines.Count; i++)
		{
			DocumentLine documentLine = documentLines[i].InitLineNode();
			array[i] = documentLine;
		}
		int treeHeight = GetTreeHeight(array.Length);
		_root = BuildTree(array, 0, array.Length, treeHeight);
		_root.Color = false;
	}

	internal static int GetTreeHeight(int size)
	{
		if (size == 0)
		{
			return 0;
		}
		return GetTreeHeight(size / 2) + 1;
	}

	private DocumentLine BuildTree(DocumentLine[] nodes, int start, int end, int subtreeHeight)
	{
		if (start == end)
		{
			return null;
		}
		int num = (start + end) / 2;
		DocumentLine documentLine = nodes[num];
		documentLine.Left = BuildTree(nodes, start, num, subtreeHeight - 1);
		documentLine.Right = BuildTree(nodes, num + 1, end, subtreeHeight - 1);
		if (documentLine.Left != null)
		{
			documentLine.Left.Parent = documentLine;
		}
		if (documentLine.Right != null)
		{
			documentLine.Right.Parent = documentLine;
		}
		if (subtreeHeight == 1)
		{
			documentLine.Color = true;
		}
		UpdateAfterChildrenChange(documentLine);
		return documentLine;
	}

	private DocumentLine GetNodeByIndex(int index)
	{
		DocumentLine documentLine = _root;
		while (true)
		{
			if (documentLine.Left != null && index < documentLine.Left.NodeTotalCount)
			{
				documentLine = documentLine.Left;
				continue;
			}
			if (documentLine.Left != null)
			{
				index -= documentLine.Left.NodeTotalCount;
			}
			if (index == 0)
			{
				break;
			}
			index--;
			documentLine = documentLine.Right;
		}
		return documentLine;
	}

	internal static int GetIndexFromNode(DocumentLine node)
	{
		int num = node.Left?.NodeTotalCount ?? 0;
		while (node.Parent != null)
		{
			if (node == node.Parent.Right)
			{
				if (node.Parent.Left != null)
				{
					num += node.Parent.Left.NodeTotalCount;
				}
				num++;
			}
			node = node.Parent;
		}
		return num;
	}

	private DocumentLine GetNodeByOffset(int offset)
	{
		if (offset == _root.NodeTotalLength)
		{
			return _root.RightMost;
		}
		DocumentLine documentLine = _root;
		while (true)
		{
			if (documentLine.Left != null && offset < documentLine.Left.NodeTotalLength)
			{
				documentLine = documentLine.Left;
				continue;
			}
			if (documentLine.Left != null)
			{
				offset -= documentLine.Left.NodeTotalLength;
			}
			offset -= documentLine.TotalLength;
			if (offset < 0)
			{
				break;
			}
			documentLine = documentLine.Right;
		}
		return documentLine;
	}

	internal static int GetOffsetFromNode(DocumentLine node)
	{
		int num = node.Left?.NodeTotalLength ?? 0;
		while (node.Parent != null)
		{
			if (node == node.Parent.Right)
			{
				if (node.Parent.Left != null)
				{
					num += node.Parent.Left.NodeTotalLength;
				}
				num += node.Parent.TotalLength;
			}
			node = node.Parent;
		}
		return num;
	}

	public DocumentLine GetByNumber(int number)
	{
		return GetNodeByIndex(number - 1);
	}

	public DocumentLine GetByOffset(int offset)
	{
		return GetNodeByOffset(offset);
	}

	public void RemoveLine(DocumentLine line)
	{
		RemoveNode(line);
		line.IsDeleted = true;
	}

	public DocumentLine InsertLineAfter(DocumentLine line, int totalLength)
	{
		DocumentLine documentLine = new DocumentLine(_document)
		{
			TotalLength = totalLength
		};
		InsertAfter(line, documentLine);
		return documentLine;
	}

	private void InsertAfter(DocumentLine node, DocumentLine newLine)
	{
		DocumentLine newNode = newLine.InitLineNode();
		if (node.Right == null)
		{
			InsertAsRight(node, newNode);
		}
		else
		{
			InsertAsLeft(node.Right.LeftMost, newNode);
		}
	}

	private void InsertAsLeft(DocumentLine parentNode, DocumentLine newNode)
	{
		parentNode.Left = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAfterChildrenChange(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(DocumentLine parentNode, DocumentLine newNode)
	{
		parentNode.Right = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAfterChildrenChange(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(DocumentLine node)
	{
		DocumentLine parent = node.Parent;
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
			DocumentLine parent2 = parent.Parent;
			DocumentLine documentLine = Sibling(parent);
			if (documentLine != null && documentLine.Color)
			{
				parent.Color = false;
				documentLine.Color = false;
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

	private void RemoveNode(DocumentLine removedNode)
	{
		if (removedNode.Left != null && removedNode.Right != null)
		{
			DocumentLine leftMost = removedNode.Right.LeftMost;
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
			UpdateAfterChildrenChange(leftMost);
			if (leftMost.Parent != null)
			{
				UpdateAfterChildrenChange(leftMost.Parent);
			}
			return;
		}
		DocumentLine parent = removedNode.Parent;
		DocumentLine documentLine = removedNode.Left ?? removedNode.Right;
		ReplaceNode(removedNode, documentLine);
		if (parent != null)
		{
			UpdateAfterChildrenChange(parent);
		}
		if (!removedNode.Color)
		{
			if (documentLine != null && documentLine.Color)
			{
				documentLine.Color = false;
			}
			else
			{
				FixTreeOnDelete(documentLine, parent);
			}
		}
	}

	private void FixTreeOnDelete(DocumentLine node, DocumentLine parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		DocumentLine documentLine = Sibling(node, parentNode);
		if (documentLine.Color)
		{
			parentNode.Color = true;
			documentLine.Color = false;
			if (node == parentNode.Left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			documentLine = Sibling(node, parentNode);
		}
		if (!parentNode.Color && !documentLine.Color && !GetColor(documentLine.Left) && !GetColor(documentLine.Right))
		{
			documentLine.Color = true;
			FixTreeOnDelete(parentNode, parentNode.Parent);
			return;
		}
		if (parentNode.Color && !documentLine.Color && !GetColor(documentLine.Left) && !GetColor(documentLine.Right))
		{
			documentLine.Color = true;
			parentNode.Color = false;
			return;
		}
		if (node == parentNode.Left && !documentLine.Color && GetColor(documentLine.Left) && !GetColor(documentLine.Right))
		{
			documentLine.Color = true;
			documentLine.Left.Color = false;
			RotateRight(documentLine);
		}
		else if (node == parentNode.Right && !documentLine.Color && GetColor(documentLine.Right) && !GetColor(documentLine.Left))
		{
			documentLine.Color = true;
			documentLine.Right.Color = false;
			RotateLeft(documentLine);
		}
		documentLine = Sibling(node, parentNode);
		documentLine.Color = parentNode.Color;
		parentNode.Color = false;
		if (node == parentNode.Left)
		{
			if (documentLine.Right != null)
			{
				documentLine.Right.Color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (documentLine.Left != null)
			{
				documentLine.Left.Color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(DocumentLine replacedNode, DocumentLine newNode)
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

	private void RotateLeft(DocumentLine p)
	{
		DocumentLine right = p.Right;
		ReplaceNode(p, right);
		p.Right = right.Left;
		if (p.Right != null)
		{
			p.Right.Parent = p;
		}
		right.Left = p;
		p.Parent = right;
		UpdateAfterRotateLeft(p);
	}

	private void RotateRight(DocumentLine p)
	{
		DocumentLine left = p.Left;
		ReplaceNode(p, left);
		p.Left = left.Right;
		if (p.Left != null)
		{
			p.Left.Parent = p;
		}
		left.Right = p;
		p.Parent = left;
		UpdateAfterRotateRight(p);
	}

	private static DocumentLine Sibling(DocumentLine node)
	{
		if (node == node.Parent.Left)
		{
			return node.Parent.Right;
		}
		return node.Parent.Left;
	}

	private static DocumentLine Sibling(DocumentLine node, DocumentLine parentNode)
	{
		if (node == parentNode.Left)
		{
			return parentNode.Right;
		}
		return parentNode.Left;
	}

	private static bool GetColor(DocumentLine node)
	{
		return node?.Color ?? false;
	}

	int IList<DocumentLine>.IndexOf(DocumentLine item)
	{
		Dispatcher.UIThread.VerifyAccess();
		if (item == null || item.IsDeleted)
		{
			return -1;
		}
		int num = item.LineNumber - 1;
		if (num < LineCount && GetNodeByIndex(num) == item)
		{
			return num;
		}
		return -1;
	}

	void IList<DocumentLine>.Insert(int index, DocumentLine item)
	{
		throw new NotSupportedException();
	}

	void IList<DocumentLine>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	void ICollection<DocumentLine>.Add(DocumentLine item)
	{
		throw new NotSupportedException();
	}

	void ICollection<DocumentLine>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<DocumentLine>.Contains(DocumentLine item)
	{
		return ((IList<DocumentLine>)this).IndexOf(item) >= 0;
	}

	void ICollection<DocumentLine>.CopyTo(DocumentLine[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Length < LineCount)
		{
			throw new ArgumentException("The array is too small", "array");
		}
		if (arrayIndex < 0 || arrayIndex + LineCount > array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Value must be between 0 and " + (array.Length - LineCount));
		}
		using IEnumerator<DocumentLine> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			DocumentLine current = enumerator.Current;
			array[arrayIndex++] = current;
		}
	}

	bool ICollection<DocumentLine>.Remove(DocumentLine item)
	{
		throw new NotSupportedException();
	}

	public IEnumerator<DocumentLine> GetEnumerator()
	{
		Dispatcher.UIThread.VerifyAccess();
		return Enumerate();
	}

	private IEnumerator<DocumentLine> Enumerate()
	{
		Dispatcher.UIThread.VerifyAccess();
		for (DocumentLine line = _root.LeftMost; line != null; line = line.NextLine)
		{
			yield return line;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
