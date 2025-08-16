using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Avalonia.Diagnostics;

internal static class TypeExtesnions
{
	private static readonly ConditionalWeakTable<Type, string> s_getTypeNameCache = new ConditionalWeakTable<Type, string>();

	public static string GetTypeName(this Type type)
	{
		if (!s_getTypeNameCache.TryGetValue(type, out string value))
		{
			value = type.Name;
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if ((object)underlyingType != null)
			{
				value = underlyingType.Name + "?";
			}
			else if (type.IsGenericType)
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				Type[] genericArguments = type.GetGenericArguments();
				value = genericTypeDefinition.Name.Substring(0, genericTypeDefinition.Name.IndexOf('`'));
				value = value + "<" + string.Join(",", genericArguments.Select(GetTypeName)) + ">";
			}
			s_getTypeNameCache.Add(type, value);
		}
		return value;
	}
}
