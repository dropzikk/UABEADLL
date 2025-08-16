using System;
using System.IO;

namespace AssetsTools.NET.Extra;

public static class Net35Polyfill
{
	public static void CopyToCompat(this Stream input, Stream output, long bytes = -1L, int bufferSize = 81920)
	{
		byte[] array = new byte[bufferSize];
		if (bytes == -1)
		{
			bytes = long.MaxValue;
		}
		int num;
		while (bytes > 0 && (num = input.Read(array, 0, (int)Math.Min(array.Length, bytes))) > 0)
		{
			output.Write(array, 0, num);
			bytes -= num;
		}
	}

	public static bool HasFlag(Enum variable, Enum value)
	{
		if (variable == null)
		{
			return false;
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (!Enum.IsDefined(variable.GetType(), value))
		{
			throw new ArgumentException($"Enumeration type mismatch.  The flag is of type '{value.GetType()}', was expecting '{variable.GetType()}'.");
		}
		ulong num = Convert.ToUInt64(value);
		return (Convert.ToUInt64(variable) & num) == num;
	}
}
