using System;
using System.Collections.Generic;

namespace Avalonia.Data.Core;

public class PropertyPathBuilder
{
	private readonly List<IPropertyPathElement> _elements = new List<IPropertyPathElement>();

	public PropertyPathBuilder Property(IPropertyInfo property)
	{
		_elements.Add(new PropertyPropertyPathElement(property));
		return this;
	}

	public PropertyPathBuilder ChildTraversal()
	{
		_elements.Add(new ChildTraversalPropertyPathElement());
		return this;
	}

	public PropertyPathBuilder EnsureType(Type type)
	{
		_elements.Add(new EnsureTypePropertyPathElement(type));
		return this;
	}

	public PropertyPathBuilder Cast(Type type)
	{
		_elements.Add(new CastTypePropertyPathElement(type));
		return this;
	}

	public PropertyPath Build()
	{
		return new PropertyPath(_elements);
	}
}
