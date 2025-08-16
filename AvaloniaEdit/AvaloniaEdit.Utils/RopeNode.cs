using System;
using System.Diagnostics;

namespace AvaloniaEdit.Utils;

internal class RopeNode<T>
{
	internal const int NodeSize = 256;

	private volatile bool _isShared;

	internal int Length;

	internal byte Height;

	internal static RopeNode<T> EmptyRopeNode { get; } = new RopeNode<T>(isShared: true)
	{
		Contents = new T[256]
	};

	internal RopeNode<T> Left { get; set; }

	internal RopeNode<T> Right { get; set; }

	internal T[] Contents { get; set; }

	internal int Balance => Right.Height - Left.Height;

	public RopeNode()
	{
	}

	protected RopeNode(bool isShared)
	{
		_isShared = isShared;
	}

	[Conditional("DATACONSISTENCYTEST")]
	internal void CheckInvariants()
	{
		if (Height == 0)
		{
			_ = Contents;
		}
		else
		{
			_ = _isShared;
		}
	}

	internal RopeNode<T> Clone()
	{
		if (Height == 0)
		{
			if (Contents == null)
			{
				return GetContentNode().Clone();
			}
			T[] array = new T[256];
			Contents.CopyTo(array, 0);
			return new RopeNode<T>
			{
				Length = Length,
				Contents = array
			};
		}
		return new RopeNode<T>
		{
			Left = Left,
			Right = Right,
			Length = Length,
			Height = Height
		};
	}

	internal RopeNode<T> CloneIfShared()
	{
		if (_isShared)
		{
			return Clone();
		}
		return this;
	}

	internal void Publish()
	{
		if (!_isShared)
		{
			Left?.Publish();
			Right?.Publish();
			_isShared = true;
		}
	}

	internal static RopeNode<T> CreateFromArray(T[] arr, int index, int length)
	{
		if (length == 0)
		{
			return EmptyRopeNode;
		}
		return CreateNodes(length).StoreElements(0, arr, index, length);
	}

	internal static RopeNode<T> CreateNodes(int totalLength)
	{
		return CreateNodes((totalLength + 256 - 1) / 256, totalLength);
	}

	private static RopeNode<T> CreateNodes(int leafCount, int totalLength)
	{
		RopeNode<T> ropeNode = new RopeNode<T>
		{
			Length = totalLength
		};
		if (leafCount == 1)
		{
			ropeNode.Contents = new T[256];
		}
		else
		{
			int num = leafCount / 2;
			int num2 = leafCount - num;
			int num3 = num2 * 256;
			ropeNode.Left = CreateNodes(num2, num3);
			ropeNode.Right = CreateNodes(num, totalLength - num3);
			ropeNode.Height = (byte)(1 + Math.Max(ropeNode.Left.Height, ropeNode.Right.Height));
		}
		return ropeNode;
	}

	internal void Rebalance()
	{
		if (Left == null)
		{
			return;
		}
		while (Math.Abs(Balance) > 1)
		{
			if (Balance > 1)
			{
				if (Right.Balance < 0)
				{
					Right = Right.CloneIfShared();
					Right.RotateRight();
				}
				RotateLeft();
				Left.Rebalance();
			}
			else if (Balance < -1)
			{
				if (Left.Balance > 0)
				{
					Left = Left.CloneIfShared();
					Left.RotateLeft();
				}
				RotateRight();
				Right.Rebalance();
			}
		}
		Height = (byte)(1 + Math.Max(Left.Height, Right.Height));
	}

	private void RotateLeft()
	{
		RopeNode<T> left = Left;
		RopeNode<T> left2 = Right.Left;
		RopeNode<T> right = Right.Right;
		Left = (Right._isShared ? new RopeNode<T>() : Right);
		Left.Left = left;
		Left.Right = left2;
		Left.Length = left.Length + left2.Length;
		Left.Height = (byte)(1 + Math.Max(left.Height, left2.Height));
		Right = right;
		Left.MergeIfPossible();
	}

