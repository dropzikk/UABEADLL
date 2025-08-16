namespace Avalonia.Controls.Primitives;

internal class ArrayList<T>
{
	private int _nextIndex;

	public T this[int i]
	{
		get
		{
			return Array[i];
		}
		set
		{
			Array[i] = value;
		}
	}

	public T[] Array { get; private set; }

	public int Capacity { get; private set; }

	public ArrayList(int capacity)
	{
		Capacity = capacity;
		Array = new T[capacity];
	}

	public void Add(T item)
	{
		if (_nextIndex >= 0 && _nextIndex < Capacity)
		{
			Array[_nextIndex] = item;
			_nextIndex++;
		}
	}
}
