using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tmds.DBus.Protocol;

internal static class TypeModel
{
	private static class SignatureCache<T>
	{
		private static readonly byte[] s_signature = GetSignature(typeof(T));

		public static ReadOnlySpan<byte> Signature => s_signature;

		private static byte[] GetSignature(Type type)
		{
			Span<byte> signature = stackalloc byte[256];
			return signature[..AppendTypeSignature(type, signature)].ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DBusType GetTypeAlignment<T>()
	{
		if (typeof(T) == typeof(object))
		{
			return DBusType.Variant;
		}
		if (typeof(T) == typeof(byte))
		{
			return DBusType.Byte;
		}
		if (typeof(T) == typeof(bool))
		{
			return DBusType.Bool;
		}
		if (typeof(T) == typeof(short))
		{
			return DBusType.Int16;
		}
		if (typeof(T) == typeof(ushort))
		{
			return DBusType.UInt16;
		}
		if (typeof(T) == typeof(int))
		{
			return DBusType.Int32;
		}
		if (typeof(T) == typeof(uint))
		{
			return DBusType.UInt32;
		}
		if (typeof(T) == typeof(long))
		{
			return DBusType.Int64;
		}
		if (typeof(T) == typeof(ulong))
		{
			return DBusType.UInt64;
		}
		if (typeof(T) == typeof(double))
		{
			return DBusType.Double;
		}
		if (typeof(T) == typeof(string))
		{
			return DBusType.String;
		}
		if (typeof(T) == typeof(ObjectPath))
		{
			return DBusType.ObjectPath;
		}
		if (typeof(T) == typeof(Signature))
		{
			return DBusType.Signature;
		}
		if (typeof(T).IsArray)
		{
			return DBusType.Array;
		}
		if (ExtractGenericInterface(typeof(T), typeof(IEnumerable<>)) != null)
		{
			return DBusType.Array;
		}
		if (typeof(T).IsAssignableTo(typeof(SafeHandle)))
		{
			return DBusType.UnixFd;
		}
		return DBusType.Struct;
	}

	public static Type? ExtractGenericInterface(Type queryType, Type interfaceType)
	{
		if (IsGenericInstantiation(queryType, interfaceType))
		{
			return queryType;
		}
		return GetGenericInstantiation(queryType, interfaceType);
	}

	public static Type DetermineVariantType(Utf8Span signature)
	{
		Func<DBusType, Type[], Type> map = delegate(DBusType dbusType, Type[] innerTypes)
		{
			switch (dbusType)
			{
			case DBusType.Byte:
				return typeof(byte);
			case DBusType.Bool:
				return typeof(bool);
			case DBusType.Int16:
				return typeof(short);
			case DBusType.UInt16:
				return typeof(ushort);
			case DBusType.Int32:
				return typeof(int);
			case DBusType.UInt32:
				return typeof(uint);
			case DBusType.Int64:
				return typeof(long);
			case DBusType.UInt64:
				return typeof(ulong);
			case DBusType.Double:
				return typeof(double);
			case DBusType.String:
				return typeof(string);
			case DBusType.ObjectPath:
				return typeof(ObjectPath);
			case DBusType.Signature:
				return typeof(Signature);
			case DBusType.UnixFd:
				return typeof(SafeHandle);
			case DBusType.Array:
				return innerTypes[0].MakeArrayType();
			case DBusType.DictEntry:
				return typeof(Dictionary<, >).MakeGenericType(innerTypes);
			case DBusType.Struct:
				switch (innerTypes.Length)
				{
				case 1:
					return typeof(ValueTuple<>).MakeGenericType(innerTypes);
				case 2:
					return typeof(ValueTuple<, >).MakeGenericType(innerTypes);
				case 3:
					return typeof(ValueTuple<, , >).MakeGenericType(innerTypes);
				case 4:
					return typeof(ValueTuple<, , , >).MakeGenericType(innerTypes);
				case 5:
					return typeof(ValueTuple<, , , , >).MakeGenericType(innerTypes);
				case 6:
					return typeof(ValueTuple<, , , , , >).MakeGenericType(innerTypes);
				case 7:
					return typeof(ValueTuple<, , , , , , >).MakeGenericType(innerTypes);
				case 8:
				case 9:
				case 10:
				{
					Type[] array = new Type[8];
					innerTypes.AsSpan(0, 7).CopyTo(array);
					Type[] array2 = array;
					array2[7] = innerTypes.Length switch
					{
						8 => typeof(ValueTuple<>).MakeGenericType(innerTypes[7]), 
						9 => typeof(ValueTuple<, >).MakeGenericType(innerTypes[7], innerTypes[8]), 
						10 => typeof(ValueTuple<, , >).MakeGenericType(innerTypes[7], innerTypes[8], innerTypes[9]), 
						_ => null, 
					};
					return typeof(ValueTuple<, , , , , , , >).MakeGenericType(array);
				}
				}
				break;
			}
			return typeof(object);
		};
		return SignatureReader.Transform(signature, map);
	}

	private static bool IsGenericInstantiation(Type candidate, Type interfaceType)
	{
		if (candidate.IsGenericType)
		{
			return candidate.GetGenericTypeDefinition() == interfaceType;
		}
		return false;
	}

	private static Type? GetGenericInstantiation(Type queryType, Type interfaceType)
	{
		Type type = null;
		Type[] interfaces = queryType.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			if (IsGenericInstantiation(type2, interfaceType))
			{
				if (type == null)
				{
					type = type2;
				}
				else if (StringComparer.Ordinal.Compare(type2.FullName, type.FullName) < 0)
				{
					type = type2;
				}
			}
		}
		if (type != null)
		{
			return type;
		}
		Type type3 = queryType?.BaseType;
		if (type3 == null)
		{
			return null;
		}
		return GetGenericInstantiation(type3, interfaceType);
	}

