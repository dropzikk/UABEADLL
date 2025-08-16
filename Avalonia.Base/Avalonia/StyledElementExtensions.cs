using System;
using Avalonia.Data;

namespace Avalonia;

public static class StyledElementExtensions
{
	public static IDisposable BindClass(this StyledElement target, string className, IBinding source, object anchor)
	{
		return ClassBindingManager.Bind(target, className, source, anchor);
	}
}
