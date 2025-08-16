using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

internal static class EnumUtils
{
	public static TEnum Parse<TEnum>(int value, TEnum defaultValue) where TEnum : struct, Enum
	{
		TEnum val = Unsafe.As<int, TEnum>(ref value);
		if (Enum.IsDefined(val))
		{
			return val;
		}
		return defaultValue;
	}

	public static bool HasFlag<TEnum>(TEnum value, TEnum flag) where TEnum : struct, Enum
	{
		uint num = Unsafe.As<TEnum, uint>(ref flag);
		return (Unsafe.As<TEnum, uint>(ref value) & num) == num;
	}
}
