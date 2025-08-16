using System;
using Avalonia.Controls.Primitives;

namespace Avalonia.Controls.Templates;

public class FuncControlTemplate : FuncTemplate<TemplatedControl, Control>, IControlTemplate, ITemplate<TemplatedControl, TemplateResult<Control>?>
{
	public FuncControlTemplate(Func<TemplatedControl, INameScope, Control> build)
		: base(build)
	{
	}

	public new TemplateResult<Control> Build(TemplatedControl param)
	{
		var (result, nameScope) = BuildWithNameScope(param);
		return new TemplateResult<Control>(result, nameScope);
	}
}
public class FuncControlTemplate<T> : FuncControlTemplate where T : TemplatedControl
{
	public FuncControlTemplate(Func<T, INameScope, Control> build)
		: base((TemplatedControl x, INameScope s) => build((T)x, s))
	{
	}
}
