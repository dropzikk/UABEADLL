using System;
using System.Collections.Generic;

namespace Avalonia.Utilities;

internal class SingleOrQueue<T>
{
	private T? _head;

	private Queue<T>? _tail;

	private Queue<T> Tail => _tail ?? (_tail = new Queue<T>());

	private bool HasTail => _tail != null;

	public bool Empty { get; private set; } = true;

	public void Enqueue(T value)
	{
		if (Empty)
		{
			_head = value;
		}
		else
		{
			Tail.Enqueue(value);
		}
		Empty = false;
	}

	public T Dequeue()
	{
		if (Empty)
		{
			throw new InvalidOperationException("Cannot dequeue from an empty queue!");
		}
		T? head = _head;
		if (HasTail && Tail.Count != 0)
		{
			_head = Tail.Dequeue();
			return head;
		}
		_head = default(T);
		Empty = true;
		return head;
	}
}
