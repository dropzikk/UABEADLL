using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Metsys.Bson;

[RequiresUnreferencedCode("Bson uses reflection")]
internal class ListWrapper : BaseWrapper
{
	private IList _list;

	public override object Collection => _list;

	public override void Add(object value)
	{
		_list.Add(value);
	}

	protected override object CreateContainer(Type type, Type itemType)
	{
		if (type.IsInterface)
		{
			return Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
		}
		if (type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) != null)
		{
			return Activator.CreateInstance(type);
		}
		return null;
	}

	protected override void SetContainer(object container)
	{
		object list;
		if (container != null)
		{
			list = (IList)container;
		}
		else
		{
			IList list2 = new ArrayList();
			list = list2;
		}
		_list = (IList)list;
	}
}
