using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Avalonia.Utilities;

internal class ThrowHelper
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNull([NotNull] object? argument, string paramName)
	{
		if (argument == null)
		{
			ThrowArgumentNullException(paramName);
		}
	}

	[DoesNotReturn]
	private static void ThrowArgumentNullException(string paramName)
	{
		throw new ArgumentNullException(paramName);
	}
}
