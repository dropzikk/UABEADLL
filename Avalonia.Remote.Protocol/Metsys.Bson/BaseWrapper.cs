using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Metsys.Bson;

[RequiresUnreferencedCode("Bson uses reflection")]
internal abstract class BaseWrapper
{
	public abstract object Collection { get; }

	public static BaseWrapper Create(Type type, Type itemType, object existingContainer)
	{
		BaseWrapper baseWrapper = CreateWrapperFromType((existingContainer == null) ? type : existingContainer.GetType(), itemType);
		baseWrapper.SetContainer(existingContainer ?? baseWrapper.CreateContainer(type, itemType));
		return baseWrapper;
	}

	private static BaseWrapper CreateWrapperFromType(Type type, Type itemType)
	{
		if (type.IsArray)
		{
			return (BaseWrapper)Activator.CreateInstance(typeof(ArrayWrapper<>).MakeGenericType(itemType));
		}
		bool flag = false;
		List<Type> list = new List<Type>(from h in type.GetInterfaces()
			select (!h.IsGenericType) ? h : h.GetGenericTypeDefinition());
		list.Insert(0, type.IsGenericType ? type.GetGenericTypeDefinition() : type);
		foreach (Type item in list)
		{
			if (typeof(IList<>).IsAssignableFrom(item) || typeof(IList).IsAssignableFrom(item))
			{
				return new ListWrapper();
			}
			if (typeof(ICollection<>).IsAssignableFrom(item))
			{
				flag = true;
			}
		}
		if (flag)
		{
			return (BaseWrapper)Activator.CreateInstance(typeof(CollectionWrapper<>).MakeGenericType(itemType));
		}
		foreach (Type item2 in list)
		{
			if (typeof(IEnumerable<>).IsAssignableFrom(item2) || typeof(IEnumerable).IsAssignableFrom(item2))
			{
				return new ListWrapper();
			}
		}
		throw new BsonException("Collection of type " + type.FullName + " cannot be deserialized");
	}

	public abstract void Add(object value);

	protected abstract object CreateContainer(Type type, Type itemType);

	protected abstract void SetContainer(object container);
}
