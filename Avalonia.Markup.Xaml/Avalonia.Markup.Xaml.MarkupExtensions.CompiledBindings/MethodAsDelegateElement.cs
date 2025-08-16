using System;
using System.Reflection;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class MethodAsDelegateElement : ICompiledBindingPathElement
{
	public MethodInfo Method { get; }

	public Type DelegateType { get; }

	public MethodAsDelegateElement(RuntimeMethodHandle method, RuntimeTypeHandle delegateType)
	{
		Method = (MethodBase.GetMethodFromHandle(method) as MethodInfo) ?? throw new ArgumentException("Invalid method handle", "method");
		DelegateType = Type.GetTypeFromHandle(delegateType);
	}
}
