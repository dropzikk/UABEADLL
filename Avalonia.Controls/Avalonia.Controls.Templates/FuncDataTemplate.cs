using System;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;
using Avalonia.Utilities;

namespace Avalonia.Controls.Templates;

public class FuncDataTemplate : FuncTemplate<object?, Control?>, IRecyclingDataTemplate, IDataTemplate, ITemplate<object?, Control?>
{
	public static readonly FuncDataTemplate Default = new FuncDataTemplate<object>(delegate(object? data, INameScope s)
	{
		if (data != null)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Bind(TextBlock.TextProperty, from x in textBlock.GetObservable(StyledElement.DataContextProperty)
				select x?.ToString());
			return textBlock;
		}
		return (Control?)null;
	}, supportsRecycling: true);

	public static readonly FuncDataTemplate Access = new FuncDataTemplate<object>(delegate(object data, INameScope s)
	{
		if (data != null)
		{
			AccessText accessText = new AccessText();
			accessText.Bind(TextBlock.TextProperty, from x in accessText.GetObservable(StyledElement.DataContextProperty)
				select x?.ToString());
			return accessText;
		}
		return (Control?)null;
	}, supportsRecycling: true);

	private readonly Func<object?, bool> _match;

	private readonly bool _supportsRecycling;

	public FuncDataTemplate(Type type, Func<object?, INameScope, Control?> build, bool supportsRecycling = false)
		: this((object? o) => IsInstance(o, type), build, supportsRecycling)
	{
	}

	public FuncDataTemplate(Func<object?, bool> match, Func<object?, INameScope, Control?> build, bool supportsRecycling = false)
		: base(build)
	{
		_match = match ?? throw new ArgumentNullException("match");
		_supportsRecycling = supportsRecycling;
	}

	public bool Match(object? data)
	{
		return _match(data);
	}

	public Control? Build(object? data, Control? existing)
	{
		if (!_supportsRecycling || existing == null)
		{
			return Build(data);
		}
		return existing;
	}

	private static bool IsInstance(object? o, Type t)
	{
		return t.IsInstanceOfType(o);
	}
}
public class FuncDataTemplate<T> : FuncDataTemplate
{
	public FuncDataTemplate(Func<T, INameScope, Control?> build, bool supportsRecycling = false)
		: base((object? o) => TypeUtilities.CanCast<T>(o), CastBuild(build), supportsRecycling)
	{
	}

	public FuncDataTemplate(Func<T, bool> match, Func<T, INameScope, Control> build, bool supportsRecycling = false)
		: base(CastMatch(match), CastBuild(build), supportsRecycling)
	{
	}

	public FuncDataTemplate(Func<T, bool> match, Func<T, Control> build, bool supportsRecycling = false)
		: this(match, (Func<T, INameScope, Control>)((T a, INameScope _) => build(a)), supportsRecycling)
	{
	}

	private static Func<object?, bool> CastMatch(Func<T, bool> f)
	{
		return (object? o) => TypeUtilities.CanCast<T>(o) && f((T)o);
	}

	private static Func<object?, INameScope, TResult> CastBuild<TResult>(Func<T, INameScope, TResult> f)
	{
		return (object? o, INameScope s) => f((T)o, s);
	}
}
