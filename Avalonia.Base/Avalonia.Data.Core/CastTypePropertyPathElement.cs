using System;

namespace Avalonia.Data.Core;

public class CastTypePropertyPathElement : IPropertyPathElement
{
	public Type Type { get; }

	public CastTypePropertyPathElement(Type type)
	{
		Type = type;
	}
}
