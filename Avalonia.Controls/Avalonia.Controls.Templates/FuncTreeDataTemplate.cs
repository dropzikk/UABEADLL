using System;
using System.Collections;
using Avalonia.Data;

namespace Avalonia.Controls.Templates;

public class FuncTreeDataTemplate : FuncDataTemplate, ITreeDataTemplate, IDataTemplate, ITemplate<object?, Control?>
{
	private readonly Func<object?, IEnumerable> _itemsSelector;

	public FuncTreeDataTemplate(Type type, Func<object?, INameScope, Control> build, Func<object?, IEnumerable> itemsSelector)
		: this((object? o) => IsInstance(o, type), build, itemsSelector)
	{
	}

	public FuncTreeDataTemplate(Func<object?, bool> match, Func<object?, INameScope, Control?> build, Func<object?, IEnumerable> itemsSelector)
		: base(match, build)
	{
		_itemsSelector = itemsSelector;
	}

	public InstancedBinding ItemsSelector(object item)
	{
		return InstancedBinding.OneTime(_itemsSelector(item));
	}

	private static bool IsInstance(object? o, Type t)
	{
		return t.IsInstanceOfType(o);
	}
}
public class FuncTreeDataTemplate<T> : FuncTreeDataTemplate
{
	public FuncTreeDataTemplate(Func<T, INameScope, Control> build, Func<T, IEnumerable> itemsSelector)
		: base(typeof(T), Cast(build), Cast(itemsSelector))
	{
	}

	public FuncTreeDataTemplate(Func<T, bool> match, Func<T, INameScope, Control> build, Func<T, IEnumerable> itemsSelector)
		: base(CastMatch(match), Cast(build), Cast(itemsSelector))
	{
	}

	private static Func<object?, bool> CastMatch(Func<T, bool> f)
	{
		return (object? o) => o is T arg && f(arg);
	}

	private static Func<object?, INameScope, TResult> Cast<TResult>(Func<T, INameScope, TResult> f)
	{
		return (object? o, INameScope s) => f((T)o, s);
	}

	private static Func<object?, TResult> Cast<TResult>(Func<T, TResult> f)
	{
		return (object? o) => f((T)o);
	}
}
