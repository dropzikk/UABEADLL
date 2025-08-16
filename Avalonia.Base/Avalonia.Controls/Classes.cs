using System;
using System.Collections.Generic;
using Avalonia.Collections;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class Classes : AvaloniaList<string>, IPseudoClasses
{
	private SafeEnumerableList<IClassesChangedListener>? _listeners;

	internal int ListenerCount => _listeners?.Count ?? 0;

	public Classes()
	{
	}

	public Classes(IEnumerable<string> items)
		: base(items)
	{
	}

	public Classes(params string[] items)
		: base(items)
	{
	}

	public static Classes Parse(string s)
	{
		return new Classes(s.Split(' '));
	}

	public override void Add(string name)
	{
		ThrowIfPseudoclass(name, "added");
		if (!Contains(name))
		{
			base.Add(name);
			NotifyChanged();
		}
	}

	public override void AddRange(IEnumerable<string> names)
	{
		List<string> list = new List<string>();
		foreach (string name in names)
		{
			ThrowIfPseudoclass(name, "added");
			if (!Contains(name))
			{
				list.Add(name);
			}
		}
		base.AddRange(list);
		NotifyChanged();
	}

	public override void Clear()
	{
		for (int num = base.Count - 1; num >= 0; num--)
		{
			if (!base[num].StartsWith(":"))
			{
				RemoveAt(num);
			}
		}
		NotifyChanged();
	}

	public override void Insert(int index, string name)
	{
		ThrowIfPseudoclass(name, "added");
		if (!Contains(name))
		{
			base.Insert(index, name);
			NotifyChanged();
		}
	}

	public override void InsertRange(int index, IEnumerable<string> names)
	{
		List<string> list = null;
		foreach (string name in names)
		{
			ThrowIfPseudoclass(name, "added");
			if (!Contains(name))
			{
				if (list == null)
				{
					list = new List<string>();
				}
				list.Add(name);
			}
		}
		if (list != null)
		{
			base.InsertRange(index, list);
			NotifyChanged();
		}
	}

	public override bool Remove(string name)
	{
		ThrowIfPseudoclass(name, "removed");
		if (base.Remove(name))
		{
			NotifyChanged();
			return true;
		}
		return false;
	}

	public override void RemoveAll(IEnumerable<string> names)
	{
		List<string> list = null;
		foreach (string name in names)
		{
			ThrowIfPseudoclass(name, "removed");
			if (list == null)
			{
				list = new List<string>();
			}
			list.Add(name);
		}
		if (list != null)
		{
			base.RemoveAll(list);
			NotifyChanged();
		}
	}

	public override void RemoveAt(int index)
	{
		ThrowIfPseudoclass(base[index], "removed");
		base.RemoveAt(index);
		NotifyChanged();
	}

	public override void RemoveRange(int index, int count)
	{
		base.RemoveRange(index, count);
		NotifyChanged();
	}

	public void Replace(IList<string> source)
	{
		List<string> list = null;
		foreach (string item in source)
		{
			ThrowIfPseudoclass(item, "added");
		}
		using (AvaloniaList<string>.Enumerator enumerator2 = GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				string current = enumerator2.Current;
				if (!current.StartsWith(":"))
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(current);
				}
			}
		}
		if (list != null)
		{
			base.RemoveAll(list);
		}
		base.AddRange(source);
		NotifyChanged();
	}

	void IPseudoClasses.Add(string name)
	{
		if (!Contains(name))
		{
			base.Add(name);
			NotifyChanged();
		}
	}

	bool IPseudoClasses.Remove(string name)
	{
		if (base.Remove(name))
		{
			NotifyChanged();
			return true;
		}
		return false;
	}

	internal void AddListener(IClassesChangedListener listener)
	{
		(_listeners ?? (_listeners = new SafeEnumerableList<IClassesChangedListener>())).Add(listener);
	}

	internal void RemoveListener(IClassesChangedListener listener)
	{
		_listeners?.Remove(listener);
	}

	private void NotifyChanged()
	{
		if (_listeners == null)
		{
			return;
		}
		foreach (IClassesChangedListener listener in _listeners)
		{
			listener.Changed();
		}
	}

	private static void ThrowIfPseudoclass(string name, string operation)
	{
		if (name.StartsWith(":"))
		{
			throw new ArgumentException($"The pseudoclass '{name}' may only be {operation} by the control itself.");
		}
	}

	public void Set(string name, bool value)
	{
		if (value)
		{
			if (!Contains(name))
			{
				Add(name);
			}
		}
		else
		{
			Remove(name);
		}
	}
}
