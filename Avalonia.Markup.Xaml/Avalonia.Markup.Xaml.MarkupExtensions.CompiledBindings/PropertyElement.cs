using System;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class PropertyElement : ICompiledBindingPathElement
{
	private readonly bool _isFirstElement;

	public IPropertyInfo Property { get; }

	public Func<WeakReference<object?>, IPropertyInfo, IPropertyAccessor> AccessorFactory { get; }

	public PropertyElement(IPropertyInfo property, Func<WeakReference<object?>, IPropertyInfo, IPropertyAccessor> accessorFactory, bool isFirstElement)
	{
		Property = property;
		AccessorFactory = accessorFactory;
		_isFirstElement = isFirstElement;
	}

	public override string ToString()
	{
		if (!_isFirstElement)
		{
			return "." + Property.Name;
		}
		return Property.Name;
	}
}
