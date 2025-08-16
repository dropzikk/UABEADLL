using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Metsys.Bson;

[RequiresUnreferencedCode("Bson uses reflection")]
internal class ArrayWrapper<T> : BaseWrapper
{
	private readonly List<T> _list = new List<T>();

	public override object Collection => _list.ToArray();

	public override void Add(object value)
	{
		_list.Add((T)value);
	}

	protected override object CreateContainer(Type type, Type itemType)
	{
		return null;
	}

	protected override void SetContainer(object container)
	{
		if (container != null)
		{
			throw new BsonException("An container cannot exist when trying to deserialize an array");
		}
	}
}
