using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors;

internal static class ThrowHelper
{
	[DoesNotReturn]
	public static void ThrowArgumentExceptionForNotNullOrWhitespace(string? value, string name)
	{
		if (value == null)
		{
			ThrowArgumentNullException(name, "Parameter \"" + name + "\" must be not null.");
		}
		else
		{
			ThrowArgumentException(name, "Parameter \"" + name + "\" must not be empty or whitespace.");
		}
	}

	[DoesNotReturn]
	public static void ThrowArgumentOutOfRangeExceptionForMustBeLessThan<T>(T value, T max, string name)
	{
		ThrowArgumentOutOfRangeException(name, $"Parameter \"{name}\" ({typeof(T)}) must be less than {max}, was {value}");
	}

	[DoesNotReturn]
	public static void ThrowArgumentOutOfRangeExceptionForMustBeLessThanOrEqualTo<T>(T value, T maximum, string name)
	{
		ThrowArgumentOutOfRangeException(name, $"Parameter \"{name}\" ({typeof(T)}) must be less than or equal to {maximum}, was {value}");
	}

	[DoesNotReturn]
	public static void ThrowArgumentOutOfRangeExceptionForMustBeGreaterThan<T>(T value, T minimum, string name)
	{
		ThrowArgumentOutOfRangeException(name, $"Parameter \"{name}\" ({typeof(T)}) must be greater than {minimum}, was {value}");
	}

	[DoesNotReturn]
	public static void ThrowArgumentOutOfRangeExceptionForMustBeGreaterThanOrEqualTo<T>(T value, T minimum, string name)
	{
		ThrowArgumentOutOfRangeException(name, $"Parameter \"{name}\" ({typeof(T)}) must be greater than or equal to {minimum}, was {value}");
	}

	[DoesNotReturn]
	public static void ThrowArgumentOutOfRangeExceptionForMustBeBetweenOrEqualTo<T>(T value, T minimum, T maximum, string name)
	{
		ThrowArgumentOutOfRangeException(name, $"Parameter \"{name}\" ({typeof(T)}) must be between or equal to {minimum} and {maximum}, was {value}");
	}

	[DoesNotReturn]
	public static void ThrowArgumentOutOfRangeExceptionForMustBeSizedAtLeast(int minLength, string parameterName)
	{
		ThrowArgumentException($"Spans must be at least of length {minLength}!", parameterName);
	}

	[DoesNotReturn]
	public static void ThrowArgumentException(string message, string name)
	{
		throw new ArgumentException(message, name);
	}

	[DoesNotReturn]
	public static void ThrowArgumentNullException(string name, string message)
	{
		throw new ArgumentNullException(name, message);
	}

	[DoesNotReturn]
	public static void ThrowArgumentOutOfRangeException(string name, string message)
	{
		throw new ArgumentOutOfRangeException(name, message);
	}
}