	private void RotateRight()
	{
		RopeNode<T> left = Left.Left;
		RopeNode<T> right = Left.Right;
		RopeNode<T> right2 = Right;
		Right = (Left._isShared ? new RopeNode<T>() : Left);
		Right.Left = right;
		Right.Right = right2;
		Right.Length = right.Length + right2.Length;
		Right.Height = (byte)(1 + Math.Max(right.Height, right2.Height));
		Left = left;
		Right.MergeIfPossible();
	}

	private void MergeIfPossible()
	{
		if (Length <= 256)
		{
			Height = 0;
			int length = Left.Length;
			if (Left._isShared)
			{
				Contents = new T[256];
				Left.CopyTo(0, Contents, 0, length);
			}
			else
			{
				Contents = Left.Contents;
			}
			Left = null;
			Right.CopyTo(0, Contents, length, Right.Length);
			Right = null;
		}
	}

	internal RopeNode<T> StoreElements(int index, T[] array, int arrayIndex, int count)
	{
		RopeNode<T> ropeNode = CloneIfShared();
		if (ropeNode.Height == 0)
		{
			Array.Copy(array, arrayIndex, ropeNode.Contents, index, count);
		}
		else
		{
			if (index + count <= ropeNode.Left.Length)
			{
				ropeNode.Left = ropeNode.Left.StoreElements(index, array, arrayIndex, count);
			}
			else if (index >= Left.Length)
			{
				ropeNode.Right = ropeNode.Right.StoreElements(index - ropeNode.Left.Length, array, arrayIndex, count);
			}
			else
			{
				int num = ropeNode.Left.Length - index;
				ropeNode.Left = ropeNode.Left.StoreElements(index, array, arrayIndex, num);
				ropeNode.Right = ropeNode.Right.StoreElements(0, array, arrayIndex + num, count - num);
			}
			ropeNode.Rebalance();
		}
		return ropeNode;
	}

	internal void CopyTo(int index, T[] array, int arrayIndex, int count)
	{
		if (Height == 0)
		{
			if (Contents == null)
			{
				GetContentNode().CopyTo(index, array, arrayIndex, count);
			}
			else
			{
				Array.Copy(Contents, index, array, arrayIndex, count);
			}
		}
		else if (index + count <= Left.Length)
		{
			Left.CopyTo(index, array, arrayIndex, count);
		}
		else if (index >= Left.Length)
		{
			Right.CopyTo(index - Left.Length, array, arrayIndex, count);
		}
		else
		{
			int num = Left.Length - index;
			Left.CopyTo(index, array, arrayIndex, num);
			Right.CopyTo(0, array, arrayIndex + num, count - num);
		}
	}

	internal RopeNode<T> SetElement(int offset, T value)
	{
		RopeNode<T> ropeNode = CloneIfShared();
		if (ropeNode.Height == 0)
		{
			ropeNode.Contents[offset] = value;
		}
		else
		{
			if (offset < ropeNode.Left.Length)
			{
				ropeNode.Left = ropeNode.Left.SetElement(offset, value);
			}
			else
			{
				ropeNode.Right = ropeNode.Right.SetElement(offset - ropeNode.Left.Length, value);
			}
			ropeNode.Rebalance();
		}
		return ropeNode;
	}

	internal static RopeNode<T> Concat(RopeNode<T> left, RopeNode<T> right)
	{
		if (left.Length == 0)
		{
			return right;
		}
		if (right.Length == 0)
		{
			return left;
		}
		if (left.Length + right.Length <= 256)
		{
			left = left.CloneIfShared();
			right.CopyTo(0, left.Contents, left.Length, right.Length);
			left.Length += right.Length;
			return left;
		}
		RopeNode<T> ropeNode = new RopeNode<T>();
		ropeNode.Left = left;
		ropeNode.Right = right;
		ropeNode.Length = left.Length + right.Length;
		ropeNode.Rebalance();
		return ropeNode;
	}

