using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Metsys.Bson;

[RequiresUnreferencedCode("Bson uses reflection")]
internal static class ListHelper
{
	public static Type GetListItemType(Type enumerableType)
	{
		if (enumerableType.IsArray)
		{
			return enumerableType.GetElementType();
		}
		if (!enumerableType.IsGenericType)
		{
			return typeof(object);
		}
		return enumerableType.GetGenericArguments()[0];
	}

	public static Type GetDictionaryKeyType(Type enumerableType)
	{
		if (!enumerableType.IsGenericType)
		{
			return typeof(object);
		}
		return enumerableType.GetGenericArguments()[0];
	}

	public static Type GetDictionaryValueType(Type enumerableType)
	{
		if (!enumerableType.IsGenericType)
		{
			return typeof(object);
		}
		return enumerableType.GetGenericArguments()[1];
	}

	public static IDictionary CreateDictionary(Type dictionaryType, Type keyType, Type valueType)
	{
		if (dictionaryType.IsInterface)
		{
			return (IDictionary)Activator.CreateInstance(typeof(Dictionary<, >).MakeGenericType(keyType, valueType));
		}
		if (dictionaryType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) != null)
		{
			return (IDictionary)Activator.CreateInstance(dictionaryType);
		}
		return new Dictionary<object, object>();
	}
}
