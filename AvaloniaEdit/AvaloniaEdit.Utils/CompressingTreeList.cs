using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace AvaloniaEdit.Utils;

internal sealed class CompressingTreeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private sealed class Node
	{
		internal Node Left { get; set; }

		internal Node Right { get; set; }

		internal Node Parent { get; set; }

		internal bool Color { get; set; }

		internal int Count { get; set; }

		internal int TotalCount { get; set; }

		internal T Value { get; set; }

		internal Node LeftMost
		{
			get
			{
				Node node = this;
				while (node.Left != null)
				{
					node = node.Left;
				}
				return node;
			}
		}

		internal Node RightMost
		{
			get
			{
				Node node = this;
				while (node.Right != null)
				{
					node = node.Right;
				}
				return node;
			}
		}

		internal Node Predecessor
		{
			get
			{
				if (Left != null)
				{
					return Left.RightMost;
				}
				Node node = this;
				Node node2;
				do
				{
					node2 = node;
					node = node.Parent;
				}
				while (node != null && node.Left == node2);
				return node;
			}
		}

		internal Node Successor
		{
			get
			{
				if (Right != null)
				{
					return Right.LeftMost;
				}
				Node node = this;
				Node node2;
				do
				{
					node2 = node;
					node = node.Parent;
				}
				while (node != null && node.Right == node2);
				return node;
			}
		}

		public Node(T value, int count)
		{
			Value = value;
			Count = count;
			TotalCount = count;
		}

		public override string ToString()
		{
			return "[TotalCount=" + TotalCount + " Count=" + Count + " Value=" + Value?.ToString() + "]";
		}
	}

	private readonly Func<T, T, bool> _comparisonFunc;

	private Node _root;

	internal const bool Red = true;

	internal const bool Black = false;

	public T this[int index]
	{
		get
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
			}
			return GetNode(ref index).Value;
		}
		set
		{
			RemoveAt(index);
			Insert(index, value);
		}
	}

	public int Count
	{
		get
		{
			if (_root != null)
			{
				return _root.TotalCount;
			}
			return 0;
		}
	}

	bool ICollection<T>.IsReadOnly => false;

	public CompressingTreeList(IEqualityComparer<T> equalityComparer)
	{
		if (equalityComparer == null)
		{
			throw new ArgumentNullException("equalityComparer");
		}
		_comparisonFunc = equalityComparer.Equals;
	}

	public CompressingTreeList(Func<T, T, bool> comparisonFunc)
	{
		_comparisonFunc = comparisonFunc ?? throw new ArgumentNullException("comparisonFunc");
	}

	public void InsertRange(int index, int count, T item)
	{
		if (index < 0 || index > Count)
		{
			throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + Count);
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", count, "Value must not be negative");
		}
		if (count == 0)
		{
			return;
		}
		if (Count + count < 0)
		{
			throw new OverflowException("Cannot insert elements: total number of elements must not exceed int.MaxValue.");
		}
		if (_root == null)
		{
			_root = new Node(item, count);
			return;
		}
		Node node = GetNode(ref index);
		if (_comparisonFunc(node.Value, item))
		{
			node.Count += count;
			UpdateAugmentedData(node);
		}
		else if (index == node.Count)
		{
			InsertAsRight(node, new Node(item, count));
		}
		else if (index == 0)
		{
			Node predecessor = node.Predecessor;
			if (predecessor != null && _comparisonFunc(predecessor.Value, item))
			{
				predecessor.Count += count;
				UpdateAugmentedData(predecessor);
			}
			else
			{
				InsertBefore(node, new Node(item, count));
			}
		}
		else
		{
			node.Count -= index;
			InsertBefore(node, new Node(node.Value, index));
			InsertBefore(node, new Node(item, count));
			UpdateAugmentedData(node);
		}
	}

	private void InsertBefore(Node node, Node newNode)
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

	public void RemoveRange(int index, int count)
	{
		if (index < 0 || index > Count)
		{
			throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + Count);
		}
		if (count < 0 || index + count > Count)
		{
			throw new ArgumentOutOfRangeException("count", count, "0 <= length, index(" + index + ")+count <= " + Count);
		}
		if (count == 0)
		{
			return;
		}
		Node node = GetNode(ref index);
		if (index + count < node.Count)
		{
			node.Count -= count;
			UpdateAugmentedData(node);
			return;
		}
		Node node2;
		if (index > 0)
		{
			count -= node.Count - index;
			node.Count = index;
			UpdateAugmentedData(node);
			node2 = node;
			node = node.Successor;
		}
		else
		{
			node2 = node.Predecessor;
		}
		while (node != null && count >= node.Count)
		{
			count -= node.Count;
			Node successor = node.Successor;
			RemoveNode(node);
			node = successor;
		}
		if (count > 0)
		{
			node.Count -= count;
			UpdateAugmentedData(node);
		}
		if (node != null && node2 != null && _comparisonFunc(node2.Value, node.Value))
		{
			node2.Count += node.Count;
			RemoveNode(node);
			UpdateAugmentedData(node2);
		}
	}

	public void SetRange(int index, int count, T item)
	{
		RemoveRange(index, count);
		InsertRange(index, count, item);
	}

	private Node GetNode(ref int index)
	{
		Node node = _root;
		while (true)
		{
			if (node.Left != null && index < node.Left.TotalCount)
			{
				node = node.Left;
				continue;
			}
			if (node.Left != null)
			{
				index -= node.Left.TotalCount;
			}
			if (index < node.Count || node.Right == null)
			{
				break;
			}
			index -= node.Count;
			node = node.Right;
		}
		return node;
	}

	private void UpdateAugmentedData(Node node)
	{
		int num = node.Count;
		if (node.Left != null)
		{
			num += node.Left.TotalCount;
		}
		if (node.Right != null)
		{
			num += node.Right.TotalCount;
		}
		if (node.TotalCount != num)
		{
			node.TotalCount = num;
			if (node.Parent != null)
			{
				UpdateAugmentedData(node.Parent);
			}
		}
	}

	public int IndexOf(T item)
	{
		int num = 0;
		if (_root != null)
		{
			for (Node node = _root.LeftMost; node != null; node = node.Successor)
			{
				if (_comparisonFunc(node.Value, item))
				{
					return num;
				}
				num += node.Count;
			}
		}
		return -1;
	}

	public int GetStartOfRun(int index)
	{
		if (index < 0 || index >= Count)
		{
			throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
		}
		int index2 = index;
		GetNode(ref index2);
		return index - index2;
	}

	public int GetEndOfRun(int index)
	{
		if (index < 0 || index >= Count)
		{
			throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
		}
		int index2 = index;
		int count = GetNode(ref index2).Count;
		return index - index2 + count;
	}

	public void Transform(Func<T, T> converter)
	{
		if (_root == null)
		{
			return;
		}
		Node node = null;
		for (Node node2 = _root.LeftMost; node2 != null; node2 = node2.Successor)
		{
			node2.Value = converter(node2.Value);
			if (node != null && _comparisonFunc(node.Value, node2.Value))
			{
				node2.Count += node.Count;
				UpdateAugmentedData(node2);
				RemoveNode(node);
			}
			node = node2;
		}
	}

	public void TransformRange(int index, int length, Func<T, T> converter)
	{
		if (_root != null)
		{
			int num = index + length;
			int num2 = index;
			while (num2 < num)
			{
				int num3 = Math.Min(num, GetEndOfRun(num2));
				T arg = this[num2];
				T item = converter(arg);
				SetRange(num2, num3 - num2, item);
				num2 = num3;
			}
		}
	}

	public void Insert(int index, T item)
	{
		InsertRange(index, 1, item);
	}

	public void RemoveAt(int index)
	{
		RemoveRange(index, 1);
	}

	public void Add(T item)
	{
		InsertRange(Count, 1, item);
	}

	public void Clear()
	{
		_root = null;
	}

	public bool Contains(T item)
	{
		return IndexOf(item) >= 0;
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

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num >= 0)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (_root == null)
		{
			yield break;
		}
		for (Node n = _root.LeftMost; n != null; n = n.Successor)
		{
			for (int i = 0; i < n.Count; i++)
			{
				yield return n.Value;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private void InsertAsLeft(Node parentNode, Node newNode)
	{
		parentNode.Left = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(Node parentNode, Node newNode)
	{
		parentNode.Right = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(Node node)
	{
		Node parent = node.Parent;
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
			Node parent2 = parent.Parent;
			Node node2 = Sibling(parent);
			if (node2 != null && node2.Color)
			{
				parent.Color = false;
				node2.Color = false;
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

	private void RemoveNode(Node removedNode)
	{
		if (removedNode.Left != null && removedNode.Right != null)
		{
			Node leftMost = removedNode.Right.LeftMost;
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
		Node parent = removedNode.Parent;
		Node node = removedNode.Left ?? removedNode.Right;
		ReplaceNode(removedNode, node);
		if (parent != null)
		{
			UpdateAugmentedData(parent);
		}
		if (!removedNode.Color)
		{
			if (node != null && node.Color)
			{
				node.Color = false;
			}
			else
			{
				FixTreeOnDelete(node, parent);
			}
		}
	}

	private void FixTreeOnDelete(Node node, Node parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		Node node2 = Sibling(node, parentNode);
		if (node2.Color)
		{
			parentNode.Color = true;
			node2.Color = false;
			if (node == parentNode.Left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			node2 = Sibling(node, parentNode);
		}
		if (!parentNode.Color && !node2.Color && !GetColor(node2.Left) && !GetColor(node2.Right))
		{
			node2.Color = true;
			FixTreeOnDelete(parentNode, parentNode.Parent);
			return;
		}
		if (parentNode.Color && !node2.Color && !GetColor(node2.Left) && !GetColor(node2.Right))
		{
			node2.Color = true;
			parentNode.Color = false;
			return;
		}
		if (node == parentNode.Left && !node2.Color && GetColor(node2.Left) && !GetColor(node2.Right))
		{
			node2.Color = true;
			node2.Left.Color = false;
			RotateRight(node2);
		}
		else if (node == parentNode.Right && !node2.Color && GetColor(node2.Right) && !GetColor(node2.Left))
		{
			node2.Color = true;
			node2.Right.Color = false;
			RotateLeft(node2);
		}
		node2 = Sibling(node, parentNode);
		node2.Color = parentNode.Color;
		parentNode.Color = false;
		if (node == parentNode.Left)
		{
			if (node2.Right != null)
			{
				node2.Right.Color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (node2.Left != null)
			{
				node2.Left.Color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(Node replacedNode, Node newNode)
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

	private void RotateLeft(Node p)
	{
		Node right = p.Right;
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

	private void RotateRight(Node p)
	{
		Node left = p.Left;
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

	private static Node Sibling(Node node)
	{
		if (node == node.Parent.Left)
		{
			return node.Parent.Right;
		}
		return node.Parent.Left;
	}

	private static Node Sibling(Node node, Node parentNode)
	{
		if (node == parentNode.Left)
		{
			return parentNode.Right;
		}
		return parentNode.Left;
	}

	private static bool GetColor(Node node)
	{
		return node?.Color ?? false;
	}

	[Conditional("DATACONSISTENCYTEST")]
	internal void CheckProperties()
	{
	}

	internal string GetTreeAsString()
	{
		return "Not available in release build.";
	}
}