	private RopeNode<T> SplitAfter(int offset)
	{
		RopeNode<T> ropeNode = new RopeNode<T>
		{
			Contents = new T[256],
			Length = Length - offset
		};
		Array.Copy(Contents, offset, ropeNode.Contents, 0, ropeNode.Length);
		Length = offset;
		return ropeNode;
	}

	internal RopeNode<T> Insert(int offset, RopeNode<T> newElements)
	{
		if (offset == 0)
		{
			return Concat(newElements, this);
		}
		if (offset == Length)
		{
			return Concat(this, newElements);
		}
		RopeNode<T> ropeNode = CloneIfShared();
		if (ropeNode.Height == 0)
		{
			return Concat(right: ropeNode.SplitAfter(offset), left: Concat(ropeNode, newElements));
		}
		if (offset < ropeNode.Left.Length)
		{
			ropeNode.Left = ropeNode.Left.Insert(offset, newElements);
		}
		else
		{
			ropeNode.Right = ropeNode.Right.Insert(offset - ropeNode.Left.Length, newElements);
		}
		ropeNode.Length += newElements.Length;
		ropeNode.Rebalance();
		return ropeNode;
	}

	internal RopeNode<T> Insert(int offset, T[] array, int arrayIndex, int count)
	{
		if (Length + count < 256)
		{
			RopeNode<T> ropeNode = CloneIfShared();
			int num = ropeNode.Length - offset;
			T[] contents = ropeNode.Contents;
			for (int num2 = num; num2 >= 0; num2--)
			{
				contents[num2 + offset + count] = contents[num2 + offset];
			}
			Array.Copy(array, arrayIndex, contents, offset, count);
			ropeNode.Length += count;
			return ropeNode;
		}
		if (Height == 0)
		{
			return Insert(offset, CreateFromArray(array, arrayIndex, count));
		}
		RopeNode<T> ropeNode2 = CloneIfShared();
		if (offset < ropeNode2.Left.Length)
		{
			ropeNode2.Left = ropeNode2.Left.Insert(offset, array, arrayIndex, count);
		}
		else
		{
			ropeNode2.Right = ropeNode2.Right.Insert(offset - ropeNode2.Left.Length, array, arrayIndex, count);
		}
		ropeNode2.Length += count;
		ropeNode2.Rebalance();
		return ropeNode2;
	}

	internal RopeNode<T> RemoveRange(int index, int count)
	{
		if (index == 0 && count == Length)
		{
			return EmptyRopeNode;
		}
		int num = index + count;
		RopeNode<T> ropeNode = CloneIfShared();
		if (ropeNode.Height == 0)
		{
			int num2 = ropeNode.Length - num;
			for (int i = 0; i < num2; i++)
			{
				ropeNode.Contents[index + i] = ropeNode.Contents[num + i];
			}
			ropeNode.Length -= count;
		}
		else
		{
			if (num <= ropeNode.Left.Length)
			{
				ropeNode.Left = ropeNode.Left.RemoveRange(index, count);
			}
			else if (index >= ropeNode.Left.Length)
			{
				ropeNode.Right = ropeNode.Right.RemoveRange(index - ropeNode.Left.Length, count);
			}
			else
			{
				int num3 = ropeNode.Left.Length - index;
				ropeNode.Left = ropeNode.Left.RemoveRange(index, num3);
				ropeNode.Right = ropeNode.Right.RemoveRange(0, count - num3);
			}
			if (ropeNode.Left.Length == 0)
			{
				return ropeNode.Right;
			}
			if (ropeNode.Right.Length == 0)
			{
				return ropeNode.Left;
			}
			ropeNode.Length -= count;
			ropeNode.MergeIfPossible();
			ropeNode.Rebalance();
		}
		return ropeNode;
	}

	internal virtual RopeNode<T> GetContentNode()
	{
		throw new InvalidOperationException("Called GetContentNode() on non-FunctionNode.");
	}
}
