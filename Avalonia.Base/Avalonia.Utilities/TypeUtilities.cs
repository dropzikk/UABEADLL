using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Avalonia.Utilities;

public static class TypeUtilities
{
	[Flags]
	private enum OperatorType
	{
		Implicit = 1,
		Explicit = 2
	}

	private static readonly int[] Conversions = new int[15]
	{
		24573, 17406, 24575, 24575, 24575, 24575, 24575, 24575, 24575, 24575,
		24573, 24573, 24573, 24576, 32767
	};

	private static readonly int[] ImplicitConversions = new int[15]
	{
		1, 7650, 7508, 8184, 7504, 8160, 7488, 8064, 7424, 7680,
		3072, 2048, 4096, 8192, 16384
	};

	private static readonly Type[] InbuiltTypes = new Type[15]
	{
		typeof(bool),
		typeof(char),
		typeof(sbyte),
		typeof(byte),
		typeof(short),
		typeof(ushort),
		typeof(int),
		typeof(uint),
		typeof(long),
		typeof(ulong),
		typeof(float),
		typeof(double),
		typeof(decimal),
		typeof(DateTime),
		typeof(string)
	};

	private static readonly Type[] NumericTypes = new Type[11]
	{
		typeof(byte),
		typeof(decimal),
		typeof(double),
		typeof(short),
		typeof(int),
		typeof(long),
		typeof(sbyte),
		typeof(float),
		typeof(ushort),
		typeof(uint),
		typeof(ulong)
	};

	public static bool AcceptsNull(Type type)
	{
		if (type.IsValueType)
		{
			return IsNullableType(type);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool AcceptsNull<T>()
	{
		return default(T) == null;
	}

	public static bool CanCast<T>(object? value)
	{
		if (!(value is T))
		{
			if (value == null)
			{
				return AcceptsNull<T>();
			}
			return false;
		}
		return true;
	}

	[RequiresUnreferencedCode("Conversion methods are required for type conversion, including op_Implicit, op_Explicit, Parse and TypeConverter.")]
	public static bool TryConvert(Type to, object? value, CultureInfo? culture, out object? result)
	{
		if (value == null)
		{
			result = null;
			return AcceptsNull(to);
		}
		if (value == AvaloniaProperty.UnsetValue)
		{
			result = value;
			return true;
		}
		Type type = Nullable.GetUnderlyingType(to) ?? to;
		Type type2 = value.GetType();
		if (type.IsAssignableFrom(type2))
		{
			result = value;
			return true;
		}
		if (type == typeof(string))
		{
			result = Convert.ToString(value, culture);
			return true;
		}
		if (type.IsEnum && type2 == typeof(string) && Enum.IsDefined(type, (string)value))
		{
			result = Enum.Parse(type, (string)value);
			return true;
		}
		if (!type2.IsEnum && type.IsEnum)
		{
			result = null;
			if (TryConvert(Enum.GetUnderlyingType(type), value, culture, out object result2))
			{
				result = Enum.ToObject(type, result2);
				return true;
			}
		}
		if (type2.IsEnum && IsNumeric(type))
		{
			try
			{
				result = Convert.ChangeType((int)value, type, culture);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}
		int num = Array.IndexOf(InbuiltTypes, type2);
		int num2 = Array.IndexOf(InbuiltTypes, type);
		if (num != -1 && num2 != -1 && (Conversions[num] & (1 << num2)) != 0)
		{
			try
			{
				result = Convert.ChangeType(value, type, culture);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}
		TypeConverter converter = TypeDescriptor.GetConverter(type);
		if (converter.CanConvertFrom(type2))
		{
			result = converter.ConvertFrom(null, culture, value);
			return true;
		}
		TypeConverter converter2 = TypeDescriptor.GetConverter(type2);
		if (converter2.CanConvertTo(type))
		{
			result = converter2.ConvertTo(null, culture, value, type);
			return true;
		}
		MethodInfo methodInfo = FindTypeConversionOperatorMethod(type2, type, OperatorType.Implicit | OperatorType.Explicit);
		if (methodInfo != null)
		{
			result = methodInfo.Invoke(null, new object[1] { value });
			return true;
		}
		result = null;
		return false;
	}

	[RequiresUnreferencedCode("Implicit conversion methods are required for type conversion.")]
	public static bool TryConvertImplicit(Type to, object? value, out object? result)
	{
		if (value == null)
		{
			result = null;
			return AcceptsNull(to);
		}
		if (value == AvaloniaProperty.UnsetValue)
		{
			result = value;
			return true;
		}
		Type type = value.GetType();
		if (to.IsAssignableFrom(type))
		{
			result = value;
			return true;
		}
		int num = Array.IndexOf(InbuiltTypes, type);
		int num2 = Array.IndexOf(InbuiltTypes, to);
		if (num != -1 && num2 != -1 && (ImplicitConversions[num] & (1 << num2)) != 0)
		{
			try
			{
				result = Convert.ChangeType(value, to, CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}
		MethodInfo methodInfo = FindTypeConversionOperatorMethod(type, to, OperatorType.Implicit);
		if (methodInfo != null)
		{
			result = methodInfo.Invoke(null, new object[1] { value });
			return true;
		}
		result = null;
		return false;
	}

	[RequiresUnreferencedCode("Conversion methods are required for type conversion, including op_Implicit, op_Explicit, Parse and TypeConverter.")]
	public static object? ConvertOrDefault(object? value, Type type, CultureInfo culture)
	{
		if (!TryConvert(type, value, culture, out object result))
		{
			return Default(type);
		}
		return result;
	}

	[RequiresUnreferencedCode("Implicit conversion methods are required for type conversion.")]
	public static object? ConvertImplicitOrDefault(object? value, Type type)
	{
		if (!TryConvertImplicit(type, value, out object result))
		{
			return Default(type);
		}
		return result;
	}

	[RequiresUnreferencedCode("Implicit conversion methods are required for type conversion.")]
	public static T ConvertImplicit<T>(object? value)
	{
		if (TryConvertImplicit(typeof(T), value, out object result))
		{
			return (T)result;
		}
		throw new InvalidCastException($"Unable to convert object '{value ?? "(null)"}' of type '{value?.GetType()}' to type '{typeof(T)}'.");
	}

	[UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "We don't care about public ctors for the value types, and always return null for the ref types.")]
	public static object? Default(Type type)
	{
		if (type.IsValueType)
		{
			return Activator.CreateInstance(type);
		}
		return null;
	}

	public static bool IsNumeric(Type type)
	{
		Type underlyingType = Nullable.GetUnderlyingType(type);
		if (underlyingType != null)
		{
			return IsNumeric(underlyingType);
		}
		return NumericTypes.Contains(type);
	}

	private static bool IsNullableType(Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		return false;
	}

	private static MethodInfo? FindTypeConversionOperatorMethod([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type fromType, Type toType, OperatorType operatorType)
	{
		bool flag = operatorType.HasAllFlags(OperatorType.Implicit);
		bool flag2 = operatorType.HasAllFlags(OperatorType.Explicit);
		MethodInfo[] methods = fromType.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.IsSpecialName && !(methodInfo.ReturnType != toType))
			{
				if (flag && methodInfo.Name == "op_Implicit")
				{
					return methodInfo;
				}
				if (flag2 && methodInfo.Name == "op_Explicit")
				{
					return methodInfo;
				}
			}
		}
		return null;
	}
}
