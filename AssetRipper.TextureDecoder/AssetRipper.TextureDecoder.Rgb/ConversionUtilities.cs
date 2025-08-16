using System;
using System.Runtime.CompilerServices;

namespace AssetRipper.TextureDecoder.Rgb;

internal static class ConversionUtilities
{
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	internal static TTo ConvertValue<TFrom, TTo>(TFrom value) where TFrom : unmanaged where TTo : unmanaged
	{
		if (typeof(TFrom) == typeof(TTo))
		{
			return Unsafe.As<TFrom, TTo>(ref value);
		}
		if (typeof(TFrom) == typeof(byte))
		{
			return ConvertUInt8<TTo>(Unsafe.As<TFrom, byte>(ref value));
		}
		if (typeof(TFrom) == typeof(ushort))
		{
			return ConvertUInt16<TTo>(Unsafe.As<TFrom, ushort>(ref value));
		}
		if (typeof(TFrom) == typeof(Half))
		{
			return ConvertHalf<TTo>(Unsafe.As<TFrom, Half>(ref value));
		}
		if (typeof(TFrom) == typeof(float))
		{
			return ConvertSingle<TTo>(Unsafe.As<TFrom, float>(ref value));
		}
		if (typeof(TFrom) == typeof(double))
		{
			return ConvertDouble<TTo>(Unsafe.As<TFrom, double>(ref value));
		}
		return default(TTo);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static TTo ConvertUInt8<TTo>(byte value) where TTo : unmanaged
	{
		if (typeof(TTo) == typeof(byte))
		{
			return Unsafe.As<byte, TTo>(ref value);
		}
		if (typeof(TTo) == typeof(ushort))
		{
			ushort source = (ushort)(value << 8);
			return Unsafe.As<ushort, TTo>(ref source);
		}
		if (typeof(TTo) == typeof(Half))
		{
			Half source2 = (Half)((float)(int)value / 255f);
			return Unsafe.As<Half, TTo>(ref source2);
		}
		if (typeof(TTo) == typeof(float))
		{
			float source3 = (float)(int)value / 255f;
			return Unsafe.As<float, TTo>(ref source3);
		}
		if (typeof(TTo) == typeof(double))
		{
			double source4 = (double)(int)value / 255.0;
			return Unsafe.As<double, TTo>(ref source4);
		}
		return default(TTo);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static TTo ConvertUInt16<TTo>(ushort value) where TTo : unmanaged
	{
		if (typeof(TTo) == typeof(byte))
		{
			byte source = (byte)((uint)value >> 8);
			return Unsafe.As<byte, TTo>(ref source);
		}
		if (typeof(TTo) == typeof(ushort))
		{
			return Unsafe.As<ushort, TTo>(ref value);
		}
		if (typeof(TTo) == typeof(Half))
		{
			Half source2 = (Half)((float)(int)value / 65535f);
			return Unsafe.As<Half, TTo>(ref source2);
		}
		if (typeof(TTo) == typeof(float))
		{
			float source3 = (float)(int)value / 65535f;
			return Unsafe.As<float, TTo>(ref source3);
		}
		if (typeof(TTo) == typeof(double))
		{
			double source4 = (double)(int)value / 65535.0;
			return Unsafe.As<double, TTo>(ref source4);
		}
		return default(TTo);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static TTo ConvertHalf<TTo>(Half value) where TTo : unmanaged
	{
		if (typeof(TTo) == typeof(byte))
		{
			byte source = ClampUInt8((float)value * 255f);
			return Unsafe.As<byte, TTo>(ref source);
		}
		if (typeof(TTo) == typeof(ushort))
		{
			ushort source2 = ClampUInt16((float)value * 65535f);
			return Unsafe.As<ushort, TTo>(ref source2);
		}
		if (typeof(TTo) == typeof(Half))
		{
			return Unsafe.As<Half, TTo>(ref value);
		}
		if (typeof(TTo) == typeof(float))
		{
			float source3 = (float)value;
			return Unsafe.As<float, TTo>(ref source3);
		}
		if (typeof(TTo) == typeof(double))
		{
			double source4 = (double)value;
			return Unsafe.As<double, TTo>(ref source4);
		}
		return default(TTo);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static TTo ConvertSingle<TTo>(float value) where TTo : unmanaged
	{
		if (typeof(TTo) == typeof(byte))
		{
			byte source = ClampUInt8(value * 255f);
			return Unsafe.As<byte, TTo>(ref source);
		}
		if (typeof(TTo) == typeof(ushort))
		{
			ushort source2 = ClampUInt16(value * 65535f);
			return Unsafe.As<ushort, TTo>(ref source2);
		}
		if (typeof(TTo) == typeof(Half))
		{
			Half source3 = (Half)value;
			return Unsafe.As<Half, TTo>(ref source3);
		}
		if (typeof(TTo) == typeof(float))
		{
			return Unsafe.As<float, TTo>(ref value);
		}
		if (typeof(TTo) == typeof(double))
		{
			double source4 = value;
			return Unsafe.As<double, TTo>(ref source4);
		}
		return default(TTo);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static TTo ConvertDouble<TTo>(double value) where TTo : unmanaged
	{
		if (typeof(TTo) == typeof(byte))
		{
			byte source = ClampUInt8(value * 255.0);
			return Unsafe.As<byte, TTo>(ref source);
		}
		if (typeof(TTo) == typeof(ushort))
		{
			ushort source2 = ClampUInt16(value * 65535.0);
			return Unsafe.As<ushort, TTo>(ref source2);
		}
		if (typeof(TTo) == typeof(Half))
		{
			Half source3 = (Half)value;
			return Unsafe.As<Half, TTo>(ref source3);
		}
		if (typeof(TTo) == typeof(float))
		{
			float source4 = (float)value;
			return Unsafe.As<float, TTo>(ref source4);
		}
		if (typeof(TTo) == typeof(double))
		{
			return Unsafe.As<double, TTo>(ref value);
		}
		return default(TTo);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static byte ClampUInt8(float x)
	{
		if (!(255f < x))
		{
			if (!(x > 0f))
			{
				return 0;
			}
			return (byte)x;
		}
		return byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static byte ClampUInt8(double x)
	{
		if (!(255.0 < x))
		{
			if (!(x > 0.0))
			{
				return 0;
			}
			return (byte)x;
		}
		return byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static ushort ClampUInt16(float x)
	{
		if (!(65535f < x))
		{
			if (!(x > 0f))
			{
				return 0;
			}
			return (ushort)x;
		}
		return ushort.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static ushort ClampUInt16(double x)
	{
		if (!(65535.0 < x))
		{
			if (!(x > 0.0))
			{
				return 0;
			}
			return (ushort)x;
		}
		return ushort.MaxValue;
	}
}
