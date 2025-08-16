using System.Collections;
using System.Collections.Specialized;
using Avalonia.Collections;
using Avalonia.Metadata;

namespace Avalonia.Controls;

[AvaloniaList(Separators = new string[] { ",", " " })]
public abstract class DefinitionList<T> : AvaloniaList<T> where T : DefinitionBase
{
	internal bool IsDirty = true;

	private Grid? _parent;

	internal Grid? Parent
	{
		get
		{
			return _parent;
		}
		set
		{
			SetParent(value);
		}
	}

	public DefinitionList()
	{
		base.ResetBehavior = ResetBehavior.Remove;
		base.CollectionChanged += OnCollectionChanged;
	}

	private void SetParent(Grid? value)
	{
		_parent = value;
		int num = 0;
		using AvaloniaList<T>.Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			current.Parent = value;
			current.Index = num++;
		}
	}

	internal void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		int num = 0;
		using (AvaloniaList<T>.Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.Index = num++;
			}
		}
		UpdateDefinitionParent(e.NewItems, wasRemoved: false);
		UpdateDefinitionParent(e.OldItems, wasRemoved: true);
		IsDirty = true;
	}

	private void UpdateDefinitionParent(IList? items, bool wasRemoved)
	{
		if (items == null)
		{
			return;
		}
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			DefinitionBase definitionBase = (DefinitionBase)items[i];
			if (wasRemoved)
			{
				definitionBase.OnExitParentTree();
				continue;
			}
			definitionBase.Parent = Parent;
			definitionBase.OnEnterParentTree();
		}
	}
}
