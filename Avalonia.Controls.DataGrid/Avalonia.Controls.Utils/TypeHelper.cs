using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Avalonia.Controls.Utils;

internal static class TypeHelper
{
	internal const char LeftIndexerToken = '[';

	internal const char PropertyNameSeparator = '.';

	internal const char RightIndexerToken = ']';

	private static Type FindGenericType(Type definition, Type type)
	{
		while (type != null && type != typeof(object))
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == definition)
			{
				return type;
			}
			if (definition.IsInterface)
			{
				Type[] interfaces = type.GetInterfaces();
				foreach (Type type2 in interfaces)
				{
					Type type3 = FindGenericType(definition, type2);
					if (type3 != null)
					{
						return type3;
					}
				}
			}
			type = type.BaseType;
		}
		return null;
	}

	private static PropertyInfo FindIndexerInMembers(MemberInfo[] members, string stringIndex, out object[] index)
	{
		index = null;
		PropertyInfo result = null;
		for (int i = 0; i < members.Length; i++)
		{
			PropertyInfo propertyInfo = (PropertyInfo)members[i];
			if (propertyInfo == null)
			{
				continue;
			}
			ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
			if (indexParameters.Length > 1)
			{
				continue;
			}
			if (indexParameters[0].ParameterType == typeof(int))
			{
				int result2 = -1;
				if (int.TryParse(stringIndex.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out result2))
				{
					index = new object[1] { result2 };
					return propertyInfo;
				}
			}
			if (indexParameters[0].ParameterType == typeof(string))
			{
				index = new object[1] { stringIndex };
				result = propertyInfo;
			}
		}
		return result;
	}

	private static string GetDefaultMemberName(this Type type)
	{
		object[] customAttributes = type.GetCustomAttributes(typeof(DefaultMemberAttribute), inherit: true);
		if (customAttributes != null && customAttributes.Length == 1)
		{
			return (customAttributes[0] as DefaultMemberAttribute).MemberName;
		}
		return null;
	}

	internal static string GetDisplayName(this Type type, string propertyPath)
	{
		PropertyInfo nestedProperty = type.GetNestedProperty(propertyPath);
		if (nestedProperty != null)
		{
			object[] customAttributes = nestedProperty.GetCustomAttributes(typeof(DisplayAttribute), inherit: true);
			if (customAttributes != null && customAttributes.Length != 0 && customAttributes[0] is DisplayAttribute displayAttribute)
			{
				return displayAttribute.GetShortName();
			}
		}
		return null;
	}

	internal static Type GetEnumerableItemType(this Type enumerableType)
	{
		Type type = FindGenericType(typeof(IEnumerable<>), enumerableType);
		if (type != null)
		{
			return type.GetGenericArguments()[0];
		}
		return enumerableType;
	}

	private static PropertyInfo GetNestedProperty(this Type parentType, string propertyPath, out Exception exception, ref object item)
	{
		exception = null;
		if (parentType == null || string.IsNullOrEmpty(propertyPath))
		{
			item = null;
			return null;
		}
		Type type = parentType;
		PropertyInfo propertyInfo = null;
		List<string> list = SplitPropertyPath(propertyPath);
		for (int i = 0; i < list.Count; i++)
		{
			propertyInfo = type.GetPropertyOrIndexer(list[i], out var index);
			if (propertyInfo == null)
			{
				item = null;
				return null;
			}
			if (!propertyInfo.CanRead)
			{
				exception = new InvalidOperationException($"The property named '{list[i]}' on type '{type.GetTypeName()}' cannot be read.");
				item = null;
				return null;
			}
			if (item != null)
			{
				item = propertyInfo.GetValue(item, index);
			}
			type = propertyInfo.PropertyType.GetNonNullableType();
		}
		return propertyInfo;
	}

	internal static PropertyInfo GetNestedProperty(this Type parentType, string propertyPath, ref object item)
	{
		Exception exception;
		return parentType.GetNestedProperty(propertyPath, out exception, ref item);
	}

	internal static PropertyInfo GetNestedProperty(this Type parentType, string propertyPath)
	{
		if (parentType != null)
		{
			object item = null;
			return parentType.GetNestedProperty(propertyPath, ref item);
		}
		return null;
	}

	internal static string GetTypeName(this Type type)
	{
		Type nonNullableType = type.GetNonNullableType();
		string text = nonNullableType.Name;
		if (type != nonNullableType)
		{
			text += "?";
		}
		return text;
	}

	internal static Type GetNestedPropertyType(this Type parentType, string propertyPath)
	{
		if (parentType == null || string.IsNullOrEmpty(propertyPath))
		{
			return parentType;
		}
		PropertyInfo nestedProperty = parentType.GetNestedProperty(propertyPath);
		if (nestedProperty != null)
		{
			return nestedProperty.PropertyType;
		}
		return null;
	}

	internal static object GetNestedPropertyValue(object item, string propertyPath, Type propertyType, out Exception exception)
	{
		exception = null;
		if (item == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(propertyPath))
		{
			return item;
		}
		object item2 = item;
		Type type = item.GetType();
		if (type != null)
		{
			PropertyInfo nestedProperty = type.GetNestedProperty(propertyPath, out exception, ref item2);
			if (nestedProperty != null && nestedProperty.PropertyType != propertyType)
			{
				return null;
			}
		}
		return item2;
	}

	internal static object GetNestedPropertyValue(object item, string propertyPath)
	{
		if (item != null)
		{
			Type type = item.GetType();
			if (string.IsNullOrEmpty(propertyPath))
			{
				return item;
			}
			if (type != null)
			{
				object item2 = item;
				type.GetNestedProperty(propertyPath, ref item2);
				return item2;
			}
		}
		return null;
	}

	internal static Type GetNonNullableType(this Type type)
	{
		if (type.IsNullableType())
		{
			return type.GetGenericArguments()[0];
		}
		return type;
	}

	internal static PropertyInfo GetPropertyOrIndexer(this Type type, string propertyPath, out object[] index)
	{
		index = null;
		if (string.IsNullOrEmpty(propertyPath) || propertyPath[0] != '[')
		{
			PropertyInfo property = type.GetProperty(propertyPath);
			if (property != null)
			{
				return property;
			}
			if (type.IsInterface)
			{
				Type[] interfaces = type.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					_ = interfaces[i];
					property = type.GetProperty(propertyPath);
					if (property != null)
					{
						return property;
					}
				}
			}
			return null;
		}
		if (propertyPath.Length < 2 || propertyPath[propertyPath.Length - 1] != ']')
		{
			return null;
		}
		string stringIndex = propertyPath.Substring(1, propertyPath.Length - 2);
		PropertyInfo propertyInfo = FindIndexerInMembers(type.GetDefaultMembers(), stringIndex, out index);
		if (propertyInfo != null)
		{
			return propertyInfo;
		}
		Type type2 = type.GetElementType();
		if (type2 == null)
		{
			Type[] genericArguments = type.GetGenericArguments();
			if (genericArguments.Length == 1)
			{
				type2 = genericArguments[0];
			}
		}
		if (type2 != null)
		{
			Type type3 = typeof(IList<>).MakeGenericType(type2);
			if ((object)type3 != null && type3.IsAssignableFrom(type))
			{
				propertyInfo = FindIndexerInMembers(type3.GetDefaultMembers(), stringIndex, out index);
			}
			Type type4 = typeof(IReadOnlyList<>).MakeGenericType(type2);
			if ((object)type4 != null && type4.IsAssignableFrom(type))
			{
				propertyInfo = FindIndexerInMembers(type4.GetDefaultMembers(), stringIndex, out index);
			}
		}
		return propertyInfo;
	}

	internal static bool IsEnumerableType(this Type enumerableType)
	{
		return FindGenericType(typeof(IEnumerable<>), enumerableType) != null;
	}

	internal static bool IsNullableType(this Type type)
	{
		if (type != null && type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		return false;
	}

	internal static bool IsNullableEnum(this Type type)
	{
		if (type.IsNullableType() && type.GetGenericArguments().Length == 1)
		{
			return type.GetGenericArguments()[0].IsEnum;
		}
		return false;
	}

	internal static string PrependDefaultMemberName(object item, string property)
	{
		if (item != null && !string.IsNullOrEmpty(property) && property[0] == '[')
		{
			Type type = item.GetType();
			if (type != null)
			{
				string defaultMemberName = type.GetNonNullableType().GetDefaultMemberName();
				if (!string.IsNullOrEmpty(defaultMemberName))
				{
					return defaultMemberName + property;
				}
			}
		}
		return property;
	}

	internal static string RemoveDefaultMemberName(string property)
	{
		if (!string.IsNullOrEmpty(property) && property[property.Length - 1] == ']')
		{
			int num = property.IndexOf('[');
			if (num >= 0)
			{
				return property.Substring(num);
			}
		}
		return property;
	}

	internal static List<string> SplitPropertyPath(string propertyPath)
	{
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(propertyPath))
		{
			int num = 0;
			for (int i = 0; i < propertyPath.Length; i++)
			{
				if (propertyPath[i] == '.')
				{
					list.Add(propertyPath.Substring(num, i - num));
					num = i + 1;
				}
				else if (num != i && propertyPath[i] == '[')
				{
					list.Add(propertyPath.Substring(num, i - num));
					num = i;
				}
				else if (i == propertyPath.Length - 1)
				{
					list.Add(propertyPath.Substring(num));
				}
			}
		}
		return list;
	}

	internal static bool GetIsReadOnly(this MemberInfo memberInfo)
	{
		if (memberInfo != null)
		{
			object[] customAttributes = memberInfo.GetCustomAttributes(typeof(ReadOnlyAttribute), inherit: true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				return (customAttributes[0] as ReadOnlyAttribute).IsReadOnly;
			}
		}
		return false;
	}

	internal static Type GetItemType(this IEnumerable list)
	{
		Type type = list.GetType();
		Type type2 = null;
		if (type.IsEnumerableType())
		{
			type2 = type.GetEnumerableItemType();
		}
		if (type2 == null || type2 == typeof(object))
		{
			IEnumerator enumerator = list.GetEnumerator();
			if (enumerator.MoveNext() && enumerator.Current != null)
			{
				return enumerator.Current.GetType();
			}
		}
		return type2;
	}
}
