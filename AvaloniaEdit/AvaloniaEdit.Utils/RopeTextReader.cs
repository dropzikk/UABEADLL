using System;
using System.Collections.Generic;
using System.IO;

namespace AvaloniaEdit.Utils;

public sealed class RopeTextReader : TextReader
{
	private readonly Stack<RopeNode<char>> _stack = new Stack<RopeNode<char>>();

	private RopeNode<char> _currentNode;

	private int _indexInsideNode;

	public RopeTextReader(Rope<char> rope)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		rope.Root.Publish();
		if (rope.Length != 0)
		{
			_currentNode = rope.Root;
			GoToLeftMostLeaf();
		}
	}

	private void GoToLeftMostLeaf()
	{
		while (_currentNode.Contents == null)
		{
			if (_currentNode.Height == 0)
			{
				_currentNode = _currentNode.GetContentNode();
				continue;
			}
			_stack.Push(_currentNode.Right);
			_currentNode = _currentNode.Left;
		}
	}

	public override int Peek()
	{
		if (_currentNode == null)
		{
			return -1;
		}
		return _currentNode.Contents[_indexInsideNode];
	}

	public override int Read()
	{
		if (_currentNode == null)
		{
			return -1;
		}
		char result = _currentNode.Contents[_indexInsideNode++];
		if (_indexInsideNode >= _currentNode.Length)
		{
			GoToNextNode();
		}
		return result;
	}

	private void GoToNextNode()
	{
		if (_stack.Count == 0)
		{
			_currentNode = null;
			return;
		}
		_indexInsideNode = 0;
		_currentNode = _stack.Pop();
		GoToLeftMostLeaf();
	}

	public override int Read(char[] buffer, int index, int count)
	{
		if (_currentNode == null)
		{
			return 0;
		}
		int num = _currentNode.Length - _indexInsideNode;
		if (count < num)
		{
			Array.Copy(_currentNode.Contents, _indexInsideNode, buffer, index, count);
			_indexInsideNode += count;
			return count;
		}
		Array.Copy(_currentNode.Contents, _indexInsideNode, buffer, index, num);
		GoToNextNode();
		return num;
	}
}
