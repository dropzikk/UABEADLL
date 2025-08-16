using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.PropertyStore;

namespace Avalonia.Styling;

public class Styles : AvaloniaObject, IAvaloniaList<IStyle>, IList<IStyle>, ICollection<IStyle>, IEnumerable<IStyle>, IEnumerable, IAvaloniaReadOnlyList<IStyle>, IReadOnlyList<IStyle>, IReadOnlyCollection<IStyle>, INotifyCollectionChanged, INotifyPropertyChanged, IStyle, IResourceNode, IResourceProvider
{
	private readonly AvaloniaList<IStyle> _styles = new AvaloniaList<IStyle>();

	private IResourceHost? _owner;

	private IResourceDictionary? _resources;

	public int Count => _styles.Count;

	public IResourceHost? Owner
	{
		get
		{
			return _owner;
		}
		private set
		{
			if (_owner != value)
			{
				_owner = value;
				this.OwnerChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public IResourceDictionary Resources
	{
		get
		{
			return _resources ?? (Resources = new ResourceDictionary());
		}
		set
		{
			value = value ?? throw new ArgumentNullException("Resources");
			IResourceHost owner = Owner;
			if (owner != null)
			{
				_resources?.RemoveOwner(owner);
			}
			_resources = value;
			if (owner != null)
			{
				_resources.AddOwner(owner);
			}
		}
	}

	bool ICollection<IStyle>.IsReadOnly => false;

	bool IResourceNode.HasResources
	{
		get
		{
			IResourceDictionary? resources = _resources;
			if (resources != null && resources.Count > 0)
			{
				return true;
			}
			using (AvaloniaList<IStyle>.Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IStyle current = enumerator.Current;
					if (current is IResourceProvider && current.HasResources)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	IStyle IReadOnlyList<IStyle>.this[int index] => _styles[index];

	IReadOnlyList<IStyle> IStyle.Children => this;

	public IStyle this[int index]
	{
		get
		{
			return _styles[index];
		}
		set
		{
			_styles[index] = value;
		}
	}

	public event NotifyCollectionChangedEventHandler? CollectionChanged;

	public event EventHandler? OwnerChanged;

	public Styles()
	{
		_styles.ResetBehavior = ResetBehavior.Remove;
		_styles.CollectionChanged += OnCollectionChanged;
		_styles.Validate = delegate(IStyle i)
		{
			if (i is ControlTheme)
			{
				throw new InvalidOperationException("ControlThemes cannot be added to a Styles collection.");
			}
		};
	}

	public Styles(IResourceHost owner)
		: this()
	{
		Owner = owner;
	}

	public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
	{
		if (_resources != null && _resources.TryGetResource(key, theme, out value))
		{
			return true;
		}
		for (int num = Count - 1; num >= 0; num--)
		{
			if (this[num].TryGetResource(key, theme, out value))
			{
				return true;
			}
		}
		value = null;
		return false;
	}

	public void AddRange(IEnumerable<IStyle> items)
	{
		_styles.AddRange(items);
	}

	public void InsertRange(int index, IEnumerable<IStyle> items)
	{
		_styles.InsertRange(index, items);
	}

	public void Move(int oldIndex, int newIndex)
	{
		_styles.Move(oldIndex, newIndex);
	}

	public void MoveRange(int oldIndex, int count, int newIndex)
	{
		_styles.MoveRange(oldIndex, count, newIndex);
	}

	public void RemoveAll(IEnumerable<IStyle> items)
	{
		_styles.RemoveAll(items);
	}

	public void RemoveRange(int index, int count)
	{
		_styles.RemoveRange(index, count);
	}

	public int IndexOf(IStyle item)
	{
		return _styles.IndexOf(item);
	}

	public void Insert(int index, IStyle item)
	{
		_styles.Insert(index, item);
	}

	public void RemoveAt(int index)
	{
		_styles.RemoveAt(index);
	}

	public void Add(IStyle item)
	{
		_styles.Add(item);
	}

	public void Clear()
	{
		_styles.Clear();
	}

	public bool Contains(IStyle item)
	{
		return _styles.Contains(item);
	}

	public void CopyTo(IStyle[] array, int arrayIndex)
	{
		_styles.CopyTo(array, arrayIndex);
	}

	public bool Remove(IStyle item)
	{
		return _styles.Remove(item);
	}

	public AvaloniaList<IStyle>.Enumerator GetEnumerator()
	{
		return _styles.GetEnumerator();
	}

	IEnumerator<IStyle> IEnumerable<IStyle>.GetEnumerator()
	{
		return _styles.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _styles.GetEnumerator();
	}

	void IResourceProvider.AddOwner(IResourceHost owner)
	{
		owner = owner ?? throw new ArgumentNullException("owner");
		if (Owner != null)
		{
			throw new InvalidOperationException("The Styles already has a owner.");
		}
		Owner = owner;
		_resources?.AddOwner(owner);
		using AvaloniaList<IStyle>.Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current is IResourceProvider resourceProvider)
			{
				resourceProvider.AddOwner(owner);
			}
		}
	}

	void IResourceProvider.RemoveOwner(IResourceHost owner)
	{
		owner = owner ?? throw new ArgumentNullException("owner");
		if (Owner != owner)
		{
			return;
		}
		Owner = null;
		_resources?.RemoveOwner(owner);
		using AvaloniaList<IStyle>.Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current is IResourceProvider resourceProvider)
			{
				resourceProvider.RemoveOwner(owner);
			}
		}
	}

	internal SelectorMatchResult TryAttach(StyledElement target, object? host)
	{
		SelectorMatchResult selectorMatchResult = SelectorMatchResult.NeverThisType;
		using AvaloniaList<IStyle>.Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current is Style style)
			{
				SelectorMatchResult selectorMatchResult2 = style.TryAttach(target, host, FrameType.Style);
				if (selectorMatchResult2 > selectorMatchResult)
				{
					selectorMatchResult = selectorMatchResult2;
				}
			}
		}
		return selectorMatchResult;
	}

	private static IReadOnlyList<T> ToReadOnlyList<T>(ICollection list)
	{
		if (list is IReadOnlyList<T> result)
		{
			return result;
		}
		T[] array = new T[list.Count];
		list.CopyTo(array, 0);
		return array;
	}

	private static void InternalAdd(IList items, IResourceHost? owner)
	{
		if (owner == null)
		{
			return;
		}
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i] is IResourceProvider resourceProvider)
			{
				resourceProvider.AddOwner(owner);
			}
		}
		(owner as IStyleHost)?.StylesAdded(ToReadOnlyList<IStyle>(items));
	}

	private static void InternalRemove(IList items, IResourceHost? owner)
	{
		if (owner == null)
		{
			return;
		}
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i] is IResourceProvider resourceProvider)
			{
				resourceProvider.RemoveOwner(owner);
			}
		}
		(owner as IStyleHost)?.StylesRemoved(ToReadOnlyList<IStyle>(items));
	}

	private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Reset)
		{
			throw new InvalidOperationException("Reset should not be called on Styles.");
		}
		IResourceHost owner = Owner;
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
			InternalAdd(e.NewItems, owner);
			break;
		case NotifyCollectionChangedAction.Remove:
			InternalRemove(e.OldItems, owner);
			break;
		case NotifyCollectionChangedAction.Replace:
			InternalRemove(e.OldItems, owner);
			InternalAdd(e.NewItems, owner);
			break;
		}
		this.CollectionChanged?.Invoke(this, e);
	}
}
