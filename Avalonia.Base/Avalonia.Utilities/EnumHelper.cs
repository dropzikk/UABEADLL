using System;

namespace Avalonia.Utilities;

internal class EnumHelper
{
	public static T Parse<T>(ReadOnlySpan<char> key, bool ignoreCase) where T : struct
	{
		return Enum.Parse<T>(key, ignoreCase);
	}
}
