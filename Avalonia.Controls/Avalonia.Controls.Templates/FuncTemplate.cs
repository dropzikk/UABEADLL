using System;
using Avalonia.Styling;

namespace Avalonia.Controls.Templates;

public class FuncTemplate<TControl> : ITemplate<TControl>, ITemplate where TControl : Control?
{
	private readonly Func<TControl> _func;

	public FuncTemplate(Func<TControl> func)
	{
		_func = func ?? throw new ArgumentNullException("func");
	}

	public TControl Build()
	{
		return _func();
	}

	object? ITemplate.Build()
	{
		return Build();
	}
}
public class FuncTemplate<TParam, TControl> : ITemplate<TParam, TControl> where TControl : Control?
{
	private readonly Func<TParam, INameScope, TControl> _func;

	public FuncTemplate(Func<TParam, INameScope, TControl> func)
	{
		_func = func ?? throw new ArgumentNullException("func");
	}

	public TControl Build(TParam param)
	{
		return BuildWithNameScope(param).control;
	}

	protected (TControl control, INameScope nameScope) BuildWithNameScope(TParam param)
	{
		NameScope nameScope = new NameScope();
		return (control: _func(param, nameScope), nameScope: nameScope);
	}
}
