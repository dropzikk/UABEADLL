using System;
using System.IO;

namespace AvaloniaEdit.Utils;

public static class CharRope
{
	public static Rope<char> Create(string text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		return new Rope<char>(InitFromString(text));
	}

	public static string ToString(this Rope<char> rope, int startIndex, int length)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (length == 0)
		{
			return string.Empty;
		}
		char[] array = new char[length];
		rope.CopyTo(startIndex, array, 0, length);
		return new string(array);
	}

	public static void WriteTo(this Rope<char> rope, TextWriter output, int startIndex, int length)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (output == null)
		{
			throw new ArgumentNullException("output");
		}
		rope.VerifyRange(startIndex, length);
		rope.Root.WriteTo(startIndex, output, length);
	}

	public static void AddText(this Rope<char> rope, string text)
	{
		rope.InsertText(rope.Length, text);
	}

	public static void InsertText(this Rope<char> rope, int index, string text)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		rope.InsertRange(index, text.ToCharArray(), 0, text.Length);
	}

	internal static RopeNode<char> InitFromString(string text)
	{
		if (text.Length == 0)
		{
			return RopeNode<char>.EmptyRopeNode;
		}
		RopeNode<char> ropeNode = RopeNode<char>.CreateNodes(text.Length);
		FillNode(ropeNode, text, 0);
		return ropeNode;
	}

	private static void FillNode(RopeNode<char> node, string text, int start)
	{
		if (node.Contents != null)
		{
			text.CopyTo(start, node.Contents, 0, node.Length);
			return;
		}
		FillNode(node.Left, text, start);
		FillNode(node.Right, text, start + node.Left.Length);
	}

	internal static void WriteTo(this RopeNode<char> node, int index, TextWriter output, int count)
	{
		if (node.Height == 0)
		{
			if (node.Contents == null)
			{
				node.GetContentNode().WriteTo(index, output, count);
			}
			else
			{
				output.Write(node.Contents, index, count);
			}
		}
		else if (index + count <= node.Left.Length)
		{
			node.Left.WriteTo(index, output, count);
		}
		else if (index >= node.Left.Length)
		{
			node.Right.WriteTo(index - node.Left.Length, output, count);
		}
		else
		{
			int num = node.Left.Length - index;
			node.Left.WriteTo(index, output, num);
			node.Right.WriteTo(0, output, count - num);
		}
	}

	public static int IndexOfAny(this Rope<char> rope, char[] anyOf, int startIndex, int length)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (anyOf == null)
		{
			throw new ArgumentNullException("anyOf");
		}
		rope.VerifyRange(startIndex, length);
		while (length > 0)
		{
			Rope<char>.RopeCacheEntry ropeCacheEntry = rope.FindNodeUsingCache(startIndex).PeekOrDefault();
			char[] contents = ropeCacheEntry.Node.Contents;
			int num = startIndex - ropeCacheEntry.NodeStartIndex;
			int num2 = Math.Min(ropeCacheEntry.Node.Length, num + length);
			for (int i = startIndex - ropeCacheEntry.NodeStartIndex; i < num2; i++)
			{
				char c = contents[i];
				foreach (char c2 in anyOf)
				{
					if (c == c2)
					{
						return ropeCacheEntry.NodeStartIndex + i;
					}
				}
			}
			length -= num2 - num;
			startIndex = ropeCacheEntry.NodeStartIndex + num2;
		}
		return -1;
	}

	public static int IndexOf(this Rope<char> rope, string searchText, int startIndex, int length, StringComparison comparisonType)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (searchText == null)
		{
			throw new ArgumentNullException("searchText");
		}
		rope.VerifyRange(startIndex, length);
		int num = rope.ToString(startIndex, length).IndexOf(searchText, comparisonType);
		if (num < 0)
		{
			return -1;
		}
		return num + startIndex;
	}

	public static int LastIndexOf(this Rope<char> rope, string searchText, int startIndex, int length, StringComparison comparisonType)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (searchText == null)
		{
			throw new ArgumentNullException("searchText");
		}
		rope.VerifyRange(startIndex, length);
		int num = rope.ToString(startIndex, length).LastIndexOf(searchText, comparisonType);
		if (num < 0)
		{
			return -1;
		}
		return num + startIndex;
	}
}
