using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Threading;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public sealed class TextSegmentCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ISegmentTree where T : TextSegment
{
	private TextSegment _root;

	private readonly bool _isConnectedToDocument;

	internal const bool Red = true;

	internal const bool Black = false;

	public T FirstSegment => (T)(_root?.LeftMost);

	public T LastSegment => (T)(_root?.RightMost);

	public int Count { get; private set; }

	bool ICollection<T>.IsReadOnly => false;

	public TextSegmentCollection()
	{
	}

	public TextSegmentCollection(TextDocument textDocument)
	{
		if (textDocument == null)
		{
			throw new ArgumentNullException("textDocument");
		}
		Dispatcher.UIThread.VerifyAccess();
		_isConnectedToDocument = true;
		WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.AddHandler(textDocument, OnDocumentChanged);
	}

	public void Disconnect(TextDocument textDocument)
	{
		if (_isConnectedToDocument)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.RemoveHandler(textDocument, OnDocumentChanged);
		}
	}

	public void UpdateOffsets(DocumentChangeEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		if (_isConnectedToDocument)
		{
			throw new InvalidOperationException("This TextSegmentCollection will automatically update offsets; do not call UpdateOffsets manually!");
		}
		OnDocumentChanged(this, e);
	}

	private void OnDocumentChanged(object sender, DocumentChangeEventArgs e)
	{
		OffsetChangeMap offsetChangeMapOrNull = e.OffsetChangeMapOrNull;
		if (offsetChangeMapOrNull != null)
		{
			foreach (OffsetChangeMapEntry item in offsetChangeMapOrNull)
			{
				UpdateOffsetsInternal(item);
			}
			return;
		}
		UpdateOffsetsInternal(e.CreateSingleChangeMapEntry());
	}

	public void UpdateOffsets(OffsetChangeMapEntry change)
	{
		if (_isConnectedToDocument)
		{
			throw new InvalidOperationException("This TextSegmentCollection will automatically update offsets; do not call UpdateOffsets manually!");
		}
		UpdateOffsetsInternal(change);
	}

	private void UpdateOffsetsInternal(OffsetChangeMapEntry change)
	{
		if (change.RemovalLength == 0)
		{
			InsertText(change.Offset, change.InsertionLength);
		}
		else
		{
			ReplaceText(change);
		}
	}

	private void InsertText(int offset, int length)
	{
		if (length == 0)
		{
			return;
		}
		foreach (T item in FindSegmentsContaining(offset))
		{
			T current = item;
			if (current.StartOffset < offset && offset < current.EndOffset)
			{
				T val = current;
				val.Length = current.Length + length;
			}
		}
		TextSegment textSegment = FindFirstSegmentWithStartAfter(offset);
		if (textSegment != null)
		{
			textSegment.NodeLength += length;
			UpdateAugmentedData(textSegment);
		}
	}

	private void ReplaceText(OffsetChangeMapEntry change)
	{
		int offset = change.Offset;
		foreach (T item in FindOverlappingSegments(offset, change.RemovalLength))
		{
			T current = item;
			if (current.StartOffset <= offset)
			{
				if (current.EndOffset >= offset + change.RemovalLength)
				{
					T val = current;
					val.Length = current.Length + (change.InsertionLength - change.RemovalLength);
				}
				else
				{
					current.Length = offset - current.StartOffset;
				}
			}
			else
			{
				int val2 = current.EndOffset - (offset + change.RemovalLength);
				RemoveSegment(current);
				current.StartOffset = offset + change.RemovalLength;
				current.Length = Math.Max(0, val2);
				AddSegment(current);
			}
		}
		TextSegment textSegment = FindFirstSegmentWithStartAfter(offset + 1);
		if (textSegment != null)
		{
			textSegment.NodeLength += change.InsertionLength - change.RemovalLength;
			UpdateAugmentedData(textSegment);
		}
	}

	public void Add(T item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (item.OwnerTree != null)
		{
			throw new ArgumentException("The segment is already added to a SegmentCollection.");
		}
		AddSegment(item);
	}

	void ISegmentTree.Add(TextSegment s)
	{
		AddSegment(s);
	}

	private void AddSegment(TextSegment node)
	{
		int offset = node.StartOffset;
		node.DistanceToMaxEnd = node.SegmentLength;
		if (_root == null)
		{
			_root = node;
			node.TotalNodeLength = node.NodeLength;
		}
		else if (offset >= _root.TotalNodeLength)
		{
			int nodeLength = (node.TotalNodeLength = offset - _root.TotalNodeLength);
			node.NodeLength = nodeLength;
			InsertAsRight(_root.RightMost, node);
		}
		else
		{
			TextSegment textSegment = FindNode(ref offset);
			int nodeLength = (node.NodeLength = offset);
			node.TotalNodeLength = nodeLength;
			textSegment.NodeLength -= offset;
			InsertBefore(textSegment, node);
		}
		node.OwnerTree = this;
		Count++;
	}

	private void InsertBefore(TextSegment node, TextSegment newNode)
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

	public T GetNextSegment(T segment)
	{
		if (!Contains(segment))
		{
			throw new ArgumentException("segment is not inside the segment tree");
		}
		return (T)segment.Successor;
	}

	public T GetPreviousSegment(T segment)
	{
		if (!Contains(segment))
		{
			throw new ArgumentException("segment is not inside the segment tree");
		}
		return (T)segment.Predecessor;
	}

	public T FindFirstSegmentWithStartAfter(int startOffset)
	{
		if (_root == null)
		{
			return null;
		}
		if (startOffset <= 0)
		{
			return (T)_root.LeftMost;
		}
		TextSegment textSegment = FindNode(ref startOffset);
		while (startOffset == 0)
		{
			TextSegment textSegment2 = ((textSegment == null) ? _root.RightMost : textSegment.Predecessor);
			startOffset += textSegment2.NodeLength;
			textSegment = textSegment2;
		}
		return (T)textSegment;
	}

	private TextSegment FindNode(ref int offset)
	{
		TextSegment textSegment = _root;
		while (true)
		{
			if (textSegment.Left != null)
			{
				if (offset < textSegment.Left.TotalNodeLength)
				{
					textSegment = textSegment.Left;
					continue;
				}
				offset -= textSegment.Left.TotalNodeLength;
			}
			if (offset < textSegment.NodeLength)
			{
				return textSegment;
			}
			offset -= textSegment.NodeLength;
			if (textSegment.Right == null)
			{
				break;
			}
			textSegment = textSegment.Right;
		}
		return null;
	}

	public ReadOnlyCollection<T> FindSegmentsContaining(int offset)
	{
		return FindOverlappingSegments(offset, 0);
	}

	public ReadOnlyCollection<T> FindOverlappingSegments(ISegment segment)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		return FindOverlappingSegments(segment.Offset, segment.Length);
	}

	public ReadOnlyCollection<T> FindOverlappingSegments(int offset, int length)
	{
		ThrowUtil.CheckNotNegative(length, "length");
		List<T> list = new List<T>();
		if (_root != null)
		{
			FindOverlappingSegments(list, _root, offset, offset + length);
		}
		return new ReadOnlyCollection<T>(list);
	}

	private void FindOverlappingSegments(List<T> results, TextSegment node, int low, int high)
	{
		if (high < 0)
		{
			return;
		}
		int num = low - node.NodeLength;
		int num2 = high - node.NodeLength;
		if (node.Left != null)
		{
			num -= node.Left.TotalNodeLength;
			num2 -= node.Left.TotalNodeLength;
		}
		if (node.DistanceToMaxEnd < num)
		{
			return;
		}
		if (node.Left != null)
		{
			FindOverlappingSegments(results, node.Left, low, high);
		}
		if (num2 >= 0)
		{
			if (num <= node.SegmentLength)
			{
				results.Add((T)node);
			}
			if (node.Right != null)
			{
				FindOverlappingSegments(results, node.Right, num, num2);
			}
		}
	}

	private void UpdateAugmentedData(TextSegment node)
	{
		int num = node.NodeLength;
		int num2 = node.SegmentLength;
		if (node.Left != null)
		{
			num += node.Left.TotalNodeLength;
			int num3 = node.Left.DistanceToMaxEnd;
			if (node.Left.Right != null)
			{
				num3 -= node.Left.Right.TotalNodeLength;
			}
			num3 -= node.NodeLength;
			if (num3 > num2)
			{
				num2 = num3;
			}
		}
		if (node.Right != null)
		{
			num += node.Right.TotalNodeLength;
			int distanceToMaxEnd = node.Right.DistanceToMaxEnd;
			distanceToMaxEnd += node.Right.NodeLength;
			if (node.Right.Left != null)
			{
				distanceToMaxEnd += node.Right.Left.TotalNodeLength;
			}
			if (distanceToMaxEnd > num2)
			{
				num2 = distanceToMaxEnd;
			}
		}
		if (node.TotalNodeLength != num || node.DistanceToMaxEnd != num2)
		{
			node.TotalNodeLength = num;
			node.DistanceToMaxEnd = num2;
			if (node.Parent != null)
			{
				UpdateAugmentedData(node.Parent);
			}
		}
	}

	void ISegmentTree.UpdateAugmentedData(TextSegment node)
	{
		UpdateAugmentedData(node);
	}

	public bool Remove(T item)
	{
		if (!Contains(item))
		{
			return false;
		}
		RemoveSegment(item);
		return true;
	}

	void ISegmentTree.Remove(TextSegment s)
	{
		RemoveSegment(s);
	}

	private void RemoveSegment(TextSegment s)
	{
		int startOffset = s.StartOffset;
		TextSegment successor = s.Successor;
		if (successor != null)
		{
			successor.NodeLength += s.NodeLength;
		}
		RemoveNode(s);
		if (successor != null)
		{
			UpdateAugmentedData(successor);
		}
		Disconnect(s, startOffset);
	}

	private void Disconnect(TextSegment s, int offset)
	{
		TextSegment textSegment2 = (s.Parent = null);
		TextSegment left = (s.Right = textSegment2);
		s.Left = left;
		s.OwnerTree = null;
		s.NodeLength = offset;
		Count--;
	}

	public void Clear()
	{
		T[] array = this.ToArray();
		_root = null;
		int num = 0;
		T[] array2 = array;
		foreach (T val in array2)
		{
			num += val.NodeLength;
			Disconnect(val, num);
		}
	}

	[Conditional("DATACONSISTENCYTEST")]
	internal void CheckProperties()
	{
	}

	internal string GetTreeAsString()
	{
		return "Not available in release build.";
	}

	private void InsertAsLeft(TextSegment parentNode, TextSegment newNode)
	{
		parentNode.Left = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(TextSegment parentNode, TextSegment newNode)
	{
		parentNode.Right = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(TextSegment node)
	{
		TextSegment parent = node.Parent;
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
			TextSegment parent2 = parent.Parent;
			TextSegment textSegment = Sibling(parent);
			if (textSegment != null && textSegment.Color)
			{
				parent.Color = false;
				textSegment.Color = false;
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

	private void RemoveNode(TextSegment removedNode)
	{
		if (removedNode.Left != null && removedNode.Right != null)
		{
			TextSegment leftMost = removedNode.Right.LeftMost;
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
		TextSegment parent = removedNode.Parent;
		TextSegment textSegment = removedNode.Left ?? removedNode.Right;
		ReplaceNode(removedNode, textSegment);
		if (parent != null)
		{
			UpdateAugmentedData(parent);
		}
		if (!removedNode.Color)
		{
			if (textSegment != null && textSegment.Color)
			{
				textSegment.Color = false;
			}
			else
			{
				FixTreeOnDelete(textSegment, parent);
			}
		}
	}

	private void FixTreeOnDelete(TextSegment node, TextSegment parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		TextSegment textSegment = Sibling(node, parentNode);
		if (textSegment.Color)
		{
			parentNode.Color = true;
			textSegment.Color = false;
			if (node == parentNode.Left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			textSegment = Sibling(node, parentNode);
		}
		if (!parentNode.Color && !textSegment.Color && !GetColor(textSegment.Left) && !GetColor(textSegment.Right))
		{
			textSegment.Color = true;
			FixTreeOnDelete(parentNode, parentNode.Parent);
			return;
		}
		if (parentNode.Color && !textSegment.Color && !GetColor(textSegment.Left) && !GetColor(textSegment.Right))
		{
			textSegment.Color = true;
			parentNode.Color = false;
			return;
		}
		if (node == parentNode.Left && !textSegment.Color && GetColor(textSegment.Left) && !GetColor(textSegment.Right))
		{
			textSegment.Color = true;
			textSegment.Left.Color = false;
			RotateRight(textSegment);
		}
		else if (node == parentNode.Right && !textSegment.Color && GetColor(textSegment.Right) && !GetColor(textSegment.Left))
		{
			textSegment.Color = true;
			textSegment.Right.Color = false;
			RotateLeft(textSegment);
		}
		textSegment = Sibling(node, parentNode);
		textSegment.Color = parentNode.Color;
		parentNode.Color = false;
		if (node == parentNode.Left)
		{
			if (textSegment.Right != null)
			{
				textSegment.Right.Color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (textSegment.Left != null)
			{
				textSegment.Left.Color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(TextSegment replacedNode, TextSegment newNode)
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

	private void RotateLeft(TextSegment p)
	{
		TextSegment right = p.Right;
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

	private void RotateRight(TextSegment p)
	{
		TextSegment left = p.Left;
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

	private static TextSegment Sibling(TextSegment node)
	{
		if (node != node.Parent.Left)
		{
			return node.Parent.Left;
		}
		return node.Parent.Right;
	}

	private static TextSegment Sibling(TextSegment node, TextSegment parentNode)
	{
		if (node == parentNode.Left)
		{
			return parentNode.Right;
		}
		return parentNode.Left;
	}

	private static bool GetColor(TextSegment node)
	{
		return node?.Color ?? false;
	}

	public bool Contains(T item)
	{
		if (item != null)
		{
			return item.OwnerTree == this;
		}
		return false;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Length < Count)
		{
			throw new ArgumentException("The array is too small", "array");
		}
		if (arrayIndex < 0 || arrayIndex + Count > array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Value must be between 0 and " + (array.Length - Count));
		}
		using IEnumerator<T> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			array[arrayIndex++] = current;
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (_root != null)
		{
			for (TextSegment current = _root.LeftMost; current != null; current = current.Successor)
			{
				yield return (T)current;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
