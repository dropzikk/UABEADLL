using System;

namespace Avalonia.Data.Core;

public class EnsureTypePropertyPathElement : IPropertyPathElement
{
	public Type Type { get; }

	public EnsureTypePropertyPathElement(Type type)
	{
		Type = type;
	}
}