	private static int AppendTypeSignature(Type type, Span<byte> signature)
	{
		if (type == typeof(object))
		{
			signature[0] = 118;
			return 1;
		}
		if (type == typeof(byte))
		{
			signature[0] = 121;
			return 1;
		}
		if (type == typeof(bool))
		{
			signature[0] = 98;
			return 1;
		}
		if (type == typeof(short))
		{
			signature[0] = 110;
			return 1;
		}
		if (type == typeof(ushort))
		{
			signature[0] = 113;
			return 1;
		}
		if (type == typeof(int))
		{
			signature[0] = 105;
			return 1;
		}
		if (type == typeof(uint))
		{
			signature[0] = 117;
			return 1;
		}
		if (type == typeof(long))
		{
			signature[0] = 120;
			return 1;
		}
		if (type == typeof(ulong))
		{
			signature[0] = 116;
			return 1;
		}
		if (type == typeof(double))
		{
			signature[0] = 100;
			return 1;
		}
		if (type == typeof(string))
		{
			signature[0] = 115;
			return 1;
		}
		if (type == typeof(ObjectPath))
		{
			signature[0] = 111;
			return 1;
		}
		if (type == typeof(Signature))
		{
			signature[0] = 103;
			return 1;
		}
		if (type.IsArray)
		{
			int num = 0;
			signature[num++] = 97;
			return num + AppendTypeSignature(type.GetElementType(), signature.Slice(num));
		}
		if (type.FullName.StartsWith("System.ValueTuple"))
		{
			int num2 = 0;
			signature[num2++] = 40;
			Type[] genericTypeArguments = type.GenericTypeArguments;
			while (true)
			{
				for (int i = 0; i < genericTypeArguments.Length && i != 7; i++)
				{
					num2 += AppendTypeSignature(genericTypeArguments[i], signature.Slice(num2));
				}
				if (genericTypeArguments.Length != 8)
				{
					break;
				}
				genericTypeArguments = genericTypeArguments[7].GenericTypeArguments;
			}
			signature[num2++] = 41;
			return num2;
		}
		Type type2;
		if ((type2 = ExtractGenericInterface(type, typeof(IDictionary<, >))) != null)
		{
			int num3 = 0;
			signature[num3++] = 97;
			signature[num3++] = 123;
			num3 += AppendTypeSignature(type2.GenericTypeArguments[0], signature.Slice(num3));
			num3 += AppendTypeSignature(type2.GenericTypeArguments[1], signature.Slice(num3));
			signature[num3++] = 125;
			return num3;
		}
		if (type.IsAssignableTo(typeof(SafeHandle)))
		{
			signature[0] = 104;
			return 1;
		}
		return 0;
	}

	public static ReadOnlySpan<byte> GetSignature<T>()
	{
		return SignatureCache<T>.Signature;
	}
}
