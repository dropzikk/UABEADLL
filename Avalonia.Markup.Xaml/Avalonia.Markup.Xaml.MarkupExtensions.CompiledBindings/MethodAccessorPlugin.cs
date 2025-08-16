using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Avalonia.Data;
using Avalonia.Data.Core.Plugins;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class MethodAccessorPlugin : IPropertyAccessorPlugin
{
	private sealed class Accessor : PropertyAccessorBase
	{
		public override Type? PropertyType { get; }

		public override object? Value { get; }

		public Accessor(WeakReference<object?> reference, MethodInfo method, Type delegateType)
		{
			if (reference == null)
			{
				throw new ArgumentNullException("reference");
			}
			if ((object)method == null)
			{
				throw new ArgumentNullException("method");
			}
			PropertyType = delegateType;
			object target;
			if (method.IsStatic)
			{
				Value = method.CreateDelegate(PropertyType);
			}
			else if (reference.TryGetTarget(out target))
			{
				Value = method.CreateDelegate(PropertyType, target);
			}
		}

		public override bool SetValue(object? value, BindingPriority priority)
		{
			return false;
		}

		protected override void SubscribeCore()
		{
			try
			{
				PublishValue(Value);
			}
			catch
			{
			}
		}

		protected override void UnsubscribeCore()
		{
		}
	}

	private readonly MethodInfo _method;

	private readonly Type _delegateType;

	public MethodAccessorPlugin(MethodInfo method, Type delegateType)
	{
		_method = method;
		_delegateType = delegateType;
	}

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public bool Match(object obj, string propertyName)
	{
		throw new InvalidOperationException("The MethodAccessorPlugin does not support dynamic matching");
	}

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public IPropertyAccessor Start(WeakReference<object?> reference, string propertyName)
	{
		return new Accessor(reference, _method, _delegateType);
	}
}
