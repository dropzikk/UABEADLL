using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data;
using Avalonia.Data.Core.Plugins;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class ArrayElementPlugin : IPropertyAccessorPlugin
{
	private class Accessor : PropertyAccessorBase
	{
		private readonly int[] _indices;

		private readonly WeakReference<Array> _reference;

		public override Type PropertyType { get; }

		public override object? Value
		{
			get
			{
				if (!_reference.TryGetTarget(out Array target))
				{
					return null;
				}
				return target.GetValue(_indices);
			}
		}

		public Accessor(WeakReference<Array> reference, int[] indices, Type elementType)
		{
			_reference = reference;
			_indices = indices;
			PropertyType = elementType;
		}

		public override bool SetValue(object? value, BindingPriority priority)
		{
			if (_reference.TryGetTarget(out Array target))
			{
				target.SetValue(value, _indices);
				return true;
			}
			return false;
		}

		protected override void SubscribeCore()
		{
			PublishValue(Value);
		}

		protected override void UnsubscribeCore()
		{
		}
	}

	private readonly int[] _indices;

	private readonly Type _elementType;

	public ArrayElementPlugin(int[] indices, Type elementType)
	{
		_indices = indices;
		_elementType = elementType;
	}

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public bool Match(object obj, string propertyName)
	{
		throw new InvalidOperationException("The ArrayElementPlugin does not support dynamic matching");
	}

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public IPropertyAccessor? Start(WeakReference<object?> reference, string propertyName)
	{
		if (reference.TryGetTarget(out object target) && target is Array target2)
		{
			return new Accessor(new WeakReference<Array>(target2), _indices, _elementType);
		}
		return null;
	}
}
