using System.Collections.Generic;

namespace Avalonia.Utilities;

internal ref struct ValueSingleOrList<T>
{
	private bool _isSingleSet;

	public T Single { get; private set; }

	public List<T> List { get; private set; }

	public bool HasList => List != null;

	public bool IsSingle
	{
		get
		{
			if (List == null)
			{
				return _isSingleSet;
			}
			return false;
		}
	}

	public void Add(T value)
	{
		if (List != null)
		{
			List.Add(value);
			return;
		}
		if (!_isSingleSet)
		{
			Single = value;
			_isSingleSet = true;
			return;
		}
		List = new List<T>();
		List.Add(Single);
		List.Add(value);
		Single = default(T);
	}

	public bool Remove(T value)
	{
		if (List != null)
		{
			return List.Remove(value);
		}
		if (!_isSingleSet)
		{
			return false;
		}
		if (EqualityComparer<T>.Default.Equals(Single, value))
		{
			Single = default(T);
			_isSingleSet = false;
			return true;
		}
		return false;
	}
}
